# Product Context

## Purpose and Rationale
This project exists to streamline and standardize the development of solutions that integrate Microsoft Dataverse with Azure services. Organizations leveraging Dataverse often require custom business logic, automation, and integrations that span both Dataverse and Azure. This template addresses the complexity of managing such cross-platform solutions by providing a unified, maintainable, and extensible codebase.

## Problems Solved
- Reduces friction in setting up new Dataverse-Azure integration projects by providing a ready-to-use structure.
- Eliminates inconsistencies in deployment and development practices across teams.
- Simplifies the management of shared business logic and domain models between Dataverse plugins and Azure Functions.
- Provides robust testing infrastructure for logic that spans both platforms, including in-memory mocks and service substitution.
- Automates infrastructure provisioning and deployment, reducing manual errors and increasing reliability.

## How It Should Work
- Developers implement business logic in shared projects, referenced by both Dataverse plugins and Azure Functions.
- Plugins are compiled for both .NET 4.6.2 (for Dataverse deployment) and the latest LTS .NET (for testing).
- Azure services are written in the latest LTS .NET and deployed via Bicep templates.
- Front-end logic for Dataverse is managed as web resources and deployed alongside plugins.
- CI/CD pipelines automate build, test, and deployment processes, supporting trunk-based development.
- One-time setup scripts (e.g., certificate generation) are provided for secure authentication and configuration.

## User Experience Goals
- Rapid project onboarding for new developers and teams.
- Consistent, predictable build and deployment processes.
- Easy extension and maintenance of business logic across both Dataverse and Azure.
- High confidence in code quality through comprehensive, cross-platform testing.
- Clear separation of concerns and code reuse, minimizing duplication and technical debt.

## Intended Users
- Developers building custom business logic and integrations for Dataverse and Azure.
- DevOps engineers responsible for deployment and infrastructure management.
- Testers validating end-to-end business processes across both platforms.
