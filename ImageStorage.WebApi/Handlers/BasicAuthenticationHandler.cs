using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using ImageStorage.Application.Services;
using ImageStorage.Domain.Entities;
using ImageStorage.Infrastructure.Session;

namespace ImageStorage.WebApi.Handlers;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ISessionContextWriter _sessionContext;
    private readonly UserService _userService;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ISessionContextWriter sessionContext,
        UserService userService)
        : base(options, logger, encoder, clock)
    {
        _sessionContext = sessionContext;
        _userService = userService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        _sessionContext.AuthorizedUserId = null;

        Endpoint? endpoint = Context.GetEndpoint();

        if (endpoint != null)
        {
            IAllowAnonymous? allowAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>();

            if (allowAnonymous != null)
            {
                return AuthenticateResult.NoResult();
            }
        }

        string? name = null;

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');
            name = credentials.FirstOrDefault();
            var password = credentials.LastOrDefault();

            User? user = await _userService.ValidateCredentials(name, password);

            if (user == null)
            {
                throw new ArgumentException("Invalid credentials");
            }

            _sessionContext.AuthorizedUserId = user.Id;
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
        }

        var claims = new[] 
        { 
            new Claim(ClaimTypes.Name, name) 
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers["WWW-Authenticate"] = "Basic realm=\"ImageStorage\", charset=\"UTF-8\"";
        await base.HandleChallengeAsync(properties);
    }
}
