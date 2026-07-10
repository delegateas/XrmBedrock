import { test, expect } from "./fixtures/pcf-page";

test.describe("SampleDatasetVanilla", () => {
    test("renders seeded rows", async ({ pcf }) => {
        await pcf.mount("SampleDatasetVanilla");
        const rows = pcf.page.locator('[data-testid="dataset-vanilla-table"] tbody tr');
        await expect(rows).toHaveCount(3);
        await pcf.page.screenshot({ path: "test/test-results/dataset-vanilla-initial.png" });
    });

    test("clicking a row calls openDatasetItem", async ({ pcf }) => {
        await pcf.mount("SampleDatasetVanilla");
        await pcf.page.locator('tr[data-record-id="r2"]').click();
        const state = await pcf.getDatasetState();
        expect(state?.openedItems).toHaveLength(1);
    });

    test("external updateView re-renders rows", async ({ pcf }) => {
        await pcf.mount("SampleDatasetVanilla");
        await pcf.fireUpdateView({}, ["records"]);
        await expect(pcf.page.locator('[data-testid="dataset-vanilla-table"]')).toBeVisible();
        await pcf.page.screenshot({ path: "test/test-results/dataset-vanilla-updated.png" });
    });
});
