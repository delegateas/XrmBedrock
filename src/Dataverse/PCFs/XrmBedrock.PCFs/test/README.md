# PCF tests

Local, **fully offline** end-to-end verification of the sample controls. An esbuild
harness bundles a control together with the vendored ComponentFramework mock
(`test/mock/`) into an HTML page; Playwright drives it headless in Chromium and reads
control outputs / side-effects back through the `window.__pcf` hooks. No Dataverse
connection is required, so the suite runs in CI and is readable by coding agents.

## Prerequisites

- **Node 20** (see `.nvmrc`). The toolchain was verified on Node 20.20.2.
- `npm ci` then `npx playwright install chromium` (first run only, to fetch the browser).

## Run

- `npm run build:pcf` — compiles all four controls via `pcf-scripts` (manifest + type gate).
- `npm test` — builds the harness, starts the esbuild dev server, runs Playwright headless. Exit code is the signal.
- `npm run test:headed` — same, but watch it run in a browser.
- `npm run test:report` — open the HTML report.

## Output paths (deterministic, agent-friendly)

- `test/playwright-report/index.html` — full HTML report
- `test/junit-results.xml` — JUnit results
- `test/test-results/<name>.png` — per-test screenshots (e.g. `field-vanilla-initial.png`)
- `test/test-results/**/video.webm` — videos, retained on failure

## Drive the harness manually

```
npm run serve:harness
```

Then open: `http://127.0.0.1:4173/?control=<ControlName>&seed=<base64-encoded-json>`

`seed` is `btoa(encodeURIComponent(JSON.stringify(seed)))`.

Available controls: `SampleFieldVanilla`, `SampleFieldReact`, `SampleDatasetVanilla`, `SampleDatasetReact`.

## Test hooks (in the browser console)

```js
window.__pcf.getOutputs()
window.__pcf.getNotifyCount()
window.__pcf.getContext()
window.__pcf.fireUpdateView({ value: "..." }, ["value"])
window.__pcf.getDatasetState()
```

## How it fits together

```
control source (SampleXxx/index.ts, App.tsx)
        │  imported by
test/harness/registry.ts  ──►  test/harness/mount.ts  ──►  test/harness/index.html
        │                              │ uses
        │                     test/mock/ComponentFrameworkMockGenerator (+ ContextMock, DatasetMock, PropertyMocks)
        │  bundled by esbuild (test/harness/esbuild.config.mjs) into dist/
        ▼
test/playwright/*.spec.ts  ──►  fixtures/pcf-page.ts  ──►  window.__pcf
```

The mock is deliberately minimal; its known gaps and the policy for closing them
live in [`../test/mock/UPSTREAM.md`](../test/mock/UPSTREAM.md). The harness is designed
so the mock context could later be swapped for a real-environment adapter without
touching the specs — but this suite commits only the offline baseline.
