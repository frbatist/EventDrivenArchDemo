using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Server;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EventDrivenArchDemo.Authentication.Controllers
{
    [ApiController]
    public class DebugController : ControllerBase
    {
        [HttpGet("~/debug/openiddict")]        
        public IActionResult TestOpenIddict()
        {
            try
            {
                var request = HttpContext.GetOpenIddictServerRequest();
                return Ok(new
                {
                    HasRequest = request != null,
                    RequestType = request?.GetType().Name,
                    ClientId = request?.ClientId,
                    ResponseType = request?.ResponseType,
                    Scope = request?.Scope,
                    RedirectUri = request?.RedirectUri,
                    RequestPath = Request.Path.Value,
                    QueryString = Request.QueryString.Value,
                    AllQueryParams = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString())
                });
            }
            catch (Exception ex)
            {
                return Ok(new { Error = ex.Message, HasOpenIddict = false });
            }
        }

        [HttpGet("~/debug/config")]
        public IActionResult DebugConfig(IServiceProvider serviceProvider)
        {
            try
            {
                var options = serviceProvider.GetRequiredService<IOptionsMonitor<OpenIddictServerOptions>>().CurrentValue;

                return Ok(new
                {
                    AuthorizationEndpoint = options.AuthorizationEndpointUris?.FirstOrDefault()?.ToString(),
                    TokenEndpoint = options.TokenEndpointUris?.FirstOrDefault()?.ToString(),
                    Issuer = options.Issuer?.ToString(),
                    EnabledFlows = new
                    {
                        AuthorizationCode = options.GrantTypes.Contains(GrantTypes.AuthorizationCode),
                        RefreshToken = options.GrantTypes.Contains(GrantTypes.RefreshToken)
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new { Error = ex.Message });
            }
        }

        [HttpGet("~/debug/routes")]
        public IActionResult DebugRoutes()
        {
            var endpointDataSource = HttpContext.RequestServices.GetRequiredService<EndpointDataSource>();
            var routes = endpointDataSource.Endpoints
                .OfType<RouteEndpoint>()
                .Select(e => new
                {
                    Pattern = e.RoutePattern?.RawText,
                    Methods = e.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods,
                    Name = e.DisplayName
                })
                .Where(r => r.Pattern != null)
                .OrderBy(r => r.Pattern)
                .ToList();

            return Ok(routes);
        }
    }
}
