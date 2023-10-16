using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace ImageStorage.WebApi.Handlers;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Endpoint? endpoint = Context.GetEndpoint();

        if (endpoint != null)
        {
            IAllowAnonymous? allowAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>();

            if (allowAnonymous != null)
            {
                return AuthenticateResult.NoResult();
            }
        }

        string username = null;

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');
            username = credentials.FirstOrDefault();
            var password = credentials.LastOrDefault();

            //if (!_userService.ValidateCredentials(username, password))
            //    throw new ArgumentException("Invalid credentials");
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
        }

        var claims = new[] 
        { 
            new Claim(ClaimTypes.Name, username) 
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
