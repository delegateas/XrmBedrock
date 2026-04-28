import * as React from "react";
import {
    FluentProvider,
    webLightTheme,
    DataGrid,
    DataGridHeader,
    DataGridHeaderCell,
    DataGridBody,
    DataGridRow,
    DataGridCell,
    TableColumnDefinition,
    createTableColumn
} from "@fluentui/react-components";

export interface AppProps {
    columns: { key: string; name: string }[];
    rows: Record<string, string>[];
    onSelectionChange: (selectedIds: string[]) => void;
    onOpen: (id: string) => void;
}

export const App: React.FC<AppProps> = ({ columns, rows, onSelectionChange, onOpen }) => {
    const colDefs: TableColumnDefinition<Record<string, string>>[] = React.useMemo(
        () => columns.map(c => createTableColumn<Record<string, string>>({
            columnId: c.key,
            renderHeaderCell: () => c.name,
            renderCell: (item) => item[c.key]
        })),
        [columns]
    );

    return (
        <FluentProvider theme={webLightTheme}>
            <DataGrid
                items={rows}
                columns={colDefs}
                getRowId={(item) => item.__id}
                selectionMode="multiselect"
                onSelectionChange={(_, data) => onSelectionChange(Array.from(data.selectedItems) as string[])}
                data-testid="dataset-react-grid"
            >
                <DataGridHeader>
                    <DataGridRow>
                        {({ renderHeaderCell }) => (
                            <DataGridHeaderCell>{renderHeaderCell()}</DataGridHeaderCell>
                        )}
                    </DataGridRow>
                </DataGridHeader>
                <DataGridBody<Record<string, string>>>
                    {({ item, rowId }) => (
                        <DataGridRow<Record<string, string>>
                            key={rowId}
                            onDoubleClick={() => onOpen(item.__id)}
                            data-record-id={item.__id}
                        >
                            {({ renderCell }) => <DataGridCell>{renderCell(item)}</DataGridCell>}
                        </DataGridRow>
                    )}
                </DataGridBody>
            </DataGrid>
        </FluentProvider>
    );
};
