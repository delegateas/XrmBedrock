import { argv } from "node:process";
import * as esbuild from "esbuild";
import * as glob from "glob";
import { relative, dirname } from "path";
import { mkdirSync, writeFileSync } from "fs";
import fs from 'fs';
import path from 'path';

const watchMode = (argv.length > 2 && "watch" === argv[2]);

// WebResource Project Directory Path
const projectDir = "./src/ctx_XrmBedrock";

// Collect all JS files in the modules directory
const entryPoints = glob.sync(`${projectDir}/out/**/*.js`).sort();

// Generate _index.js that imports all modules and XrmQuery
const indexPath = `${projectDir}/out/_index.js`;

mkdirSync(path.dirname(indexPath), { recursive: true });

let importLines = entryPoints
    .map(f => `import "./${relative(dirname(indexPath), f).replace(/\\/g, "/")}";`)
    .join("\n");

writeFileSync(indexPath, importLines);

// Build options for ESBuild
// This will bundle into a single WebResourceBundle.js file
const buildOptions = {
    entryPoints: [indexPath],
    bundle: true,
    outfile: `${projectDir}/WebResourceBundle.js`,
    sourcemap: true,
    format: "cjs",
    logLevel: "info",
    banner: {
        js: "var module = module || {};\n"
    }
};

if (watchMode) {
    console.info("Starting ESBuild in watch mode");
    const ctx = await esbuild.context(buildOptions);
    await ctx.watch();
} else {
    console.info("Starting ESBuild for a single run");
    await esbuild.build(buildOptions);
}