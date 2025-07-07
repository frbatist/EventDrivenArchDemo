using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;

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

            // Postman OIDC client
            var clientId = config["Seed:PostmanClient:ClientId"];
            var clientSecret = config["Seed:PostmanClient:ClientSecret"];
            if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
            {
                if (await appManager.FindByClientIdAsync(clientId) == null)
                {
                    await appManager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        DisplayName = "Postman",
                        Permissions =
                        {
                            OpenIddictConstants.Permissions.Endpoints.Token,
                            OpenIddictConstants.Permissions.Endpoints.Authorization,
                            OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                            OpenIddictConstants.Permissions.ResponseTypes.Code,
                            OpenIddictConstants.Permissions.GrantTypes.Password,
                            OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                            OpenIddictConstants.Permissions.Prefixes.Scope + "openid",
                            OpenIddictConstants.Permissions.Prefixes.Scope + "profile",
                            OpenIddictConstants.Permissions.Prefixes.Scope + "email",                            
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
