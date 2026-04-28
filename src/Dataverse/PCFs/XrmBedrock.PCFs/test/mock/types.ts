export type ControlConstructor<TInputs, TOutputs> =
    new () => ComponentFramework.StandardControl<TInputs, TOutputs>
            | ComponentFramework.ReactControl<TInputs, TOutputs>;

export interface ReactRenderer {
    render(element: React.ReactElement, container: HTMLDivElement): void;
    unmount(container: HTMLDivElement): void;
}
