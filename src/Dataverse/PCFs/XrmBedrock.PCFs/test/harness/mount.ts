import * as ReactDOM from "react-dom";
import { ComponentFrameworkMockGenerator } from "@mock";
import type { ReactRenderer } from "@mock";
import { registry } from "./registry";

const url = new URL(window.location.href);
const controlName = url.searchParams.get("control") ?? "SampleFieldVanilla";
const seedRaw = url.searchParams.get("seed");
const seed = seedRaw ? JSON.parse(decodeURIComponent(atob(seedRaw))) : undefined;

const meta = document.getElementById("harness-meta")!;
const root = document.getElementById("pcf-root") as HTMLDivElement;

const entry = registry[controlName];
if (!entry) {
    meta.textContent = `Unknown control: ${controlName}`;
    throw new Error(`Unknown control: ${controlName}`);
}

// DEVIATION FROM SPEC: react-dom@16.14 has no `react-dom/client` (createRoot) —
// that is a React 18 API. Use the React 16 legacy render/unmount instead. React
// is pinned at 16.14.0 to match the model-driven-app host, so this is the correct
// renderer for the pinned version. If React is bumped to 18 here, switch to
// `react-dom/client`'s createRoot.
const reactRenderer: ReactRenderer = {
    render(element, container) {
        ReactDOM.render(element, container);
    },
    unmount(container) {
        ReactDOM.unmountComponentAtNode(container);
        container.innerHTML = "";
    }
};

const params = entry.buildParams(seed);
const generator = new ComponentFrameworkMockGenerator(
    entry.ctor as never,
    params as never,
    root,
    { virtual: entry.virtual, reactRenderer }
);
generator.init();

// Test hooks — Playwright drives these via page.evaluate.
declare global {
    interface Window {
        __pcf: {
            controlName: string;
            getOutputs: () => unknown[];
            getNotifyCount: () => number;
            getContext: () => unknown;
            fireUpdateView: (patch: Record<string, unknown>, updatedProperties?: string[]) => void;
            getDatasetState: () => { selectedIds: string[]; openedItems: unknown[]; refreshCount: number } | null;
        };
    }
}

window.__pcf = {
    controlName,
    getOutputs: () => generator.outputsHistory,
    getNotifyCount: () => generator.notifyCount,
    getContext: () => generator.context,
    fireUpdateView: (patch, updatedProperties = []) => {
        Object.entries(patch).forEach(([key, value]) => {
            const prop = (generator.context.parameters as Record<string, unknown>)[key];
            if (prop && typeof prop === "object" && "raw" in (prop as object)) {
                (prop as { raw: unknown }).raw = value;
            }
        });
        generator.fireUpdateView(updatedProperties);
    },
    getDatasetState: () => {
        const ds = (generator.context.parameters as { records?: unknown }).records as
            { getSelectedRecordIds(): string[]; openedItems: unknown[]; refreshCount: number } | undefined;
        if (!ds || !("openedItems" in ds)) return null;
        return {
            selectedIds: ds.getSelectedRecordIds(),
            openedItems: ds.openedItems,
            refreshCount: ds.refreshCount
        };
    }
};

meta.textContent = `Mounted: ${controlName} (${entry.virtual ? "virtual" : "standard"})`;
meta.setAttribute("data-control", controlName);
meta.setAttribute("data-ready", "true");
