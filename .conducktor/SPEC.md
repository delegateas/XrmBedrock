# PCF Support for XrmBedrock

## Context

XrmBedrock currently has no first-class PowerApps Component Framework (PCF) support. This spec adds it. One `.pcfproj` hosts every control via the sibling-folder convention; controls are auto-discovered by `pcf-scripts` from the presence of `ControlManifest.Input.xml`. There is no separately-maintained `.cdsproj` per control — deployment runs entirely through `pac cli`.

Local verification runs entirely **offline** via Playwright against an HTML harness wired to a vendored mock context. No Dataverse connection is required for tests, which makes the suite suitable for headless CI and for coding agents that need to read pass/fail from stdout and inspect screenshots, videos, and the HTML report.

Deployment reuses the existing per-environment Power Platform service connections (Dev / Test / UAT / Prod). The pipeline pushes to a throwaway solution `XrmBedrockPCFTemp`, then an F# script moves every `customcontrol` (component type `66`) into the target solution defined in `_Config.fsx`, then deletes the throwaway. This dance happens every pipeline run; all PCFs in the repo are deployed together.

The vendored mock is cherry-picked from `Shko-Online/ComponentFramework-Mock` (MIT). Class names mirror upstream so code lifted directly from upstream slots in. If during implementation we find we need >70% of upstream surface, abandon cherry-picking and copy the full `src/` tree as-is.

This document is a **handover spec**: every config file, sample control, the mock skeleton, the harness, a full Playwright spec, and the F# move-and-clean script are sketched concretely below. An implementer (human or coding agent) should not need to read PCF documentation or the Shko-Online repo to complete this work.

## Goals

- Single `.pcfproj` hosting many controls, sibling-folder convention.
- Four sample controls covering the full matrix: `{field, dataset} × {vanilla TS, React virtual}`.
- Deployment via `pac cli` only — no separately-maintained `.cdsproj`.
- Local end-to-end verification via Playwright against an HTML harness using a vendored mock context.
- Coding agents can run the full suite headlessly and verify outcomes by reading screenshots, videos, and the HTML report.
- Reuse existing per-environment Power Platform service connections (Dev / Test / UAT / Prod).
- All PCFs in the repo deployed together, every pipeline run, into the existing target solution.

## Non-goals

- Storybook or any per-component unit testing layer.
- Maintaining a separate `.cdsproj` per control.
- Optimizing for `pac pcf debug` / live-reload against a real environment — agents and humans both work against the local harness.
- Visual regression diffing (defer; can layer on later).
- Cross-browser testing — Chromium only, since model-driven apps run on Chromium-based hosts.

## Requirements

- **Project root:** `src/Dataverse/PCFs/XrmBedrock.PCFs/`. Every control is a sibling folder under it. Controls are auto-discovered via the presence of `ControlManifest.Input.xml`. `package.json`, `tsconfig.json`, `pcfconfig.json`, `.eslintrc.json`, `.nvmrc`, and `XrmBedrock.PCFs.pcfproj` live at this root.
- **Sample controls:** four samples named `SampleFieldVanilla`, `SampleFieldReact`, `SampleDatasetVanilla`, `SampleDatasetReact`, all under namespace `XrmBedrock`. Field samples bind a single property; dataset samples bind a `data-set` named `records` with a property-set `primaryName`. React samples are virtual (`control-type="virtual"`) and declare `<platform-library name="React" version="16.14.0" />` and `<platform-library name="Fluent" version="9.46.2" />`.
- **React pinning:** React is pinned at `16.14.0` in `package.json` because that's the version model-driven apps load for virtual controls. If the platform moves to 17/18, bump here; for now match the host. Fluent pinned at `^9.46.0` in npm and `9.46.2` in manifests.
- **Mock:** vendored under `test/mock/`. Cherry-pick from `Shko-Online/ComponentFramework-Mock` (MIT). Class names mirror upstream: `ContextMock`, `DatasetMock`, `StringPropertyMock`, `NumberPropertyMock`, `ComponentFrameworkMockGenerator`. `UPSTREAM.md` records source repo URL, commit SHA, copy date, and the **Known gaps** fix backlog (see Mock fidelity policy below). `LICENSE` file checked in alongside.
- **Mock initial scope (each entry is a tracked gap, not a permanent deviation):** metadata mock subsystem absent, localization/translation harness absent, `formatting`/`webAPI`/`navigation`/`utils` stubbed as empty objects. Casts use `as never` / `as unknown` for fields the four samples never read. Each of these is recorded as an `open` entry under `Known gaps` in `test/mock/UPSTREAM.md` at vendoring time and is expected to be closed (mock surface filled in) as controls land that need it.
- **Harness:** `test/harness/index.html` + `mount.ts` + `registry.ts`, bundled by `esbuild`. URL pattern `http://127.0.0.1:4173/?control=<ControlName>&seed=<base64-encoded-json>`. Test hooks exposed on `window.__pcf` (see Phase 4 for the full surface).
- **Playwright:** Chromium-only. Config under `test/playwright/playwright.config.ts`. `webServer` auto-starts the esbuild dev server on port 4173. `fullyParallel: false`, `workers: 1`, `timeout: 30_000`, `retries: 0`. Reporters: `list` (stdout for agents), `html` (open=`never`), `junit`. `trace`/`video`/`screenshot` retained on failure.
- **Test count:** 12 (3 per control × 4 controls). Total runtime budget under 60s on a clean clone.
- **Deterministic output paths (agent-friendly):** `test/playwright-report/index.html`, `test/junit-results.xml`, `test/test-results/<spec>-<test>-<label>.png`, `test/test-results/**/video.webm`.
- **Exclude `test/**` from `.pcfproj`** so `pcf-scripts` doesn't try to type-check Playwright specs and the harness as part of the control build.
- **Deployment dance (every pipeline run):** authenticate via existing Power Platform service connection → pre-clean (delete `XrmBedrockPCFTemp` if present) → `pac pcf push --publisher-prefix <prefix> --solution-name XrmBedrockPCFTemp` → F# script moves every `customcontrol` (component type `66`) from temp to target → F# script deletes the throwaway.
- **Naming:** throwaway solution `XrmBedrockPCFTemp` (overridable per pipeline run); target solution + publisher prefix read from `_Config.fsx` (`PublisherInfo`), matching plugins/webresources today.
- **Auth:** reuse existing `PowerPlatformSPN` service connections (`Dataverse Dev`, `Dataverse Test`, `Dataverse UAT`, `Dataverse Prod`) and the existing library variables `DataverseUrl` / `DataverseAppId` / `DataverseSecret`. No new service connections, no new app registrations, no library-variable-group changes.
- **Pipeline ordering within each environment stage:** (1) Plugins build & sign (existing) → (2) WebResources build (existing) → (3) **PCF build + Playwright tests (new — fast offline gate)** → (4) Solution import (existing — establishes target solution if missing) → (5) **PCF push & move (new — runs after target solution exists)** → (6) DAXIF post-deploy steps (existing). Step 3 is positioned deliberately: offline + fast, so regressions fail before consuming environment time.
- **Manifest versioning policy.** Each `ControlManifest.Input.xml` ships with `version="0.0.1"` checked in. The pipeline auto-bumps **only the patch component** (the third / lowest number in `major.minor.patch`) from the build identifier — `major` and `minor` stay developer-controlled so they can hand-bump for breaking/feature changes. Patch is rewritten to `$(Build.BuildId)` immediately before `pac pcf push`, so every pipeline run produces a strictly increasing patch number. Local iteration (`npm run build:pcf`) leaves the checked-in version untouched, so there is no drift between local and pipeline behaviour: developers never bump patch by hand, and the pipeline never touches major/minor.
- **Localization policy.** Every sample control ships with **two** locales out of the box via the standard `strings/<ControlName>.<lcid>.resx` mechanism: **en-US (LCID `1033`)** and **da-DK (LCID `1030`)**. Both `.resx` files live under each control's `strings/` subfolder and are referenced from the control's `<resources>` block as `<resx path="strings/<ControlName>.<lcid>.resx" version="1.0.0" />`. Every `*-key` (display-name and description) referenced from a `ControlManifest.Input.xml` must have a `<data name="…">` entry in **both** resx files — there are no fallback-only keys. Adding a new locale later means dropping a new `<ControlName>.<lcid>.resx` next to the existing pair and adding a `<resx>` line to each manifest; no other spec or pipeline change required.
- **Mock fidelity policy.** The vendored mock is a living artifact expected to track real Dataverse runtime behaviour, not a frozen approximation. The mock starts deliberately minimal (only what the four samples exercise), but every divergence from real runtime — discovered by a failing Playwright test, by a behaviour difference observed against a real environment during deployment validation, or by a contributor reading the mock — is recorded in `test/mock/UPSTREAM.md` under a **Known gaps** section as a tracked fix backlog, not a "buyer beware" disclosure. Each entry captures: what's missing or wrong, where it was discovered, current status (`open` / `in progress` / `closed-<commit-sha>`). Agents and contributors are expected to **close** gaps (extend the mock toward fidelity) rather than route around them. New controls that hit unstubbed surface area (e.g. `formatting`, `webAPI`, metadata) trigger a gap entry plus the work to implement that surface in the mock; a stubbed `as never` field is acceptable as a temporary state only when accompanied by a logged gap. The gap list is the canonical place to find "what fidelity work remains" — kept current so an agent can pick the next item without spelunking the codebase.

## Open questions

- **Visual regression.** `expect(page).toHaveScreenshot()` with stored baselines is a small additional layer; deliberately omitted to keep "few but powerful" tests honest.
- **`platform-library` Fluent version.** Pinned at `9.46.2` in the manifests — confirm against the version Microsoft currently ships in MDA at implementation time. If different, bump in both manifests and `package.json`.
- **`pcf-scripts` React peer.** If `pcf-scripts` doesn't accept React 16.14 cleanly under modern Node, fall back to whatever React version `pcf-scripts` resolves and update `App.tsx` accordingly.
- **`_Config.fsx` connection helper contract.** The F# script in §6.2 uses pseudocode (`AuthType=ClientSecret;...`); the implementer must wire it to whatever `XrmConnection` helper the rest of XrmBedrock already uses (shared from `_Config.fsx`).

## Phases

### Phase 1: Project scaffolding

Create the PCF project root at `src/Dataverse/PCFs/XrmBedrock.PCFs/` with all root config files. No control code yet; this phase establishes the build environment.

Folder layout to create (parent only):

```
src/Dataverse/PCFs/
  XrmBedrock.PCFs/
    XrmBedrock.PCFs.pcfproj
    package.json
    tsconfig.json
    pcfconfig.json
    .eslintrc.json
    .nvmrc
```

**`XrmBedrock.PCFs.pcfproj`** — note the `ExcludeDirectories` for `test/**` is critical (without it `pcf-scripts` will try to type-check Playwright specs and the harness as part of the control build). The `<None Include>` lines list all four manifests up-front so the project is ready for Phase 2 to populate them:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <ProjectTypeGuids>{4F65AAD9-5749-4A0B-AAD2-AAEB5A3D3CB4};{9092AA53-FB77-4645-B42D-1CCCA6BD08BD}</ProjectTypeGuids>
    <PowerAppsTargetsPath>$(MSBuildExtensionsPath)\Microsoft\VisualStudio\v$(VisualStudioVersion)\PowerApps</PowerAppsTargetsPath>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.PowerApps.MSBuild.Pcf" Version="1.*" />
  </ItemGroup>

  <ItemGroup>
    <ExcludeDirectories Include="$(MSBuildThisFileDirectory)node_modules\**" />
    <ExcludeDirectories Include="$(MSBuildThisFileDirectory)test\**" />
  </ItemGroup>

  <ItemGroup>
    <None Include="SampleFieldVanilla\ControlManifest.Input.xml" />
    <None Include="SampleFieldReact\ControlManifest.Input.xml" />
    <None Include="SampleDatasetVanilla\ControlManifest.Input.xml" />
    <None Include="SampleDatasetReact\ControlManifest.Input.xml" />
  </ItemGroup>
</Project>
```

**`package.json`** — React pinned at `16.14.0` (matches MDA host); Fluent at `^9.46.0`; Playwright `^1.45.0`; `pcf-scripts`/`pcf-start` at `^1.36.0`; TypeScript `^5.4.0`; esbuild `^0.21.0`:

```json
{
  "name": "xrmbedrock-pcfs",
  "version": "0.1.0",
  "private": true,
  "scripts": {
    "build:pcf": "pcf-scripts build",
    "build:harness": "node test/harness/esbuild.config.mjs",
    "serve:harness": "node test/harness/esbuild.config.mjs --serve",
    "test": "npm run build:harness && playwright test --config test/playwright/playwright.config.ts",
    "test:headed": "npm run build:harness && playwright test --config test/playwright/playwright.config.ts --headed",
    "test:report": "playwright show-report test/playwright-report",
    "lint": "eslint . --ext .ts,.tsx"
  },
  "dependencies": {
    "react": "16.14.0",
    "react-dom": "16.14.0",
    "@fluentui/react-components": "^9.46.0"
  },
  "devDependencies": {
    "@playwright/test": "^1.45.0",
    "@types/node": "^20.0.0",
    "@types/powerapps-component-framework": "^1.3.15",
    "@types/react": "16.14.0",
    "@types/react-dom": "16.14.0",
    "esbuild": "^0.21.0",
    "eslint": "^8.57.0",
    "pcf-scripts": "^1.36.0",
    "pcf-start": "^1.36.0",
    "typescript": "^5.4.0"
  }
}
```

**`tsconfig.json`** — extends `pcf-scripts` base; `paths` aliases `@mock` to `test/mock` so harness/registry can import the vendored mock cleanly:

```json
{
  "extends": "./node_modules/pcf-scripts/tsconfig_base.json",
  "compilerOptions": {
    "target": "ES2020",
    "module": "ESNext",
    "moduleResolution": "Node",
    "jsx": "react",
    "esModuleInterop": true,
    "strict": true,
    "skipLibCheck": true,
    "baseUrl": ".",
    "paths": {
      "@mock": ["test/mock"],
      "@mock/*": ["test/mock/*"]
    }
  },
  "include": [
    "**/*.ts",
    "**/*.tsx"
  ],
  "exclude": [
    "node_modules",
    "out"
  ]
}
```

**`pcfconfig.json`**:

```json
{
  "featureFlags": {
    "pcfAllowLateBound": "off"
  }
}
```

**`.eslintrc.json`**:

```json
{
  "extends": ["eslint:recommended"],
  "parser": "@typescript-eslint/parser",
  "plugins": ["@typescript-eslint"],
  "ignorePatterns": ["node_modules", "out", "test/harness/dist"]
}
```

**`.nvmrc`**:

```
20
```

**Verifiable when:** `cd src/Dataverse/PCFs/XrmBedrock.PCFs && npm install` succeeds; `npm run lint` runs (with no controls to lint yet, exits 0); `dotnet build` of the `.pcfproj` reports the project loads (it will report "no controls" until Phase 2 populates them — that's expected).

### Phase 2: Implement the four sample controls

Add four sibling folders under `src/Dataverse/PCFs/XrmBedrock.PCFs/`. Each contains a `ControlManifest.Input.xml`, an `index.ts`, and (for vanilla controls) CSS, or (for React virtual controls) an `App.tsx`. All under namespace `XrmBedrock`. After Phase 2, `npm run build:pcf` should succeed.

Folder layout to create (every control ships with both `en-US` (LCID `1033`) and `da-DK` (LCID `1030`) resx files under `strings/`; full content in §2.5):

```
SampleFieldVanilla/
  ControlManifest.Input.xml
  index.ts
  css/SampleFieldVanilla.css
  strings/SampleFieldVanilla.1033.resx
  strings/SampleFieldVanilla.1030.resx
SampleFieldReact/
  ControlManifest.Input.xml
  index.ts
  App.tsx
  strings/SampleFieldReact.1033.resx
  strings/SampleFieldReact.1030.resx
SampleDatasetVanilla/
  ControlManifest.Input.xml
  index.ts
  css/SampleDatasetVanilla.css
  strings/SampleDatasetVanilla.1033.resx
  strings/SampleDatasetVanilla.1030.resx
SampleDatasetReact/
  ControlManifest.Input.xml
  index.ts
  App.tsx
  strings/SampleDatasetReact.1033.resx
  strings/SampleDatasetReact.1030.resx
```

#### 2.1 SampleFieldVanilla — standard control, single bound `SingleLine.Text` property

**`SampleFieldVanilla/ControlManifest.Input.xml`**:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<manifest>
  <control namespace="XrmBedrock"
           constructor="SampleFieldVanilla"
           version="0.0.1"
           display-name-key="SampleFieldVanilla_Display_Key"
           description-key="SampleFieldVanilla_Desc_Key"
           control-type="standard">
    <property name="value"
              display-name-key="value_Display_Key"
              description-key="value_Desc_Key"
              of-type="SingleLine.Text"
              usage="bound"
              required="true" />
    <resources>
      <code path="index.ts" order="1" />
      <css path="css/SampleFieldVanilla.css" order="1" />
      <resx path="strings/SampleFieldVanilla.1033.resx" version="1.0.0" />
      <resx path="strings/SampleFieldVanilla.1030.resx" version="1.0.0" />
    </resources>
  </control>
</manifest>
```

**`SampleFieldVanilla/index.ts`**:

```ts
import { IInputs, IOutputs } from "./generated/ManifestTypes";

export class SampleFieldVanilla implements ComponentFramework.StandardControl<IInputs, IOutputs> {
    private container: HTMLDivElement;
    private input: HTMLInputElement;
    private currentValue: string = "";
    private notifyOutputChanged: () => void;

    public init(
        context: ComponentFramework.Context<IInputs>,
        notifyOutputChanged: () => void,
        _state: ComponentFramework.Dictionary,
        container: HTMLDivElement
    ): void {
        this.container = container;
        this.notifyOutputChanged = notifyOutputChanged;

        this.input = document.createElement("input");
        this.input.type = "text";
        this.input.classList.add("xrmbedrock-field-vanilla");
        this.input.setAttribute("data-testid", "field-vanilla-input");
        this.input.addEventListener("input", this.handleInput);
        this.container.appendChild(this.input);
    }

    public updateView(context: ComponentFramework.Context<IInputs>): void {
        const incoming = context.parameters.value.raw ?? "";
        if (incoming !== this.currentValue) {
            this.currentValue = incoming;
            this.input.value = incoming;
        }
    }

    public getOutputs(): IOutputs {
        return { value: this.currentValue };
    }

    public destroy(): void {
        this.input.removeEventListener("input", this.handleInput);
    }

    private handleInput = (): void => {
        this.currentValue = this.input.value;
        this.notifyOutputChanged();
    };
}
```

**`SampleFieldVanilla/css/SampleFieldVanilla.css`**:

```css
.xrmbedrock-field-vanilla {
  width: 100%;
  padding: 6px 8px;
  font: inherit;
  border: 1px solid #c8c6c4;
  border-radius: 2px;
}
.xrmbedrock-field-vanilla:focus {
  outline: 2px solid #0078d4;
  outline-offset: -2px;
}
```

#### 2.2 SampleFieldReact — virtual control, bound `Whole.None` value with `min`/`max` inputs

**`SampleFieldReact/ControlManifest.Input.xml`** — note `control-type="virtual"`, `external-service-usage enabled="false"`, and the two `<platform-library>` entries (React `16.14.0`, Fluent `9.46.2`):

```xml
<?xml version="1.0" encoding="utf-8" ?>
<manifest>
  <control namespace="XrmBedrock"
           constructor="SampleFieldReact"
           version="0.0.1"
           display-name-key="SampleFieldReact_Display_Key"
           description-key="SampleFieldReact_Desc_Key"
           control-type="virtual">
    <external-service-usage enabled="false" />
    <property name="value"
              display-name-key="value_Display_Key"
              description-key="value_Desc_Key"
              of-type="Whole.None"
              usage="bound"
              required="true" />
    <property name="min"
              display-name-key="min_Display_Key"
              description-key="min_Desc_Key"
              of-type="Whole.None"
              usage="input"
              default-value="0" />
    <property name="max"
              display-name-key="max_Display_Key"
              description-key="max_Desc_Key"
              of-type="Whole.None"
              usage="input"
              default-value="100" />
    <resources>
      <code path="index.ts" order="1" />
      <platform-library name="React" version="16.14.0" />
      <platform-library name="Fluent" version="9.46.2" />
      <resx path="strings/SampleFieldReact.1033.resx" version="1.0.0" />
      <resx path="strings/SampleFieldReact.1030.resx" version="1.0.0" />
    </resources>
  </control>
</manifest>
```

**`SampleFieldReact/index.ts`**:

```ts
import * as React from "react";
import { App, AppProps } from "./App";
import { IInputs, IOutputs } from "./generated/ManifestTypes";

export class SampleFieldReact implements ComponentFramework.ReactControl<IInputs, IOutputs> {
    private currentValue: number = 0;
    private notifyOutputChanged: () => void;

    public init(
        _context: ComponentFramework.Context<IInputs>,
        notifyOutputChanged: () => void
    ): void {
        this.notifyOutputChanged = notifyOutputChanged;
    }

    public updateView(context: ComponentFramework.Context<IInputs>): React.ReactElement {
        const props: AppProps = {
            value: context.parameters.value.raw ?? 0,
            min: context.parameters.min.raw ?? 0,
            max: context.parameters.max.raw ?? 100,
            onChange: (next: number) => {
                this.currentValue = next;
                this.notifyOutputChanged();
            }
        };
        return React.createElement(App, props);
    }

    public getOutputs(): IOutputs {
        return { value: this.currentValue };
    }

    public destroy(): void { /* no-op */ }
}
```

**`SampleFieldReact/App.tsx`** — uses Fluent v9 `SpinButton` inside a `FluentProvider`. The `data-testid` cast satisfies TS that `SpinButton`'s `input` prop accepts arbitrary HTML attributes:

```tsx
import * as React from "react";
import {
    FluentProvider,
    webLightTheme,
    SpinButton,
    SpinButtonOnChangeData,
    SpinButtonChangeEvent
} from "@fluentui/react-components";

export interface AppProps {
    value: number;
    min: number;
    max: number;
    onChange: (next: number) => void;
}

export const App: React.FC<AppProps> = ({ value, min, max, onChange }) => {
    const handleChange = (
        _ev: SpinButtonChangeEvent,
        data: SpinButtonOnChangeData
    ) => {
        const next = data.value ?? Number(data.displayValue ?? value);
        if (Number.isFinite(next)) onChange(next);
    };

    return (
        <FluentProvider theme={webLightTheme}>
            <SpinButton
                value={value}
                min={min}
                max={max}
                onChange={handleChange}
                input={{ "data-testid": "field-react-input" } as React.InputHTMLAttributes<HTMLInputElement>}
            />
        </FluentProvider>
    );
};
```

#### 2.3 SampleDatasetVanilla — standard control with a bound `data-set` named `records`

**`SampleDatasetVanilla/ControlManifest.Input.xml`**:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<manifest>
  <control namespace="XrmBedrock"
           constructor="SampleDatasetVanilla"
           version="0.0.1"
           display-name-key="SampleDatasetVanilla_Display_Key"
           description-key="SampleDatasetVanilla_Desc_Key"
           control-type="standard">
    <data-set name="records" display-name-key="records_Display_Key">
      <property-set name="primaryName"
                    display-name-key="primaryName_Display_Key"
                    description-key="primaryName_Desc_Key"
                    of-type="SingleLine.Text"
                    usage="bound"
                    required="true" />
    </data-set>
    <resources>
      <code path="index.ts" order="1" />
      <css path="css/SampleDatasetVanilla.css" order="1" />
      <resx path="strings/SampleDatasetVanilla.1033.resx" version="1.0.0" />
      <resx path="strings/SampleDatasetVanilla.1030.resx" version="1.0.0" />
    </resources>
  </control>
</manifest>
```

**`SampleDatasetVanilla/index.ts`** — renders a `<table data-testid="dataset-vanilla-table">`, wires row `click` to `ds.openDatasetItem(record.getNamedReference())`, and adds a "Next page" button when `ds.paging?.hasNextPage`:

```ts
import { IInputs, IOutputs } from "./generated/ManifestTypes";

export class SampleDatasetVanilla implements ComponentFramework.StandardControl<IInputs, IOutputs> {
    private container: HTMLDivElement;
    private context: ComponentFramework.Context<IInputs>;

    public init(
        context: ComponentFramework.Context<IInputs>,
        _notifyOutputChanged: () => void,
        _state: ComponentFramework.Dictionary,
        container: HTMLDivElement
    ): void {
        this.container = container;
        this.context = context;
    }

    public updateView(context: ComponentFramework.Context<IInputs>): void {
        this.context = context;
        this.render();
    }

    private render(): void {
        const ds = this.context.parameters.records;
        this.container.innerHTML = "";

        const table = document.createElement("table");
        table.classList.add("xrmbedrock-dataset-vanilla");
        table.setAttribute("data-testid", "dataset-vanilla-table");

        const thead = document.createElement("thead");
        const headRow = document.createElement("tr");
        ds.columns.forEach(col => {
            const th = document.createElement("th");
            th.textContent = col.displayName;
            th.dataset.columnName = col.name;
            headRow.appendChild(th);
        });
        thead.appendChild(headRow);
        table.appendChild(thead);

        const tbody = document.createElement("tbody");
        ds.sortedRecordIds.forEach(id => {
            const record = ds.records[id];
            const tr = document.createElement("tr");
            tr.setAttribute("data-record-id", id);
            tr.addEventListener("click", () => {
                ds.openDatasetItem(record.getNamedReference());
            });
            ds.columns.forEach(col => {
                const td = document.createElement("td");
                td.textContent = String(record.getFormattedValue(col.name) ?? "");
                tr.appendChild(td);
            });
            tbody.appendChild(tr);
        });
        table.appendChild(tbody);
        this.container.appendChild(table);

        if (ds.paging?.hasNextPage) {
            const next = document.createElement("button");
            next.textContent = "Next page";
            next.setAttribute("data-testid", "dataset-vanilla-next");
            next.addEventListener("click", () => ds.paging.loadNextPage());
            this.container.appendChild(next);
        }
    }

    public getOutputs(): IOutputs {
        return {};
    }

    public destroy(): void {
        this.container.innerHTML = "";
    }
}
```

**`SampleDatasetVanilla/css/SampleDatasetVanilla.css`**:

```css
.xrmbedrock-dataset-vanilla { width: 100%; border-collapse: collapse; font: inherit; }
.xrmbedrock-dataset-vanilla th, .xrmbedrock-dataset-vanilla td { padding: 6px 8px; border-bottom: 1px solid #edebe9; text-align: left; }
.xrmbedrock-dataset-vanilla tbody tr { cursor: pointer; }
.xrmbedrock-dataset-vanilla tbody tr:hover { background: #f3f2f1; }
```

#### 2.4 SampleDatasetReact — virtual control rendering Fluent v9 `DataGrid`

**`SampleDatasetReact/ControlManifest.Input.xml`**:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<manifest>
  <control namespace="XrmBedrock"
           constructor="SampleDatasetReact"
           version="0.0.1"
           display-name-key="SampleDatasetReact_Display_Key"
           description-key="SampleDatasetReact_Desc_Key"
           control-type="virtual">
    <external-service-usage enabled="false" />
    <data-set name="records" display-name-key="records_Display_Key">
      <property-set name="primaryName"
                    display-name-key="primaryName_Display_Key"
                    description-key="primaryName_Desc_Key"
                    of-type="SingleLine.Text"
                    usage="bound"
                    required="true" />
    </data-set>
    <resources>
      <code path="index.ts" order="1" />
      <platform-library name="React" version="16.14.0" />
      <platform-library name="Fluent" version="9.46.2" />
      <resx path="strings/SampleDatasetReact.1033.resx" version="1.0.0" />
      <resx path="strings/SampleDatasetReact.1030.resx" version="1.0.0" />
    </resources>
  </control>
</manifest>
```

**`SampleDatasetReact/index.ts`**:

```ts
import * as React from "react";
import { App, AppProps } from "./App";
import { IInputs, IOutputs } from "./generated/ManifestTypes";

export class SampleDatasetReact implements ComponentFramework.ReactControl<IInputs, IOutputs> {
    public init(_context: ComponentFramework.Context<IInputs>, _notify: () => void): void { /* no-op */ }

    public updateView(context: ComponentFramework.Context<IInputs>): React.ReactElement {
        const ds = context.parameters.records;
        const props: AppProps = {
            columns: ds.columns.map(c => ({ key: c.name, name: c.displayName })),
            rows: ds.sortedRecordIds.map(id => {
                const r = ds.records[id];
                const row: Record<string, string> = { __id: id };
                ds.columns.forEach(c => row[c.name] = String(r.getFormattedValue(c.name) ?? ""));
                return row;
            }),
            onSelectionChange: (selectedIds) => ds.setSelectedRecordIds(selectedIds),
            onOpen: (id) => ds.openDatasetItem(ds.records[id].getNamedReference())
        };
        return React.createElement(App, props);
    }

    public getOutputs(): IOutputs { return {}; }
    public destroy(): void { /* no-op */ }
}
```

**`SampleDatasetReact/App.tsx`**:

```tsx
import * as React from "react";
import {
    FluentProvider,
    webLightTheme,
    DataGrid,
    DataGridHeader,
    DataGridHeaderCell,
    DataGridBody,
    DataGridRow,
    DataGridCell,
    TableColumnDefinition,
    createTableColumn
} from "@fluentui/react-components";

export interface AppProps {
    columns: { key: string; name: string }[];
    rows: Record<string, string>[];
    onSelectionChange: (selectedIds: string[]) => void;
    onOpen: (id: string) => void;
}

export const App: React.FC<AppProps> = ({ columns, rows, onSelectionChange, onOpen }) => {
    const colDefs: TableColumnDefinition<Record<string, string>>[] = React.useMemo(
        () => columns.map(c => createTableColumn<Record<string, string>>({
            columnId: c.key,
            renderHeaderCell: () => c.name,
            renderCell: (item) => item[c.key]
        })),
        [columns]
    );

    return (
        <FluentProvider theme={webLightTheme}>
            <DataGrid
                items={rows}
                columns={colDefs}
                getRowId={(item) => item.__id}
                selectionMode="multiselect"
                onSelectionChange={(_, data) => onSelectionChange(Array.from(data.selectedItems) as string[])}
                data-testid="dataset-react-grid"
            >
                <DataGridHeader>
                    <DataGridRow>
                        {({ renderHeaderCell }) => (
                            <DataGridHeaderCell>{renderHeaderCell()}</DataGridHeaderCell>
                        )}
                    </DataGridRow>
                </DataGridHeader>
                <DataGridBody<Record<string, string>>>
                    {({ item, rowId }) => (
                        <DataGridRow<Record<string, string>>
                            key={rowId}
                            onDoubleClick={() => onOpen(item.__id)}
                            data-record-id={item.__id}
                        >
                            {({ renderCell }) => <DataGridCell>{renderCell(item)}</DataGridCell>}
                        </DataGridRow>
                    )}
                </DataGridBody>
            </DataGrid>
        </FluentProvider>
    );
};
```

#### 2.5 Localization resources — en-US (1033) + da-DK (1030)

Every control ships with two `.resx` files referenced from its manifest's `<resources>` block (already shown in §§2.1–2.4). LCIDs: **1033 = en-US**, **1030 = da-DK**. Every `*-key` referenced from a manifest must have a matching `<data name="…">` entry in **both** resx files for that control — there are no fallback-only keys.

All eight resx files share the same root/header boilerplate; only the `<data>` entries differ. Use this template for every file (the `xsd:schema` block is the standard Visual Studio resx schema; copy it verbatim):

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype"><value>text/microsoft-resx</value></resheader>
  <resheader name="version"><value>2.0</value></resheader>
  <resheader name="reader"><value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value></resheader>
  <resheader name="writer"><value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value></resheader>
  <!-- <data name="…" xml:space="preserve"><value>…</value></data> entries from the table below -->
</root>
```

Per-file `<data>` entries (insert in place of the `<!-- … -->` placeholder above):

**`SampleFieldVanilla/strings/SampleFieldVanilla.1033.resx`** (en-US):

```xml
  <data name="SampleFieldVanilla_Display_Key" xml:space="preserve"><value>Sample Field (Vanilla)</value></data>
  <data name="SampleFieldVanilla_Desc_Key"    xml:space="preserve"><value>Vanilla TypeScript field sample control</value></data>
  <data name="value_Display_Key"              xml:space="preserve"><value>Value</value></data>
  <data name="value_Desc_Key"                 xml:space="preserve"><value>Bound text value</value></data>
```

**`SampleFieldVanilla/strings/SampleFieldVanilla.1030.resx`** (da-DK):

```xml
  <data name="SampleFieldVanilla_Display_Key" xml:space="preserve"><value>Eksempelfelt (Vanilla)</value></data>
  <data name="SampleFieldVanilla_Desc_Key"    xml:space="preserve"><value>Vanilla TypeScript-eksempelfeltkontrolelement</value></data>
  <data name="value_Display_Key"              xml:space="preserve"><value>Værdi</value></data>
  <data name="value_Desc_Key"                 xml:space="preserve"><value>Bunden tekstværdi</value></data>
```

**`SampleFieldReact/strings/SampleFieldReact.1033.resx`** (en-US):

```xml
  <data name="SampleFieldReact_Display_Key" xml:space="preserve"><value>Sample Field (React)</value></data>
  <data name="SampleFieldReact_Desc_Key"    xml:space="preserve"><value>React virtual field sample control</value></data>
  <data name="value_Display_Key"            xml:space="preserve"><value>Value</value></data>
  <data name="value_Desc_Key"               xml:space="preserve"><value>Bound numeric value</value></data>
  <data name="min_Display_Key"              xml:space="preserve"><value>Min</value></data>
  <data name="min_Desc_Key"                 xml:space="preserve"><value>Minimum allowed value</value></data>
  <data name="max_Display_Key"              xml:space="preserve"><value>Max</value></data>
  <data name="max_Desc_Key"                 xml:space="preserve"><value>Maximum allowed value</value></data>
```

**`SampleFieldReact/strings/SampleFieldReact.1030.resx`** (da-DK):

```xml
  <data name="SampleFieldReact_Display_Key" xml:space="preserve"><value>Eksempelfelt (React)</value></data>
  <data name="SampleFieldReact_Desc_Key"    xml:space="preserve"><value>React virtuelt eksempelfeltkontrolelement</value></data>
  <data name="value_Display_Key"            xml:space="preserve"><value>Værdi</value></data>
  <data name="value_Desc_Key"               xml:space="preserve"><value>Bunden numerisk værdi</value></data>
  <data name="min_Display_Key"              xml:space="preserve"><value>Min</value></data>
  <data name="min_Desc_Key"                 xml:space="preserve"><value>Mindste tilladte værdi</value></data>
  <data name="max_Display_Key"              xml:space="preserve"><value>Maks</value></data>
  <data name="max_Desc_Key"                 xml:space="preserve"><value>Største tilladte værdi</value></data>
```

**`SampleDatasetVanilla/strings/SampleDatasetVanilla.1033.resx`** (en-US):

```xml
  <data name="SampleDatasetVanilla_Display_Key" xml:space="preserve"><value>Sample Dataset (Vanilla)</value></data>
  <data name="SampleDatasetVanilla_Desc_Key"    xml:space="preserve"><value>Vanilla TypeScript dataset sample control</value></data>
  <data name="records_Display_Key"              xml:space="preserve"><value>Records</value></data>
  <data name="primaryName_Display_Key"          xml:space="preserve"><value>Primary name</value></data>
  <data name="primaryName_Desc_Key"             xml:space="preserve"><value>Primary name column</value></data>
```

**`SampleDatasetVanilla/strings/SampleDatasetVanilla.1030.resx`** (da-DK):

```xml
  <data name="SampleDatasetVanilla_Display_Key" xml:space="preserve"><value>Eksempeldatasæt (Vanilla)</value></data>
  <data name="SampleDatasetVanilla_Desc_Key"    xml:space="preserve"><value>Vanilla TypeScript-eksempeldatasæt-kontrolelement</value></data>
  <data name="records_Display_Key"              xml:space="preserve"><value>Poster</value></data>
  <data name="primaryName_Display_Key"          xml:space="preserve"><value>Primært navn</value></data>
  <data name="primaryName_Desc_Key"             xml:space="preserve"><value>Primær navnekolonne</value></data>
```

**`SampleDatasetReact/strings/SampleDatasetReact.1033.resx`** (en-US):

```xml
  <data name="SampleDatasetReact_Display_Key" xml:space="preserve"><value>Sample Dataset (React)</value></data>
  <data name="SampleDatasetReact_Desc_Key"    xml:space="preserve"><value>React virtual dataset sample control</value></data>
  <data name="records_Display_Key"            xml:space="preserve"><value>Records</value></data>
  <data name="primaryName_Display_Key"        xml:space="preserve"><value>Primary name</value></data>
  <data name="primaryName_Desc_Key"           xml:space="preserve"><value>Primary name column</value></data>
```

**`SampleDatasetReact/strings/SampleDatasetReact.1030.resx`** (da-DK):

```xml
  <data name="SampleDatasetReact_Display_Key" xml:space="preserve"><value>Eksempeldatasæt (React)</value></data>
  <data name="SampleDatasetReact_Desc_Key"    xml:space="preserve"><value>React virtuelt eksempeldatasæt-kontrolelement</value></data>
  <data name="records_Display_Key"            xml:space="preserve"><value>Poster</value></data>
  <data name="primaryName_Display_Key"        xml:space="preserve"><value>Primært navn</value></data>
  <data name="primaryName_Desc_Key"           xml:space="preserve"><value>Primær navnekolonne</value></data>
```

**Verifiable when:** `npm run build:pcf` exits 0 with zero warnings on a clean clone; the `generated/ManifestTypes` files appear under each control folder; lint passes; every control has both a `1033.resx` and `1030.resx` under `strings/`; every `*-key` referenced in any of the four manifests has a matching `<data name="…">` entry in **both** resx files for its control (verifiable by `grep -E '"[A-Za-z_]+_(Display|Desc)_Key"' */ControlManifest.Input.xml | …` cross-referenced against the resx contents).

### Phase 3: Vendored mock under `test/mock/`

Cherry-pick a minimal mock from `Shko-Online/ComponentFramework-Mock` (MIT). Class names mirror upstream so upstream code can be lifted in directly when more fidelity is needed. Current scope is the minimum the four samples exercise. If during implementation we find we need >70% of upstream surface, abandon cherry-picking and copy the full upstream `src/` tree as-is.

Files to create:

```
test/mock/
  index.ts
  types.ts
  PropertyMocks.ts
  DatasetMock.ts
  ContextMock.ts
  ComponentFrameworkMockGenerator.ts
  LICENSE
  UPSTREAM.md
```

`LICENSE` is the upstream MIT license text checked in alongside.

**`test/mock/types.ts`**:

```ts
export type ControlConstructor<TInputs, TOutputs> =
    new () => ComponentFramework.StandardControl<TInputs, TOutputs>
            | ComponentFramework.ReactControl<TInputs, TOutputs>;

export interface ReactRenderer {
    render(element: React.ReactElement, container: HTMLDivElement): void;
    unmount(container: HTMLDivElement): void;
}
```

**`test/mock/PropertyMocks.ts`**:

```ts
export class StringPropertyMock implements ComponentFramework.PropertyTypes.StringProperty {
    raw: string | null;
    formatted?: string;
    error: boolean = false;
    errorMessage: string = "";
    type: string = "SingleLine.Text";
    attributes?: ComponentFramework.PropertyHelper.FieldPropertyMetadata.StringMetadata;
    security?: ComponentFramework.PropertyHelper.SecurityValues;

    constructor(value: string | null = null) { this.raw = value; }
}

export class NumberPropertyMock implements ComponentFramework.PropertyTypes.NumberProperty {
    raw: number | null;
    formatted?: string;
    error: boolean = false;
    errorMessage: string = "";
    type: string = "Whole.None";
    attributes?: ComponentFramework.PropertyHelper.FieldPropertyMetadata.WholeNumberMetadata;
    security?: ComponentFramework.PropertyHelper.SecurityValues;

    constructor(value: number | null = null) { this.raw = value; }
}
```

**`test/mock/DatasetMock.ts`** — exposes `openedItems` and `refreshCount` so Playwright assertions can verify side effects without spying:

```ts
type Column = ComponentFramework.PropertyHelper.DataSetApi.Column;
type EntityRecord = ComponentFramework.PropertyHelper.DataSetApi.EntityRecord;

export interface DatasetSeed {
    entityName?: string;
    columns: Column[];
    records: Array<Record<string, unknown> & { id?: string }>;
    pageSize?: number;
}

export class DatasetMock implements ComponentFramework.PropertyTypes.DataSet {
    records: { [id: string]: EntityRecord } = {};
    sortedRecordIds: string[] = [];
    columns: Column[];
    paging: ComponentFramework.PropertyHelper.DataSetApi.Paging;
    loading: boolean = false;
    filtering: ComponentFramework.PropertyHelper.DataSetApi.Filtering = {} as never;
    linking: ComponentFramework.PropertyHelper.DataSetApi.Linking = {} as never;
    sorting: ComponentFramework.PropertyHelper.DataSetApi.SortStatus[] = [];
    error: boolean = false;
    errorMessage: string = "";

    private selected: Set<string> = new Set();
    private entityName: string;

    public openedItems: ComponentFramework.EntityReference[] = [];
    public refreshCount: number = 0;

    constructor(seed: DatasetSeed) {
        this.entityName = seed.entityName ?? "account";
        this.columns = seed.columns;
        seed.records.forEach((r, idx) => {
            const id = r.id ?? `record-${idx}`;
            this.sortedRecordIds.push(String(id));
            this.records[String(id)] = this.makeRecord(String(id), r);
        });
        this.paging = this.makePaging(seed.pageSize ?? 25);
    }

    addColumn?: (name: string, entityAlias?: string) => void;

    getSelectedRecordIds = (): string[] => Array.from(this.selected);
    setSelectedRecordIds = (ids: string[]): void => { this.selected = new Set(ids); };
    clearSelectedRecordIds = (): void => { this.selected.clear(); };
    getTargetEntityType = (): string => this.entityName;
    getTitle = (): string => "";
    getViewId = (): string => "";
    refresh = (): void => { this.refreshCount++; };
    openDatasetItem = (ref: ComponentFramework.EntityReference): void => { this.openedItems.push(ref); };

    private makeRecord(id: string, data: Record<string, unknown>): EntityRecord {
        return {
            getRecordId: () => id,
            getValue: (col: string) => data[col] as never,
            getFormattedValue: (col: string) => String(data[col] ?? ""),
            getNamedReference: () => ({
                id: { guid: id } as never,
                name: String(data.name ?? id),
                etn: this.entityName
            } as ComponentFramework.EntityReference),
            isDirty: () => false
        } as EntityRecord;
    }

    private makePaging(pageSize: number): ComponentFramework.PropertyHelper.DataSetApi.Paging {
        return {
            pageSize,
            totalResultCount: this.sortedRecordIds.length,
            hasNextPage: false,
            hasPreviousPage: false,
            firstPageNumber: 1,
            lastPageNumber: 1,
            loadNextPage: () => { /* no-op */ },
            loadPreviousPage: () => { /* no-op */ },
            loadExactPage: () => { /* no-op */ },
            reset: () => { /* no-op */ },
            setPageSize: (n: number) => { (this.paging as { pageSize: number }).pageSize = n; }
        };
    }
}
```

**`test/mock/ContextMock.ts`** — `as never` and `as unknown` casts are deliberate; fields the four samples never read are stubbed to satisfy the typechecker. If a control later reaches into `formatting` or `webAPI`, fill those in then.

```ts
export class ContextMock<TInputs> {
    parameters: TInputs;
    updatedProperties: string[] = [];
    mode = {
        isControlDisabled: false,
        isVisible: true,
        label: "",
        setControlState: () => true,
        setFullScreen: () => { /* no-op */ },
        setNotification: () => true,
        clearNotification: () => true,
        trackContainerResize: () => { /* no-op */ }
    } as ComponentFramework.Mode;
    client = {
        getClient: () => "Web",
        getFormFactor: () => 2,
        isOffline: () => false,
        isNetworkAvailable: () => true,
        disableScroll: false
    } as ComponentFramework.Client;
    factory = {
        getPopupService: () => ({} as never),
        requestRender: () => { /* no-op */ }
    } as unknown as ComponentFramework.Factory;
    formatting = {} as ComponentFramework.Formatting;
    navigation = {} as ComponentFramework.Navigation;
    resources = {
        getString: (key: string) => key,
        getResource: () => { /* no-op */ }
    } as unknown as ComponentFramework.Resources;
    utils = {} as ComponentFramework.Utility;
    webAPI = {} as ComponentFramework.WebApi;
    userSettings = {
        userId: "00000000-0000-0000-0000-000000000000",
        userName: "Test User",
        languageId: 1033,
        isRTL: false,
        dateFormattingInfo: {} as never,
        numberFormattingInfo: {} as never,
        getTimeZoneOffsetMinutes: () => 0,
        securityRoles: []
    } as unknown as ComponentFramework.UserSettings;

    constructor(parameters: TInputs) { this.parameters = parameters; }
}
```

**`test/mock/ComponentFrameworkMockGenerator.ts`** — drives a control through `init` → `updateView`, captures `notifyOutputChanged` count and `getOutputs()` history, and renders virtual controls via the injected `ReactRenderer`:

```ts
import { ContextMock } from "./ContextMock";
import { ControlConstructor, ReactRenderer } from "./types";

export interface MockGeneratorOptions {
    virtual?: boolean;
    reactRenderer?: ReactRenderer;
}

export class ComponentFrameworkMockGenerator<TInputs, TOutputs> {
    public readonly context: ContextMock<TInputs>;
    public readonly outputsHistory: TOutputs[] = [];
    public notifyCount: number = 0;

    private control: ComponentFramework.StandardControl<TInputs, TOutputs>
                   | ComponentFramework.ReactControl<TInputs, TOutputs>;
    private isVirtual: boolean;
    private renderer?: ReactRenderer;
    private container: HTMLDivElement;

    constructor(
        Ctor: ControlConstructor<TInputs, TOutputs>,
        parameters: TInputs,
        container: HTMLDivElement,
        options: MockGeneratorOptions = {}
    ) {
        this.control = new Ctor();
        this.context = new ContextMock(parameters);
        this.container = container;
        this.isVirtual = options.virtual ?? false;
        this.renderer = options.reactRenderer;
    }

    public init(): void {
        if (this.isVirtual) {
            (this.control as ComponentFramework.ReactControl<TInputs, TOutputs>).init(
                this.context as never,
                this.notify,
                {}
            );
            this.renderVirtual();
        } else {
            (this.control as ComponentFramework.StandardControl<TInputs, TOutputs>).init(
                this.context as never,
                this.notify,
                {},
                this.container
            );
            (this.control as ComponentFramework.StandardControl<TInputs, TOutputs>).updateView(this.context as never);
        }
    }

    public fireUpdateView(updatedProperties: string[] = []): void {
        this.context.updatedProperties = updatedProperties;
        if (this.isVirtual) {
            this.renderVirtual();
        } else {
            (this.control as ComponentFramework.StandardControl<TInputs, TOutputs>).updateView(this.context as never);
        }
    }

    public destroy(): void {
        this.control.destroy();
        if (this.isVirtual && this.renderer) this.renderer.unmount(this.container);
    }

    private notify = (): void => {
        this.notifyCount++;
        const out = this.control.getOutputs?.();
        if (out) this.outputsHistory.push(out);
    };

    private renderVirtual(): void {
        if (!this.renderer) throw new Error("Virtual control requires reactRenderer in options");
        const element = (this.control as ComponentFramework.ReactControl<TInputs, TOutputs>).updateView(this.context as never);
        this.renderer.render(element, this.container);
    }
}
```

**`test/mock/index.ts`**:

```ts
export * from "./types";
export * from "./PropertyMocks";
export * from "./DatasetMock";
export * from "./ContextMock";
export * from "./ComponentFrameworkMockGenerator";
```

**`test/mock/UPSTREAM.md`** — provenance + known-gaps backlog + refresh procedure. The "Known gaps" section is a fix backlog, not a disclosure list: every entry is something we plan to close so the mock tracks real Dataverse runtime behaviour. Add an entry whenever a divergence is discovered (failing test, real-runtime mismatch, contributor reading the mock); close it when the mock surface is filled in (record `closed-<commit-sha>`):

```markdown
# Vendored mock — upstream provenance

Source: https://github.com/Shko-Online/ComponentFramework-Mock
License: MIT (see ./LICENSE)
Copied from commit: <SHA>
Date copied: <YYYY-MM-DD>

## Approach
Cherry-pick. Class names mirror upstream so code can be lifted in directly when we
need additional fidelity. Current scope is the minimum needed for the four sample
controls in this repo, but the mock is treated as a living artifact: gaps are
tracked below as work to do, not as permanent deviations.

## Known gaps (fix backlog)

Format per entry:
- **<short title>** — what's missing or wrong.
  - Discovered: <how — failing test name, real-runtime divergence, code review, …>
  - Status: `open` | `in progress` | `closed-<commit-sha>`
  - Notes: any concrete pointers (file/line, surface area, real-runtime behaviour to match).

Initial entries (all `open` at vendoring time):

- **Metadata subsystem absent** — `ContextMock` does not expose attribute metadata
  (`PropertyHelper.FieldPropertyMetadata.*` is unimplemented beyond the typed shapes
  on the property mocks).
  - Discovered: code review at vendoring time; none of the four samples exercise it.
  - Status: open
  - Notes: needed before any control reads `.attributes` off a property.

- **Localization / translation harness absent** — `context.resources.getString` is
  a passthrough that returns the key unchanged; no resource bundle resolution.
  - Discovered: code review at vendoring time.
  - Status: open
  - Notes: real runtime resolves `<key>` against `strings/<ControlName>.<lcid>.resx`.
    The four sample controls now ship en-US (`1033`) + da-DK (`1030`) resx files
    (per the localization policy in the top-level Requirements), so closing this
    gap means parsing the per-control resx pair on harness mount, picking the
    locale from a query param (e.g. `?lcid=1030`, defaulting to `1033`), and
    returning the matching `<data name="…">` value from `getString`.

- **`formatting`, `webAPI`, `navigation`, `utils` stubbed as empty objects** — any
  control that reaches into these fields will hit `undefined` at runtime under the
  harness.
  - Discovered: code review at vendoring time; cast as `as never` / `as unknown` in
    `ContextMock`.
  - Status: open
  - Notes: fill in per-field as the first control to need each surface lands. A
    stubbed empty object is acceptable only when paired with an open gap entry here.

## Refresh procedure
1. Pull upstream at the SHA you want.
2. Diff the relevant files; copy in deltas (close any matching gap entries above).
3. Update SHA + date in the header.
4. Run `npm test` — must pass.

## Closing a gap
1. Implement the missing surface in the mock (mirror the upstream class/file name
   when possible so future upstream merges stay clean).
2. Add or extend a Playwright test that exercises the now-faithful behaviour.
3. Update the gap entry to `closed-<commit-sha>` (do not delete — closed entries are
   the audit trail of what fidelity work has shipped).
```

Implementer must replace `<SHA>` and `<YYYY-MM-DD>` with the actual values used at vendoring time. The three initial gap entries are pre-populated so the backlog is non-empty from day one.

**Verifiable when:** mock files compile under the project's `tsconfig.json` (`tsc --noEmit` succeeds with `@mock` paths resolving); `LICENSE` and `UPSTREAM.md` present; `UPSTREAM.md` records non-placeholder SHA + date.

### Phase 4: HTML harness with esbuild bundler

Create `test/harness/` with an HTML page, a TS mount script, a control registry, and an esbuild config that supports both one-shot build and watch+serve modes. The harness exposes a `window.__pcf` API that Playwright drives via `page.evaluate`.

Files to create:

```
test/harness/
  index.html
  mount.ts
  registry.ts
  esbuild.config.mjs
```

**`test/harness/index.html`** — provides `#pcf-root` (mount point with `data-testid="pcf-root"`) and `#harness-meta` (status with `data-testid="harness-meta"` + `data-ready="true"` once mounted, used by Playwright as a readiness signal):

```html
<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>XrmBedrock PCF Harness</title>
    <link rel="stylesheet" href="/dist/styles.css" />
    <style>
      html, body { margin: 0; padding: 0; font-family: 'Segoe UI', system-ui, sans-serif; }
      #harness-shell { padding: 16px; max-width: 960px; }
      #pcf-root { padding: 12px; border: 1px dashed #c8c6c4; min-height: 60px; }
      #harness-meta { font-size: 12px; color: #605e5c; margin-bottom: 8px; }
    </style>
  </head>
  <body>
    <div id="harness-shell">
      <div id="harness-meta" data-testid="harness-meta">Harness loading…</div>
      <div id="pcf-root" data-testid="pcf-root"></div>
    </div>
    <script type="module" src="/dist/mount.js"></script>
  </body>
</html>
```

**`test/harness/registry.ts`** — maps control name → constructor, virtual flag, and `buildParams` factory that produces the `parameters` object given an optional seed. Default dataset seed has three records (`Contoso`/`Copenhagen`, `Fabrikam`/`Aarhus`, `Northwind`/`Odense`):

```ts
import { SampleFieldVanilla } from "../../SampleFieldVanilla";
import { SampleFieldReact } from "../../SampleFieldReact";
import { SampleDatasetVanilla } from "../../SampleDatasetVanilla";
import { SampleDatasetReact } from "../../SampleDatasetReact";
import { StringPropertyMock, NumberPropertyMock, DatasetMock } from "@mock";
import type { ControlConstructor } from "@mock";

export interface RegistryEntry {
    ctor: ControlConstructor<unknown, unknown>;
    virtual: boolean;
    buildParams: (seed?: unknown) => unknown;
}

export const registry: Record<string, RegistryEntry> = {
    SampleFieldVanilla: {
        ctor: SampleFieldVanilla as never,
        virtual: false,
        buildParams: (seed: { value?: string } = {}) => ({
            value: new StringPropertyMock(seed.value ?? "hello world")
        })
    },
    SampleFieldReact: {
        ctor: SampleFieldReact as never,
        virtual: true,
        buildParams: (seed: { value?: number; min?: number; max?: number } = {}) => ({
            value: new NumberPropertyMock(seed.value ?? 5),
            min: new NumberPropertyMock(seed.min ?? 0),
            max: new NumberPropertyMock(seed.max ?? 10)
        })
    },
    SampleDatasetVanilla: {
        ctor: SampleDatasetVanilla as never,
        virtual: false,
        buildParams: (seed?: never) => ({
            records: new DatasetMock(seed ?? defaultDatasetSeed())
        })
    },
    SampleDatasetReact: {
        ctor: SampleDatasetReact as never,
        virtual: true,
        buildParams: (seed?: never) => ({
            records: new DatasetMock(seed ?? defaultDatasetSeed())
        })
    }
};

function defaultDatasetSeed() {
    return {
        entityName: "account",
        columns: [
            { name: "name", displayName: "Name", dataType: "SingleLine.Text", alias: "", order: 0, visualSizeFactor: 1 },
            { name: "city", displayName: "City", dataType: "SingleLine.Text", alias: "", order: 1, visualSizeFactor: 1 }
        ],
        records: [
            { id: "r1", name: "Contoso", city: "Copenhagen" },
            { id: "r2", name: "Fabrikam", city: "Aarhus" },
            { id: "r3", name: "Northwind", city: "Odense" }
        ]
    };
}
```

**`test/harness/mount.ts`** — reads `?control=<name>&seed=<base64-encoded-json>` from the URL, creates a `ReactDOMClient.Root` (React 18-style API for `react-dom/client`; works with React 16 since react-dom 18+ exposes the legacy client too), mounts via `ComponentFrameworkMockGenerator`, and installs the `window.__pcf` API:

```ts
import * as ReactDOMClient from "react-dom/client";
import { ComponentFrameworkMockGenerator, ReactRenderer } from "@mock";
import { registry } from "./registry";

const url = new URL(window.location.href);
const controlName = url.searchParams.get("control") ?? "SampleFieldVanilla";
const seedRaw = url.searchParams.get("seed");
const seed = seedRaw ? JSON.parse(decodeURIComponent(atob(seedRaw))) : undefined;

const meta = document.getElementById("harness-meta")!;
const root = document.getElementById("pcf-root") as HTMLDivElement;

const entry = registry[controlName];
if (!entry) {
    meta.textContent = `Unknown control: ${controlName}`;
    throw new Error(`Unknown control: ${controlName}`);
}

let reactRoot: ReactDOMClient.Root | null = null;
const reactRenderer: ReactRenderer = {
    render(element, container) {
        if (!reactRoot) reactRoot = ReactDOMClient.createRoot(container);
        reactRoot.render(element);
    },
    unmount(container) {
        reactRoot?.unmount();
        reactRoot = null;
        container.innerHTML = "";
    }
};

const params = entry.buildParams(seed);
const generator = new ComponentFrameworkMockGenerator(
    entry.ctor as never,
    params as never,
    root,
    { virtual: entry.virtual, reactRenderer }
);
generator.init();

// Test hooks — Playwright drives these via page.evaluate.
declare global {
    interface Window {
        __pcf: {
            controlName: string;
            getOutputs: () => unknown[];
            getNotifyCount: () => number;
            getContext: () => unknown;
            fireUpdateView: (patch: Record<string, unknown>, updatedProperties?: string[]) => void;
            getDatasetState: () => { selectedIds: string[]; openedItems: unknown[]; refreshCount: number } | null;
        };
    }
}

window.__pcf = {
    controlName,
    getOutputs: () => generator.outputsHistory,
    getNotifyCount: () => generator.notifyCount,
    getContext: () => generator.context,
    fireUpdateView: (patch, updatedProperties = []) => {
        Object.entries(patch).forEach(([key, value]) => {
            const prop = (generator.context.parameters as Record<string, unknown>)[key];
            if (prop && typeof prop === "object" && "raw" in (prop as object)) {
                (prop as { raw: unknown }).raw = value;
            }
        });
        generator.fireUpdateView(updatedProperties);
    },
    getDatasetState: () => {
        const ds = (generator.context.parameters as { records?: unknown }).records as
            { getSelectedRecordIds(): string[]; openedItems: unknown[]; refreshCount: number } | undefined;
        if (!ds || !("openedItems" in ds)) return null;
        return {
            selectedIds: ds.getSelectedRecordIds(),
            openedItems: ds.openedItems,
            refreshCount: ds.refreshCount
        };
    }
};

meta.textContent = `Mounted: ${controlName} (${entry.virtual ? "virtual" : "standard"})`;
meta.setAttribute("data-control", controlName);
meta.setAttribute("data-ready", "true");
```

**`test/harness/esbuild.config.mjs`** — supports `--serve` flag; serves on port 4173; copies `index.html` into `dist/`; aliases `@mock`:

```js
import { context, build } from "esbuild";
import { fileURLToPath } from "node:url";
import { dirname, resolve } from "node:path";
import { existsSync, mkdirSync, copyFileSync } from "node:fs";

const __dirname = dirname(fileURLToPath(import.meta.url));
const projectRoot = resolve(__dirname, "../..");
const outdir = resolve(__dirname, "dist");
const serve = process.argv.includes("--serve");

if (!existsSync(outdir)) mkdirSync(outdir, { recursive: true });
copyFileSync(resolve(__dirname, "index.html"), resolve(outdir, "index.html"));

const opts = {
    entryPoints: [resolve(__dirname, "mount.ts")],
    bundle: true,
    outdir,
    format: "esm",
    platform: "browser",
    target: "es2020",
    sourcemap: true,
    loader: { ".css": "css" },
    tsconfig: resolve(projectRoot, "tsconfig.json"),
    alias: {
        "@mock": resolve(projectRoot, "test/mock")
    },
    logLevel: "info"
};

if (serve) {
    const ctx = await context(opts);
    await ctx.watch();
    const { host, port } = await ctx.serve({ servedir: outdir, port: 4173 });
    console.log(`Harness on http://${host}:${port}`);
} else {
    await build(opts);
}
```

**Verifiable when:** `npm run build:harness` produces `test/harness/dist/{index.html, mount.js, mount.js.map}`; `npm run serve:harness` serves at `http://127.0.0.1:4173`; manually visiting `http://127.0.0.1:4173/?control=SampleFieldVanilla` mounts the control and `[data-testid="harness-meta"]` shows `data-ready="true"`.

### Phase 5: Playwright tests

Create `test/playwright/` with config, fixture, and four spec files (one per control, three tests each = 12 total). All Chromium-only. The `webServer` block auto-starts the esbuild watch server. Total runtime budget under 60s.

Files to create:

```
test/playwright/
  playwright.config.ts
  fixtures/
    pcf-page.ts
    dataset-records.json
  field-vanilla.spec.ts
  field-react.spec.ts
  dataset-vanilla.spec.ts
  dataset-react.spec.ts
test/README.md
```

(`fixtures/dataset-records.json` is reserved for tests that need richer dataset seeds; the four base specs use the registry default seed, so this file may be empty or omitted in initial implementation — keep the path reserved.)

**`test/playwright/playwright.config.ts`** — `fullyParallel: false`, `workers: 1`, `timeout: 30_000`, `retries: 0`. Three reporters: `list` (stdout for agents), `html` (`outputFolder: test/playwright-report`, `open: never`), `junit` (`outputFile: test/junit-results.xml`). `outputDir: test/test-results`. `trace: retain-on-failure`, `video: retain-on-failure`, `screenshot: only-on-failure`. `webServer.command` invokes the esbuild config in `--serve` mode and waits on `http://127.0.0.1:4173`. `reuseExistingServer: !process.env.CI`:

```ts
import { defineConfig, devices } from "@playwright/test";
import { resolve } from "node:path";

const harnessDir = resolve(__dirname, "../harness/dist");

export default defineConfig({
    testDir: ".",
    fullyParallel: false,
    workers: 1,
    timeout: 30_000,
    retries: 0,
    reporter: [
        ["list"],
        ["html", { outputFolder: resolve(__dirname, "../playwright-report"), open: "never" }],
        ["junit", { outputFile: resolve(__dirname, "../junit-results.xml") }]
    ],
    outputDir: resolve(__dirname, "../test-results"),
    use: {
        baseURL: "http://127.0.0.1:4173",
        trace: "retain-on-failure",
        video: "retain-on-failure",
        screenshot: "only-on-failure"
    },
    projects: [{ name: "chromium", use: { ...devices["Desktop Chrome"] } }],
    webServer: {
        command: `node ${resolve(__dirname, "../harness/esbuild.config.mjs")} --serve`,
        url: "http://127.0.0.1:4173",
        reuseExistingServer: !process.env.CI,
        timeout: 30_000,
        cwd: resolve(__dirname, "../..")
    },
    metadata: { harnessDir }
});
```

**`test/playwright/fixtures/pcf-page.ts`** — extends Playwright's `test` with a `pcf` fixture that wraps `page` with `mount`/`fireUpdateView`/`getOutputs`/`getNotifyCount`/`getDatasetState`. The seed is base64-encoded JSON in the URL (`btoa(encodeURIComponent(JSON.stringify(seed)))`). `mount` waits on `[data-testid="harness-meta"][data-ready="true"]`:

```ts
import { test as base, expect, Page } from "@playwright/test";

export interface PcfPage {
    mount: (controlName: string, seed?: unknown) => Promise<void>;
    fireUpdateView: (patch: Record<string, unknown>, updatedProperties?: string[]) => Promise<void>;
    getOutputs: () => Promise<unknown[]>;
    getNotifyCount: () => Promise<number>;
    getDatasetState: () => Promise<{ selectedIds: string[]; openedItems: unknown[]; refreshCount: number } | null>;
    page: Page;
}

export const test = base.extend<{ pcf: PcfPage }>({
    pcf: async ({ page }, use) => {
        const api: PcfPage = {
            page,
            mount: async (controlName, seed) => {
                const url = new URL("/", "http://127.0.0.1:4173");
                url.searchParams.set("control", controlName);
                if (seed !== undefined) {
                    const encoded = btoa(encodeURIComponent(JSON.stringify(seed)));
                    url.searchParams.set("seed", encoded);
                }
                await page.goto(url.toString());
                await page.locator('[data-testid="harness-meta"][data-ready="true"]').waitFor();
            },
            fireUpdateView: (patch, updatedProperties = []) =>
                page.evaluate(
                    ([p, u]) => window.__pcf.fireUpdateView(p as never, u as never),
                    [patch, updatedProperties]
                ),
            getOutputs: () => page.evaluate(() => window.__pcf.getOutputs()),
            getNotifyCount: () => page.evaluate(() => window.__pcf.getNotifyCount()),
            getDatasetState: () => page.evaluate(() => window.__pcf.getDatasetState())
        };
        await use(api);
    }
});

export { expect };
```

**`test/playwright/field-vanilla.spec.ts`** — three tests: renders seeded value, typing fires `notifyOutputChanged`, external `updateView` re-renders. Each test takes a screenshot to `test-results/`:

```ts
import { test, expect } from "./fixtures/pcf-page";

test.describe("SampleFieldVanilla", () => {
    test("renders seeded value", async ({ pcf }) => {
        await pcf.mount("SampleFieldVanilla", { value: "initial value" });

        const input = pcf.page.locator('[data-testid="field-vanilla-input"]');
        await expect(input).toHaveValue("initial value");
        await pcf.page.screenshot({ path: "test-results/field-vanilla-initial.png" });
    });

    test("typing fires notifyOutputChanged with new value", async ({ pcf }) => {
        await pcf.mount("SampleFieldVanilla", { value: "" });

        await pcf.page.locator('[data-testid="field-vanilla-input"]').fill("typed by user");

        await expect.poll(() => pcf.getNotifyCount()).toBeGreaterThan(0);
        const outputs = await pcf.getOutputs();
        expect(outputs[outputs.length - 1]).toEqual({ value: "typed by user" });
    });

    test("external updateView re-renders the input", async ({ pcf }) => {
        await pcf.mount("SampleFieldVanilla", { value: "first" });

        await pcf.fireUpdateView({ value: "second" }, ["value"]);

        await expect(pcf.page.locator('[data-testid="field-vanilla-input"]')).toHaveValue("second");
        await pcf.page.screenshot({ path: "test-results/field-vanilla-updated.png" });
    });
});
```

**`test/playwright/field-react.spec.ts`** — three tests: renders with min/max bounds, increment via Fluent SpinButton emits new value, external updateView re-renders. Increments are driven by the SpinButton's accessible "increment" button:

```ts
import { test, expect } from "./fixtures/pcf-page";

test.describe("SampleFieldReact", () => {
    test("renders seeded value with min/max bounds", async ({ pcf }) => {
        await pcf.mount("SampleFieldReact", { value: 5, min: 0, max: 10 });
        await expect(pcf.page.locator('[data-testid="field-react-input"]')).toHaveValue("5");
        await pcf.page.screenshot({ path: "test-results/field-react-initial.png" });
    });

    test("incrementing emits new value", async ({ pcf }) => {
        await pcf.mount("SampleFieldReact", { value: 5, min: 0, max: 10 });
        await pcf.page.getByRole("button", { name: /increment/i }).click();
        await expect.poll(() => pcf.getNotifyCount()).toBeGreaterThan(0);
        const outputs = await pcf.getOutputs();
        expect((outputs[outputs.length - 1] as { value: number }).value).toBe(6);
    });

    test("external updateView re-renders", async ({ pcf }) => {
        await pcf.mount("SampleFieldReact", { value: 5, min: 0, max: 10 });
        await pcf.fireUpdateView({ value: 8 }, ["value"]);
        await expect(pcf.page.locator('[data-testid="field-react-input"]')).toHaveValue("8");
        await pcf.page.screenshot({ path: "test-results/field-react-updated.png" });
    });
});
```

**`test/playwright/dataset-vanilla.spec.ts`** — three tests: renders 3 seeded rows, row click calls `openDatasetItem`, external updateView re-renders:

```ts
import { test, expect } from "./fixtures/pcf-page";

test.describe("SampleDatasetVanilla", () => {
    test("renders seeded rows", async ({ pcf }) => {
        await pcf.mount("SampleDatasetVanilla");
        const rows = pcf.page.locator('[data-testid="dataset-vanilla-table"] tbody tr');
        await expect(rows).toHaveCount(3);
        await pcf.page.screenshot({ path: "test-results/dataset-vanilla-initial.png" });
    });

    test("clicking a row calls openDatasetItem", async ({ pcf }) => {
        await pcf.mount("SampleDatasetVanilla");
        await pcf.page.locator('tr[data-record-id="r2"]').click();
        const state = await pcf.getDatasetState();
        expect(state?.openedItems).toHaveLength(1);
    });

    test("external updateView re-renders rows", async ({ pcf }) => {
        await pcf.mount("SampleDatasetVanilla");
        await pcf.fireUpdateView({}, ["records"]);
        await expect(pcf.page.locator('[data-testid="dataset-vanilla-table"]')).toBeVisible();
        await pcf.page.screenshot({ path: "test-results/dataset-vanilla-updated.png" });
    });
});
```

**`test/playwright/dataset-react.spec.ts`** — three tests: rows visible, selection updates `dataset.selectedIds`, double-click opens record:

```ts
import { test, expect } from "./fixtures/pcf-page";

test.describe("SampleDatasetReact", () => {
    test("renders rows in DataGrid", async ({ pcf }) => {
        await pcf.mount("SampleDatasetReact");
        await expect(pcf.page.locator('[data-record-id="r1"]')).toBeVisible();
        await pcf.page.screenshot({ path: "test-results/dataset-react-initial.png" });
    });

    test("selection updates dataset.selectedIds", async ({ pcf }) => {
        await pcf.mount("SampleDatasetReact");
        await pcf.page.locator('[data-record-id="r1"] input[type="checkbox"]').check();
        const state = await pcf.getDatasetState();
        expect(state?.selectedIds).toContain("r1");
    });

    test("double-click opens record", async ({ pcf }) => {
        await pcf.mount("SampleDatasetReact");
        await pcf.page.locator('[data-record-id="r2"]').dblclick();
        const state = await pcf.getDatasetState();
        expect(state?.openedItems).toHaveLength(1);
    });
});
```

**`test/README.md`** — documents commands, deterministic artifact paths, manual harness URL pattern, and `window.__pcf` console hooks:

```markdown
# PCF tests

## Run
- `npm test` — headless, exit code is the signal
- `npm run test:headed` — see what Playwright does
- `npm run test:report` — open the HTML report

## Output paths (deterministic, agent-friendly)
- `test/playwright-report/index.html` — full HTML report
- `test/junit-results.xml` — JUnit results
- `test/test-results/<spec>-<test>-<label>.png` — screenshots
- `test/test-results/**/video.webm` — videos on failure

## Drive the harness manually
http://127.0.0.1:4173/?control=<ControlName>&seed=<base64-encoded-json>
Run `npm run serve:harness` first.

Available controls: SampleFieldVanilla, SampleFieldReact, SampleDatasetVanilla, SampleDatasetReact

## Test hooks (in browser console)
window.__pcf.getOutputs()
window.__pcf.getNotifyCount()
window.__pcf.fireUpdateView({ value: "..." }, ["value"])
window.__pcf.getDatasetState()
```

Local testing flow:

```bash
cd src/Dataverse/PCFs/XrmBedrock.PCFs
npm ci
npm run build:pcf       # validates manifests + types via pcf-scripts
npm test                # builds harness, starts esbuild server, runs Playwright
```

`npm test` exits non-zero on any failure. Coding agents read pass/fail from the `list` reporter on stdout; humans open `test/playwright-report/index.html` for traces.

**Verifiable when:** `npm test` completes in under 60s with all 12 tests passing on a clean clone; `test/playwright-report/index.html`, `test/junit-results.xml`, and at least one screenshot per test are produced; `npm test` exit code is non-zero if any single test fails.

### Phase 6: F# move-and-clean script

Add `src/Tools/Daxif/MoveAndCleanPCF.fsx`. The pipeline calls this after `pac pcf push` to move every `customcontrol` (component type `66`) from the throwaway solution into the target solution and then delete the throwaway. The script must be **idempotent**: a second consecutive run succeeds with no duplicated components and no orphaned throwaway solution.

`src/Tools/Daxif/MoveAndCleanPCF.fsx`:

```fsharp
#r "nuget: Microsoft.PowerPlatform.Dataverse.Client, 1.1.32"
#r "nuget: Microsoft.CrmSdk.CoreAssemblies, 9.0.2.49"

open System
open Microsoft.PowerPlatform.Dataverse.Client
open Microsoft.Xrm.Sdk
open Microsoft.Xrm.Sdk.Messages
open Microsoft.Xrm.Sdk.Query

// ---- argument parsing ----
let args = fsi.CommandLineArgs |> Array.skip 1
let argValue (name: string) =
    args
    |> Array.tryFindIndex ((=) name)
    |> Option.bind (fun i -> if i + 1 < args.Length then Some args.[i + 1] else None)

let envName = argValue "--env" |> Option.defaultWith (fun () -> failwith "--env required")
let target  = argValue "--target" |> Option.defaultWith (fun () -> failwith "--target required")
let temp    = argValue "--temp" |> Option.defaultWith (fun () -> failwith "--temp required")

// ---- connection ----
// Use the same XrmConnection helper the rest of XrmBedrock uses (e.g. shared from
// _Config.fsx). The pseudocode below shows the contract.
let svc : IOrganizationService =
    let url       = Environment.GetEnvironmentVariable("DataverseUrl")
    let appId     = Environment.GetEnvironmentVariable("DataverseAppId")
    let secret    = Environment.GetEnvironmentVariable("DataverseSecret")
    let conn = sprintf "AuthType=ClientSecret;Url=%s;ClientId=%s;ClientSecret=%s" url appId secret
    new ServiceClient(conn) :> IOrganizationService

// ---- helpers ----
let getSolutionId (uniqueName: string) : Guid option =
    let q = QueryExpression("solution", ColumnSet = ColumnSet("solutionid"))
    q.Criteria.AddCondition("uniquename", ConditionOperator.Equal, uniqueName)
    let r = svc.RetrieveMultiple(q)
    if r.Entities.Count = 0 then None
    else Some (r.Entities.[0].Id)

let customControlComponents (solutionId: Guid) : Guid list =
    let q = QueryExpression("solutioncomponent", ColumnSet = ColumnSet("objectid", "componenttype"))
    q.Criteria.AddCondition("solutionid", ConditionOperator.Equal, solutionId)
    q.Criteria.AddCondition("componenttype", ConditionOperator.Equal, 66)
    svc.RetrieveMultiple(q).Entities
    |> Seq.map (fun e -> e.GetAttributeValue<Guid>("objectid"))
    |> List.ofSeq

let addToSolution (solutionUniqueName: string) (componentId: Guid) =
    let req = AddSolutionComponentRequest()
    req.ComponentType <- 66
    req.ComponentId <- componentId
    req.SolutionUniqueName <- solutionUniqueName
    req.AddRequiredComponents <- true
    req.IncludedComponentSettingsValues <- null
    svc.Execute(req) |> ignore

let deleteSolution (uniqueName: string) =
    match getSolutionId uniqueName with
    | None -> printfn "[skip] solution %s not present" uniqueName
    | Some id ->
        svc.Delete("solution", id)
        printfn "[ok] deleted solution %s" uniqueName

// ---- main ----
printfn "Environment: %s" envName
printfn "Target solution: %s" target
printfn "Temp solution: %s" temp

match getSolutionId temp with
| None ->
    printfn "Temp solution %s not found — nothing to move" temp
    exit 0
| Some tempId ->
    let components = customControlComponents tempId
    printfn "Found %d customcontrol component(s) in %s" components.Length temp

    if components.IsEmpty then
        printfn "Nothing to move; deleting temp anyway"
    else
        for cid in components do
            printfn "  -> moving %A into %s" cid target
            addToSolution target cid

    deleteSolution temp
    printfn "Done."
```

Naming + config:

- Throwaway solution: `XrmBedrockPCFTemp` (overridable per pipeline run via `--temp`).
- Target solution: read from `_Config.fsx` (passed via `--target`).
- Publisher prefix: read from `_Config.fsx`'s `PublisherInfo` (used by the `pac pcf push` step in the pipeline, not by this script).

The connection block is pseudocode — implementer must wire it to whatever `XrmConnection` helper the rest of XrmBedrock already uses (shared from `_Config.fsx`). The script reads `DataverseUrl` / `DataverseAppId` / `DataverseSecret` env vars (existing library variables; do not introduce new ones).

**Verifiable when:** running `dotnet fsi src/Tools/Daxif/MoveAndCleanPCF.fsx --env Dev --target <target> --temp XrmBedrockPCFTemp` against an env where `pac pcf push` has just populated `XrmBedrockPCFTemp` results in (a) every component type 66 from temp present in target, (b) `XrmBedrockPCFTemp` deleted; running it a second time exits cleanly with `Temp solution XrmBedrockPCFTemp not found — nothing to move`.

### Phase 7: Azure pipeline integration

Add a reusable pipeline template at `.pipelines/Azure/PCF-Deploy.yml` and wire it into existing per-environment stages. PCF tests sit at step 3 of each stage deliberately — they're offline and fast, so a regression there fails the pipeline before it consumes environment time.

`.pipelines/Azure/PCF-Deploy.yml`:

```yaml
parameters:
  - name: environment
    type: string
  - name: serviceConnection
    type: string
  - name: targetSolution
    type: string
  - name: tempSolution
    type: string
    default: XrmBedrockPCFTemp
  - name: publisherPrefix
    type: string
  - name: pcfWorkingDirectory
    type: string
    default: src/Dataverse/PCFs/XrmBedrock.PCFs

steps:
  - task: UseNode@1
    inputs:
      version: '20.x'

  - script: npm ci
    workingDirectory: ${{ parameters.pcfWorkingDirectory }}
    displayName: npm ci

  - script: npx playwright install --with-deps chromium
    workingDirectory: ${{ parameters.pcfWorkingDirectory }}
    displayName: Install Playwright browsers

  - script: npm run build:pcf
    workingDirectory: ${{ parameters.pcfWorkingDirectory }}
    displayName: Build PCF bundle (pcf-scripts)

  - script: npm test
    workingDirectory: ${{ parameters.pcfWorkingDirectory }}
    displayName: Playwright PCF tests (offline)

  - task: PublishTestResults@2
    condition: succeededOrFailed()
    inputs:
      testResultsFormat: JUnit
      testResultsFiles: ${{ parameters.pcfWorkingDirectory }}/test/junit-results.xml
      testRunTitle: PCF Playwright (${{ parameters.environment }})

  - publish: ${{ parameters.pcfWorkingDirectory }}/test/playwright-report
    artifact: pcf-playwright-report-${{ parameters.environment }}
    condition: succeededOrFailed()

  - task: PowerPlatformToolInstaller@2
    displayName: Install Power Platform Build Tools

  - task: PowerPlatformDeleteSolution@2
    displayName: Pre-clean throwaway solution
    continueOnError: true
    inputs:
      authenticationType: PowerPlatformSPN
      PowerPlatformSPN: ${{ parameters.serviceConnection }}
      SolutionName: ${{ parameters.tempSolution }}

  - bash: |
      set -euo pipefail
      echo "Stamping patch version = $(Build.BuildId) into all ControlManifest.Input.xml"
      find . -name 'ControlManifest.Input.xml' -print -exec \
        sed -i -E "s|(version=\"[0-9]+\.[0-9]+\.)[0-9]+(\")|\1$(Build.BuildId)\2|" {} \;
      echo "Resulting versions:"
      grep -E 'version="[0-9]+\.[0-9]+\.[0-9]+"' $(find . -name 'ControlManifest.Input.xml')
    workingDirectory: ${{ parameters.pcfWorkingDirectory }}
    displayName: Stamp manifest patch version from build id

  - task: PowerPlatformPushPCF@2
    displayName: pac pcf push -> ${{ parameters.tempSolution }}
    inputs:
      authenticationType: PowerPlatformSPN
      PowerPlatformSPN: ${{ parameters.serviceConnection }}
      workingDirectory: ${{ parameters.pcfWorkingDirectory }}
      pushArguments: >-
        --publisher-prefix ${{ parameters.publisherPrefix }}
        --solution-name ${{ parameters.tempSolution }}

  - task: DotNetCoreCLI@2
    displayName: Move components & delete throwaway
    env:
      DataverseUrl: $(DataverseUrl)
      DataverseAppId: $(DataverseAppId)
      DataverseSecret: $(DataverseSecret)
    inputs:
      command: custom
      custom: fsi
      arguments: >-
        src/Tools/Daxif/MoveAndCleanPCF.fsx
        --env ${{ parameters.environment }}
        --target ${{ parameters.targetSolution }}
        --temp ${{ parameters.tempSolution }}
```

Hooking into existing pipelines — required ordering within each environment stage:

1. Plugins build & sign (existing).
2. WebResources build (existing).
3. **PCF build + Playwright tests (new — fast offline gate).**
4. Solution import (existing — establishes target solution if missing).
5. **PCF push & move (new — runs after target solution exists).**
6. DAXIF post-deploy steps (existing).

Auth: reuses the existing per-environment Power Platform service connection (`Dataverse Dev`, `Dataverse Test`, `Dataverse UAT`, `Dataverse Prod`) via `PowerPlatformSPN`. The F# script reuses the existing `DataverseUrl` / `DataverseAppId` / `DataverseSecret` library variables. No new federated credentials, no new app registration permissions, no changes to library variable groups.

**Verifiable when:** a pipeline run on a fresh Dev environment (a) publishes JUnit test results showing all 12 PCF tests passing, (b) publishes a `pcf-playwright-report-Dev` artifact, (c) ends with all four sample controls present in the target solution at version `0.0.<Build.BuildId>` (patch matches the run's build id; major/minor unchanged from the checked-in `0.0.1`), (d) leaves no `XrmBedrockPCFTemp` solution in the environment, (e) introduces no new Azure DevOps service connection or Azure app registration. The stamp step's stdout shows the rewritten `version="…"` line for each of the four manifests.

### Phase 8: Acceptance verification

Walk through every acceptance criterion from the original spec on a clean clone and a fresh Dev environment. This phase is a checklist, not new code; it's the gate that confirms the integration is complete.

Acceptance criteria (each must be true):

- [ ] `npm run build:pcf` succeeds with zero warnings on a clean clone.
- [ ] `npm test` runs Playwright headless, all 12 tests pass, generates HTML report and at least one screenshot per test.
- [ ] An agent can run `npm test` from `src/Dataverse/PCFs/XrmBedrock.PCFs` and parse pass/fail from the `list` reporter's stdout.
- [ ] `MoveAndCleanPCF.fsx` runs idempotently — a second consecutive run succeeds with no duplicated components and no orphaned throwaway solution.
- [ ] A pipeline run on a fresh Dev environment ends with all four sample controls present in the target solution and no `XrmBedrockPCFTemp` solution in the environment.
- [ ] No new Azure DevOps service connection or Azure app registration is created.
- [ ] `test/mock/UPSTREAM.md` records source repo URL, commit SHA, and date; contains a `Known gaps` section pre-populated with the three initial entries (metadata, localization, `formatting`/`webAPI`/`navigation`/`utils` stubs), each marked `open`; and includes the "Closing a gap" workflow so future contributors/agents know how to discharge an entry.
- [ ] `test/README.md` documents the harness URL pattern, deterministic artifact paths, and `window.__pcf` test hooks.
- [ ] The four sample controls cover the matrix: vanilla field, React virtual field, vanilla dataset, React virtual dataset.
- [ ] Every sample control ships with both `<ControlName>.1033.resx` (en-US) and `<ControlName>.1030.resx` (da-DK) under `strings/`, both referenced from the control's `<resources>` block, and every `*-key` named in the manifest has a `<data name="…">` entry in both resx files.

**Verifiable when:** every checkbox above is marked true with linked evidence (build log line, screenshot path, pipeline run URL, environment query result).
