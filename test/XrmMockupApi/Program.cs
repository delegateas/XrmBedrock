using Azure.DataverseService.Foundation.Dao;
using Dataverse.Plugins;
using DG.Tools.XrmMockup;
using Microsoft.OpenApi.Models;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using XrmMockupApi.Converters;
using XrmMockupApi.Converters.SystemTextJson;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JSON options with Newtonsoft.Json
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add Newtonsoft.Json for request/response serialization
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new OrganizationRequestConverter());
    options.SerializerSettings.Converters.Add(new ParameterCollectionConverter());
    options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
});

builder.Services.AddSingleton(serviceProvider =>
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

    var settings = new XrmMockupSettings
    {
        BasePluginTypes = new Type[] { typeof(Plugin) },
        BaseCustomApiTypes = new Tuple<string, Type>[] { new("mgs", typeof(CustomAPI)) },
        EnableProxyTypes = true,
        IncludeAllWorkflows = false,
        MetadataDirectoryPath = "..\\SharedTest\\MetadataGenerated",
    };

    logger.LogInformation("Initializing XrmMockup365 with metadata path: {MetadataPath}", settings.MetadataDirectoryPath);

    return XrmMockup365.GetInstance(settings);
});

builder.Services.AddScoped(serviceProvider =>
{
    var xrm = serviceProvider.GetRequiredService<XrmMockup365>();
    var logger = serviceProvider.GetRequiredService<ILogger<DataverseAccessObjectAsync>>();
    return new DataverseAccessObjectAsync(xrm.GetAdminService(), logger);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithOpenApi();

app.MapPost("/execute", async (HttpContext context, DataverseAccessObjectAsync dao, ILogger<Program> logger) =>
{
    try
    {
        // Read the request body as string
        using var reader = new StreamReader(context.Request.Body);
        var requestBody = await reader.ReadToEndAsync();

        logger.LogInformation("Received execute request: {RequestBody}", requestBody);

        // Deserialize using custom converters
        var request = JsonConvert.DeserializeObject<OrganizationRequest>(requestBody, new JsonSerializerSettings
        {
            Converters = { new OrganizationRequestConverter(), new ParameterCollectionConverter() },
            TypeNameHandling = TypeNameHandling.Auto,
        });

        if (request == null)
        {
            return Results.BadRequest(new { Error = "Invalid request format" });
        }

        logger.LogInformation("Executing request: {RequestName}", request.RequestName);

        var response = await dao.ExecuteAsync(request);

        logger.LogInformation("Request executed successfully");

        return Results.Json(response, new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(),
                new OrganizationResponseJsonConverter(),
                new ParameterCollectionJsonConverter(),
            },
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error executing request: {Message}", ex.Message);
        return Results.BadRequest(new { Error = ex.Message, Type = ex.GetType().Name });
    }
})
.WithName("ExecuteRequest")
.WithOpenApi(operation =>
{
    operation.RequestBody = new OpenApiRequestBody
    {
        Content =
        {
            ["application/json"] = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
(StringComparer.Ordinal) {
                        ["RequestName"] = new OpenApiSchema { Type = "string" },
                        ["Parameters"] = new OpenApiSchema { Type = "object" },
                    },
                    Required = new HashSet<string>(StringComparer.Ordinal) { "RequestName" },
                },
                Example = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["RequestName"] = new Microsoft.OpenApi.Any.OpenApiString("Create"),
                    ["Parameters"] = new Microsoft.OpenApi.Any.OpenApiObject
                    {
                        ["Target"] = new Microsoft.OpenApi.Any.OpenApiObject
                        {
                            ["LogicalName"] = new Microsoft.OpenApi.Any.OpenApiString("contact"),
                            ["Attributes"] = new Microsoft.OpenApi.Any.OpenApiObject
                            {
                                ["firstname"] = new Microsoft.OpenApi.Any.OpenApiString("John"),
                                ["lastname"] = new Microsoft.OpenApi.Any.OpenApiString("Doe"),
                            },
                        },
                    },
                },
            },
        },
    };
    return operation;
});

app.MapGet("/metadata/{entityName}", async (string entityName, DataverseAccessObjectAsync dao, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Retrieving metadata for entity: {EntityName}", entityName);

        var retrieveEntityRequest = new Microsoft.Xrm.Sdk.Messages.RetrieveEntityRequest
        {
            LogicalName = entityName,
            EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.All,
        };

        var response = (Microsoft.Xrm.Sdk.Messages.RetrieveEntityResponse)await dao.ExecuteAsync(retrieveEntityRequest);

        logger.LogInformation("Retrieved metadata for entity: {EntityName}", entityName);

        // Return the raw EntityMetadata
        var jsonResponse = JsonConvert.SerializeObject(response.EntityMetadata, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        });

        return Results.Ok(jsonResponse);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error retrieving metadata for entity: {EntityName}", entityName);
        return Results.BadRequest(new { Error = ex.Message, Type = ex.GetType().Name });
    }
})
.WithName("GetEntityMetadata")
.WithOpenApi();

app.Run();