import { context, build } from "esbuild";
import { fileURLToPath } from "node:url";
import { dirname, resolve } from "node:path";
import { existsSync, mkdirSync, copyFileSync } from "node:fs";

const __dirname = dirname(fileURLToPath(import.meta.url));
// test/harness -> project root is two levels up.
const projectRoot = resolve(__dirname, "../..");
const outdir = resolve(__dirname, "dist");
const serve = process.argv.includes("--serve");

if (!existsSync(outdir)) mkdirSync(outdir, { recursive: true });
copyFileSync(resolve(__dirname, "index.html"), resolve(outdir, "index.html"));

/** @type {import("esbuild").BuildOptions} */
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
    // `@mock` resolves to the vendored mock's barrel. The controls are imported
    // by relative path from registry.ts, so they need no alias.
    alias: {
        "@mock": resolve(projectRoot, "test/mock")
    },
    logLevel: "info"
};

if (serve) {
    const ctx = await context(opts);
    await ctx.watch();
    const { host, port } = await ctx.serve({ servedir: outdir, host: "127.0.0.1", port: 4173 });
    console.log(`Harness on http://${host}:${port}`);
} else {
    await build(opts);
}
