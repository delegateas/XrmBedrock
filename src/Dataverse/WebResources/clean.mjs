import { argv } from "node:process";
import { rm } from "node:fs/promises";
import { glob } from "glob";

const projectDir = "./src/templatepublisherprefix_templateprojectname";
const bundleOnly = argv.includes("--bundle-only");
const generatedFiles = [
    `${projectDir}/WebResourceBundle.js`,
    `${projectDir}/WebResourceBundle.js.map`
];

if (!bundleOnly) {
    const sourceFiles = await glob(`${projectDir}/**/*.ts`, {
        ignore: [
            `${projectDir}/**/*.d.ts`,
            `${projectDir}/out/**`
        ]
    });

    for (const sourceFile of sourceFiles) {
        const outputBase = sourceFile.slice(0, -3);
        generatedFiles.push(`${outputBase}.js`, `${outputBase}.js.map`);
    }
}

await Promise.all(generatedFiles.map((file) => rm(file, { force: true })));
await rm(`${projectDir}/out`, { recursive: true, force: true });
