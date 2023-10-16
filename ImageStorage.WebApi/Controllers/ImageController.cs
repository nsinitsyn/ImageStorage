using ImageStorage.Application.Common;
using ImageStorage.Application.RequestModels;
using ImageStorage.Application.ResponseModels;
using ImageStorage.Application.Services;
using ImageStorage.Domain.Entities;
using ImageStorage.WebApi.Helpers;
using ImageStorage.WebApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageStorage.WebApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class ImageController : ControllerBase
    {
        private readonly UserApplicationService _userApplicationService;
        private readonly ILogger<ImageController> _logger;

        public ImageController(
            UserApplicationService userApplicationService,
            ILogger<ImageController> logger)
        {
            _userApplicationService = userApplicationService;
            _logger = logger;
        }

        [HttpPost, Route("[action]")]
        public async Task<IActionResult> UploadImage(IFormFile uploadedFile)
        {
            if (uploadedFile == null)
            {
                return BadRequest();
            }

            OperationResult<Image> result = await _userApplicationService.AddImage(new UserAddImageRequest
            {
                FileName = uploadedFile.FileName,
                FileUploader = new FileUploader(uploadedFile)
            });

            if(result.IsSucceeded)
            {
                Image image = result.Value!;
                ImageViewModel viewModel = ImageViewModel.FromImage(image);

                return Created(viewModel.Link, viewModel);
            }

            return BadRequest();
        }

        [HttpGet, Route("[action]")]
        public async Task<IActionResult> GetImageContent(Guid imageId)
        {
            OperationResult<UserGetImageContentResponse> result = await _userApplicationService.GetImageContent(
                new UserGetImageContentRequest { ImageId = imageId });

            if(result.IsSucceeded)
            {
                FileStream fileStream = result.Value!.FileStream;
                string fileName = result.Value!.FileName;

                return File(fileStream, "application/octet-stream", fileName);
            }

            return BadRequest();
        }

        [HttpGet, Route("[action]")]
        public async Task<IActionResult> GetImages()
        {
            OperationResult<IReadOnlyCollection<Image>> result = await _userApplicationService.GetUserImages();

            if(result.IsSucceeded)
            {
                return Ok(result.Value!.Select(ImageViewModel.FromImage));
            }

            return BadRequest();
        }

        [HttpGet, Route("[action]")]
        public async Task<IActionResult> GetOtherUserImages(Guid userId)
        {
            OperationResult<IReadOnlyCollection<Image>> result = await _userApplicationService.GetOtherUserImages(
                new UserGetOtherUserImagesRequest { UserId = userId });

            if (result.IsSucceeded)
            {
                return Ok(result.Value!.Select(ImageViewModel.FromImage));
            }

            return BadRequest();
        }
    }
}