# XrmBedrock Examples

This branch contains practical examples demonstrating XrmBedrock architectural patterns for building Dataverse plugins. These examples serve as learning templates and reference implementations showing best practices for plugin development, business logic organization, and comprehensive testing.

## What's Included

This repository contains two complete "Area" implementations with multiple plugin scenarios:

- **Customer Area** - Account management with 4 different plugin patterns
- **Activity Area** - Task validation with business rules

Each example includes:
- Business logic implementation
- Plugin registration
- Comprehensive integration tests
- Dependency injection setup

## Customer Area Examples

Located in `src/Dataverse/SharedPluginLogic/Logic/CustomerArea/`

### 1. ValidatePhoneNumber

**Purpose**: Validates that phone numbers follow a specific format (must start with '+' and contain only digits and spaces)

**Pattern**: Input validation using regex

**Key Files**:
- Logic: `CustomerService.cs:ValidatePhoneNumber()`
- Registration: `Registrations/CustomerArea/ValidatePhoneNumber.cs`
- Tests: `test/IntegrationTests/CustomerArea/ValidatePhoneNumberTests.cs`

**Details**:
- Execution Stage: PreValidation (runs early to block invalid data)
- Events: Create and Update on Account entity
- Demonstrates: Early validation, regex patterns, OneOf error handling

### 2. CreateCreditAssessmentTask

**Purpose**: Automatically creates a task for credit assessment when a new account is marked as "Supplier"

**Pattern**: Automated task creation based on entity type

**Key Files**:
- Logic: `CustomerService.cs:CreateCreditAssessmentTask()`
- Registration: `Registrations/CustomerArea/AccountCreditAssessment.cs`
- Tests: `test/IntegrationTests/CustomerArea/AccountCreditAssessmentTests.cs`

**Details**:
- Execution Stage: PostOperation (after account is created)
- Events: Create on Account entity
- Demonstrates: Conditional entity creation, checking account type, using DAO pattern

### 3. CopyParentTelephone

**Purpose**: Automatically copies telephone number from parent account to child account if not already set

**Pattern**: Parent-child data inheritance

**Key Files**:
- Logic: `CustomerService.cs:CopyParentTelephone()`
- Registration: `Registrations/CustomerArea/CopyParentTelephone.cs`
- Tests: `test/IntegrationTests/CustomerArea/CopyParentTelephoneTests.cs`

**Details**:
- Execution Stage: PreOperation (modifies target before save)
- Events: Create on Account entity
- Demonstrates: Related entity lookups, conditional field copying, null handling

### 4. UpdateTelephoneOnSubaccounts

**Purpose**: Cascades telephone number changes from parent account to all child accounts that have the old phone number

**Pattern**: Cascading updates with conditional logic

**Key Files**:
- Logic: `CustomerService.cs:UpdateTelephoneOnSubaccounts()`
- Registration: `Registrations/CustomerArea/UpdateTelephoneOnSubaccounts.cs`
- Tests: `test/IntegrationTests/CustomerArea/UpdateTelephoneOnSubaccountsTests.cs`

**Details**:
- Execution Stage: PostOperation (after update completes)
- Events: Update on Account entity
- Features: Uses PreImage/PostImage to detect changes, filtered attributes for performance
- Demonstrates: Change detection, bulk updates, filtering by relationship

## Activity Area Examples

Located in `src/Dataverse/SharedPluginLogic/Logic/ActivityArea/`

### ValidateTaskIsWithinBusinessHours

**Purpose**: Validates that tasks assigned to other users are scheduled within business hours (8 AM - 5 PM)

**Pattern**: Business rule validation with user context checking

**Key Files**:
- Logic: `ActivityService.cs:ValidateTaskIsWithinBusinessHours()`
- Registration: `Registrations/ActivityArea/ValidateTask.cs`
- Tests: `test/IntegrationTests/ActivityArea/ValidateTaskTests.cs`

**Details**:
- Execution Stage: PreValidation
- Events: Create and Update on Task entity
- Logic: Only validates when task owner is different from current user
- Demonstrates: Time-based validation, user context awareness, filtered attributes, logging

## Key Architectural Patterns Demonstrated

1. **Area-based Organization** - Code organized by business domain (CustomerArea, ActivityArea)
2. **Service Layer Pattern** - Business logic separated into service classes
3. **Plugin Registration Pattern** - Clean, fluent API for registering plugin steps
4. **Execution Stages** - Examples of PreValidation, PreOperation, and PostOperation
5. **Image Usage** - PreImage and PostImage for detecting changes
6. **Filtered Attributes** - Performance optimization by triggering only on specific field changes
7. **Context Handling** - Using `GetTarget()`, `GetTargetMergedWithPreImage()`, `GetRequiredPreImage()`, `GetRequiredPostImage()`
8. **DAO Pattern** - Using `IAdminDataverseAccessObjectService` for data operations
9. **Integration Testing** - Comprehensive tests showing how to validate plugin behavior
10. **Dependency Injection** - Service registration and resolution using Microsoft.Extensions.DependencyInjection

## Getting Started

### Explore the Examples

1. **Read the business logic**: Start with `CustomerService.cs` or `ActivityService.cs`
2. **Review plugin registrations**: See how plugins are registered in `Registrations/` folders
3. **Study the tests**: Integration tests demonstrate how each plugin works
4. **Trace the patterns**: Follow the Area-based structure and DI setup

### File Structure

```
src/Dataverse/SharedPluginLogic/
├── Logic/
│   ├── ActivityArea/
│   │   ├── ActivityService.cs          # Business logic
│   │   └── AddServices.cs              # DI registration
│   └── CustomerArea/
│       ├── CustomerService.cs          # Business logic
│       └── AddServices.cs              # DI registration
└── Registrations/
    ├── ActivityArea/
    │   └── ValidateTask.cs             # Plugin registration
    └── CustomerArea/
        ├── AccountCreditAssessment.cs
        ├── CopyParentTelephone.cs
        ├── UpdateTelephoneOnSubaccounts.cs
        └── ValidatePhoneNumber.cs

test/IntegrationTests/
├── ActivityArea/
│   └── ValidateTaskTests.cs            # 5 test methods
└── CustomerArea/
    ├── AccountCreditAssessmentTests.cs # 3 test methods
    ├── CopyParentTelephoneTests.cs     # 3 test methods
    ├── UpdateTelephoneOnSubaccountsTests.cs # 2 test methods
    └── ValidatePhoneNumberTests.cs     # 2 test methods
```

### Build and Test

```bash
# Build the solution
dotnet build --configuration Release

# Run all integration tests
dotnet test --configuration Release
```

## How to Use These Examples

These examples are designed to be:

1. **Learning Templates** - Study them to understand XrmBedrock patterns
2. **Reference Implementations** - See best practices in action
3. **Starting Points** - Copy and modify for your own features
4. **Test Examples** - Learn how to write comprehensive integration tests

Each example is intentionally simple but covers common plugin scenarios and demonstrates the full development lifecycle from business logic through plugin registration to integration testing.

## What's Next

Use these examples as a foundation to:
- Build your own Areas for different business domains
- Implement similar patterns for your specific requirements
- Create comprehensive test suites for your plugins
- Follow established architectural patterns throughout your solution
