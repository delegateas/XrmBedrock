---
description: Main entry point for AI-based development and developer reference
globs: 
alwaysApply: true
---

This file provides guidance to code agents when working with code in this repository.

**IMPORTANT**: This codebase has comprehensive development rules in the `.ai_rules` folder that MUST be followed:

- **Main Strategy** (`.ai_rules/main.md`) - Problem-solving approach and general guidelines  
- **Backend C# Rules** (`.ai_rules/backend/backend.md`) - Code style, naming conventions, and patterns
- **Azure Business Logic** (`.ai_rules/backend/azure.md`) - Azure Functions and Minimal API patterns
- **Dataverse DAO** (`.ai_rules/backend/dao.md`) - Data access patterns and best practices
- **Plugin Development** (`.ai_rules/backend/plugin.md`) - Dataverse plugin structure and registration
- **Integration Testing** (`.ai_rules/test/integrationtest.md`) - Testing patterns and requirements

Always consult the relevant rule files before making code changes. Key principles:
- Use automated tests only (no manual/visual testing)
- Follow established patterns for each component type
- Use OneOf pattern for error handling
- Maintain consistent naming and structure across areas/domains

## High-Level Problem Solving Strategy

1. Understand the problem deeply. Carefully read the instructions and think critically about what is required.
2. Investigate the codebase. Explore relevant files, search for key functions, and gather context.
3. Develop a clear, step-by-step plan. Break down the fix into manageable, incremental steps.
4. Before each code change, always consult the relevant rule files, and follow the rules very carefully. All rule files are located in the .ai_rules folder.
   - Failure to follow the rules is the main reason for making unacceptable changes.
5. Iterate until you are extremely confident the fix is complete.
   - When changing code, do not add comments about what you changed.
6. After each change, make sure you follow the rules in [Backend Rules](.ai_rules/backend/backend.md) or [Frontend Rules](.ai_rules/frontend/frontend.md) on how to correctly build and test.
    - Failure to correctly build and test is the second most common reason for making unacceptable changes.

## Architecture Overview

XrmBedrock is a Microsoft Dataverse-based member management system built with .NET 8.0. The solution follows a clean architecture pattern with the following key components:

- **Web API** (`src/Azure/*Api`) - ASP.NET Core 8.0 minimal API providing RESTful endpoints
- **Azure Functions** (`src/Azure/*FunctionApp`) - Serverless compute for background processing and integrations
- **Dataverse Services** (`src/Azure/DataverseService`) - Business logic layer for Dataverse operations
- **Dataverse Plugins** (`src/Dataverse/SharedPluginLogic`) - Custom business logic running within Dataverse (both .NET Framework 4.6.2 and .NET 8.0)
- **Shared Libraries** (`src/Shared/*`) - Common utilities, domain models, and data access objects

## Development Commands

### Build and Test
```bash
# Build entire solution
dotnet build --configuration Release

# Run all tests
dotnet test --configuration Release

# Run tests with detailed output
dotnet test --configuration Release --logger "console;verbosity=detailed"
```

### Dataverse Development (F# Scripts)
```bash
# Generate C# context from Dataverse
dotnet fsi tools/Daxif/GenerateCSharpContext.fsx

# Generate TypeScript definitions
dotnet fsi tools/Daxif/GenerateTypeScriptContext.fsx
```

## Rules for implementing changes

Always consult the relevant rule files before each code change.

Please note that I often correct or even revert code you generated. If you notice that, take special care not to revert my changes.

**Testing Requirements:**
- Never perform manual or visual testing through browsers
- Always write automated tests

Commit messages should be in imperative form, start with a capital letter, avoid ending punctuation, be a single line, and concisely describe changes and motivation.

Be very careful with comments, and add them only very sparingly. Never add comments about changes made (these belong in pull requests).

When making changes, always take speial care not to change parts of the code that are not in scope.

## Project Structure

This is a mono repository with multiple connected systems. All these systems use Dataverse as their main storage system, and is therefore tightly connected to that data model.

- [.config](.config): Contains dotnet tools used in the project.
- [.github](.github): GitHub workflows and other GitHub artifacts.
- [.pipelines](.pipeline): Azure Devops yaml definitions.
- [src](src): Contains application code:
  - [Azure](src/Azure): Contains several (Function or Web App) that provide outside access to Dataverse or do Asynchronous business logic. Follows rules defined in [Azure](.ai_rules/backend/azure.md).
    - [DataverseService](src/Azure/DataverseService): Contains all services that contact from Azure to Dataverse.
  - [Dataverse](src/Dataverse): Contains the code running in Dataverse.
    - [SharedPluginLogic](src/Dataverse/SharedPluginLogic): A shared code project. Provides plugin code for the synchronous business logic. Follows rules defined in [Plugin](.ai_rules/backend/plugin.md).
    - [WebResources](src/Dataverse/WebResources): Provides frontend code for Dataverse. Follows rules defined in [Dataverse Frontend](.ai_rules/frontend/dataverse.md).
    - [Plugins](src/Dataverse/Plugins): A .NET 4.6.2 class library. The output of this project will be deployed to Dataverse. Uses the output of SharedPluginLogic. No files should be added to this project directly.
    - [PluginsNetCore](src/Dataverse/PluginsNetCore): A modern .NET class library. The output of this project will be used in tests. Uses the output of SharedPluginLogic. No files should be added to this project directly.
  - [Shared](src/Shared): Contains code that is shared between Azure and Dataverse business logic.
    - [SharedContext](src/Shared/SharedContext): A shared code project. Contains the proxy classes that are generated from Dataverse. This defines the tables, attributes, and relations available in Dataverse. All files in this project are generated. No files should be added to this project directly.
    - [NetCoreContext](src/Shared/NetCoreContext): A class library that exposes the files in SharedContext as a newer .NET. No files should be added to this project directly.
    - [SharedDataverseLogic](src/Shared/SharedDataverseLogic): A shared code project. Contains any backend business logic that is used both in Azure and Dataverse.
    - [SharedDomain](src/Shared/SharedDomain): A shared code project. Contains any backend domain classes used to communicate between Azure and Dataverse. This project should only contain domain classes used for this purpose. Domain classes that are used exclusively in either Azure or Dataverse should be defined there instead of this shared place.
  - [Tools](src/Tools): Contains several tools that are run manually. No files should be modified.
- [test](test): Contains all tests for the application code.
  - [IntegrationTests](test/IntegrationTests): Contains tests that run Azure and Dataverse business logic together locally. Follows the rules defined in [IntegrationTest](.ai_rules/test/integrationtest.md).
  - [SharedTest](test/SharedTest): Contains the files shared between test projects.
- [Infrastructure](Infrastructure): Azure Bicep scripts (IaC).

## Key Architectural Patterns

### Error Handling
The codebase uses the OneOf library for functional error handling. Services return `OneOf<TSuccess, TError>` results:
```csharp
public async Task<OneOf<Contact, NotFound, ValidationError>> GetContactAsync(Guid id)
```

**IMPORTANT**: When modifying OneOf return types, you MUST follow the comprehensive call tree analysis process detailed in [Backend Rules](.ai_rules/backend/backend.md) to ensure all callers throughout the entire call chain are properly updated.

### Data Access Pattern
All Dataverse operations use Data Access Objects (DAOs) in `src/Shared/SharedContext`:
- Entities are defined with proper attributes for Dataverse mapping
- Early-bound entities are generated via Daxif scripts
- Services use `IDataverseService` for CRUD operations

### Authentication
- Web API uses Microsoft Identity Web with Azure AD
- Functions use managed identity for Dataverse access
- Plugins use service principal authentication

### Code Quality
The solution enforces strict code quality standards:
- Nullable reference types enabled
- Warnings as errors in Release builds
- StyleCop, SonarAnalyzer, and SecurityCodeScan analyzers
- All async methods follow proper naming conventions

## Important Considerations

### Dataverse Plugin Development
- Plugins exist in two versions: .NET Framework 4.6.2 (legacy) and .NET 8.0
- Shared logic goes in `SharedPluginLogic` project

### Testing Strategy
- Integration tests use XrmMockup365 for Dataverse mocking
- WireMock.Net for external service mocking
- All new features require corresponding tests

### TypeScript/JavaScript Development
- Web resources are in `src/Dataverse/WebResources`
- TypeScript compilation is configured in project files
- Generated types from `GenerateTypeScriptContext.fsx` provide type safety