using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.RequestModels;
using ImageStorage.Application.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageStorage.WebApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost, Route("[action]"), AllowAnonymous]
        public async Task<IActionResult> Register(
            UserRegisterRequest request,
            [FromServices] IUseCaseHandler<UserRegisterRequest, UserRegisterResponse> handler)
        {
            UserRegisterResponse result;

            try
            {
                result = await handler.Handle(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register exception. Request: {request}", request);
                return StatusCode(500);
            }

            if (result.IsSucceeded)
            {
                return Ok(result.Value!.Id);
            }

            return BadRequest(result.ToString());
        }

        [HttpPost, Route("[action]")]
        public async Task<IActionResult> AddFriend(
            UserAddFriendRequest request,
            [FromServices] IUseCaseHandler<UserAddFriendRequest, UserAddFriendResponse> handler)
        {
            UserAddFriendResponse result;

            try
            {
                result = await handler.Handle(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddFriend exception. Request: {request}", request);
                return StatusCode(500);
            }

            if (result.IsSucceeded)
            {
                return Ok();
            }

            return BadRequest(result.ToString());
        }
    }
}