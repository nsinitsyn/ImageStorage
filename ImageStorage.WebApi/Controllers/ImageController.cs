using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.Requests;
using ImageStorage.Application.Responses;
using ImageStorage.Domain.Entities;
using ImageStorage.WebApi.Helpers;
using ImageStorage.WebApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageStorage.WebApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class ImageController : BaseController
    {
        public ImageController(ILogger<BaseController> logger) : base(logger) { }

        [HttpPost, Route("[action]")]
        public async Task<IActionResult> UploadImage(
            IFormFile uploadedFile,
            [FromServices] IUseCaseHandler<AddImageRequest, AddImageResponse> handler)
        {
            if (uploadedFile == null)
            {
                return BadRequest();
            }

            return await Invoke(
                request: new AddImageRequest
                {
                    FileName = uploadedFile.FileName,
                    FileUploader = new FileUploader(uploadedFile)
                },
                handler,
                succeededActionResultFactory: result =>
                {
                    Image image = result.Value!;
                    ImageViewModel viewModel = ImageViewModel.FromImage(image);

                    return Created(viewModel.Link, viewModel);
                });
        }

        [HttpGet, Route("[action]")]
        public Task<IActionResult> GetImageContent(
            Guid imageId,
            [FromServices] IUseCaseHandler<GetImageContentRequest, GetImageContentResponse> handler)
        {
            return Invoke(
                request: new GetImageContentRequest { ImageId = imageId },
                handler,
                succeededActionResultFactory: result =>
                {
                    FileStream fileStream = result.FileStream;
                    string fileName = result.FileName;

                    return File(fileStream, "application/octet-stream", fileName);
                });
        }

        [HttpGet, Route("[action]")]
        public Task<IActionResult> GetImages(
            [FromServices] IUseCaseHandler<GetUserImagesRequest, GetUserImagesResponse> handler)
        {
            return Invoke(
                request: new GetUserImagesRequest(),
                handler,
                succeededActionResultFactory: result => Ok(result.Value!.Select(ImageViewModel.FromImage)));
        }

        [HttpGet, Route("[action]")]
        public Task<IActionResult> GetOtherUserImages(
            Guid userId,
            [FromServices] IUseCaseHandler<GetOtherUserImagesRequest, GetOtherUserImagesResponse> handler)
        {
            return Invoke(
                request: new GetOtherUserImagesRequest { UserId = userId }, 
                handler,
                succeededActionResultFactory: result => Ok(result.Value!.Select(ImageViewModel.FromImage)));
        }
    }
}