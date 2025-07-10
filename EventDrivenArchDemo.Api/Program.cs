using EventDrivenArchDemo.Api.Data;
using EventDrivenArchDemo.Api.Domain.Messaging;
using EventDrivenArchDemo.Api.Infra.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BookRentalShopContext>(options =>
    options.UseSqlServer(sqlConnectionString));

// Configure certificate trust for Docker
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

var authServerUrl = builder.Configuration["AUTH_SERVER_URL"] ?? "https://eventdrivenarchdemo.authentication:443";
var authConfig = builder.Configuration.GetSection("Authentication:OpenIddict");
var clientId = authConfig["ClientId"] ?? "EventDrivenArchDemo.Api";
var clientSecret = authConfig["ClientSecret"] ?? "SuperSecretClientSecret";

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer(authServerUrl);
        options.AddAudiences(clientId);

        options.UseIntrospection()
               .SetClientId(clientId)
               .SetClientSecret(clientSecret);

        options.UseSystemNetHttp();
        options.UseAspNetCore();

        options.Configure(opt =>
        {
            Console.WriteLine("OpenIddict validation configured");
        });
    });

builder.Services.AddAuthorization();

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