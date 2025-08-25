---
description: Rules for working with Dataverse Access Object (DAO) in Azure and Plugins.
globs: **/src/Azure/**/*.cs, **/src/Dataverse/**/*.cs
alwaysApply: false
---

# Dataverse Access Object (DAO)

The Dataverse Access Object (DAO) provides a service layer for interacting with Dataverse entities. It abstracts the complexity of the underlying Dataverse SDK and provides both synchronous and asynchronous methods for data operations.

This documentation is relevant to:
- [DataverseService](azure.md) in Azure Functions and Web APIs
- [Plugin business logic](plugin.md) in Dataverse plugins

## DAO Versions

AdminDao has both sync and async versions of most endpoints:

- **Use async versions in Azure** (Azure Functions, Web APIs): `IDataverseAccessObjectAsync adminDao`
- **Use sync versions in plugins**: `IAdminDataverseAccessObjectService adminDao`

The variable is typically named `adminDao` in both contexts.

## Common Operations

### 1. Retrieve Operations

#### Retrieve List with Filtering and Selection
Use LINQ expressions to filter and select specific fields:

```csharp
// Retrieve name and description for accounts that have a name and are active
// Azure (async version)
var data = await adminDao.RetrieveListAsync(xrm =>
    xrm.AccountSet
    .Where(x => x.Name != null &&
        x.statecode == AccountState.Active)
    .Select(x => new { x.Name, x.Description }))

// Plugin (sync version)
var data = adminDao.RetrieveList(xrm =>
    xrm.AccountSet
    .Where(x => x.Name != null &&
        x.statecode == AccountState.Active)
    .Select(x => new { x.Name, x.Description }))
```

#### Retrieve Multiple by IDs
Efficiently retrieve multiple records by their IDs:

```csharp
// Retrieve accounts by a collection of ids
adminDao.RetrieveMultipleByIds<Account>(accountids)
```

#### Retrieve with Joins
Perform complex queries that join multiple tables:

```csharp
// Retrieve a list with a query that joins multiple tables
var subscriptionTransactions = adminDao.RetrieveList(xrm =>
    from transaction in xrm.demo_TransactionSet
    join subscription in xrm.demo_SubscriptionSet on transaction.demo_Regarding.Id equals subscription.Id
    where subscription.demo_customer.Id == customerId
    where subscription.statecode == demo_SubscriptionState.Active
    where transaction.statecode == demo_TransactionState.Active
    select transaction);
```

#### Retrieve Single Record with Specific Fields
Retrieve a single record with only the fields you need:

```csharp
// Azure (async version)
var invoice = await adminDao.RetrieveAsync<demo_Invoice>(
    invoiceId,
    x => x.demo_InvoiceDate,
    x => x.demo_Paymentagreement);

// Plugin (sync version)
var invoice = adminDao.Retrieve<demo_Invoice>(
    invoiceId,
    x => x.demo_InvoiceDate,
    x => x.demo_Paymentagreement);
```

### 2. Update Operations

Update existing records by creating a new entity instance with the ID and changed fields:

```csharp
// Azure (async version)
await adminDao.UpdateAsync(new demo_Invoice(invoiceId)
{
    statuscode = demo_Invoice_statuscode.ReadyForDelivery,
});

// Plugin (sync version)
adminDao.Update(new demo_Invoice(invoiceId)
{
    statuscode = demo_Invoice_statuscode.ReadyForDelivery,
});
```

### 3. Create Operations

Create new records and get the generated ID:

```csharp
// Azure (async version)
var invoiceId = await adminDao.CreateAsync(new demo_Invoice()
{
    demo_InvoiceNumber = "12345678",
});

// Plugin (sync version)
var invoiceId = adminDao.Create(new demo_Invoice()
{
    demo_InvoiceNumber = "12345678",
});
```

### 4. Delete Operations

```csharp
// Azure (async version)
await adminDao.DeleteAsync<demo_Invoice>(invoiceId);

// Plugin (sync version)
adminDao.Delete<demo_Invoice>(invoiceId);
```

### 5. Execute Operations

Execute Dataverse requests directly:

```csharp
// Azure (async version)
await adminDao.ExecuteAsync(request);

// Plugin (sync version)
adminDao.Execute(request);
```

## Best Practices

### 1. Dependency Injection

#### In Azure Services
Inject the async version in constructor:

```csharp
public class DataverseDomainService
{
    private readonly IDataverseAccessObjectAsync adminDao;

    public DataverseDomainService(IDataverseAccessObjectAsync adminDao)
    {
        this.adminDao = adminDao;
    }
}
```

#### In Plugin Services
Inject the sync version in constructor:

```csharp
public class PluginDomainService
{
    private readonly IAdminDataverseAccessObjectService adminDao;

    public PluginDomainService(IAdminDataverseAccessObjectService adminDao)
    {
        this.adminDao = adminDao;
    }
}
```

### 2. Field Selection

Always specify which fields you need to minimize data transfer and improve performance:

```csharp
// Good - specify only needed fields
var contact = await adminDao.Retrieve<Contact>(
    contactId,
    x => x.FirstName,
    x => x.LastName,
    x => x.EmailAddress1);

// Avoid - retrieving all fields
var contact = await adminDao.Retrieve<Contact>(contactId);
```

### 3. Filtering

Use strongly-typed LINQ expressions for filtering:

```csharp
// Good - strongly typed filtering
adminDao.RetrieveList(xrm =>
    xrm.ContactSet
    .Where(x => x.StateCode == ContactState.Active)
    .Where(x => x.EmailAddress1 != null))

// Avoid - string-based filtering in higher-level methods
```

### 4. Performance Considerations

- Use `RetrieveList` for bulk retrieval instead of multiple `Retrieve` calls
- Specify only the fields you need in retrieve operations
- Use appropriate filtering to limit result sets
- Pagination is handled automatically by dao.

## Common Patterns

### Conditional Logic Based on Existing Data
```csharp
// Check existing state before making changes
var existingInvoice = await adminDao.Retrieve<demo_Invoice>(
    invoiceId,
    x => x.statuscode,
    x => x.demo_InvoiceDate);

if (existingInvoice.statuscode != demo_Invoice_statuscode.ReadyForDelivery)
{
    await adminDao.UpdateAsync(new demo_Invoice(invoiceId)
    {
        statuscode = demo_Invoice_statuscode.ReadyForDelivery,
    });
}
```

### Retrieving Related Entity Names
When you need to retrieve entity references with their names, Dataverse has limitations on complex joins. Use this pattern instead:

```csharp
// ❌ DON'T: Complex joins are not supported in Dataverse
var data = await adminDao.RetrieveFirstOrDefaultAsync(xrm =>
    from memberSituation in xrm.msys_MemberSituationSet
    join situation in xrm.msys_SituationSet on memberSituation.msys_Situation.Id equals situation.Id
    join education in xrm.msys_EducationSet on memberSituation.msys_Education.Id equals education.Id into educationJoin
    from education in educationJoin.DefaultIfEmpty()
    // This type of complex join with multiple left joins is NOT supported
    where memberSituation.Id == memberSituationId
    select new { ... });

// ✅ DO: Retrieve references first, then get names individually
private async Task<dynamic?> GetMemberSituationWithRelatedEntitiesAsync(Guid memberSituationId)
{
    // 1. First retrieve the main entity with its reference fields
    var memberSituation = await dao.RetrieveFirstOrDefaultAsync<msys_MemberSituation>(
        memberSituationId,
        x => x.msys_Name,
        x => x.msys_StartDate,
        x => x.msys_EndDate,
        x => x.statuscode,
        x => x.msys_Situation,      // Reference field
        x => x.msys_Education,      // Reference field
        x => x.msys_Employment,     // Reference field  
        x => x.msys_PaymentMethod); // Reference field

    if (memberSituation == null)
        return null;

    // 2. Then retrieve different fields for each referenced entity individually
    string? situationName = null;
    if (memberSituation.msys_Situation?.Id != null)
    {
        var situation = await dao.RetrieveAsync<msys_Situation>(
            memberSituation.msys_Situation.Id, x => x.msys_Name);
        situationName = situation.msys_Name;
    }

    string? educationInfo = null;
    if (memberSituation.msys_Education?.Id != null)
    {
        var education = await dao.RetrieveAsync<msys_Education>(
            memberSituation.msys_Education.Id, x => x.msys_Description, x => x.msys_Level);
        educationInfo = $"{education.msys_Description} (Level: {education.msys_Level})";
    }

    string? employmentTitle = null;
    if (memberSituation.msys_Employment?.Id != null)
    {
        var employment = await dao.RetrieveAsync<msys_Employment>(
            memberSituation.msys_Employment.Id, x => x.msys_JobTitle, x => x.msys_Department);
        employmentTitle = employment.msys_JobTitle;
    }

    string? paymentMethodType = null;
    if (memberSituation.msys_PaymentMethod?.Id != null)
    {
        var paymentMethod = await dao.RetrieveAsync<msys_PaymentMethod>(
            memberSituation.msys_PaymentMethod.Id, x => x.msys_Type, x => x.msys_AccountNumber);
        paymentMethodType = paymentMethod.msys_Type;
    }
    
    return new
    {
        memberSituation.Id,
        memberSituation.msys_Name,
        memberSituation.msys_StartDate,
        memberSituation.msys_EndDate,
        memberSituation.statuscode,
        memberSituation.msys_Situation,
        SituationName = situationName,
        memberSituation.msys_Education,
        EducationInfo = educationInfo,
        memberSituation.msys_Employment,
        EmploymentTitle = employmentTitle,
        memberSituation.msys_PaymentMethod,
        PaymentMethodType = paymentMethodType,
    };
}
```

**Key Points:**
- Use `RetrieveAsync` (not `RetrieveFirstOrDefaultAsync`) when you know the entity exists from a reference
- Retrieve the main entity first to get all reference fields
- Then make individual calls to get relevant fields for each referenced entity
- **Retrieve different fields based on entity type**: Don't just get `msys_Name` for everything - get the fields that matter for each entity:
  - Education: Description, Level, Institution, etc.
  - Employment: JobTitle, Department, Salary, etc.  
  - PaymentMethod: Type, AccountNumber, Bank, etc.
  - Contact: FirstName, LastName, Email, Phone, etc.
- You can retrieve multiple fields per entity in a single call for efficiency
- Always check if reference IDs are not null before attempting to retrieve them