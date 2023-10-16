using ImageStorage.Application.Common;
using ImageStorage.Application.RequestModels;
using ImageStorage.Application.Services;
using ImageStorage.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageStorage.WebApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserApplicationService _userApplicationService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserApplicationService userApplicationService,
            ILogger<UserController> logger)
        {
            _userApplicationService = userApplicationService;
            _logger = logger;
        }

        [HttpPost, Route("[action]"), AllowAnonymous]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            OperationResult<User> result;

            try
            {
                result = await _userApplicationService.RegisterUser(request);
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
        public async Task<IActionResult> AddFriend(UserAddFriendRequest request)
        {
            OperationResult result;

            try
            {
                result = await _userApplicationService.AddFriend(request);
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