import * as React from "react";
import {
    FluentProvider,
    webLightTheme,
    SpinButton,
    SpinButtonOnChangeData,
    SpinButtonChangeEvent
} from "@fluentui/react-components";

export interface AppProps {
    value: number;
    min: number;
    max: number;
    onChange: (next: number) => void;
}

export const App: React.FC<AppProps> = ({ value, min, max, onChange }) => {
    const handleChange = (
        _ev: SpinButtonChangeEvent,
        data: SpinButtonOnChangeData
    ) => {
        const next = data.value ?? Number(data.displayValue ?? value);
        if (Number.isFinite(next)) onChange(next);
    };

    return (
        <FluentProvider theme={webLightTheme}>
            <SpinButton
                value={value}
                min={min}
                max={max}
                onChange={handleChange}
                input={{ "data-testid": "field-react-input" } as React.InputHTMLAttributes<HTMLInputElement>}
            />
        </FluentProvider>
    );
};
