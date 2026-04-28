import * as React from "react";
import { App, AppProps } from "./App";
import { IInputs, IOutputs } from "./generated/ManifestTypes";

export class SampleFieldReact implements ComponentFramework.ReactControl<IInputs, IOutputs> {
    private currentValue: number = 0;
    private notifyOutputChanged: () => void;

    public init(
        _context: ComponentFramework.Context<IInputs>,
        notifyOutputChanged: () => void
    ): void {
        this.notifyOutputChanged = notifyOutputChanged;
    }

    public updateView(context: ComponentFramework.Context<IInputs>): React.ReactElement {
        const props: AppProps = {
            value: context.parameters.value.raw ?? 0,
            min: context.parameters.min.raw ?? 0,
            max: context.parameters.max.raw ?? 100,
            onChange: (next: number) => {
                this.currentValue = next;
                this.notifyOutputChanged();
            }
        };
        return React.createElement(App, props);
    }

    public getOutputs(): IOutputs {
        return { value: this.currentValue };
    }

    public destroy(): void { /* no-op */ }
}
