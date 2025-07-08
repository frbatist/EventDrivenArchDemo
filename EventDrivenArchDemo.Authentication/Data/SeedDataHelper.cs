using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EventDrivenArchDemo.Authentication.Data
{
    public static class SeedDataHelper
    {
        public async static Task SeedDataAsync(IServiceProvider services, IConfiguration config)
        {
            using var scope = services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var appManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            // Admin user
            var adminEmail = config["Seed:AdminUser:Email"];
            var adminPassword = config["Seed:AdminUser:Password"];
            if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
            {
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var user = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                    await userManager.CreateAsync(user, adminPassword);
                }
            }           
            
            var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

            var apiClientId = config["Seed:ApiClient:ClientId"];
            var apiClientSecret = config["Seed:ApiClient:ClientSecret"];
            if (!string.IsNullOrWhiteSpace(apiClientId) && !string.IsNullOrWhiteSpace(apiClientSecret))
            {
                // Create API scope
                if (await scopeManager.FindByNameAsync(apiClientId) == null)
                {
                    await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
                    {
                        Name = apiClientId,
                        DisplayName = apiClientId,
                        Resources = { apiClientId }
                    });
                }

                var apiClient = await appManager.FindByClientIdAsync(apiClientId);
                if (apiClient != null)
                {
                    await appManager.DeleteAsync(apiClient);
                }

                await appManager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = apiClientId,
                    ClientSecret = apiClientSecret,
                    ConsentType = ConsentTypes.Implicit,
                    DisplayName = "EventDrivenArchDemo API Client",
                    Permissions =
                    {
                        Permissions.Endpoints.Introspection
                    }
                });
            }


            // Postman OIDC client
            var postmanClientId = config["Seed:PostmanClient:ClientId"];
            var postmanClientSecret = config["Seed:PostmanClient:ClientSecret"];
            if (!string.IsNullOrWhiteSpace(postmanClientId) && !string.IsNullOrWhiteSpace(postmanClientSecret))
            {
                if (await appManager.FindByClientIdAsync(postmanClientId) == null)
                {
                    await appManager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = postmanClientId,
                        ClientSecret = postmanClientSecret,
                        DisplayName = "Postman",
                        Permissions =
                        {
                            Permissions.Endpoints.Token,
                            Permissions.Endpoints.Authorization,
                            Permissions.GrantTypes.AuthorizationCode,
                            Permissions.ResponseTypes.Code,
                            Permissions.GrantTypes.Password,
                            Permissions.GrantTypes.RefreshToken,
                            Permissions.Scopes.Email,
                            Permissions.Scopes.Profile,
                            Permissions.Prefixes.Scope + postmanClientId,
                            Permissions.Prefixes.Scope + "openid",                            
                        },
                        RedirectUris =
                        {
                            new Uri("https://www.getpostman.com/oauth2/callback"),
                            new Uri("https://oauth.pstmn.io/v1/callback"),
                            new Uri("http://localhost:5000/api/Rents"),
                        },
                    });
                }
            }
        }        
    }
}
