# XrmBedrock

A dotnet-new template for Dataverse plugins, Custom APIs, TypeScript webresources,
XrmMockup tests and optional Azure services. Requires .NET 10 SDK and Node.js 24 LTS.
Run `nvm install && nvm use` from the solution root (`.nvmrc` selects Node.js 24).
GitHub Actions and Azure DevOps also resolve the latest available 24.x release.

## Create a project

```bash
dotnet new install XrmBedrock
dotnet new xrmbedrock -n MyProject --company-name Acme --solution MySolution --publisher-prefix ctx --dev-url https://yourorg.crm4.dynamics.com
```

Post-setup generates signing keys, restores local dotnet tools, installs npm dependencies,
generates contexts/test metadata and builds the solution. It stops on failure and prints
all tool output, including device-code prompts, to the console and `PostSetup.log`.
The post-setup Release build keeps NuGet vulnerability warnings (`NU1901`–`NU1904`)
visible without treating them as errors, so setup can complete before packages are fixed.
All other build checks and normal Release/CI warning policies remain unchanged.
Fix vulnerable packages before running a normal Release build.
It preserves existing signing keys when rerun and never creates a git commit automatically.
For manual setup use `dotnet run --project Setup/PostSetup`; add `-- --skip-generation`
only when contexts and test metadata have already been generated.

## Dataverse tools

All tools are pinned in `.config/dotnet-tools.json`. Run them **from the solution root**:

```bash
dotnet tool restore
dotnet tool run xrmcontext
dotnet tool run xdt
dotnet tool run xrmmockup-metadata
dotnet build --configuration Release
dotnet test --configuration Release
dotnet tool run xrmsync --dry-run --prefix ctx
dotnet tool run xrmsync --prefix ctx
```

`appsettings.json` is the shared DataverseConnection and generator/sync configuration.
It selects the development URL, solution, output paths and extra entities. Set
`DataverseCredentialType=devicecode` in the environment for headless setup, or `azcli`
to use an authenticated Azure CLI session; `browser` is the default. Browser/device-code
sign-ins are cached per normalized environment URL and reused across all tools.
On headless Linux the cache may use unencrypted files; keep the user's cache private.
Do not commit tokens or secrets. `DataverseUrl` overrides the URL without editing files.
`DOTNET_ENVIRONMENT=test`, `uat` or `prod` selects an environment-specific settings file.

XrmContext writes proxies to `src/Shared/SharedContext/Generated`, XDT writes declarations
and runtime option-set objects to `src/Dataverse/WebResources/typings/XRM`, and XrmMockup
writes metadata to `test/Tests/MetadataGenerated`. XDT uses npm `@types/xrm` and
`@delegateas/xrmquery`; no bundled legacy XrmQuery scripts or executables are needed.

Plugins and Custom APIs inherit from XrmPluginCore through the template's DI base class.
The template includes infrastructure and reusable platform utilities only—no demo
plugins, Custom APIs, form handlers or tests that need deleting before development.
Add your own registrations, services, webresources and tests as needed. The PublishAll
plugin is a platform utility for automatically publishing marked duplicate-detection rules.
Dependencies are merged with a cross-platform ILRepack
MSBuild task. Authenticode signing is optional (`-p:PackAndSignPlugin=true` on Windows),
required if using certificate-based plugin managed identity. Webresources are bundled
from TypeScript into `dist/<prefix>_<project>/` (empty projects build successfully); XrmSync uses that bundle folder and prepends `<publisher>_<solution>/` to each relative path.

## HTML webresource pages

HTML pages live in the existing `src/Dataverse/WebResources` project, alongside form scripts.
The default contains no pages or page templates. Add a folder for each page manually:

```text
src/Dataverse/WebResources/
  src/html/<page-name>/
    index.html
    main.ts
    styles.css
```

Use kebab-case page folders and camel-case TypeScript filenames. Each immediate folder
under `src/html/` containing `index.html` is a separate page; folders without that file
can hold shared code. All three entry files are required, even when the TypeScript or
stylesheet is empty.

### HTML shell

`index.html` is the document shell, not the final deployed file. Include a doctype,
`lang`, a non-empty title and viewport metadata, plus **exactly one** `<!--STYLE-->` and
`<!--SCRIPT-->` marker. The build replaces the markers with bundled CSS and JavaScript:

```html
<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Page title</title>
    <!--STYLE-->
  </head>
  <body>
    <!-- Page markup goes here. -->
    <!--SCRIPT-->
  </body>
</html>
```

Implement browser behavior in `main.ts` and styling in `styles.css`. An unused `main.ts`
can contain `export {};` to remain a TypeScript module; `styles.css` may be empty.
HTML pages execute their bundled JavaScript directly, unlike form-event scripts: they
do not need exported handlers on the `WebResources` global.

### Self-contained assets and security

- Each deployed page must be one self-contained HTML file. Do not add external
  `<script src>`, stylesheet `<link>`, image URLs, `srcset` or CDN references.
- Import JavaScript/TypeScript dependencies from `main.ts`. CSS is bundled separately
  from `styles.css`; local CSS imports and supported image/font assets are embedded.
  Import images from TypeScript or reference local assets with CSS `url(...)` so the
  build converts them to data URLs. Merely writing a local image path in HTML does not
  embed the file. Supported embedded asset formats are PNG, JPG/JPEG, GIF, SVG, WebP,
  WOFF and WOFF2.
- Use safe DOM construction (`createElement`, `append`, `textContent`), not unsanitized
  HTML insertion. Handle rejected promises; do not disable the security checks.
- Use typed `@delegateas/xrmquery` for Dataverse queries and shared generated typings
  under `typings/`. Regenerate types with `dotnet tool run xdt` from the solution root;
  never edit generated declarations or runtime option sets.
- HTML linting enforces accessibility, including labels, image alternative text,
  explicit button types and valid headings. TypeScript uses DataverseHtml's type-aware,
  promise, unsafe DOM/eval/regexp, import, naming and complexity checks.

### Build, preview and removal

Run these commands from `src/Dataverse/WebResources`:

```bash
npm run lint
npm run typecheck
npm run build                    # typecheck + strict lint + scripts and HTML
npm run watch                    # rebuild scripts and HTML on save
```

`npm run build` fails on type or lint errors. Watch reports HTML lint errors without
stopping rebuilds; use a full build before deployment. Restart watch after adding or
removing page folders. Do not edit generated `dist/` files.

Pages become `dist/<prefix>_<project>/html/<page-name>.html`, sharing the existing
package, typings, solution build and XrmSync deployment entry. From the solution root,
run `dotnet tool run xrmsync webresources --profile default` to sync the built resources;
add `--watch` for upload-on-change. Preview a deployed page at
`<DataverseUrl>/WebResources/<publisher>_<solution>/html/<page-name>.html`.
There is no local server or offline runtime.

**Webresource sync includes deletions** (`NoDelete: false`). Removing a page folder,
then running a full build and sync, removes its obsolete deployed resource. Build the
entire webresource project before syncing: XrmSync treats the shared output folder as
the source of truth for the configured solution's JavaScript, HTML and CSS webresources,
and can delete remote resources absent from that folder. Review a dry run before applying
changes, especially when the solution contains resources maintained elsewhere.

## Webresource bundle modes

Webresources use `WebResources.esproj` with the Visual Studio JavaScript SDK.
Open the solution in Visual Studio with JavaScript/TypeScript project support
installed. Build and Clean use the same npm commands as the command line;
builds install locked dependencies with `npm ci`. There is no local web server:
Dataverse hosts the generated bundles. Node.js 24 must be available on `PATH`.


Run these commands from `src/Dataverse/WebResources`:

```bash
npm ci
npm run build                 # defaults to single (package.json config.bundleMode)
npm run build:single          # dist/<prefix>_<project>/WebResources.js
npm run build:individual      # one bundle per source file, preserving relative paths
npm run watch:single
npm run watch:individual
npm test
```

Set `config.bundleMode` in `package.json` to `single` or `individual` to choose the
mode used by ordinary npm builds, post-setup and solution builds. Override it for
one solution build with `dotnet build -p:WebResourcesBundleMode=individual`.
Both modes include dependencies and source maps, exclude `.d.ts` entries, and clear
old output when building or switching modes. Empty projects build successfully.
Restart watch after adding or removing entry files; edits to existing files and
their dependencies rebuild automatically.

In **single** mode, register the same `WebResources.js` library on every form.
In **individual** mode, register the relevant file as before. Export handlers from
TypeScript modules; both modes expose the same file-based global namespace:

```typescript
// src/<prefix>_<project>/forms/account.ts
export function onLoad(context: Xrm.Events.EventContext): void {
    const form = context.getFormContext();
    // Form-specific logic goes here; importing the module should not run it.
}
```

Register `WebResources.forms.account.onLoad` as the event handler and enable
**Pass execution context as first parameter**. Different files may export the same
handler names without collisions. Single mode loads all entry modules, so keep
form-specific work inside handlers rather than executing it at module load time.
Existing handlers explicitly attached to `window` keep their names; unexported
module-local functions are not public event handlers. A source file cannot share
its extensionless path with a source folder (for example `forms.ts` and
`forms/account.ts`) because their public namespaces would conflict.

**XrmSync normally removes obsolete components.** Use `--additive` when deploying to a
shared/existing solution, and always inspect a dry run first. `--prefix` must match the
solution publisher prefix. Sync does not create the Dataverse solution/publisher: create
those in your environment before running the generators or sync.

## Solution export and deployment

PAC handles solution export, import and publish. SolutionExtender captures extended
metadata and performs unmanaged pre-/post-import reconciliation. Both tools are pinned
in `.config/dotnet-tools.json`; there is no DAXIF runtime or F# Interactive dependency.

Export preserves the revision increment, exports with PAC and immediately captures
extended metadata from the same source. Deployment runs **pre-import → synchronous
PAC import → post-import → PAC publish**, stopping on any failure. All phases use
the same unmanaged ZIP. Legacy DAXIF extended artifacts must be freshly exported and
captured with SolutionExtender before deploying; managed reconciliation is unsupported.

**Reconciliation can delete shared components.** Review plans and back up first.
The deployment pipeline applies changes and restores workflow ownership/state.

## Azure DevOps tool authentication

Generation, sync and solution operations run inside `AzureCLI@2` tasks using each
Azure Resource Manager service connection and its authenticated Azure CLI session.
Workload identity federation is supported. PAC uses `--managedIdentity` (default Azure
identity); SolutionExtender uses `--auth azcli`. Both receive the same explicit
`DataverseUrl`. The service connection principal needs Dataverse application-user
permissions; no `DataverseAppId`/`DataverseSecret` variable-group credentials or Power
Platform service connection are needed for solution publishing/deployment.

Dataverse artifacts include the pinned tool manifest, appsettings, merged plugin
assembly and extended ZIP. The optional `CopyUATToTest.yaml`
environment-copy/data-transfer workflow still uses Power Platform connections;
it is separate from solution build/deployment.

# Azure Setup

## Federated Credentials

To configure federated credentials for the Dataverse Managed Identity, run the following script. It loads the generated `plugincert.pfx` and prints the issuer, subject, thumbprint, and hash needed for Azure AD configuration:

```bash
pwsh Setup/printFederatedCredentials.ps1 -password "<your-cert-password>" -environmentId "<environment-guid>" -tenantId "<tenant-guid>"
```

## Storage account environment variable

Create a new environment variable that will contain the storage account URL.

Update the reference to that variable in `src/Dataverse/SharedPluginLogic/Logic/Azure/AzureConfigSetter.cs`.

## Infrastructure validation

To locally validate your `main.bicep`, run:

```bash
az login
az deployment group validate --resource-group <your-resource-group> --template-file main.bicep
```

Note: When using `dotnet new`, `solutionId` and `companyId` in `Infrastructure/main.bicep` are set automatically via template parameters.

# Azure DevOps

## Environment
Under Pipelines > Environment, create an environment per Dataverse environment.
Note: The pipeline template uses Dev, Test, UAT, Prod.
Use these to control approvals of deployments, regarding approval gates etc.

## Library
Under Pipelines > Library, create a variable group per environment.
Note: The pipeline template uses Dev, Test, UAT, Prod.
The template assumes the following variables exist.
* ResourceGroupName
* DataverseUrl
* AzureClientId
* AzureTenantId (Needed for the managed identity record)
* AzureClientEAObjectId (Object id of the Enterprise Application related to the App registration)

## Service Connection
Create one **Azure Resource Manager** service connection per environment, named
`Dev`, `Test`, `UAT` and `Prod`, using workload identity federation. Give its principal
Azure permissions and the Dataverse application-user permissions needed by the modern
tools. No Power Platform connection is needed for solution publishing/deployment.

## App registration privileges
Remember to give your app reg permission to assign roles.
The easiest method is to add it as owner to the subscription and restrict the role assignment to the ones bicep assigns.
The template uses Storage Queue Data Contributor

## Managed identity
A managed identity is created by the bicep deploy. This is what Azure uses to call back into Dataverse. Make sure it is created as an app user. Search for the client id of the managed identity, you will not find it by name.

## Pipeline and PR validation
Remember to "uncomment" the `Build.yaml` workflow trigger, for use of build validation in pull requests.

The Azure DevOps pipelines live in `.pipelines/`. Entry-point pipelines sit at the root; all reusable steps/jobs/stages live under `.pipelines/templates/`:

* `Build.yaml` / `BuildAndDeploy.yaml` — the default pipelines, running build + test in a single job.
* `Build.Parallel.yaml` / `BuildAndDeploy.Parallel.yaml` — an alternative that scales across many build agents: it splits the work into parallel `Build`, `Analyze`, and `Test` stages, caches NuGet packages, and reuses a published build artifact so tests run without recompiling. The `Test` stage is a single job by default; see the commented `matrix` example in `Build.Parallel.yaml` for how to shard tests across agents. Register whichever pair fits your build capacity.

## TODO
* Improve validation of infrastructure to be easier to manage
* Auto set storage account environment variable
* Auto user creation for managed identity
