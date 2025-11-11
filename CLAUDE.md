---
description: Quick start guide for AI assistants working on this codebase
globs:
alwaysApply: true
---

# Development Guide

This is a .NET solution with Azure backend components, Dataverse backend and frontend components.

## Working with this Codebase

This project uses specialized **slash commands** and **AI agents** to streamline development:

- Agents will automatically handle area-specific tasks following project standards
- All specialized tooling enforces consistent patterns and best practices

## Build & Test

Always validate changes before completing work:

```bash
# Build the solution
dotnet build --configuration Release

# Run tests
dotnet test --configuration Release
```

## Development Workflow

1. Understand the requirement
2. Use appropriate slash commands or let agents handle specialized tasks
3. Build and test to ensure quality
4. Follow established patterns in the codebase

**Quality standards are enforced** - ensure your changes build successfully and pass all tests.

## Project Structure

This is a mono repository with multiple connected systems. All these systems use Dataverse as their main storage system, and is therefore tightly connected to that data model.

- [.config](.config): Contains dotnet tools used in the project.
- [.pipelines](.pipeline): Azure Devops yaml definitions.
- [src](src): Contains application code:
  - [Azure](src/Azure): Contains several projects that provide outside access to Dataverse or do Asynchronous business logic.
    - [DataverseService](src/Azure/DataverseService): Contains all services that contact from Azure to Dataverse.
    - [*FunctionApp](src/Azure/*FunctionApp): Azure Function App that uses services from DataverseService.
    - [*Api](src/Azure/*Api): Minimal Api Web App that uses services from DataverseService.
  - [Dataverse](src/Dataverse): Contains the code running in Dataverse.
    - [SharedPluginLogic](src/Dataverse/SharedPluginLogic): A shared code project. Provides plugin code for the synchronous business logic.
    - [WebResources](src/Dataverse/WebResources): Provides frontend code for Dataverse.
    - [Plugins](src/Dataverse/Plugins): Read only! A .NET 4.6.2 class library. The output of this project will be deployed to Dataverse. Uses the output of SharedPluginLogic.
    - [PluginsNetCore](src/Dataverse/PluginsNetCore): Read only! A modern .NET class library. The output of this project will be used in tests. Uses the output of SharedPluginLogic. No files should be added to this project directly.
  - [Shared](src/Shared): Contains code that is shared between Azure and Dataverse business logic.
    - [SharedDomain](src/Shared/SharedDomain): A shared code project. Contains any backend domain classes used to communicate between Azure and Dataverse. This project should only contain domain classes used for this purpose. Domain classes that are used exclusively in either Azure or Dataverse should be defined there instead of this shared place.
    - [SharedDataverseLogic](src/Shared/SharedDataverseLogic): A shared code project. Contains any backend business logic that is used both in Azure and Dataverse.
    - [SharedContext](src/Shared/SharedContext): Read only! A shared code project. Contains the proxy classes that are generated from Dataverse. This defines the tables, attributes, and relations available in Dataverse.
    - [NetCoreContext](src/Shared/NetCoreContext): Read only! A class library that exposes the files in SharedContext as a newer .NET.
- [test](test): Contains all tests for the application code.
  - [IntegrationTests](test/IntegrationTests): Contains tests that run Azure and Dataverse business logic together locally.
  - [SharedTest](test/SharedTest): Contains the files shared between test projects.
- [Infrastructure](Infrastructure): Azure Bicep scripts (IaC).