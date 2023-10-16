using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.RequestModels;
using ImageStorage.Application.ResponseModels;
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
        private readonly ILogger<ImageController> _logger;

        public ImageController(ILogger<ImageController> logger)
        {
            _logger = logger;
        }

        [HttpPost, Route("[action]")]
        public async Task<IActionResult> UploadImage(
            IFormFile uploadedFile,
            [FromServices] IUseCaseHandler<UserAddImageRequest, UserAddImageResponse> handler)
        {
            if (uploadedFile == null)
            {
                return BadRequest();
            }

            UserAddImageResponse result = await handler.Handle(new UserAddImageRequest
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
        public async Task<IActionResult> GetImageContent(
            Guid imageId,
            [FromServices] IUseCaseHandler<UserGetImageContentRequest, UserGetImageContentResponse> handler)
        {
            UserGetImageContentResponse result = await handler.Handle(new UserGetImageContentRequest { ImageId = imageId });

            if(result.IsSucceeded)
            {
                FileStream fileStream = result.FileStream;
                string fileName = result.FileName;

                return File(fileStream, "application/octet-stream", fileName);
            }

            return BadRequest();
        }

        [HttpGet, Route("[action]")]
        public async Task<IActionResult> GetImages(
            [FromServices] IUseCaseHandler<GetUserImagesRequest, GetUserImagesResponse> handler)
        {
            GetUserImagesResponse result = await handler.Handle(new GetUserImagesRequest());

            if(result.IsSucceeded)
            {
                return Ok(result.Value!.Select(ImageViewModel.FromImage));
            }

            return BadRequest();
        }

        [HttpGet, Route("[action]")]
        public async Task<IActionResult> GetOtherUserImages(
            Guid userId,
            [FromServices] IUseCaseHandler<UserGetOtherUserImagesRequest, UserGetOtherUserImagesResponse> handler)
        {
            UserGetOtherUserImagesResponse result = await handler.Handle(new UserGetOtherUserImagesRequest { UserId = userId });

            if (result.IsSucceeded)
            {
                return Ok(result.Value!.Select(ImageViewModel.FromImage));
            }

            return BadRequest();
        }
    }
}