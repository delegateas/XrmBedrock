# Vendored mock — upstream provenance

Source: https://github.com/Shko-Online/ComponentFramework-Mock
License: MIT (see ./LICENSE)
Copied from commit: d356ee181ae9338bfeb8aa2a1f9db1e1941d6f4b
Date copied: 2026-04-28

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
