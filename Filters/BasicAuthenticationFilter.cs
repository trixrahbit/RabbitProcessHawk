using System;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitProcessHawk {
public class BasicAuthenticationFilter : IAsyncAuthorizationFilter
{
    private readonly string _realm;
    private readonly IUserService _userService;

    public BasicAuthenticationFilter(string realm, IUserService userService)
    {
        _realm = realm;
        _userService = userService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
        if (AuthenticationHeaderValue.TryParse(authHeader, out var headerValue))
        {
            if (headerValue.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
            {
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(headerValue.Parameter)).Split(':');
                var username = credentials[0];
                var password = credentials[1];

                if (_userService.ValidateUser(username, password))
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, username) };
                    var identity = new ClaimsIdentity(claims, "Basic");
                    var principal = new ClaimsPrincipal(identity);
                    await context.HttpContext.SignInAsync(principal);
                    return;
                }
            }
        }

        context.HttpContext.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{_realm}\"";
        context.Result = new UnauthorizedResult();
    }
}
}