/*
*This is auto generated from the ControlManifest.Input.xml file
*/

// Define IInputs and IOutputs Type. They should match with ControlManifest.
export interface IInputs {
    value: ComponentFramework.PropertyTypes.WholeNumberProperty;
    min: ComponentFramework.PropertyTypes.WholeNumberProperty;
    max: ComponentFramework.PropertyTypes.WholeNumberProperty;
    records: ComponentFramework.PropertyTypes.DataSet;
}
export interface IOutputs {
    value?: number;
}
