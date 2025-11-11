# XrmBedrock Template Migration Plan

**Goal:** Transform XrmBedrock into a clean `dotnet new` template while maintaining up-to-date examples in separate branches.

**Status:** Not Started
**Last Updated:** 2025-10-01

---

## Overview

This plan uses a branch-based approach where:
- `main` = Clean framework template (no examples)
- `examples/starter` = Basic working examples
- `examples/advanced` = Complex patterns (optional)
- CI automatically merges main → example branches and validates

**Safety First:** Create and verify example branches BEFORE cleaning main.

---

## Progress Tracker

- [ ] Phase 1: Preparation & Inventory
- [ ] Phase 2: Create Example Branches
- [ ] Phase 3: Setup CI Workflow
- [ ] Phase 4: Test Auto-Merge Workflow
- [ ] Phase 5: Clean Main Branch
- [ ] Phase 6: Create dotnet new Template Configuration
- [ ] Phase 7: Update Example Branches
- [ ] Phase 8: Update Documentation
- [ ] Phase 9: Final Validation

---

## Phase 1: Preparation & Inventory

**Status:** ⬜ Not Started

### 1.1 Create Branch for Work
```bash
git checkout -b feature/template-setup
```

### 1.2 Document Current State
Create inventory in this document:

**Framework Code (Keep in main):**
- `src/Shared/SharedContext/` - DAO foundation
- `src/Shared/SharedDataverseLogic/` - Base utilities
- `src/Shared/SharedDomain/` - (empty after cleanup)
- `src/Shared/NetCoreContext/` - Context wrapper
- `src/Azure/DataverseService/Foundation/` - DAO base classes
- `src/Dataverse/SharedPluginLogic/Logic/` - Base plugin infrastructure
  - `ArgumentNullExceptionHelper.cs`
  - `Azure/` (Azure integration base)
  - `ExecutionContextExtensions.cs`
  - `ExtendedTracingService.cs`
  - `ImageType.cs`
  - `StaticExtensions.cs`
  - `Utility/` (base utilities)
  - `_Dao/` (base DAO services)
- `src/Dataverse/SharedPluginLogic/Plugins/` - Plugin infrastructure
  - `DataverseLogger.cs`
  - `DummyManagedIdentityService.cs`
  - `PluginSetupCustomDependencies.cs`
  - `Utility/AutopublishDuplicateRulesOnPublishAll.cs`
- `src/Dataverse/Plugins/` - Plugin wrapper (keep `Plugin.cs`)
- `src/Dataverse/PluginsNetCore/` - NetCore plugin wrapper
- `src/Dataverse/ConsoleJobs/` - Console jobs project
- `src/Dataverse/WebResources/` - Web resources project
- `src/Tools/` - DAXIF tooling
- `test/SharedTest/` - Test infrastructure
- `test/TestMetadata/` - Test metadata
- `test/IntegrationTests/` - Test infrastructure files:
  - `TestBase.cs`
  - `XrmMockupFixture.cs`
  - `XrmMockupCollectionFixture.cs`
  - `MessageExecutor.cs`
  - `AwaitingMessage.cs`

**Example Code (Extract to example branches):**
- `src/Azure/EconomyAreaFunctionApp/` - Example Azure Function
- `src/Azure/DataverseService/EconomyArea/` - Example service
- `src/Shared/SharedDomain/EconomyArea/` - Example domain
- `src/Shared/SharedDomain/QueueNames.cs` - Example constants
- `src/Dataverse/SharedPluginLogic/Logic/EconomyArea/` - Example plugin logic
- `src/Dataverse/SharedPluginLogic/Logic/ExampleActivityArea/` - Example activity logic
- `src/Dataverse/SharedPluginLogic/Logic/ExampleCustomerArea/` - Example customer logic
- `src/Dataverse/SharedPluginLogic/Plugins/APIs/CreateTransactions.cs` - Example API
- `src/Dataverse/SharedPluginLogic/Plugins/EconomyArea/` - Example plugins
- `src/Dataverse/SharedPluginLogic/Plugins/ExampleActivityArea/` - Example plugins
- `src/Dataverse/SharedPluginLogic/Plugins/ExampleCustomerArea/` - Example plugins
- `src/Dataverse/Plugins/CustomAPI.cs` - Example Custom API
- `test/IntegrationTests/ExampleActivityArea/` - Example tests
- `test/IntegrationTests/ExampleCustomerArea/` - Example tests
- `test/IntegrationTests/InvoiceGenerationTests.cs` - Example test

**Generated/Setup Files (Handle in .gitignore):**
- `Setup/` - Initial setup scripts (delete after cleanup)
- `bin/`, `obj/` - Build artifacts
- `memory-bank/` - AI context

---

## Phase 2: Create Example Branches (Keep Everything)

**Status:** ⬜ Not Started

### 2.1 Create examples/starter Branch
```bash
git checkout -b examples/starter main
git push -u origin examples/starter
```

**Actions:**
- [ ] Create branch
- [ ] Verify build succeeds: `dotnet build --configuration Release`
- [ ] Verify tests pass: `dotnet test --configuration Release`
- [ ] Push branch

**Notes:**
- This is a full copy of main - no deletions yet
- Purpose: Preserve working state before main cleanup

### 2.2 Create examples/advanced Branch (Optional)
```bash
git checkout -b examples/advanced main
git push -u origin examples/advanced
```

**Actions:**
- [ ] Create branch (if desired)
- [ ] Add additional complex examples (optional)
- [ ] Verify build + tests
- [ ] Push branch

**Notes:**
- Can be created later if not needed immediately
- Could show more complex patterns like multiple areas, advanced plugins, etc.

---

## Phase 3: Setup CI Workflow (Test Without Main Changes)

**Status:** ⬜ Not Started

### 3.1 Create `.github/workflows/validate-examples.yaml`

**Actions:**
- [ ] Create workflow file with auto-merge logic
- [ ] Configure three jobs:
  1. `validate-framework` - Builds/tests main branch
  2. `sync-examples` - Auto-merges main → example branches after validation
  3. `validate-example-changes` - Validates direct commits to example branches

**Key Features:**
- Fails PRs to main if any example branch would break
- Automatically merges main to examples on successful push
- Validates example-only changes independently

**Workflow File Content:**
```yaml
name: Validate Example Branches

on:
  push:
    branches:
      - main
      - 'examples/**'
  pull_request:
    branches:
      - main

jobs:
  validate-framework:
    if: github.ref == 'refs/heads/main' || github.base_ref == 'main'
    runs-on: windows-latest
    environment: dev
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET 8
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.x
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore --configuration Release
      - name: Test
        run: dotnet test --no-build --configuration Release

  sync-examples:
    if: github.ref == 'refs/heads/main' && github.event_name == 'push'
    needs: validate-framework
    strategy:
      fail-fast: false
      matrix:
        branch: [examples/starter]
    runs-on: windows-latest
    environment: dev
    steps:
      - uses: actions/checkout@v4
        with:
          ref: ${{ matrix.branch }}
          fetch-depth: 0
          token: ${{ secrets.GITHUB_TOKEN }}

      - name: Configure git
        run: |
          git config user.name "github-actions[bot]"
          git config user.email "github-actions[bot]@users.noreply.github.com"

      - name: Merge main into example branch
        run: |
          git fetch origin main
          git merge origin/main --no-edit

      - name: Setup .NET 8
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.x

      - name: Restore dependencies
        run: dotnet restore

      - name: Build example
        run: dotnet build --no-restore --configuration Release

      - name: Test example
        run: dotnet test --no-build --configuration Release

      - name: Push merged branch
        run: git push origin ${{ matrix.branch }}

  validate-example-changes:
    if: startsWith(github.ref, 'refs/heads/examples/')
    runs-on: windows-latest
    environment: dev
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET 8
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.x
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore --configuration Release
      - name: Test
        run: dotnet test --no-build --configuration Release
```

### 3.2 Test CI Workflow

**Actions:**
- [ ] Commit workflow to `feature/template-setup`
- [ ] Push branch
- [ ] Create PR to main (DO NOT MERGE)
- [ ] Verify workflow runs successfully in PR
- [ ] Close PR without merging

**Notes:**
- This tests the workflow logic without affecting main
- Ensures CI is working before we rely on it

---

## Phase 4: Test Auto-Merge Workflow (Dry Run)

**Status:** ⬜ Not Started

### 4.1 Test Example Branch Validation
```bash
git checkout examples/starter
# Make trivial change (add comment)
git commit -m "Test: Add comment for CI validation"
git push
```

**Actions:**
- [ ] Make trivial change to examples/starter
- [ ] Push change
- [ ] Verify CI `validate-example-changes` job runs
- [ ] Verify build + test passes

### 4.2 Test Manual Merge Simulation
```bash
git checkout examples/starter
git fetch origin main
git merge origin/main --no-edit
dotnet build --configuration Release
dotnet test --configuration Release
```

**Actions:**
- [ ] Manually test merge from main → examples/starter
- [ ] Verify build succeeds
- [ ] Verify tests pass
- [ ] Document any conflicts (shouldn't be any yet)
- [ ] Reset branch (don't push): `git reset --hard origin/examples/starter`

**Notes:**
- This simulates what CI will do automatically
- Verifies merge works cleanly with current state

---

## Phase 5: Clean Main Branch (Only After Verification)

**Status:** ⬜ Not Started

### 5.1 Create feature/clean-main Branch
```bash
git checkout main
git checkout -b feature/clean-main
```

### 5.2 Remove Example Projects

**Actions:**
- [ ] Delete `src/Azure/EconomyAreaFunctionApp/` directory
- [ ] Delete `test/IntegrationTests/ExampleActivityArea/` directory
- [ ] Delete `test/IntegrationTests/ExampleCustomerArea/` directory
- [ ] Delete `test/IntegrationTests/InvoiceGenerationTests.cs`
- [ ] Remove `EconomyAreaFunctionApp` from `XrmBedrock.sln`
- [ ] Verify solution file is valid

### 5.3 Remove Example Code from Shared Projects

**In `src/Shared/SharedDomain/`:**
- [ ] Delete `EconomyArea/` folder
- [ ] Delete `QueueNames.cs`

**In `src/Azure/DataverseService/`:**
- [ ] Delete `EconomyArea/` folder

**In `src/Dataverse/SharedPluginLogic/Logic/`:**
- [ ] Delete `EconomyArea/` folder
- [ ] Delete `ExampleActivityArea/` folder
- [ ] Delete `ExampleCustomerArea/` folder

**In `src/Dataverse/SharedPluginLogic/Plugins/`:**
- [ ] Delete `APIs/CreateTransactions.cs`
- [ ] Delete `EconomyArea/` folder
- [ ] Delete `ExampleActivityArea/` folder
- [ ] Delete `ExampleCustomerArea/` folder

**In `src/Dataverse/Plugins/`:**
- [ ] Delete `CustomAPI.cs`

**Note:** Keep `Plugin.cs` - it's framework infrastructure

### 5.4 Test Clean Build

**Actions:**
- [ ] Build: `dotnet build --configuration Release`
- [ ] Test: `dotnet test --configuration Release`
- [ ] Verify both succeed with only framework code

**Expected Result:**
- Build succeeds (no missing references)
- Tests pass (only framework infrastructure tests remain)

### 5.5 Create PR for Clean Main

**Actions:**
- [ ] Commit all deletions: `git commit -m "Remove example code from framework"`
- [ ] Push: `git push -u origin feature/clean-main`
- [ ] Create PR: `feature/clean-main` → `main`
- [ ] **Critical:** Wait for CI to run
- [ ] **Expected:** CI should FAIL on `sync-examples` job
- [ ] **DO NOT MERGE YET**

**Why It Should Fail:**
- The auto-merge to example branches will create conflicts
- Example branches reference the deleted code
- This proves our safety net works!

**Notes:**
- Failing CI is GOOD here - it means the safety mechanism works
- We'll resolve this in Phase 7

---

## Phase 6: Create dotnet new Template Configuration

**Status:** ⬜ Not Started

### 6.1 Create Template Structure
```bash
git checkout feature/clean-main
mkdir .template.config
```

**Actions:**
- [ ] Create `.template.config/` directory
- [ ] Create `template.json`
- [ ] Create `.template.config/README.md` (optional)

### 6.2 Configure template.json

**File:** `.template.config/template.json`
```json
{
  "$schema": "http://json.schemastore.org/template",
  "author": "Your Name/Organization",
  "classifications": ["Dataverse", "Framework", "Azure"],
  "identity": "XrmBedrock.Template",
  "name": "XrmBedrock Dataverse Framework",
  "shortName": "xrmbedrock",
  "sourceName": "XrmBedrock",
  "preferNameDirectory": true,
  "tags": {
    "language": "C#",
    "type": "project"
  },
  "sources": [
    {
      "modifiers": [
        {
          "exclude": [
            "**/bin/**",
            "**/obj/**",
            "**/.vs/**",
            "**/.vscode/**",
            "**/.git/**",
            "**/.gitignore",
            "**/.gitattributes",
            "**/memory-bank/**",
            "**/Setup/**",
            "**/docs/template-migration-plan.md",
            ".template.config/**"
          ]
        }
      ]
    }
  ],
  "symbols": {
    "SkipRestore": {
      "type": "parameter",
      "datatype": "bool",
      "defaultValue": "false",
      "description": "If specified, skips the automatic restore of the project on create."
    }
  },
  "postActions": [
    {
      "condition": "(!SkipRestore)",
      "description": "Restore NuGet packages required by this project.",
      "manualInstructions": [
        {
          "text": "Run 'dotnet restore'"
        }
      ],
      "actionId": "210D431B-A78B-4D2F-B762-4ED3E3EA9025",
      "continueOnError": true
    }
  ]
}
```

**Actions:**
- [ ] Create template.json with above content
- [ ] Update author field
- [ ] Review exclusions
- [ ] Commit: `git commit -m "Add dotnet new template configuration"`

### 6.3 Test Template Locally

**Actions:**
```bash
# From XrmBedrock directory on feature/clean-main branch
dotnet new install .

# Test in a temporary directory
cd ..
mkdir test-template-install
cd test-template-install
dotnet new xrmbedrock --name TestProject

# Verify
cd TestProject
dotnet build --configuration Release
```

**Checklist:**
- [ ] Template installs successfully
- [ ] Template creates project with correct structure
- [ ] Generated project builds successfully
- [ ] No example code in generated project
- [ ] Uninstall test template: `dotnet new uninstall XrmBedrock.Template`
- [ ] Delete test directory

---

## Phase 7: Update Example Branches to Merge Clean Main

**Status:** ⬜ Not Started

### 7.1 Prepare for Conflicts

**Expected Conflicts:**
- Solution file (EconomyAreaFunctionApp removed)
- AddServices files (example service registrations)
- Test files (example test imports)

**Resolution Strategy:**
- Keep all example code
- Accept framework deletions for non-example code
- Update references to match new structure

### 7.2 Manually Resolve Example Branch Conflicts

**For examples/starter:**
```bash
git checkout examples/starter
git fetch origin
git merge origin/feature/clean-main
```

**Actions:**
- [ ] Attempt merge
- [ ] Resolve conflicts:
  - Keep example projects in solution file
  - Keep example code files
  - Accept framework changes
- [ ] Test build: `dotnet build --configuration Release`
- [ ] Test: `dotnet test --configuration Release`
- [ ] Commit: `git commit -m "Merge clean framework from main"`
- [ ] Push: `git push origin examples/starter`

**For examples/advanced (if created):**
- [ ] Repeat above steps for examples/advanced

### 7.3 Merge Clean Main

**Actions:**
- [ ] Merge PR: `feature/clean-main` → `main`
- [ ] CI will attempt auto-merge to example branches
- [ ] **Expected:** Should succeed now that conflicts are resolved
- [ ] Verify all CI jobs pass

**Notes:**
- Example branches were pre-resolved in 7.2
- Auto-merge should be fast-forward or clean merge

---

## Phase 8: Update Documentation

**Status:** ⬜ Not Started

### 8.1 Update README.md in main

**Actions:**
- [ ] Replace content with clean template documentation
- [ ] Add installation instructions
- [ ] Add link to example branches
- [ ] Remove manual setup instructions
- [ ] Commit: `git commit -m "Update README for template usage"`

**New README.md Content:**
```markdown
# XrmBedrock

A modern, opinionated framework for Microsoft Dataverse development with .NET 8.

## Features

- Clean architecture with DAO pattern
- Azure Functions integration
- Dataverse plugin infrastructure (.NET 4.6.2 and .NET 8)
- Integration testing with XrmMockup365
- OneOf pattern for error handling
- Comprehensive AI coding rules

## Installation

### As a Template
```bash
dotnet new install XrmBedrock
dotnet new xrmbedrock --name MyProject
cd MyProject
dotnet build --configuration Release
```

### From Source
```bash
git clone https://github.com/your-org/XrmBedrock.git
cd XrmBedrock
dotnet build --configuration Release
```

## Examples

Working examples are maintained in separate branches and automatically validated:

- **[Starter Examples](https://github.com/your-org/XrmBedrock/tree/examples/starter)** - Basic patterns (Customer area, simple plugins, Azure Functions)
- **[Advanced Examples](https://github.com/your-org/XrmBedrock/tree/examples/advanced)** - Complex patterns (multiple areas, custom APIs, advanced scenarios)

Examples are automatically tested against the latest framework version to ensure they're always up-to-date.

## Documentation

- [AI Development Rules](.ai_rules/main.md) - Comprehensive coding standards
- [Build & Test Commands](CLAUDE.md#development-commands)
- [Architecture Overview](CLAUDE.md#architecture-overview)

## Development

### Build
```bash
dotnet build --configuration Release
```

### Test
```bash
dotnet test --configuration Release
```

### Generate Dataverse Context
```bash
dotnet fsi src/Tools/Daxif/GenerateCSharpContext.fsx
dotnet fsi src/Tools/Daxif/GenerateTypeScriptContext.fsx
```

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on:
- Adding examples
- Framework improvements
- Branch strategy

## License

[Your License]
```

### 8.2 Update README in Example Branches

**Actions:**
- [ ] Checkout examples/starter
- [ ] Add banner at top of README
- [ ] Commit and push

**Banner to Add:**
```markdown
> **📚 Example Branch**
> This branch demonstrates XrmBedrock usage patterns.
> For the clean template, see the [main branch](https://github.com/your-org/XrmBedrock).
> This branch is automatically validated and kept up-to-date with framework changes.
```

### 8.3 Update CLAUDE.md

**Actions:**
- [ ] Document branch strategy
- [ ] Add note about example branches
- [ ] Update project structure section
- [ ] Commit: `git commit -m "Update CLAUDE.md with branch strategy"`

**Add Section:**
```markdown
## Repository Structure

### Branches

- **main** - Clean framework template (no examples)
  - Use this for `dotnet new` installations
  - Contains only framework infrastructure

- **examples/starter** - Basic working examples
  - Demonstrates fundamental patterns
  - Automatically validated against main

- **examples/advanced** - Complex patterns (optional)
  - Shows advanced scenarios
  - Automatically validated against main

### Example Validation

Examples are automatically validated via CI:
1. Changes to main trigger auto-merge to example branches
2. Example branches must build and pass tests
3. PRs to main are blocked if they would break examples
4. Direct commits to example branches are independently validated

This ensures examples are always fresh and working.
```

### 8.4 Create CONTRIBUTING.md (Optional)

**Actions:**
- [ ] Create CONTRIBUTING.md
- [ ] Document how to improve examples
- [ ] Document framework contribution process
- [ ] Explain CI workflow
- [ ] Commit: `git commit -m "Add contributing guidelines"`

---

## Phase 9: Final Validation

**Status:** ⬜ Not Started

### 9.1 Verify All Branches Build

**Main Branch:**
```bash
git checkout main
git pull
dotnet build --configuration Release
dotnet test --configuration Release
```
- [ ] Clean build succeeds
- [ ] Tests pass
- [ ] No example code present

**examples/starter Branch:**
```bash
git checkout examples/starter
git pull
dotnet build --configuration Release
dotnet test --configuration Release
```
- [ ] Build succeeds with examples
- [ ] Tests pass
- [ ] Examples demonstrate basic patterns

**examples/advanced Branch (if created):**
- [ ] Build succeeds
- [ ] Tests pass
- [ ] Advanced examples work

### 9.2 Test Template Installation End-to-End

**Actions:**
```bash
# Install template from local path (testing)
cd C:\dev\XrmBedrock
git checkout main
dotnet new install .

# Create fresh project
cd C:\dev\test-projects
dotnet new xrmbedrock --name FreshTestProject

# Verify
cd FreshTestProject
dotnet build --configuration Release
```

**Checklist:**
- [ ] Template installs without errors
- [ ] New project created successfully
- [ ] Project structure matches expectations
- [ ] Build succeeds
- [ ] No example code included
- [ ] Framework code is present
- [ ] Uninstall: `dotnet new uninstall XrmBedrock.Template`

### 9.3 Test Auto-Merge Workflow

**Make Framework Change:**
```bash
git checkout main
# Make trivial framework change (e.g., add comment to base class)
git commit -m "Test: Verify auto-merge workflow"
git push
```

**Actions:**
- [ ] Push trivial change to main
- [ ] Monitor CI pipeline
- [ ] Verify `validate-framework` job passes
- [ ] Verify `sync-examples` job runs
- [ ] Verify example branches are auto-updated
- [ ] Check example branches still build/test

**Verify Example Branches:**
```bash
git checkout examples/starter
git pull
# Verify latest commit is auto-merge from main
git log -1
```
- [ ] Auto-merge commit present
- [ ] Branch builds successfully
- [ ] Tests pass

### 9.4 Test Breaking Change Detection

**Make Breaking Change:**
```bash
git checkout -b test/breaking-change main
# Make a breaking change (e.g., rename a base class method used in examples)
git commit -m "Test: Breaking change detection"
git push -u origin test/breaking-change
```

**Create PR:**
- [ ] Create PR: test/breaking-change → main
- [ ] Wait for CI
- [ ] **Expected:** CI should FAIL on example validation
- [ ] Verify PR is blocked
- [ ] Close PR without merging
- [ ] Delete branch

**Notes:**
- This proves the safety net works
- Breaking changes are caught before merge

---

## Safety Checkpoints Summary

✅ **Checkpoint 1:** Example branches created and verified (Phase 2)
✅ **Checkpoint 2:** CI workflow tested without main changes (Phase 3-4)
✅ **Checkpoint 3:** Clean main branch tested in feature branch (Phase 5)
✅ **Checkpoint 4:** Template configuration tested locally (Phase 6)
✅ **Checkpoint 5:** Example branches updated with clean main (Phase 7)
✅ **Checkpoint 6:** Documentation updated (Phase 8)
✅ **Checkpoint 7:** Full system validated (Phase 9)

---

## Rollback Strategy

### If Issues Arise in Phase 1-6
- Example branches are safe (unchanged until Phase 7)
- Main is safe (unchanged until Phase 7)
- Simply abandon feature branches

### If Issues Arise in Phase 7+
- Revert main to commit before feature/clean-main merge
- Example branches retain working state
- Re-apply fixes and retry

### Emergency Rollback
```bash
# If main is broken
git checkout main
git revert <merge-commit-sha>
git push

# If example branches are broken
git checkout examples/starter
git reset --hard <last-good-commit>
git push --force
```

---

## Notes & Decisions

### Branch Naming
- **Decision:** Use `examples/` prefix for example branches
- **Rationale:** Clear namespace, easy to filter in CI

### Merge Conflict Strategy
- **Decision:** Fail loudly on conflicts
- **Rationale:** Manual resolution ensures examples stay meaningful

### Template Naming
- **Decision:** `xrmbedrock` short name
- **Rationale:** Matches solution name, easy to type

### NuGet Publishing
- **Decision:** TBD (local install vs. nuget.org)
- **Options:**
  1. Local only: `dotnet new install <path>`
  2. NuGet.org: `dotnet new install XrmBedrock`
  3. Private feed: `dotnet new install XrmBedrock --nuget-source <feed>`

---

## Future Enhancements

- [ ] Add more example branches (e.g., examples/minimal, examples/enterprise)
- [ ] Automate NuGet package creation
- [ ] Add project wizard/scaffolding tool
- [ ] Create video tutorials using example branches
- [ ] Add example-specific documentation per branch
- [ ] Consider GitHub Pages site generated from examples

---

## Completed Phases

_(Move completed phase checkboxes here for quick reference)_

---

**Last Updated:** 2025-10-01
**Status:** Ready to begin Phase 1
