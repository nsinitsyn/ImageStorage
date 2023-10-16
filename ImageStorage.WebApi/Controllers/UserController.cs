using ImageStorage.Application.Handlers.Base;
using ImageStorage.Application.Requests;
using ImageStorage.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageStorage.WebApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class UserController : BaseController
    {
        public UserController(ILogger<BaseController> logger) : base(logger) { }

        [HttpPost, Route("[action]"), AllowAnonymous]
        public Task<IActionResult> Register(
            RegisterUserRequest request,
            [FromServices] IUseCaseHandler<RegisterUserRequest, RegisterUserResponse> handler)
        {
            return Invoke(request, handler, result => Ok(result.Value!.Id));
        }

        [HttpPost, Route("[action]")]
        public Task<IActionResult> AddFriend(
            AddFriendRequest request,
            [FromServices] IUseCaseHandler<AddFriendRequest, AddFriendResponse> handler)
        {
            return Invoke(request, handler, _ => Ok());
        }
    }
}