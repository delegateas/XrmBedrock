using System.IdentityModel.Tokens.Jwt;
using Azure.Identity;
using DataverseService;
using DataverseService.CustomerArea;
using ExternalApi.CustomerArea;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            policy
                .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    });
});

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(
        options =>
        {
            builder.Configuration.Bind("AzureAd", options);
            options.TokenValidationParameters.NameClaimType = "name";
            options.TokenValidationParameters.RoleClaimType = "roles";
        },
        options =>
        {
            builder.Configuration.Bind("AzureAd", options);
        });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("ClientCredentialsOnly", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("appid")
              .RequireClaim("idtyp", "app"));

    options.AddPolicy("InteractiveUsers", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("scp"));

    options.AddPolicy("ApiAccess", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context =>
                  context.User.HasClaim(c => c.Type == "appid") ||
                  context.User.HasClaim(c => c.Type == "scp")));

    options.FallbackPolicy = builder.Environment.IsDevelopment()
        ? null
        : new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
                context.User.HasClaim(c => c.Type == "appid") ||
                context.User.HasClaim(c => c.Type == "scp"))
            .Build();
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "External API",
        Version = "v1",
        Description = "External API for accessing membership data",
    });

    if (!builder.Environment.IsDevelopment())
    {
        var tenantId = builder.Configuration["AzureAd:TenantId"];
        var clientId = builder.Configuration["AzureAd:ClientId"];

        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize"),
                    TokenUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token"),
                    Scopes = new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        { $"api://{clientId}/access_as_user", "Access API as user" },
                    },
                },
            },
            Description = "OAuth2 authentication using Authorization Code flow with PKCE",
        });

        c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("oauth2"),
                [$"api://{clientId}/access_as_user"]
            },
        });
    }
});

builder.Services.AddHealthChecks();

builder.Services.AddDataverse(
    builder.Environment.IsDevelopment()
        ? new AzureCliCredential()
        : new ManagedIdentityCredential(Environment.GetEnvironmentVariable("AZURE_CLIENT_ID")));

builder.Services.AddCustomerServices();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("DevelopmentPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok("External API is running")).AllowAnonymous();

app.MapHealthChecks("/health").AllowAnonymous();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "External API v1");

    c.OAuthClientId(builder.Configuration["SwaggerAd:ClientId"]);
    c.OAuthUsePkce();

    c.OAuthAppName("External API - Swagger UI");
    c.OAuthScopeSeparator(" ");

    c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");

    c.DisplayOperationId();
    c.DisplayRequestDuration();
});

app.MapSubscriptionEndpoints();

app.Run();
