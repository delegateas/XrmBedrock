---
description: Rules for business logic in Dataverse Plugins.
globs: **/src/Dataverse/SharedPluginLogic/**/*.cs
alwaysApply: false
---

# Dataverse business logic
Business logic in Dataverse is split into the [Registrations](src/Dataverse/SharedPluginLogic/Registrations) and the [Business Logic](src/Dataverse/SharedPluginLogic/Logic). The code here is used as part of the Dataverse Execution Pipeline.

For detailed information about interacting with Dataverse data, see [Dataverse Access Object (DAO)](dao.md).

## Dataverse Execution Pipeline

Dataverse processes business logic through a pipeline that executes in stages around the main database operation. We can inject business logic between these stages using plugins or custom api's.

1. **PreValidation**: Plugins can be defined here. Occurs before security checks and data validation. Used for data manipulation related to security or validation logic. This step is not part of the database transaction, so it is cheap to throw exceptions here.
2. **PreOperation**: Plugins can be defined here. Executes after validation but before the main database operation. Ideal for modifying data before it's committed.
3. **MainOperation**: Custom Api's can be defined here. The core database operation (Create, Update, Delete).
4. **PostOperation**: Plugins can be defined here. Runs after the main operation completes successfully. Used for operations that depend on the completed transaction, such as creating related records or external integrations.

Each stage can access different types of data images (PreImage, PostImage) to compare before and after states of the entity.

## Plugin Images and Performance Optimization

Plugin images are snapshots of entity data at specific points in the execution pipeline. Using images correctly can significantly improve performance by reducing database queries.

### PostImage Usage Pattern

- **PostOperation plugins**: Use `context.GetRequiredPostImage<T>()` to get the complete record state after the database operation
- **Performance benefit**: Eliminates redundant `userDao.Retrieve()` calls for the same record
- **Critical requirement**: Always register ONLY the specific attributes needed in the PostImage using `.AddImage()` - never retrieve all attributes
- **Best practice**: Combine Target (what changed) with PostImage (complete record with registered attributes)

Example pattern:
```csharp
// In the service method
var target = context.GetTarget<Account>();
var fullRecord = context.GetRequiredPostImage<Account>();

// Use target for what changed, fullRecord for complete state
if (target.statecode == AccountState.Active && 
    SomeCondition(fullRecord))
{
    // Process using fullRecord data without additional fetches
}
```

### Image Type Guidelines

- **PreImage**: Contains entity state before the operation (available in PreOperation and PostOperation)
- **PostImage**: Contains entity state after the operation (only available in PostOperation)
- **Both**: Provides both states for comparison (only when needed)

## Registrations

Carefully follow these instructions when implementing a plugin registration or custom api registration for Dataverse in the backend, including structure, route conventions, and usage patterns.

All code must be grouped by area and domain

- [[area-name]Area](src/Dataverse/SharedPluginLogic/Registrations/[area-name]Area).
    - [[Domain]Area](src/Dataverse/SharedPluginLogic/Registrations/[area-name]Area/[Domain]Area):
        This folder contains any registrations defined under this domain.

### Plugins

Plugins are registered in the constructor of a class. Each registration is structured as follows.

```csharp
RegisterPluginStep<[Table]>(
    EventOperation.[Operation],
    ExecutionStage.[Stage],
    provider => provider.GetRequiredService<[Service]>().[Function]())
    .AddFilteredAttributes([AttributeFilters])
    .AddImage(ImageType.[ImageType], [AttributeFilters]);
```

- [Table] is a reference to a table from the shared context.
- [Operation] is the main operation the plugin is registered on.
- [Stage] is the stage in execution pipeline the plugin is registered on.
- [Service] is a service from the [Business Logic](src/Dataverse/SharedPluginLogic/Logic)
- [Function] is the function that should be called on the service when the plugin is executed.
- [AttributeFilters] fills a param argument that uses a lambda function to define, which attributes are relevant. Each argument should look like `x => x.[attribute]` where [attribute] is the relevant attribute.
- [ImageType] is a type of plugin image. `PreImage`, `PostImage`, or `Both`.


This is an example of a plugin registration class.

```csharp
/// <summary>
/// This plugin is intended to create the primary Contact for an active Account
/// </summary>
public class CreatePrimaryContact : Plugin
{
    public CreatePrimaryContact()
    {
        RegisterPluginStep<Account>(
                EventOperation.Create,
                ExecutionStage.PostOperation,
                provider => provider.GetRequiredService<AccountService>().CreatePrimaryContact());

        RegisterPluginStep<Account>(
                EventOperation.Update,
                ExecutionStage.PreOperation,
                provider => provider.GetRequiredService<AccountService>().CreatePrimaryContact())
                .AddFilteredAttributes(
                    x => x.Statecode)
                .AddImage(
                    ImageType.PreImage,
                    x => x.Name); // Register only needed attributes to avoid additional fetches
    }
}
```

### Custom Api

Custom Api's are registered in the constructor of a class. Each registration is structured as follows.

```csharp
RegisterCustomAPI("[ApiName]", x => x.GetRequiredService<[Service]>().[Function]())
    .AddRequestParameter(new CustomAPIConfig.CustomAPIRequestParameter([ParamName], RequestParameterType.[ParamType]))
    .AddResponseProperty(new CustomAPIConfig.CustomAPIResponseProperty([ParamName], RequestParameterType.[ParamType]));
```

- [ApiName] is the name of the custom api.
- [Service] is a service from the [Business Logic](src/Dataverse/SharedPluginLogic/Logic)
- [Function] is the function that should be called on the service when the custom api is executed.
- [ParamName] is the name of the request or response parameter.
- [ParamType] is the type of the request or response parameter.

This is an example of a Custom Api class

```csharp
/// <summary>
/// This Api creates an invoice for an event registration.
/// </summary>
public class CreateInvoiceForRegistration : CustomAPI
{
    public CreateInvoiceForRegistration()
        : base(typeof(CreateInvoiceForRegistration))
    {
        RegisterCustomAPI("CreateInvoiceForRegistration", x => x.GetRequiredService<InvoiceService>().CreateInvoiceForRegistration())
            .AddRequestParameter(new CustomAPIConfig.CustomAPIRequestParameter("RegistrationId", RequestParameterType.Guid))
            .AddResponseProperty(new CustomAPIConfig.CustomAPIResponseProperty("InvoiceId", RequestParameterType.Guid));
    }
}
```

## Business Logic

Carefully follow these instructions when implementing the services used for the Dataverse backend, including structure, route conventions, and usage patterns.

All code must be grouped by area and domain like this

- [[area-name]Area](src/Dataverse/SharedPluginLogic/Logic/[area-name]Area).
    - [[Domain]Area](src/Dataverse/SharedPluginLogic/Logic/[area-name]Area/[Domain]Area): Contains all Dtos and services that are exclusively used in this domain.
        - [[Domain]Service.cs](src/Dataverse/SharedPluginLogic/Logic/[area-name]Area/[Domain]Area/[Domain]Service.cs): Defines the entry point service for this domain. This is called by plugins or custom api's.

### Code Rules
- Retrieve the target from the request using the `IPluginExecutionContext` interface.
- All operations to Dataverse that should be done on behalf of the user should be done through the `IUserDataverseAccessObjectService` interface.
- All operations to Dataverse that should be done by an admin user should be done through the `IAdminDataverseAccessObjectService` interface. See [Dataverse Access Object (DAO)](dao.md) for detailed usage patterns and best practices.
- All logging is done through the `ITracingService` interface.
- In `PreValidation` and `PreOperation` operations, all mutations to the record in the operation should be done on the Target instead of a seperate operation. The purpose is to reduce the amount of api calls towards Dataverse. 
- Storage Queue Messages can easily be sent by calling the specified message on `IAzureService`.
- Avoid overly defensive code, including unnecessary null checks for plugin context Target entities. Do **not** check if the Target is `null` when using `context.GetTarget<T>()` in plugin services. The framework guarantees the presence of the Target for the registered entity and operation.
- Assume that InitiatingUserId on the Plugin Context is a valid user that exists in Dataverse.
- In PostOperation plugins, use `context.GetRequiredPostImage<T>()` instead of fetching the record with `userDao.Retrieve()` when you need the complete entity state. Always register only the required attributes in the PostImage for optimal performance and minimal data transfer.

### Namespace and Using Rules
- Ensure namespaces follow the folder structure exactly. For example, a service in `Logic/CustomerArea/ContactArea/ContactService.cs` must have the namespace `DataverseLogic.CustomerArea.ContactArea`.
- Plugins in `Registrations/CustomerArea/ContactArea/ContactCreationPlugin.cs` must have the namespace `DataverseRegistration.CustomerArea.ContactArea`.

### Project File Registration
- Whenever you add new `.cs` files to the SharedPluginLogic project, you must also add them to `SharedPluginLogic.projitems`. This ensures the files are included in the shared project and are available for all consuming projects.
- Before committing, verify that all new or moved files are present in `SharedPluginLogic.projitems`.

### Service Registration
- Every new service class (e.g., `ContactService`) must be registered in `AddServices.cs` in the `SharedDataverseLogic` project. This is required for dependency injection to work in plugins and custom APIs.
- Before committing, verify that all new services are registered in `AddServices.cs`.
