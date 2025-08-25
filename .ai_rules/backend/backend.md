---
description: Core rules for C# development and tooling
globs: *.cs,*.csproj,*.sln,Directory.Packages.props,global.json,dotnet-tools.json
alwaysApply: false
---
# Backend

Carefully follow these instructions for C# backend development, including code style, naming, exceptions, logging, and build/test/format workflow.

## Code Style

- Always use these C# features:
  - File-level namespaces.
  - Primary constructors.
  - Array initializers.
  - Pattern matching with `is null` and `is not null` instead of `== null` and `!= null`.
- Records for immutable types.
- Mark all C# types as sealed.
- Use `var` when possible.
- Use simple collection types like `UserId[]` instead of `List<UserId>` whenever possible.
- **Collection Performance**: Only convert to arrays/HashSet when performance is needed:
  - Use `ToArray()` only when the collection is consumed multiple times or passed to methods requiring arrays
  - Use `ToHashSet()` for fast `Contains()` operations on large collections
  - Keep `IEnumerable<T>` for single-pass iterations and LINQ chains
  - Example: `var productIds = products.Select(p => p.Id).ToHashSet(); // Fast Contains()`
- Use clear names instead of making comments.
- Never use acronyms. E.g., use `SharedAccessSignature` instead of `Sas`.
- Do **not** prefix private fields with `_`. Use `camelCase` for private fields (e.g., `private readonly ITracingService tracingService;`). Follow the existing codebase conventions for field naming.
- Boolean properties and fields should use descriptive names that clearly indicate their purpose. Use appropriate prefixes such as:
  - `Is` for state or condition (e.g., `IsEmploymentRequired`, `IsActive`, `IsCompleted`)
  - `Has` for possession or presence (e.g., `HasQuestions`, `HasChildren`, `HasErrors`)
  - `Should` for actions or recommendations (e.g., `ShouldSend`, `ShouldValidate`, `ShouldProcess`)
  - `Can` for capability or permission (e.g., `CanEdit`, `CanDelete`, `CanAccess`)
  - Other descriptive verbs when appropriate (e.g., `Enabled`, `Required`, `Visible`)
  Maintain consistency within the same context and choose the prefix that best expresses the boolean's semantic meaning.
- Avoid using exceptions for control flow:
  - When exceptions are thrown, always use meaningful exceptions following .NET conventions.
  - Exception messages should include a period.
- Log only meaningful events at appropriate severity levels.
  - Logging messages should not include a period.
  - Use structured logging.
- Never introduce new NuGet dependencies.
- Don't do defensive coding (e.g., do not add exception handling to handle situations we don't know will happen).
- For internal APIs, do not check if referenced entities exist before using them - trust that the caller provides valid references and let Dataverse throw exceptions for invalid references.
- Do not add null checks for plugin context Target entities unless there is a documented scenario where Target may be missing. The framework guarantees the presence of the Target for the registered entity and operation.
- Avoid try-catch unless we cannot fix the reason. We have global exception handling to handle unknown exceptions.
- Don't add comments unless the code is truly not expressing the intent.
- Never add XML comments.
- Business logic is grouped by area. If something does not fit under an existing area you should ask what area it should be under. After asking, if that area does not exist, you create a folder with the area.
- Prefer constructor dependency injection for dependency management. Only deviate from this when applying a strategy + factory pattern. 

## OneOf Return Type Changes

**CRITICAL**: When modifying OneOf return types, you MUST follow this comprehensive process to avoid breaking changes throughout the call tree:

### 1. Impact Analysis (MANDATORY)
Before making any OneOf return type changes:
- Use search tools (Grep, Glob) to find ALL direct callers of the method
- For each caller found, recursively find their callers until you reach the top of the call tree
- Document the complete call chain including:
  - Service methods
  - API endpoints  
  - Test methods
  - Any other consumers

### 2. Change Implementation Order
Always implement changes in this exact order:
1. **Bottom-up approach**: Start with the lowest-level method (the one being changed)
2. **Update each caller level**: Work your way up the call tree, updating each level before proceeding to the next
3. **Update all tests**: Modify tests at each level to match the new signatures
4. **Update API endpoints**: Change endpoint return types and response mappings
5. **Final verification**: Ensure no `OneOf<...>` references remain that should have been updated

### 3. Required Updates for Each Caller
When a OneOf return type changes, each caller typically needs:
- **Method signature updates**: Change return types to match new OneOf variants
- **Match logic updates**: Update `.Match()` calls to handle new/removed error types  
- **Error handling updates**: Remove/add handling for error types that were removed/added
- **Test updates**: Modify test expectations and assertions
- **Documentation updates**: Update any comments or documentation referencing the old return types

### 4. Search Patterns to Use
Use these search patterns to find all affected code:
```bash
# Find direct method calls
grep -r "MethodName" --include="*.cs"

# Find OneOf usage with specific types
grep -r "OneOf<.*NotFound" --include="*.cs"

# Find test methods testing the changed method
grep -r "Test.*MethodName" --include="*.cs"

# Find API endpoints calling the method
grep -r "Results<.*NotFound" --include="*.cs"
```

### 5. Validation Checklist
Before considering the change complete, verify:
- [ ] All direct callers have been updated
- [ ] All indirect callers (callers of callers) have been updated  
- [ ] All test methods have been updated
- [ ] All API endpoints have been updated
- [ ] Build succeeds with no warnings
- [ ] All tests pass
- [ ] No TODO comments or incomplete implementations remain

### 6. Common Pitfalls to Avoid
- **Incomplete caller updates**: Failing to find and update all levels of the call tree
- **Test oversight**: Missing test updates that still expect old return types
- **API endpoint gaps**: Forgetting to update endpoint return types and mappings
- **Match logic errors**: Leaving old `.Match()` calls that reference removed error types

### Example Workflow
```csharp
// 1. Original method
OneOf<Success, NotFound, ValidationError> OriginalMethod()

// 2. Changed to remove NotFound
OneOf<Success, NotFound> OriginalMethod()

// 3. Update all callers - find with: grep -r "OriginalMethod" --include="*.cs"
// 4. For each caller, remove NotFound handling and update return type
// 5. Update tests to expect new signature
// 6. Update API endpoints to remove NotFound responses
// 7. Build and test to ensure everything works
```

## Implementation

IMPORTANT: Always follow these steps very carefully when implementing changes:

1. Always start new changes by writing new test cases (or change existing tests). Remember to consult [Integration Test](.ai_rules/test/integrationtest.md) for details.
2. Format the code by running `dotnet format`. This will format the code according to the .editorconfig file.
3. Build and test your changes:
   - Always run `dotnet build --configuration Release` to build the backend in release mode.
   - Run `dotnet test --configuration Release` to run all tests.

When you see paths like `/[area-name]/` in rules, replace `[area-name]` with the specific area name (e.g., `CustomerArea`, `FinanceArea`) you're working with. When you see a `[Domain]` used, you should replace it with the relevant domain you are working on within that area.