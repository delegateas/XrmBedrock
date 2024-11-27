using DataverseService;
using DataverseService.ActivityArea;
using DataverseService.CustomerArea;
using DataverseService.Foundation.Dao;
using DataverseService.UtilityArea;
using MedlemssystemApi.BusinessLogic;
using MedlemssystemApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharedContext.Dao;
using SharedDataverseLogic.ActivityArea;
using SharedDomain;

namespace MedlemssystemApi;

internal static class Program
{
#pragma warning disable MA0051 // Method is too long
    private static void Main(string[] args)
#pragma warning restore MA0051 // Method is too long
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the DI container
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddLogging();
        builder.Services.AddApplicationInsightsTelemetry();

        // Needs a cache for the token provider
        builder.Services.AddScoped<IMemoryCache, MemoryCache>();

        // Dataverse core
        builder.Services.AddDataverse();
        builder.Services.AddHttpContextAccessor();

        // Dataverse Services (keep sorted)
        builder.Services.AddScoped<IAdminDataverseAccessObjectService, AdminDataverseAccessObjectService>();
        builder.Services.AddScoped<IDataverseAccountService, DataverseAccountService>();
        builder.Services.AddScoped<IDataverseActivityService, DataverseActivityService>();
        builder.Services.AddScoped<IDataverseImageService, DataverseImageService>();
        builder.Services.AddScoped<IDataverseNotificationService, DataverseNotificationService>();
        builder.Services.AddScoped<ILoggingComponent, LoggingComponent>();
        builder.Services.AddScoped<ISharedDataverseActivityService, SharedDataverseActivityService>();

        // BusinessLogic (keep sorted)
        builder.Services.AddScoped<IAccountBusinessLogic, AccountBusinessLogic>();
        builder.Services.AddScoped<IActivityBusinessLogic, ActivityBusinessLogic>();

        // Fetch tenantId and clientId from DataverseUtilityService
        var tenantId = "TODO: Dont get from Dataverse as configuration.msys_AzureTenantId";
        var clientId = "TODO: Dont get from Dataverse as configuration.msys_AzureClientId";
        var clientSecret = Environment.GetEnvironmentVariable("WEBAPP_CLIENT_SECRET"); // TODO: Do we really need this?

        // Swagger configuration with OAuth2 using Authorization Code Flow and client secret
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

            // Define OAuth2 Authorization Code Flow (without PKCE)
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Description = "OAuth 2.0 Authorization Code Flow (with client secret)",
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize"),
                        TokenUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token"),
                        Scopes = new Dictionary<string, string>(StringComparer.Ordinal)
                        {
                                { $"api://{clientId}/access_the_api/.default", "Access API" },
                        },
                    },
                },
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2",
                            },
                            Scheme = "oauth2",
                            Name = "oauth2",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    },
            });
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
            options.Audience = $"api://{clientId}"; // Should match your app registration’s audience
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = $"https://sts.windows.net/{tenantId}/",
                ValidateAudience = true,
                ValidAudience = $"api://{clientId}/access_the_api",
                ValidateLifetime = true,
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine("Token validated successfully.");
                    return Task.CompletedTask;
                },
            };
        });

        // Build the app after adding Swagger configuration
        var app = builder.Build();

        // Ensure authentication and authorization middleware are in place
        app.UseAuthentication();
        app.UseAuthorization();

        // Enable Swagger UI and Swagger endpoint in all environments
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "MedlemssystemApi V1");
            c.OAuthClientId(clientId);
            c.OAuthClientSecret(clientSecret);
            c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
        });

        app.UseMiddleware<RequestInitializationMiddleware>();

        // Basic middleware for all environments
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Map controllers and APIs
        app.MapControllers();

        // Run the app
        app.Run();

    }
}