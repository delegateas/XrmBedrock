# Web resource development

Namespace-style Dataverse web resources are compiled as individual JavaScript files by default, matching the approach used by XrmFramework. A TypeScript file such as:

```text
src/templatepublisherprefix_templateprojectname/forms/account.ts
```

is emitted beside it as:

```text
src/templatepublisherprefix_templateprojectname/forms/account.js
```

This preserves global TypeScript namespaces across separately registered Dataverse web resources. Every generated JavaScript file contains an inline source map, so browser developer tools can map it back to its TypeScript source without requesting a separate `.map` file.

## Individual web resources — default

```bash
cd src/Dataverse/WebResources
npm install
npm run dev
```

`npm run dev` performs an initial build and then runs:

- TypeScript in watch mode using `tsconfig.json`
- A local HTTP server on `127.0.0.1:9999`

For example, the generated account resource above is available at:

```text
http://localhost:9999/forms/account.js
```

Configure the browser request-replacement tool to redirect each deployed web-resource request to its corresponding local URL. The server enables CORS and disables caching.

For a one-time compilation, run:

```bash
npm run build
```

## Optional bundled build

Projects that intentionally use the bundled/module-based approach can still build and develop that way:

```bash
npm run build:bundle
npm run dev:bundle
```

The bundle is emitted as:

```text
http://localhost:9999/WebResourceBundle.js
```

Bundle compilation uses `tsconfig.bundle.json` and esbuild. The final bundle also contains an inline source map.

## Cleaning generated files

```bash
npm run clean
```

The clean command removes JavaScript generated from TypeScript, bundle output, and temporary bundle files without deleting hand-maintained JavaScript under `lib`.
