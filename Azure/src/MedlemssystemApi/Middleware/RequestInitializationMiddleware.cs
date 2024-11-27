using Newtonsoft.Json;

namespace MedlemssystemApi.Middleware
{
    public class RequestInitializationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<RequestInitializationMiddleware> logger;

        public RequestInitializationMiddleware(RequestDelegate next, ILogger<RequestInitializationMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public Task InvokeAsync(HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            logger.LogInformation("InvokeAsync method started.");

            // Check if the request is from Swagger UI
            bool isFromSwagger = context.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase) ||
                                 context.Request.Headers["Referer"].ToString().Contains("/swagger", StringComparison.OrdinalIgnoreCase);

            var appRegistrationClientId = context.User.Claims.FirstOrDefault(c => c.Type == "azp")?.Value
                        ?? context.User.Claims.FirstOrDefault(c => c.Type == "appid")?.Value;

            // Validate appRegistrationClientId
            if (string.IsNullOrEmpty(appRegistrationClientId))
            {
                return HandleMissingClientId(context, isFromSwagger);
            }

            // Load managed identities and validate
            var managedIdentitiesAsKeyValue = GetManagedIdentities();
            if (!managedIdentitiesAsKeyValue.TryGetValue(appRegistrationClientId, out var managedIdentityClientId))
            {
                throw LogAndThrowInvalidOperationException($"No managed identity found for app registration client ID: {appRegistrationClientId}.");
            }

            // Add the managed identity client ID to the HttpContext
            logger.LogInformation($"Client ID: {managedIdentityClientId} detected and added to HttpContext.");
            context.Items["ClientId"] = managedIdentityClientId;

            logger.LogInformation("InvokeAsync: Middleware execution finished.");
            return next(context);
        }

        private Task HandleMissingClientId(HttpContext context, bool isFromSwagger)
        {
            if (isFromSwagger)
            {
                logger.LogInformation("Request is coming from Swagger UI with a null or empty App Registration Client ID.");
                return next(context);
            }

            throw LogAndThrowInvalidOperationException("App registration client ID is null or empty, and the request is not from Swagger UI.");
        }

        private Dictionary<string, string> GetManagedIdentities()
        {
            var managedIdentitiesJson = Environment.GetEnvironmentVariable("MANAGED_IDENTITIES_FOR_EXTERNAL");
            if (string.IsNullOrEmpty(managedIdentitiesJson))
            {
                throw LogAndThrowInvalidOperationException("Managed identities environment variable is not set or empty.");
            }

            var managedIdentitiesAsKeyValue = JsonConvert.DeserializeObject<Dictionary<string, string>>(managedIdentitiesJson);
            if (managedIdentitiesAsKeyValue == null)
            {
                throw LogAndThrowInvalidOperationException("Failed to deserialize managed identities JSON.");
            }

            return managedIdentitiesAsKeyValue;
        }

        private InvalidOperationException LogAndThrowInvalidOperationException(string message)
        {
            logger.LogError(message);
            return new InvalidOperationException(message);
        }
    }
}