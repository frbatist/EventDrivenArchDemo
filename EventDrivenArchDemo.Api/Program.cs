using EventDrivenArchDemo.Api.Data;
using EventDrivenArchDemo.Api.Domain.Messaging;
using EventDrivenArchDemo.Api.Infra.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BookRentalShopContext>(options =>
    options.UseSqlServer(sqlConnectionString));

if (builder.Environment.IsDevelopment())
{
    builder.Services.ConfigureAll<HttpClientFactoryOptions>(options =>
    {
        options.HttpMessageHandlerBuilderActions.Add(handlerBuilder =>
        {
            if (handlerBuilder.PrimaryHandler is HttpClientHandler handler)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {                    
                    return true;
                };
            }
        });
    });
}

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

//builder.Services.AddOpenIddict()
//    .AddValidation(options =>
//    {        
//        // Get values from configuration
//        var authConfig = builder.Configuration.GetSection("Authentication:OpenIddict");
//        var issuerUrl = authConfig["IssuerUrl"] ?? "https://eventdrivenarchdemo.authentication:443";
//        var audience = authConfig["Audience"] ?? "EventDrivenArchDemo.Api";
//        var clientId = authConfig["ClientId"] ?? "EventDrivenArchDemo.Api";
//        var clientSecret = authConfig["ClientSecret"] ?? "SuperSecretClientSecret";

//        // Apply configuration
//        options.SetIssuer(issuerUrl); 
//        options.AddAudiences(audience);
//        options.UseIntrospection()
//               .SetClientId(clientId)
//               .SetClientSecret(clientSecret);
//        options.UseAspNetCore();        
//        options.UseSystemNetHttp();
//    });


var issuerUrl = builder.Configuration["AUTH_SERVER_URL"] ?? "https://localhost:5003";
var authConfig = builder.Configuration.GetSection("Authentication:OpenIddict");
var audience = authConfig["Audience"] ?? "EventDrivenArchDemo.Api";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = issuerUrl;
        options.Audience = audience;
        options.RequireHttpsMetadata = true;

        if (builder.Environment.IsDevelopment())
        {
            options.BackchannelHttpHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuerUrl,
            ValidAudience = audience,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("JWT Token validated successfully");
            var claims = context.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
            Console.WriteLine($"Claims: {string.Join(", ", claims)}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"JWT Challenge: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMessagePublisher, RabbitMqMessagePublisher>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookRentalShopContext>();
    db.Database.Migrate();
}

await RabbitMqInitializer.InitializeAsync(app.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
