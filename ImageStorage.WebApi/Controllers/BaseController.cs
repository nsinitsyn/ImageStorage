using ImageStorage.Application.Common;
using ImageStorage.Application.Handlers.Base;
using Microsoft.AspNetCore.Mvc;

namespace ImageStorage.WebApi.Controllers;

public class BaseController : ControllerBase
{
    protected readonly ILogger<BaseController> Logger;

    public BaseController(ILogger<BaseController> logger)
    {
        Logger = logger;
    }

    public async Task<IActionResult> Invoke<TRequest, TResponse>(
        TRequest request,
        IUseCaseHandler<TRequest, TResponse> handler,
        Func<TResponse, IActionResult> succeededActionResultFactory)
        where TResponse : OperationResult
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        TResponse result;

        try
        {
            result = await handler.Handle(request);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception during request handling. Request: {request}", request);
            return StatusCode(500);
        }

        if (result.IsSucceeded)
        {
            return succeededActionResultFactory(result);
        }

        if (result.Errors.Any(x => x.Code == OperationErrorCode.NotFound))
        {
            return NotFound();
        }

        if (result.Errors.Any(x => x.Code == OperationErrorCode.AccessDenied))
        {
            return StatusCode(403);
        }

        if (result.Errors.Any(x => x.Code == OperationErrorCode.ServerError))
        {
            return StatusCode(500);
        }

        if (result.Errors.Any(x => x.Code == OperationErrorCode.NotAuthorized))
        {
            Logger.LogWarning("Unauthorized error code in controller: {request}", request);
            return Unauthorized();
        }

        // Доменные ошибки
        return BadRequest(result.ToString());
    }
}
