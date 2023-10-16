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
        public void UploadImage()
        {

        }

        [HttpGet, Route("[action]")]
        public void GetUserImage()
        {

        }

        [HttpGet, Route("[action]")]
        public void GetUserImages()
        {

        }

        [HttpGet, Route("[action]")]
        public void GetFriendImage()
        {

        }

        [HttpGet, Route("[action]")]
        public void GetFriendImages()
        {

        }
    }
}