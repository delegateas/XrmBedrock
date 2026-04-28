import * as React from "react";
import { App, AppProps } from "./App";
import { IInputs, IOutputs } from "./generated/ManifestTypes";

export class SampleDatasetReact implements ComponentFramework.ReactControl<IInputs, IOutputs> {
    public init(_context: ComponentFramework.Context<IInputs>, _notify: () => void): void { /* no-op */ }

    public updateView(context: ComponentFramework.Context<IInputs>): React.ReactElement {
        const ds = context.parameters.records;
        const props: AppProps = {
            columns: ds.columns.map(c => ({ key: c.name, name: c.displayName })),
            rows: ds.sortedRecordIds.map(id => {
                const r = ds.records[id];
                const row: Record<string, string> = { __id: id };
                ds.columns.forEach(c => row[c.name] = String(r.getFormattedValue(c.name) ?? ""));
                return row;
            }),
            onSelectionChange: (selectedIds) => ds.setSelectedRecordIds(selectedIds),
            onOpen: (id) => ds.openDatasetItem(ds.records[id].getNamedReference())
        };
        return React.createElement(App, props);
    }

    public getOutputs(): IOutputs { return {}; }
    public destroy(): void { /* no-op */ }
}
