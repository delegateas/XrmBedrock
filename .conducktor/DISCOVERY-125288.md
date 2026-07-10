# DISCOVERY — ADO #125288: Build PCFs locally & test them locally with Playwright

**Work item:** [Delegate / ABS Tooling #125288](https://dev.azure.com/Delegate/ABS%20Tooling/_workitems/edit/125288) — *"Gør det nemt at bygge PCFer lokalt, og teste dem lokalt med playwright."*
**Branch under evaluation:** `pcf-7f1` (feature branch `feature/125288-pcf-local-testing` cut from it).
**Authoritative design:** [`.conducktor/SPEC.md`](SPEC.md) (8 phases). Phases 1–3 are committed; Phases 4–5 are the gap this task targets.
**Stage:** 1 (Discovery). No baseline built yet — only cheap feasibility spikes.

---

## 1. What already exists (verified on `pcf-7f1`)

| Phase | State | Notes |
|---|---|---|
| 1 — Project scaffold | ✅ committed & faithful to SPEC | `pcfproj`, `package.json`, `tsconfig.json`, `pcfconfig.json`, `.eslintrc.json`, `.nvmrc` (=`20`) all present at `src/Dataverse/PCFs/XrmBedrock.PCFs/`. |
| 2 — Four sample controls | ✅ committed & faithful | `SampleFieldVanilla`, `SampleFieldReact`, `SampleDatasetVanilla`, `SampleDatasetReact` — each with manifest, `index.ts`, CSS/`App.tsx`, and both `1033`+`1030` resx. |
| 3 — Vendored mock | ✅ committed & faithful | `test/mock/` with `ContextMock`, `DatasetMock`, `PropertyMocks`, `ComponentFrameworkMockGenerator`, `LICENSE`, `UPSTREAM.md` (SHA `d356ee1…`, 2026-04-28, 3 open gaps). |
| 4 — esbuild HTML harness | ❌ **MISSING** | `test/harness/` does not exist. `package.json` already declares `build:harness`/`serve:harness` pointing at `test/harness/esbuild.config.mjs`. |
| 5 — Playwright tests | ❌ **MISSING** | `test/playwright/` does not exist. `package.json` `test` script already points at `test/playwright/playwright.config.ts`. |
| 6 — F# move-and-clean | ⛔ out of scope | Deprecated tooling; excluded per task constraints. |
| 7 — Azure pipeline | ⛔ out of scope | Excluded per task constraints. |

**Conclusion:** the scaffold, controls and mock are real and match the SPEC. The declared npm flow (`build:harness` → `playwright test`) references files that don't exist yet. That gap is the target.

---

## 2. Environment reality (spikes run this session, on the actual machine)

All spikes were run from `src/Dataverse/PCFs/XrmBedrock.PCFs/`.

| Finding | Evidence | Impact |
|---|---|---|
| **Node 20 is NOT installed.** Only `v26.1.0` on PATH. No `nvm`, `nvm-windows`, `fnm`, `volta`, or `nvs` found. | `node -v` → `v26.1.0`; no version manager on PATH or in usual dirs. | `.nvmrc` pins `20`; task mandates Node 20. **Provisioning Node 20 is a prerequisite for Stage 2.** ⚠️ decision needed. |
| **`npm ci` fails on a peer conflict.** | `ERESOLVE`: `esbuild@0.21.5` (pinned `^0.21.0`) vs `pcf-scripts@1.51.1` peer-wanting `esbuild@^0.25.8`. | Clean-clone install is broken as pinned. Workaround `npm ci --legacy-peer-deps` **succeeds**. Real fix in Stage 2 (bump esbuild or pin pcf-scripts). |
| **pcf-scripts toolchain RUNS under Node 26.** | `pcf-scripts start` progressed: Initialize → Validate manifest → Validate control → **Generate manifest types** → Generate design types → (ESLint). | The "Node 26 breaks pcf-scripts" assumption did **not** reproduce in build/typegen. The break is elsewhere (below). |
| **`build:pcf` fails only at the ESLint gate — a Phase 1 config defect.** | `no-undef` on `ComponentFramework`, `HTMLDivElement`, `document`, `HTMLInputElement`; base `no-unused-vars` on `_context`/`next`; **generated files linted**. | `.eslintrc.json` extends only `eslint:recommended` with the TS parser — no `env: browser`, no `plugin:@typescript-eslint/recommended`, no `ignorePatterns` for `**/generated/**`. Blocks build on **any** Node version. Small, must fix in Stage 2. |
| **Playwright browsers not installed.** | `~/AppData/Local/ms-playwright` absent. | Stage 2 / CI needs `npx playwright install chromium`. |

**Resolved tool versions** (after `npm ci --legacy-peer-deps`): `pcf-scripts`/`pcf-start` `1.51.1`, `esbuild` `0.21.5`, `@playwright/test` `1.59.1`, `typescript` `5.9.3`, `@fluentui/react-components` `9.73.8` (floated from `^9.46.0`), `react`/`react-dom` `16.14.0` (deduped everywhere), `@types/powerapps-component-framework` `1.3.18`.

> Net: the two things that actually block a green build today are **(a) the ESLint config** and **(b) the esbuild/pcf-scripts peer pin** — neither is a Node-version problem. Node 20 is still required by policy (`.nvmrc` + CI `UseNode@1 20.x`), and must be provisioned.

---

## 3. Approach comparison

Scores: **1 = poor / 5 = excellent**. "Agent/CI-friendly" = can a headless coding agent run it and read pass/fail + artifacts from stdout/files with no human, no browser UI, no live tenant.

| # | Approach | Effort to stand up | Fidelity to real MDA | Agent/CI-friendly | Offline | Needs Dataverse | Verdict |
|---|---|:--:|:--:|:--:|:--:|:--:|---|
| 1 | **esbuild HTML harness + vendored mock + Playwright** (SPEC 4–5) | 3 | 3 | **5** | ✅ | ❌ | **Baseline.** Deterministic, headless, screenshots/video/JUnit/HTML report. Scaffold half-built already. |
| 2 | **Stock `pcf-scripts build` + `pcf-start`/`npm start`** | 2 | 3–4 | 2 | ✅ (harness) | ❌ | Keep as the **manual dev inner-loop** + the canonical bundle/manifest validator. Not an automated test runner (opens a browser, no assertions, single-control focus). |
| 3 | **Copilot in Maker Portal** (scaffold/run controls) | 1 | 4 | 1 | ❌ | ✅ | Authoring/demo aid only. Not reproducible, not headless, not in-repo. Out of scope as a test strategy. |
| 4 | **Fiddler AutoResponder sideload** into a real form | 4 | **5** | 1 | ❌ | ✅ | Heavyweight "does it work in-situ" check. Manual, per-machine proxy setup, real tenant. Valuable occasionally; not a baseline. |
| 5 | **Live-environment test** (deploy to real Dataverse, drive it) | 4 | **5** | 2–3 | ❌ | ✅ | Highest fidelity. **Separate track** — needs a connection/secrets, slower, flakier. Design the mock so it can later be swapped for this; do not commit it now. |
| 6 | **Mock-data strategy** (cross-cutting) | — | — | — | — | — | Not an approach; the fidelity dial that determines how good #1 is. See §4. |

### Tradeoff notes per approach

- **#1 SPEC harness (recommended baseline).** esbuild bundles one control + the vendored `ComponentFrameworkMockGenerator` into an HTML page; Playwright drives it via `window.__pcf` hooks and asserts outputs/notify-count/dataset side-effects. Fully offline → perfect for headless CI and agents (`list` reporter → stdout, HTML report + JUnit + screenshots at deterministic paths). Fidelity is "as good as the mock" (currently minimal, gap-tracked). Effort is moderate but ~halved because Phases 1–3 exist and the SPEC sketches Phase 4–5 files concretely. Chromium-only matches MDA hosts.
- **#2 Stock tooling.** `pcf-scripts build` is the **authority** on "does this control compile + is the manifest valid + do types generate" — we should keep using it as `build:pcf` (it already regenerates `generated/ManifestTypes.d.ts` per control). `pcf-start`/`npm start` gives a live watch harness for a human eyeballing a control, but: it opens a real browser, has no assertion layer, and is oriented at one control at a time — so it's a dev inner-loop, not the automated gate. Complements #1; doesn't replace it.
- **#3 Copilot / Maker Portal.** Fast for scaffolding or a live demo, but nothing is reproducible or checked into the repo, requires a tenant + interactive UI, and can't be read by an agent. Not a local-testing strategy.
- **#4 Fiddler AutoResponder.** Genuinely useful when you must confirm behaviour *inside a real form* without a full deploy (map the platform's bundle URL to your local `bundle.js`). But it's manual, machine-specific (system proxy/HTTPS decryption), and needs a live record/form. Note it as an in-situ smoke option, not automation.
- **#5 Live-environment.** The only thing that catches real-runtime divergences (real `formatting`, `webAPI`, metadata, platform Fluent version, RTL/locale). Requires a service connection + secrets and is inherently slower/flakier. Keep it a **separate, opt-in track** layered on the same harness/registry later (swap the mock context for a thin real-context adapter). Per task constraints, describe-don't-build.

---

## 4. Mock-data fidelity — how faithfully each approach fakes the runtime

| Runtime surface | Mock (#1) today | Mock (#1) achievable | pcf-start (#2) | Live (#5) |
|---|---|---|---|---|
| `context.parameters` (bound fields) | ✅ `StringPropertyMock`/`NumberPropertyMock` | ✅ | ✅ (manual data) | ✅ real |
| Dataset records / columns / paging | ✅ `DatasetMock` (seeded, side-effects observable) | ✅ + paging behaviour | ⚠️ real view | ✅ real |
| `notifyOutputChanged` / `getOutputs` | ✅ captured & asserted | ✅ | ❌ no assertions | ✅ (via form) |
| `formatting` | ❌ `{}` stub (open gap) | ⚠️ re-implementable | ✅ real | ✅ real |
| `webAPI` | ❌ `{}` stub (open gap) | ⚠️ fakeable per test | ⚠️ needs tenant | ✅ real |
| `navigation` / `utils` | ❌ `{}` stub (open gap) | ⚠️ fakeable | ✅ real | ✅ real |
| Localization (`resources.getString`) | ⚠️ passthrough (returns key) | ✅ parse resx pair per `?lcid` (gap has a plan) | ✅ real resx | ✅ real |
| Metadata subsystem | ❌ absent (open gap) | ⚠️ re-implementable | ✅ real | ✅ real |

The mock is deliberately minimal and every divergence is tracked in `test/mock/UPSTREAM.md` (3 open gaps: metadata, localization, `formatting`/`webAPI`/`navigation`/`utils`). Baseline strategy: **only fake what the four samples read; log every stub as a gap; close gaps as controls demand them.** This is exactly the discipline the SPEC mandates and Stage 2 must respect.

---

## 5. Recommendation

**Adopt Approach #1 (SPEC Phases 4–5) as the offline baseline**, keep Approach #2 as the complementary build-validator + manual dev loop, and hold Approach #5 as a documented, separate future track. Concretely for Stage 2:

1. **Build Phase 4 (`test/harness/`)** — `index.html`, `mount.ts`, `registry.ts`, `esbuild.config.mjs` — per the SPEC sketch. Import the generated `ManifestTypes` and `@mock` types; no hand-rolled types.
2. **Build Phase 5 (`test/playwright/`)** — `playwright.config.ts`, `fixtures/pcf-page.ts`, four spec files (12 tests), `test/README.md`. Deterministic artifact paths; Chromium-only; `list`+`html`+`junit` reporters.
3. **Keep `build:pcf` = `pcf-scripts build`** as the compile/manifest gate (Approach #2), run before `npm test`.

**Two Phase-1 fixes are required to reach the SPEC's "green build" and must ship with Stage 2** (flag as deviations from the committed scaffold, not from the SPEC's intent):

- **A. Fix `.eslintrc.json`** — add `env: { browser: true, es2021: true, node: true }`, extend `plugin:@typescript-eslint/recommended` (which disables core `no-undef` in favour of the TS compiler), switch to `@typescript-eslint/no-unused-vars` with `argsIgnorePattern: "^_"`, and add `**/generated/**` to `ignorePatterns`. Without this, `build:pcf` cannot pass on any Node version.
- **B. Resolve the esbuild peer conflict** — recommend bumping `esbuild` to `^0.25.8` in `package.json` to align with `pcf-scripts@1.51`'s peer, so a clean `npm ci` works with no flag. (The SPEC pins `^0.21.0`; the esbuild `context()`/`serve()`/`build()` API used by the harness config is unchanged across 0.21→0.25, so the Phase-4 config is unaffected.) Alternative: keep 0.21 and document `npm ci --legacy-peer-deps` everywhere. **Recommend the bump** — a clean `npm ci` is more agent/CI-friendly than a required flag.

**Prerequisite — Node 20 provisioning (⚠️ needs your call):** `.nvmrc` and the intended CI both pin Node 20, but only Node 26 is installed and there's no version manager. Spikes show the build toolchain *runs* on 26, but I will honor the pin. Options in the summary below.

**Explicitly out of scope** (per task): F# scripts (Phase 6), Azure pipeline (Phase 7), any live-Dataverse deploy/test. The mock stays swappable so track #5 can be layered later, but only the offline baseline is committed.

---

## 6. Open risks / to validate during Stage 2

- **Fluent v9 selectors under React 16 + platform-library.** Specs target the SpinButton "increment" button and DataGrid row checkboxes by role/`data-record-id`; the resolved Fluent is `9.73.8` (not the manifest's `9.46.2`) — the accessible names/DOM may differ. Verify selectors when tests run; adjust if Fluent moved them.
- **`react-dom/client` legacy root under React 16.14.** The SPEC's `mount.ts` uses `ReactDOMClient.createRoot`. With `react-dom@16.14.0` there is no `react-dom/client`; may need `ReactDOM.render`/`unmountComponentAtNode`. Confirm at harness-build time and adapt the `ReactRenderer`.
- **`generated/ManifestTypes.d.ts` staleness.** The committed `SampleFieldVanilla/generated` currently holds a merged (all-controls) type set; `build:pcf` regenerates it per-control (observed "Generating manifest types" step). Run `build:pcf` before typechecking the harness.
- **esbuild `serve` + Playwright `webServer` port 4173.** Confirm the `--serve` config binds `127.0.0.1:4173` and Playwright's `reuseExistingServer` handshake works headless.
