import { test, expect } from "./fixtures/pcf-page";

test.describe("SampleFieldVanilla", () => {
    test("renders seeded value", async ({ pcf }) => {
        await pcf.mount("SampleFieldVanilla", { value: "initial value" });

        const input = pcf.page.locator('[data-testid="field-vanilla-input"]');
        await expect(input).toHaveValue("initial value");
        await pcf.page.screenshot({ path: "test/test-results/field-vanilla-initial.png" });
    });

    test("typing fires notifyOutputChanged with new value", async ({ pcf }) => {
        await pcf.mount("SampleFieldVanilla", { value: "" });

        await pcf.page.locator('[data-testid="field-vanilla-input"]').fill("typed by user");

        await expect.poll(() => pcf.getNotifyCount()).toBeGreaterThan(0);
        const outputs = await pcf.getOutputs();
        expect(outputs[outputs.length - 1]).toEqual({ value: "typed by user" });
    });

    test("external updateView re-renders the input", async ({ pcf }) => {
        await pcf.mount("SampleFieldVanilla", { value: "first" });

        await pcf.fireUpdateView({ value: "second" }, ["value"]);

        await expect(pcf.page.locator('[data-testid="field-vanilla-input"]')).toHaveValue("second");
        await pcf.page.screenshot({ path: "test/test-results/field-vanilla-updated.png" });
    });
});
