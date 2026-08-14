import { argv } from "node:process";
import * as esbuild from "esbuild";
import { globSync } from "glob";
import { dirname, relative } from "node:path";
import { mkdirSync, readFileSync, watch, writeFileSync } from "node:fs";

const watchMode = argv.includes("watch");

// WebResource project directory path
const projectDir = "./src/templatepublisherprefix_templateprojectname";
const outputDir = `${projectDir}/out`;
const indexPath = `${outputDir}/_index.js`;

mkdirSync(outputDir, { recursive: true });

function refreshIndex() {
    const entryPoints = globSync(`${outputDir}/**/*.js`, {
        ignore: indexPath
    }).sort();

    const importLines = entryPoints
        .map((file) => `import "./${relative(dirname(indexPath), file).replace(/\\/g, "/")}";`)
        .join("\n");

    let currentContents;
    try {
        currentContents = readFileSync(indexPath, "utf8");
    } catch {
        currentContents = undefined;
    }

    if (currentContents !== importLines) {
        writeFileSync(indexPath, importLines);
    }
}

refreshIndex();

const buildOptions = {
    entryPoints: [indexPath],
    bundle: true,
    outfile: `${projectDir}/WebResourceBundle.js`,
    // Keep local debugging self-contained: no separate .map request is needed.
    sourcemap: "inline",
    sourcesContent: true,
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

    // esbuild follows files already imported by _index.js. Refreshing the index
    // also lets watch mode discover newly added or removed TypeScript outputs.
    let refreshTimer;
    watch(outputDir, { recursive: true }, (_eventType, fileName) => {
        if (!fileName?.endsWith(".js") || fileName.endsWith("_index.js")) {
            return;
        }

        clearTimeout(refreshTimer);
        refreshTimer = setTimeout(refreshIndex, 50);
    });
} else {
    console.info("Starting ESBuild for a single run");
    await esbuild.build(buildOptions);
}
