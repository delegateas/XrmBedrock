import { SampleFieldVanilla } from "../../SampleFieldVanilla";
import { SampleFieldReact } from "../../SampleFieldReact";
import { SampleDatasetVanilla } from "../../SampleDatasetVanilla";
import { SampleDatasetReact } from "../../SampleDatasetReact";
import { StringPropertyMock, NumberPropertyMock, DatasetMock } from "@mock";
import type { ControlConstructor } from "@mock";

// Pull the vanilla controls' CSS into the harness bundle so esbuild emits mount.css
// (index.html links it). React controls style themselves via FluentProvider.
import "../../SampleFieldVanilla/css/SampleFieldVanilla.css";
import "../../SampleDatasetVanilla/css/SampleDatasetVanilla.css";

export interface RegistryEntry {
    ctor: ControlConstructor<unknown, unknown>;
    virtual: boolean;
    buildParams: (seed?: unknown) => unknown;
}

export const registry: Record<string, RegistryEntry> = {
    SampleFieldVanilla: {
        ctor: SampleFieldVanilla as never,
        virtual: false,
        buildParams: (seed: { value?: string } = {}) => ({
            value: new StringPropertyMock(seed.value ?? "hello world")
        })
    },
    SampleFieldReact: {
        ctor: SampleFieldReact as never,
        virtual: true,
        buildParams: (seed: { value?: number; min?: number; max?: number } = {}) => ({
            value: new NumberPropertyMock(seed.value ?? 5),
            min: new NumberPropertyMock(seed.min ?? 0),
            max: new NumberPropertyMock(seed.max ?? 10)
        })
    },
    SampleDatasetVanilla: {
        ctor: SampleDatasetVanilla as never,
        virtual: false,
        buildParams: (seed?: never) => ({
            records: new DatasetMock(seed ?? defaultDatasetSeed())
        })
    },
    SampleDatasetReact: {
        ctor: SampleDatasetReact as never,
        virtual: true,
        buildParams: (seed?: never) => ({
            records: new DatasetMock(seed ?? defaultDatasetSeed())
        })
    }
};

function defaultDatasetSeed() {
    return {
        entityName: "account",
        columns: [
            { name: "name", displayName: "Name", dataType: "SingleLine.Text", alias: "", order: 0, visualSizeFactor: 1 },
            { name: "city", displayName: "City", dataType: "SingleLine.Text", alias: "", order: 1, visualSizeFactor: 1 }
        ],
        records: [
            { id: "r1", name: "Contoso", city: "Copenhagen" },
            { id: "r2", name: "Fabrikam", city: "Aarhus" },
            { id: "r3", name: "Northwind", city: "Odense" }
        ]
    };
}
