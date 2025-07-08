using EventDrivenArchDemo.Authentication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.AddRazorPages();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var issuerUrl = builder.Configuration["ISSUER_URL"] ?? "https://localhost:5003";

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<ApplicationDbContext>();
    })
    .AddServer(options =>
    {

        options.SetIssuer(new Uri(issuerUrl));

        //    options.SetIssuer(new Uri(issuerUrl))
        //.SetTokenEndpointUris($"{issuerUrl}/connect/token")
        //.SetUserInfoEndpointUris($"{issuerUrl}/connect/userinfo")
        //.SetAuthorizationEndpointUris($"{issuerUrl}//connect/authorize")
        //.SetIntrospectionEndpointUris($"{issuerUrl}/connect/introspect")
        //.SetRevocationEndpointUris($"{issuerUrl}/connect/revoke")
        //.SetJsonWebKeySetEndpointUris($"{issuerUrl}/.well-known/jwks");

        options.SetTokenEndpointUris("connect/token");
        options.SetUserInfoEndpointUris("connect/userinfo");
        options.SetAuthorizationEndpointUris("/connect/authorize");        
        options.SetIntrospectionEndpointUris("connect/introspect");
        options.SetRevocationEndpointUris("connect/revoke");
        options.AllowAuthorizationCodeFlow().AllowRefreshTokenFlow();        
        options.RegisterScopes("openid", "profile", "email");



        
        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough()
               .EnableAuthorizationEndpointPassthrough()
               .EnableUserInfoEndpointPassthrough()               
               .EnableStatusCodePagesIntegration();

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    await SeedDataHelper.SeedDataAsync(app.Services, app.Configuration);
}
else
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
