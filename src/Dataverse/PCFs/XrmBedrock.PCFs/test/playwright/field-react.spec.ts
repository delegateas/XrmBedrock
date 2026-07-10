import { test, expect } from "./fixtures/pcf-page";

test.describe("SampleFieldReact", () => {
    test("renders seeded value with min/max bounds", async ({ pcf }) => {
        await pcf.mount("SampleFieldReact", { value: 5, min: 0, max: 10 });
        await expect(pcf.page.locator('[data-testid="field-react-input"]')).toHaveValue("5");
        await pcf.page.screenshot({ path: "test/test-results/field-react-initial.png" });
    });

    test("incrementing emits new value", async ({ pcf }) => {
        await pcf.mount("SampleFieldReact", { value: 5, min: 0, max: 10 });
        await pcf.page.getByRole("button", { name: /increment/i }).click();
        await expect.poll(() => pcf.getNotifyCount()).toBeGreaterThan(0);
        const outputs = await pcf.getOutputs();
        expect((outputs[outputs.length - 1] as { value: number }).value).toBe(6);
    });

    test("external updateView re-renders", async ({ pcf }) => {
        await pcf.mount("SampleFieldReact", { value: 5, min: 0, max: 10 });
        await pcf.fireUpdateView({ value: 8 }, ["value"]);
        await expect(pcf.page.locator('[data-testid="field-react-input"]')).toHaveValue("8");
        await pcf.page.screenshot({ path: "test/test-results/field-react-updated.png" });
    });
});
