---
description: Rules for testing Dataverse and Azure logic
globs: **/test/IntegrationTests/**/*.cs
alwaysApply: false
---


# Integration Test Rules

## Test Implementation Guidelines

1. **Never perform visual tests through the browser**. Always write automated tests instead.

2. **Testing Minimal API endpoints**: Test the static functions directly, not through WebApplicationFactory. Call the endpoint static functions with appropriate parameters and verify the results.

3. **Test data creation**: Always use or create Producer functions for test data creation. Never create test data manually in tests. If a Producer function doesn't exist for the data you need, create one first.

4. **Test structure**: Follow the Arrange-Act-Assert pattern with clear separation of concerns.

## Test Base Class Usage

All integration tests must inherit from `TestBase` which provides:
- Access to `Xrm` (XrmMockup365 instance)
- Access to `AdminDao` (IDataverseAccessObjectAsync for database operations)
- Access to `Producer` (DataProducer for creating test data)
- Access to `MessageExecutor` (for message execution)
- Access to `Server` (WireMockServer for external API mocking)
- Automatic cleanup and snapshot restoration after each test

Test classes should use the `[Collection("Xrm Collection")]` attribute and inherit from `TestBase(XrmMockupFixture fixture)` using primary constructor syntax.

## Web API Testing Patterns

### Structure
- Tests should be organized in folders matching the API structure (e.g., `*Api/TestPersonEndpoints/`)
- Each endpoint should have comprehensive test coverage for success and failure scenarios
- Use descriptive test method names that clearly indicate the scenario being tested

### Testing HTTP Endpoints
When testing web API endpoints:
1. Call the static endpoint methods directly with request objects and service dependencies
2. Assert on the HTTP result types (Ok<T>, NotFound<T>, etc.) using FluentAssertions
3. Verify response data matches expected values
4. Test both success and error scenarios
5. Verify database state changes when applicable

Example pattern:
```csharp
[Fact]
public async Task FindPerson_WithCitizenId()
{
    // Arrange
    var contact = Producer.ProduceValidContact(new Contact() { /* properties */ });
    
    // Act  
    var response = await ContactEndpoints.FindContact(new FindContactRequest(citizenId, null), personService);
    
    // Assert
    response.Result.Should().BeOfType<Ok<FindContactResponse>>()
        .Which.Value.Should().BeEquivalentTo(expectedResponse);
}
```

### Service Dependencies
- Create required services (e.g., DataverseContactService) in test constructor using AdminDao
- Pass service instances to endpoint methods as parameters

## Dataverse Plugin Testing Patterns

### Testing Plugin Logic
Plugin tests focus on business logic execution rather than plugin registration:
1. Create test data using Producer methods
2. Perform the operation that triggers the plugin logic (Create, Update, etc.)
3. Assert on the resulting state changes in related entities
4. Test edge cases and validation scenarios

Example pattern:
```csharp
[Fact]
public void PrimaryEmployment_Create_NoPrimaryExists()
{
    // Arrange
    var employee = Producer.ProduceValidContact(null);
    
    // Act
    var employment = Producer.ProduceValidEmployment(new msys_Employment
    {
        msys_Employee = employee.ToEntityReference(),
    });
    
    // Assert
    employee = AdminDao.Retrieve<Contact>(employee.Id, x => x.msys_PrimaryEmployment);
    employee.msys_PrimaryEmployment.Id.Should().Be(employment.Id);
}
```

## Custom API Testing Patterns

### Testing Custom Actions
For testing custom APIs (actions):
1. Use `AdminDao.Execute()` with `OrganizationRequest` to call the custom action
2. Set up required test data using Producer methods
3. Pass parameters in the `Parameters` collection
4. Assert on the `Results` collection, which contains the response paramters of the custom api

Example pattern:
```csharp
[Fact]
public void CreateInvoiceForRegistration_ValidRegistration_ReturnsSuccess()
{
    // Arrange
    var registration = Producer.ProduceValidRegistration(/* setup */);
    
    // Act
    var response = AdminDao.Execute(new OrganizationRequest("demo_CreateInvoiceForRegistration")
    {
        Parameters = { { "RegistrationId", registration.Id } }
    });
    
    // Assert
    response.Results["InvoiceId"].Should().NotBeNull();
}
```

## Producer Pattern

### Usage Guidelines
- Use existing Producer functions in the test project for creating test data
- If no Producer exists for your entity, create one following the existing patterns
- Producers should handle all the complexities of creating valid test data
- Producer methods should accept partial entity objects and fill in required fields

### Producer Method Patterns
Producer methods follow the pattern:
- Accept nullable entity parameter or entity with specific properties set
- Fill in all required fields for valid entity creation
- Return the created entity with populated ID
- Handle entity relationships properly

#### EntityReference Property Handling
When setting EntityReference properties in Producer methods, use lambda expressions to defer entity creation:

```csharp
// Correct pattern - use lambda expressions for EntityReference properties
internal demo_Employment ProduceValidEmployment(demo_Employment? employment) =>
    adminDao.Producer(employment, e =>
    {
        e.EnsureValue(e => e.demo_Employee, () => ProduceValidContact(null).ToEntityReference());
        e.EnsureValue(e => e.demo_Position, () => ProduceValidPosition(null).ToEntityReference());
        e.EnsureValue(e => e.demo_Workplace, () => ProduceValidWorkplace(null).ToEntityReference());
    });

// Avoid - directly passing entity objects for EntityReference properties
// e.EnsureValue(e => e.demo_Employee, ProduceValidContact(null)); // Incorrect
```

This pattern ensures that related entities are only created when the EntityReference property is actually null, improving performance and avoiding unnecessary entity creation.

## Entity Association Testing
For testing many-to-many relationships:
```csharp
AdminDao.AssociateEntities(
    "relationshipSchemaName", 
    sourceEntity.ToEntityReference(), 
    targetEntity.ToEntityReference());
```

## Test Data Cleanup
- Tests automatically clean up using XrmMockup snapshot restoration
- No manual cleanup required in test methods
- Each test starts with a clean state

## User Flow Integration Tests

### Purpose and Requirements
User flow tests validate complete end-to-end user journeys through the system by orchestrating multiple endpoints in realistic scenarios. These tests are **MANDATORY** and must be maintained for each major happy path through the system.

### Location and Organization
- **Location**: `test/IntegrationTests/*Api/TestUserFlows/`
- **Class naming**: `TestComplete[DomainArea]Flow.cs` (e.g., `TestCompleteUserOnboardingFlow.cs`)
- **Test naming**: `Complete[FlowName]_[Scenario]_CompletesSuccessfully`

### Test Structure Requirements

1. **One test per major happy path scenario**: Each distinct user journey through the system should have exactly one comprehensive flow test
2. **End-to-end coverage**: Tests must cover the complete user journey from initial contact to final goal completion
3. **Realistic data flow**: Use realistic test data that represents actual user scenarios
4. **Immediate verification**: Verify data integrity immediately after each major action within the same method

### Implementation Patterns

#### Test Method Structure
```csharp
[Fact]
public async Task CompleteUserOnboardingFlow_WithEmploymentAndEducationRequirements_CompletesSuccessfully()
{
    var testData = SetupTestDataForCompleteFlow();
    var foundPerson = await ExecutePersonLookupAndUpdate(testData);
    await CreateWorkplaceAndEducation(foundPerson.ContactId);
    await CreatePaymentMethod(foundPerson.ContactId);
}
```

#### Verification Co-location
**CRITICAL**: Verifications must be performed immediately after the action that creates the data, within the same method:

```csharp
private async Task CreateEmployment(Guid contactId)
{
    // Create employment
    var createEmploymentResponse = await ContactEndpoints.CreateEmployment(/*...*/);
    createEmploymentResponse.Result.Should().BeOfType<Created<Guid>>();
    var employmentId = ((Created<Guid>)createEmploymentResponse.Result).Value;

    // Verify employment immediately after creation
    VerifyEmploymentCreation(contactId, workplace, position, employmentId);
}
```

#### Method Responsibilities
- **Setup methods**: Create required test data (situations, existing contacts, etc.)
- **Action methods**: Perform one major system operation + immediate verification
- **Verification methods**: Validate that data was created/updated correctly

### Maintenance Requirements

1. **Update when endpoints change**: When endpoint signatures or behavior changes, update corresponding flow tests
2. **Add new scenarios**: When new user journeys are added to the system, create corresponding flow tests
3. **Maintain realistic data**: Ensure test data reflects actual user scenarios and business rules
4. **Verify complete coverage**: Each major system workflow should have at least one end-to-end flow test

### Anti-Patterns to Avoid

❌ **Don't create monolithic verification methods** - verify data immediately after creation 
❌ **Don't use unrealistic data** - use data that represents actual user scenarios
❌ **Don't test individual endpoints in flow tests** - use dedicated endpoint tests for that
❌ **Don't create flow tests for every minor variation** - focus on major happy paths

### Benefits
- **Regression protection**: Catch breaking changes across multiple endpoints
- **Integration validation**: Ensure endpoints work correctly together
- **Business logic verification**: Validate complete user journeys end-to-end
- **Documentation**: Serve as executable documentation of system workflows

## Assertion Patterns
Use FluentAssertions for all assertions:
- `.Should().Be()` for value equality
- `.Should().BeEquivalentTo()` for object comparison
- `.Should().BeOfType<T>()` for type checking
- `.Should().NotContainKey()` for dictionary/Results checking
- `.Should().Contain()` for partial string matching