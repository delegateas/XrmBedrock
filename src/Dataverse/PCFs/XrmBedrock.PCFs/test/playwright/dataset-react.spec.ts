import { test, expect } from "./fixtures/pcf-page";

test.describe("SampleDatasetReact", () => {
    test("renders rows in DataGrid", async ({ pcf }) => {
        await pcf.mount("SampleDatasetReact");
        await expect(pcf.page.locator('[data-record-id="r1"]')).toBeVisible();
        await pcf.page.screenshot({ path: "test/test-results/dataset-react-initial.png" });
    });

    test("selection updates dataset.selectedIds", async ({ pcf }) => {
        await pcf.mount("SampleDatasetReact");
        await pcf.page.locator('[data-record-id="r1"] input[type="checkbox"]').check();
        const state = await pcf.getDatasetState();
        expect(state?.selectedIds).toContain("r1");
    });

    test("double-click opens record", async ({ pcf }) => {
        await pcf.mount("SampleDatasetReact");
        await pcf.page.locator('[data-record-id="r2"]').dblclick();
        const state = await pcf.getDatasetState();
        expect(state?.openedItems).toHaveLength(1);
    });
});
