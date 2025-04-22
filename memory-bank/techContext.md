# Tech Context

## Technologies Used
- **Microsoft Dataverse**: Target platform for plugin deployment and data integration.
- **Azure**: Cloud platform for hosting services, functions, and infrastructure.
- **.NET**: 
  - Plugins: .NET 4.6.2 (for Dataverse deployment), latest LTS .NET (for testing).
  - Azure Functions and services: latest LTS .NET.
- **F#**: Used for scripting, automation, and code generation (via F# Interactive).
- **TypeScript**: For Dataverse web resources (front-end logic). All authored code in src/Dataverse/WebResources must be TypeScript; JavaScript files are generated for deployment only.
- **Bicep**: Infrastructure-as-code for Azure resource provisioning.
- **PowerShell**: For setup and deployment scripts.

## Development Setup
- **Solution Structure**: Managed via Visual Studio solution (.sln) and project files (.csproj, .fsx, .shproj).
- **CI/CD**: Azure Pipelines defined in `.pipelines/` directory (YAML).
- **Infrastructure Deployment**: Bicep templates in `Infrastructure/`, with parameter files for different environments.
- **Setup Scripts**: PowerShell scripts in `Setup/` for one-time configuration (e.g., certificate generation).
- **Testing**: Test projects in `test/` directory, using XrmMockup for Dataverse and custom mocks for Azure.

## Technical Constraints
- **TypeScript-Only WebResources**: The src/Dataverse/WebResources project enforces a TypeScript-only policy. Authored JavaScript files are not permitted; only TypeScript source is allowed, and JavaScript is generated for deployment.
- **Plugin Deployment**: Must target .NET 4.6.2 for compatibility with Dataverse.
- **Testing**: Plugins must also be compiled for latest LTS .NET to enable modern test frameworks and cross-platform logic validation.
- **Azure Services**: Must be compatible with latest LTS .NET and Azure Functions runtime.
- **Infrastructure**: All Azure resources must be provisioned via Bicep for consistency and repeatability.

## Dependencies
- **NuGet Packages**: Managed via `packages.config` and project files.
- **XrmMockup**: For in-memory Dataverse service mocking in tests.
- **NSubstitute**: For dependency substitution in tests (e.g., loggers).
- **Azure SDKs**: For Azure service integration.
- **Bicep CLI**: For deploying infrastructure templates.

## Tool Usage Patterns
- **Dataverse Schema Reference**: The file `src/Shared/SharedContext/XrmContext.cs` is the canonical source for Dataverse tables and columns in code. It contains a class for each table and an attribute for each column, providing the definitive mapping between the Dataverse schema and C# code.
- **F# Scripts**: Used for code generation (C#, TypeScript contexts), solution management, and automation tasks.
- **TypeScript Tooling**: tsconfig.json and ESLint are used to enforce TypeScript standards and code quality in WebResources. The build process compiles TypeScript to JavaScript for deployment to Dataverse.
- **PowerShell**: Used for setup, deployment, and certificate management.
- **CI/CD Pipelines**: Automate build, test, and deployment for both Dataverse and Azure components.
- **Testing Utilities**: MessageExecutor and custom mocks for simulating cross-platform interactions.

## Environment Management
- **Parameterization**: Bicep parameter files for dev, test, uat, and prod environments.
- **Secrets Management**: Azure Key Vault integration via Bicep modules.

## Developer Experience
- **EditorConfig**: Enforced code style via `.editorconfig`.
- **Source Control**: Git, with `.gitignore` for common exclusions.
- **Strong Naming**: Use of `.snk` files for assembly signing where required.
