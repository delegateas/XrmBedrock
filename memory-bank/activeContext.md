# Active Context

## Current Work Focus
- Establishing the memory bank documentation as the foundation for all future development and onboarding.
- Ensuring all core project context, architecture, and technical details are captured and accessible.
- Current business logic files are provided as samples/placeholders and are not intended for production use.

## Recent Changes
- Initial creation of the memory bank files: projectbrief.md, productContext.md, systemPatterns.md, techContext.md, activeContext.md, and progress.md.
- Documentation of repository structure, technical decisions, and integration patterns.
- Added documentation for the plugin extensibility and service registration pattern in systemPatterns.md, with cross-references in techContext.md and activeContext.md.
- Added example-plugins and tests for these to illustrate to developers and agents how to do this in a good way using the XrmBedrock platform

## Next Steps
- Generate new proxy classes for your own Dataverse environment using the provided tooling.
- Write and implement your own business logic to replace the sample files.
- Follow the documented plugin extensibility and service registration pattern when adding new plugins or services, and update documentation as the pattern evolves.
- Review and refine memory bank content as the project evolves.
- Begin onboarding new developers using the memory bank as the primary source of project context.
- Implement additional documentation for complex features, integrations, or testing strategies as needed.
- Update activeContext.md and progress.md after each significant change or milestone.

## Active Decisions and Considerations
- All business logic should be implemented in shared projects to maximize reuse and testability.
- Plugins must be dual-targeted for .NET 4.6.2 (Dataverse) and latest LTS .NET (testing).
- Infrastructure changes must be managed via Bicep and tracked in version control.
- Testing should leverage XrmMockup and MessageExecutor for cross-platform logic validation.
- Use of strong types from XrmContext.cs is mandatory for all Dataverse entity interactions to ensure type safety and adherence to project standards.
- Private properties and fields should not be prepended with an underscore to maintain consistent naming conventions across the codebase.
- Use the GetTarget method from IPluginExecutionContext to retrieve the target entity as a strongly typed object in plugin implementations.

## Important Patterns and Preferences
- The file `src/Shared/SharedContext/XrmContext.cs` is the canonical mapping of Dataverse tables and columns to C# code, with a class for each table and an attribute for each column. This pattern is essential for onboarding and for understanding the data model in code.
- Trunk-based development with automated CI/CD for build, test, and deployment.
- Clear separation of concerns between Azure, Dataverse, and shared logic.
- Use of F# scripts for automation and code generation.
- Parameterized infrastructure for environment-specific deployments.

## Learnings and Project Insights
- Early investment in documentation and shared logic reduces onboarding time and technical debt.
- Automated testing and infrastructure provisioning are critical for reliability in cross-platform solutions.
- Mocking and dependency substitution are essential for effective integration testing.
- Using strong types from XrmContext.cs improves code quality and maintainability for Dataverse interactions.
