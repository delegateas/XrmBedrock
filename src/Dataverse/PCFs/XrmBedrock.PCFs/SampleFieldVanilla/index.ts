import { IInputs, IOutputs } from "./generated/ManifestTypes";

export class SampleFieldVanilla implements ComponentFramework.StandardControl<IInputs, IOutputs> {
    private container: HTMLDivElement;
    private input: HTMLInputElement;
    private currentValue: string = "";
    private notifyOutputChanged: () => void;

    public init(
        context: ComponentFramework.Context<IInputs>,
        notifyOutputChanged: () => void,
        _state: ComponentFramework.Dictionary,
        container: HTMLDivElement
    ): void {
        this.container = container;
        this.notifyOutputChanged = notifyOutputChanged;

        this.input = document.createElement("input");
        this.input.type = "text";
        this.input.classList.add("xrmbedrock-field-vanilla");
        this.input.setAttribute("data-testid", "field-vanilla-input");
        this.input.addEventListener("input", this.handleInput);
        this.container.appendChild(this.input);
    }

    public updateView(context: ComponentFramework.Context<IInputs>): void {
        const incoming = context.parameters.value.raw ?? "";
        if (incoming !== this.currentValue) {
            this.currentValue = incoming;
            this.input.value = incoming;
        }
    }

    public getOutputs(): IOutputs {
        return { value: this.currentValue };
    }

    public destroy(): void {
        this.input.removeEventListener("input", this.handleInput);
    }

    private handleInput = (): void => {
        this.currentValue = this.input.value;
        this.notifyOutputChanged();
    };
}
