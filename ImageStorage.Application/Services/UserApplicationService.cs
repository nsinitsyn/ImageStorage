using ImageStorage.Application.Common;
using ImageStorage.Application.Dependencies;
using ImageStorage.Application.Exceptions;
using ImageStorage.Application.RequestModels;
using ImageStorage.Application.ResponseModels;
using ImageStorage.Domain.Entities;
using ImageStorage.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ImageStorage.Application.Services;

public class UserApplicationService
{
    private readonly IDbAccessor _dbAccessor;
    private readonly IImagesStorageAccessor _imagesStorageAccessor;
    private readonly IHashCalculator _hashCalculator;
    private readonly ISessionContext _sessionContext;

    public UserApplicationService(
        IDbAccessor dbAccessor,
        IImagesStorageAccessor imagesStorageAccessor,
        IHashCalculator hashCalculator, 
        ISessionContext sessionContext)
    {
        _dbAccessor = dbAccessor;
        _imagesStorageAccessor = imagesStorageAccessor;
        _hashCalculator = hashCalculator;
        _sessionContext = sessionContext;
    }

    public async Task<User?> ValidateCredentials(string? name, string? password)
    {
        if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
        {
            return null;
        }

        byte[] passwordHash = _hashCalculator.GetHash(password);

        return await _dbAccessor.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name && x.PasswordHash.SequenceEqual(passwordHash));
    }

    public async Task<OperationResult<User>> RegisterUser(UserRegisterRequest request)
    {
        var result = new OperationResult<User>();

        if (_sessionContext.AuthorizedUserId != null)
        {
            result.AddError(new("Authorized user cannot register."));
            return result;
        }

        if (await _dbAccessor.Users.AsNoTracking().AnyAsync(x => x.Name == request.Name))
        {
            result.AddError(new("User with the same name has already registered."));
        }

        if (await  _dbAccessor.Users.AsNoTracking().AnyAsync(x => x.Email == request.Email))
        {
            result.AddError(new("User with the same email has already registered."));
        }

        if(!result.IsSucceeded)
        {
            return result;
        }

        byte[] passwordHash = _hashCalculator.GetHash(request.Password);

        var user = User.CreateUser(request.Name, request.Email, passwordHash);
        _dbAccessor.AddUser(user);

        try
        {
            var savedEntitiesCount = await _dbAccessor.SaveChangesAsync();

            if(savedEntitiesCount == 0)
            {
                // todo: ошибка не для клиента и код 500 должен быть
                result.AddError(new("Cannot save user to database."));
                return result;
            }
        }
        catch(ConcurrencyConflictException ex)
        {
            result.AddError(new($"User with the same {ex.PropertyName} has already registered."));
            return result;
        }

        // todo: ошибка с требованием ручного вмешательства
        _imagesStorageAccessor.EnsureUserDirectoryExists(user.Id);

        result.Value = user;

        return result;
    }

    public async Task<OperationResult> AddFriend(UserAddFriendRequest request)
    {
        var result = new OperationResult();

        Guid? userId = _sessionContext.AuthorizedUserId;

        if(userId == null)
        {
            result.AddError(new($"User not authorized."));
            return result;
        }

        if(userId == request.FriendId)
        {
            result.AddError(new($"Cannot add yourself as friend."));
            return result;
        }

        User user = await _dbAccessor.Users
            .Include(x => x.Friends)
            .FirstAsync(x => x.Id == userId);

        User? friend = await _dbAccessor.Users
            .FirstOrDefaultAsync(x => x.Id == request.FriendId);

        // todo: это 404
        if(friend == null)
        {
            result.AddError(new($"Cannot find friend user with id={request.FriendId}."));
        }

        if (!result.IsSucceeded)
        {
            return result;
        }

        try
        {
            user!.AddFriend(friend!);
        }
        catch(DomainException ex)
        {
            // todo: эти ошибки не показываются в UI
            result.AddError(new(ex.Message));
            return result;
        }

        try
        {
            var savedEntitiesCount = await _dbAccessor.SaveChangesAsync();

            if (savedEntitiesCount == 0)
            {
                // todo: ошибка не для клиента и код 500 должен быть
                result.AddError(new("Cannot save user to database."));
                return result;
            }
        }
        catch (ConcurrencyConflictException ex)
        {
            // todo: отлавливать ошибку, что этого друга уже добавили.
            // result.AddError(new($"User with the same {ex.PropertyName} has already registered."));
        }

        return result;
    }

    public async Task<OperationResult<Image>> AddImage(UserAddImageRequest request)
    {
        var result = new OperationResult<Image>();

        // todo: выкидывает exception
        Guid userId = _sessionContext.GetRequiredAuthorizedUserId();

        Image image = Image.CreateImage(request.FileName);

        // todo: 400, если не подходит расширение
        var fileStream = _imagesStorageAccessor.CreateFileStreamForSaving(userId, request.FileName, image.Id);
        await request.FileUploader.CopyToAsync(fileStream);

        User user = await _dbAccessor.Users
            .Include(x => x.Images)
            .FirstAsync(x => x.Id == userId);

        user.AddImage(image);

        try
        {
            var savedEntitiesCount = await _dbAccessor.SaveChangesAsync();

            // todo:
        }
        catch (Exception ex)
        {
            // todo: откат сохранения в репозиторий
        }

        result.Value = image;

        return result;
    }

    public async Task<OperationResult<UserGetImageContentResponse>> GetImageContent(UserGetImageContentRequest request)
    {
        var result = new OperationResult<UserGetImageContentResponse>();

        // todo: выкидывает exception
        Guid userId = _sessionContext.GetRequiredAuthorizedUserId();

        Image? image = await _dbAccessor.Images
            .AsNoTracking()
            .Include(x => x.User)
                .ThenInclude(x => x.Friends)
            .FirstOrDefaultAsync(x => x.Id == request.ImageId);

        if(image == null || !image.UserHasAccess(userId))
        {
            // todo:
            //result.AddError();
            return result;
        }

        FileStream fileStream = _imagesStorageAccessor.OpenFileStreamForReading(image.UserId, request.ImageId);

        result.Value = new UserGetImageContentResponse
        {
            FileName = image.FileName,
            FileStream = fileStream
        };

        return result;
    }

    public async Task<OperationResult<IReadOnlyCollection<Image>>> GetUserImages()
    {
        var result = new OperationResult<IReadOnlyCollection<Image>>();

        Guid userId = _sessionContext.GetRequiredAuthorizedUserId();

        User user = await _dbAccessor.Users
            .AsNoTracking()
            .Include(x => x.Images)
            .FirstAsync(x => x.Id == userId);

        result.Value = user.Images;

        return result;
    }

    public async Task<OperationResult<IReadOnlyCollection<Image>>> GetOtherUserImages(UserGetOtherUserImagesRequest request)
    {
        var result = new OperationResult<IReadOnlyCollection<Image>>();

        Guid userId = _sessionContext.GetRequiredAuthorizedUserId();

        User otherUser = await _dbAccessor.Users
            .AsNoTracking()
            .Include(x => x.Friends)
            .Include(x => x.Images)
            .FirstAsync(x => x.Id == request.UserId);

        if(!otherUser.IsFriend(userId))
        {
            // todo:
            return result;
        }

        result.Value = otherUser.Images;

        return result;
    }
}