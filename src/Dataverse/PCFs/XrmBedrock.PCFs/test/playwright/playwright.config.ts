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
