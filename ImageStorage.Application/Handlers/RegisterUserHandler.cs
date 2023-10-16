using ImageStorage.Application.Common;
using ImageStorage.Application.Exceptions;
using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.Requests;
using ImageStorage.Application.Responses;
using ImageStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImageStorage.Application.Handlers;

public class RegisterUserHandler : BaseUseCaseHandler<RegisterUserRequest, RegisterUserResponse>
{
    public RegisterUserHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public override async Task<RegisterUserResponse> Handle(RegisterUserRequest request)
    {
        var result = new RegisterUserResponse();

        if (SessionContext.AuthorizedUserId != null)
        {
            result.AddError(new("Authorized user cannot register."));
            return result;
        }

        if (await DbAccessor.Users.AsNoTracking().AnyAsync(x => x.Name == request.Name))
        {
            result.AddError(new("User with the same name has already registered."));
        }

        if (await DbAccessor.Users.AsNoTracking().AnyAsync(x => x.Email == request.Email))
        {
            result.AddError(new("User with the same email has already registered."));
        }

        if (!result.IsSucceeded)
        {
            return result;
        }

        byte[] passwordHash = HashCalculator.GetHash(request.Password);

        var user = User.CreateUser(request.Name, request.Email, passwordHash);
        DbAccessor.AddUser(user);

        try
        {
            var savedEntitiesCount = await DbAccessor.SaveChangesAsync();

            if (savedEntitiesCount == 0)
            {
                result.AddError(new(OperationErrorCode.ServerError, "Cannot save user to database."));
                return result;
            }
        }
        catch (ConcurrencyConflictException ex)
        {
            result.AddError(new($"User with the same {ex.PropertyName} has already registered."));
            return result;
        }

        ImagesStorageAccessor.EnsureUserDirectoryExists(user.Id);

        result.Value = user;

        return result;
    }
}
