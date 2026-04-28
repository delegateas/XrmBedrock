export class StringPropertyMock implements ComponentFramework.PropertyTypes.StringProperty {
    raw: string | null;
    formatted?: string;
    error: boolean = false;
    errorMessage: string = "";
    type: string = "SingleLine.Text";
    attributes?: ComponentFramework.PropertyHelper.FieldPropertyMetadata.StringMetadata;
    security?: ComponentFramework.PropertyHelper.SecurityValues;

    constructor(value: string | null = null) { this.raw = value; }
}

export class NumberPropertyMock implements ComponentFramework.PropertyTypes.NumberProperty {
    raw: number | null;
    formatted?: string;
    error: boolean = false;
    errorMessage: string = "";
    type: string = "Whole.None";
    attributes?: ComponentFramework.PropertyHelper.FieldPropertyMetadata.WholeNumberMetadata;
    security?: ComponentFramework.PropertyHelper.SecurityValues;

    constructor(value: number | null = null) { this.raw = value; }
}
