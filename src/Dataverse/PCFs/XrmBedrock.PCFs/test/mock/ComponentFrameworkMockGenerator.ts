import { ContextMock } from "./ContextMock";
import { ControlConstructor, ReactRenderer } from "./types";

export interface MockGeneratorOptions {
    virtual?: boolean;
    reactRenderer?: ReactRenderer;
}

export class ComponentFrameworkMockGenerator<TInputs, TOutputs> {
    public readonly context: ContextMock<TInputs>;
    public readonly outputsHistory: TOutputs[] = [];
    public notifyCount: number = 0;

    private control: ComponentFramework.StandardControl<TInputs, TOutputs>
                   | ComponentFramework.ReactControl<TInputs, TOutputs>;
    private isVirtual: boolean;
    private renderer?: ReactRenderer;
    private container: HTMLDivElement;

    constructor(
        Ctor: ControlConstructor<TInputs, TOutputs>,
        parameters: TInputs,
        container: HTMLDivElement,
        options: MockGeneratorOptions = {}
    ) {
        this.control = new Ctor();
        this.context = new ContextMock(parameters);
        this.container = container;
        this.isVirtual = options.virtual ?? false;
        this.renderer = options.reactRenderer;
    }

    public init(): void {
        if (this.isVirtual) {
            (this.control as ComponentFramework.ReactControl<TInputs, TOutputs>).init(
                this.context as never,
                this.notify,
                {}
            );
            this.renderVirtual();
        } else {
            (this.control as ComponentFramework.StandardControl<TInputs, TOutputs>).init(
                this.context as never,
                this.notify,
                {},
                this.container
            );
            (this.control as ComponentFramework.StandardControl<TInputs, TOutputs>).updateView(this.context as never);
        }
    }

    public fireUpdateView(updatedProperties: string[] = []): void {
        this.context.updatedProperties = updatedProperties;
        if (this.isVirtual) {
            this.renderVirtual();
        } else {
            (this.control as ComponentFramework.StandardControl<TInputs, TOutputs>).updateView(this.context as never);
        }
    }

    public destroy(): void {
        this.control.destroy();
        if (this.isVirtual && this.renderer) this.renderer.unmount(this.container);
    }

    private notify = (): void => {
        this.notifyCount++;
        const out = this.control.getOutputs?.();
        if (out) this.outputsHistory.push(out);
    };

    private renderVirtual(): void {
        if (!this.renderer) throw new Error("Virtual control requires reactRenderer in options");
        const element = (this.control as ComponentFramework.ReactControl<TInputs, TOutputs>).updateView(this.context as never);
        this.renderer.render(element, this.container);
    }
}
