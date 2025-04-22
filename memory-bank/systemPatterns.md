# System Patterns

## System Architecture
The repository is organized to support modular, cross-platform development for Microsoft Dataverse and Azure. The architecture separates concerns into distinct areas:
- **Azure Services**: Implemented as .NET (latest LTS) projects, deployed as Azure Functions or other services.
- **Dataverse Plugins**: Two plugin projects—one targeting .NET 4.6.2 for Dataverse deployment, and one targeting latest LTS .NET for testing.
- **Shared Projects**: Contain domain models, proxy classes, and business logic reused by both Azure and Dataverse components.
- **Web Resources**: TypeScript-only front-end logic for Dataverse forms and commands. All authored code must be TypeScript; only the generated JavaScript files are deployed to Dataverse.
- **Infrastructure**: Bicep templates for Azure resource provisioning.
- **Setup**: Scripts for one-time setup tasks (e.g., certificate generation).
- **Tools**: F# scripts for automation, code generation, and developer utilities.
- **Test**: Projects for integration and cross-platform business logic testing.

## Key Technical Decisions
- **TypeScript-Only Web Resources**: The src/Dataverse/WebResources project enforces a TypeScript-only policy. No authored JavaScript files are permitted; all source code must be TypeScript, and only the compiled JavaScript is deployed.
- **Trunk-Based Development**: All changes are integrated into a single main branch, with CI/CD pipelines for build, test, and deployment.
- **CI/CD Pipelines**: Defined in YAML under `.pipelines/`, with separate pipelines for build (pull requests) and build-and-deploy (trunk).
- **Infrastructure as Code**: Azure resources are provisioned using Bicep, ensuring reproducibility and version control.
- **Plugin Dual Targeting**: Plugins are compiled for both .NET 4.6.2 (Dataverse) and latest LTS .NET (testing), enabling robust test coverage.
- **Shared Logic**: Business logic is centralized in shared projects to avoid duplication and ensure consistency.
- **Testability**: Use of XrmMockup for in-memory Dataverse mocks and MessageExecutor for simulating Azure service interactions.

## Design Patterns in Use
- **Dependency Injection**: Used in Azure services and test harnesses to substitute dependencies (e.g., loggers, service mocks).
- **Separation of Concerns**: Clear boundaries between infrastructure, business logic, and integration code.
- **Mocking and Substitution**: For testing, dependencies are substituted or mocked as needed, supporting isolated and integration tests.
- **Code Generation**: F# scripts automate generation of context classes and domain models for Dataverse and TypeScript.

## Component Relationships
- **Azure and Dataverse**: Shared projects provide common logic and models, referenced by both Azure and Dataverse plugin projects.
- **Testing**: Test projects reference both Azure and Dataverse components, using mocks and message executors to simulate cross-platform flows.
- **Web Resources**: JavaScript code is deployed to Dataverse and interacts with plugin logic via Dataverse APIs.

## Critical Implementation Paths
- **Business Logic Flow**: Business logic is implemented in shared projects, invoked by both plugins (Dataverse) and Azure Functions.
- **Testing Cross-Platform Logic**: Tests use XrmMockup and MessageExecutor to simulate Dataverse and Azure interactions, ensuring correctness across boundaries.
- **Deployment**: CI/CD pipelines automate build, test, and deployment, with Bicep templates provisioning required Azure infrastructure.

## Extensibility
- New Azure services, plugins, or shared logic can be added by following the established project structure and referencing shared projects as needed.
- Additional F# scripts or setup scripts can be introduced for new automation or configuration requirements.
