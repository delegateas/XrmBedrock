using Azure.Core;
using Azure.Identity;
using DataverseService.Foundation.Dao;
using DataverseService.Foundation.Logging;
using LF.Services.Dataverse.Dao;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using SharedDataverseLogic;
using SharedDomain;

namespace DataverseService;

public static class Startup
{
    public static IServiceCollection AddDataverse(this IServiceCollection services, bool enableAffinityCookie = true)
    {
        services.AddScoped<IOrganizationServiceAsync2>(provider => GetServiceClient(provider, enableAffinityCookie: enableAffinityCookie));
        services.AddScoped<IDataverseAccessObjectAsync, DataverseAccessObjectAsync>();
        services.AddScoped<IExtendedTracingService, ExtendedTracingService>();
        services.AddScoped<ILoggingComponent, LoggingComponent>();

        return services;
    }

    private static ServiceClient GetServiceClient(IServiceProvider provider, bool enableAffinityCookie = true)
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

        var logger = loggerFactory.CreateLogger("DataverseService");
        var httpContextAccessor = provider.GetService<IHttpContextAccessor>();
        var clientId = httpContextAccessor?.HttpContext?.Items["ClientId"]?.ToString();

        logger.LogTrace("Using managed identity to connect to Dataverse.");
        if (clientId != null)
            logger.LogInformation($"Client ID found: {clientId}");

        var dataverseUrl = configuration.GetValue<string>("DataverseUrl");
        var cache = provider.GetService<IMemoryCache>();

        var client = new ServiceClient(instanceUrl: new Uri(dataverseUrl), tokenProviderFunction: url => TokenProviderFunction(url, cache, clientId, logger));
        client.EnableAffinityCookie = enableAffinityCookie;
        return client;
    }

    private static async Task<string> TokenProviderFunction(string dataverseUrl, IMemoryCache cache, string? clientId, ILogger logger)
    {
        var cacheKey = $"AccessToken_{dataverseUrl}";
        if (!string.IsNullOrEmpty(clientId))
        {
            cacheKey += $"_{clientId}";
        }

        logger.LogTrace($"Attempting to retrieve access token for {dataverseUrl} with clientId {clientId}");

        return (await cache.GetOrCreateAsync(cacheKey, async cacheEntry =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(50);
            var credential = GetTokenCredential(clientId, logger);
            var scope = BuildScopeString(dataverseUrl);

            return await FetchAccessToken(credential, scope, logger);
        })).Token;
    }

    private static DefaultAzureCredential GetTokenCredential(string? clientId, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            logger.LogInformation("Using Default Managed Identity");
            return new DefaultAzureCredential();  // in azure this will be managed identity, locally this depends... se midway of this post for the how local identity is chosen: https://dreamingincrm.com/2021/11/16/connecting-to-dataverse-from-function-app-using-managed-identity/
        }

        logger.LogTrace($"Using Managed Identity with Client ID: {clientId}");
        return new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            ManagedIdentityClientId = clientId,
        });
    }

    private static string BuildScopeString(string dataverseUrl)
    {
        return $"{GetCoreUrl(dataverseUrl)}/.default";
    }

    private static async Task<AccessToken> FetchAccessToken(TokenCredential credential, string scope, ILogger logger)
    {
        var tokenRequestContext = new TokenRequestContext(new[] { scope });

        try
        {
            logger.LogTrace("Requesting access token...");
            var accessToken = await credential.GetTokenAsync(tokenRequestContext, CancellationToken.None);
            logger.LogTrace("Access token successfully retrieved.");
            return accessToken;
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to retrieve access token: {ex.Message}");
            throw;
        }
    }

    private static string GetCoreUrl(string url)
    {
        var uri = new Uri(url);
        return $"{uri.Scheme}://{uri.Host}";
    }
}