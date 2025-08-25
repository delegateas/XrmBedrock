---
description: Rules for business logic in Azure.
globs: **/src/Azure/**/*.cs
alwaysApply: false
---
# Azure business logic
Business logic in Azure is created using either Azure Function or Azure Web App with minimal API. If any logic needs to call Dataverse they do so through services defined in DataverseService.

For detailed information about interacting with Dataverse data, see [Dataverse Access Object (DAO)](dao.md).

## Azure Function

Carefully follow these instructions when implementing an Azure Function in the backend, including structure, route conventions, and usage patterns.

1. Create a function in a `[area-name]` folder that triggered by either a storage queue message, or scheduled.
2. The function should call a `Dataverse[Domain]Service` to handle the business logic.

## Minimal Api

Carefully follow these instructions when implementing minimal API endpoints in the backend, including structure, route conventions, and usage patterns.

## Implementation

1. Create API endpoint classes in the `/src/Azure/*Api/[area-name]/` directory, organized by domain.
2. Create an endpoint class with proper naming (`[Domain]Endpoints.cs`). For example `ContactEndpoints.cs`.
3. Define a constant string for `RoutesPrefix`: `/[Domain]`:
   ```csharp
   private const string RoutesPrefix = "/contacts";
   ```
4. Structure each endpoint in a single function with the definition `public static void Map[Domain]Endpoints(this IEndpointRouteBuilder app)`. Each definition should be structured exactly like this (no logic in the body):
   ```csharp
    app.MapPost($"{RoutesPrefix}/[Endpoint]", [Endpoint][Domain])
    .WithName("[Endpoint][Domain]")
    .WithOpenApi(operation =>
    {
        operation.Summary = "[Short summary here]";
        operation.Description = """
        [Description here]""";
        return operation;
    });
   ```

   The first line contains the mapping from http request through the route to a static function in the endpoint class.
   The second line defines the Open API name for this endpoint.
   The next lines contains the description of the Open API endpoint.
5. Write the handling function for the endpoint as a static function in the same endpoint class. It follows this structure:
```csharp
public static async Task<Results<[Responses]>> [Endpoint][Domain]([Endpoint][Domain]Request request, Dataverse[Domain]Service service)
{
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(service);

    var result = await service.[Endpoint][Domain]([RequestExtract]);

    return result.Match<Results<[Responses]>>(
        [MappingLogic]
        );
```

- [Responses] are the exact HttpResults that can be returned from the logic. If an object is returned, the response object should be stored in the shared domain.
- [Endpoint][Domain]Request refers to a file in the shared domain that defines the request.
- Dataverse[Domain]Service refers to a file in the DataverseService project.
    - This service returns a OneOf response that is the possible successful and errorful outputs.
- [RequestExtract] passes the relevant parameters to the service.
- [MappingLogic] maps the OneOf response to HttpResults.

It will look like this example, where

- `[Endpoint]` is `Find`
- `[Domain]` is `Contact`
- The response is `Ok<FindContactResponse>` or `NotFound<string>`

```csharp
public static async Task<Results<Ok<FindContactResponse>, NotFound<string>>> FindContact(FindContactRequest request, DataversePersonService service)
{
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(service);

    var result = await service.FindPerson(request.CitizenId);

    return result.Match<Results<Ok<FindContactResponse>, NotFound<string>>>(
        person => TypedResults.Ok(new FindContactResponse(
            person.CitizenId,
            person.ContactInfo.FirstName,
            person.ContactInfo.LastName,
            person.ContactInfo.Email,
            person.ContactInfo.Phone)),
        notFound => TypedResults.NotFound("Unable to find person using CitizenId"));
```
6. Write the business logic in the Dataverse[Domain]Service.
7. Never use IDataverseAccesObjectAsync directly in Azure/*Api services. All Dataverse operations must go through services in Azure/DataverseService.
8. Ensure that the endpoints and Dataverse Services are registered in the `Program.cs` file by calling `app.Map[Domain]Endpoints();`

This is an example of a `Program.cs` file
```csharp
using DataverseService;
using DataverseService.CustomerArea;
using Api.CustomerArea;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Add authorization with default policy requiring authentication
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDataverse();
builder.Services.AddCustomerServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapContactEndpoints();
app.Run();
```

## DataverseService

Carefully follow these instructions when implementing a Dataverse Service in the backend, including structure, route conventions, and usage patterns.

Dataverse services are organized by `[area-name]`. A folder is structured like this

- [[area-name]Area](src/Azure/DataverseService/[area-name]Area)
    - [Dto](src/Azure/DataverseService/[area-name]Area/Dto): Contains definitions for objects passed for data externally as well as internally. All files in here should be records.
    - [Errors](src/Azure/DataverseService/[area-name]Area/Dto): Contains definitions for objects passed as an error externally as well as internally. All files in here should be records.
    - [AddServices](src/Azure/DataverseService/[area-name]Area/AddServices.cs): A class that groups all services from this area into an easy to function that helps add them to a service collection. The following is an example of such a function, where `[area-name]` is `Customer` that has a single Dataverse Service called `DataversePersonService`.

    ```csharp
    public static class AddServices
    {
        public static void AddCustomerServices(this IServiceCollection services)
        {
            services.AddScoped<DataversePersonService>();
        }
    }
    ```
    - [Dataverse Services](src/Azure/DataverseService/[area-name]Area/*.cs): The other files define DataverseServices. These files contain business logic.

### Code Rules
- It is important that control flow is handled by returning OneOf objects that are composed of records from `Dto` and `Errors`. This enables strongly-typed error handling.
- Use IDataverseAccesObjectAsync for all operations towards Dataverse. See [Dataverse Access Object (DAO)](dao.md) for detailed usage patterns and best practices.
- **Internal API Trust Model**: Since these APIs are internal-only, do not perform defensive existence checks for referenced entities (contactId, situationId, employmentId, etc.). Trust that callers provide valid references and let Dataverse throw exceptions for invalid references. This simplifies code and improves performance.

## Dataverse Data Types

When creating DTOs for Dataverse entities, use the correct .NET data types:
- **Lookup fields**: Always use `Guid` (not string) for lookup references in Dataverse
- **Text fields**: Use `string`
- **Number fields**: Use appropriate numeric types (`int`, `decimal`, etc.)
- **Date fields**: Use `DateTime` or `DateTimeOffset`
- **Boolean fields**: Use `bool`