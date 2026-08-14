import concurrently from "concurrently";

const bundleMode = process.argv.includes("bundle");

const commands = [
    {
        command: bundleMode
            ? "tsc -p tsconfig.bundle.json --watch --preserveWatchOutput"
            : "tsc -p tsconfig.json --watch --preserveWatchOutput",
        name: "tsc",
        prefixColor: "auto"
    },
    ...(bundleMode
        ? [{
            command: "node esbuild.config.mjs watch",
            name: "esbuild",
            prefixColor: "auto"
        }]
        : []),
    {
        command: "npm run serve",
        name: "http",
        prefixColor: "auto"
    }
];

const { result } = concurrently(commands, {
    killOthers: ["failure"]
});

result.then(
    () => {
        console.log("All development processes exited successfully");
    },
    (error) => {
        console.error("One or more development processes failed:", error);
        process.exitCode = 1;
    }
);
