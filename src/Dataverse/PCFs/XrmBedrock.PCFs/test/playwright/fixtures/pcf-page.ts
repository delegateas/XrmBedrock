import { test as base, expect, Page } from "@playwright/test";

export interface PcfPage {
    mount: (controlName: string, seed?: unknown) => Promise<void>;
    fireUpdateView: (patch: Record<string, unknown>, updatedProperties?: string[]) => Promise<void>;
    getOutputs: () => Promise<unknown[]>;
    getNotifyCount: () => Promise<number>;
    getDatasetState: () => Promise<{ selectedIds: string[]; openedItems: unknown[]; refreshCount: number } | null>;
    page: Page;
}

export const test = base.extend<{ pcf: PcfPage }>({
    pcf: async ({ page }, use) => {
        const api: PcfPage = {
            page,
            mount: async (controlName, seed) => {
                const url = new URL("/", "http://127.0.0.1:4173");
                url.searchParams.set("control", controlName);
                if (seed !== undefined) {
                    const encoded = btoa(encodeURIComponent(JSON.stringify(seed)));
                    url.searchParams.set("seed", encoded);
                }
                await page.goto(url.toString());
                await page.locator('[data-testid="harness-meta"][data-ready="true"]').waitFor();
            },
            fireUpdateView: (patch, updatedProperties = []) =>
                page.evaluate(
                    ([p, u]) => window.__pcf.fireUpdateView(p as never, u as never),
                    [patch, updatedProperties] as const
                ),
            getOutputs: () => page.evaluate(() => window.__pcf.getOutputs()),
            getNotifyCount: () => page.evaluate(() => window.__pcf.getNotifyCount()),
            getDatasetState: () => page.evaluate(() => window.__pcf.getDatasetState())
        };
        await use(api);
    }
});

export { expect };
