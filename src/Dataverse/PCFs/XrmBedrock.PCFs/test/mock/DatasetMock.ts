type Column = ComponentFramework.PropertyHelper.DataSetApi.Column;
type EntityRecord = ComponentFramework.PropertyHelper.DataSetApi.EntityRecord;

export interface DatasetSeed {
    entityName?: string;
    columns: Column[];
    records: Array<Record<string, unknown> & { id?: string }>;
    pageSize?: number;
}

export class DatasetMock implements ComponentFramework.PropertyTypes.DataSet {
    records: { [id: string]: EntityRecord } = {};
    sortedRecordIds: string[] = [];
    columns: Column[];
    paging: ComponentFramework.PropertyHelper.DataSetApi.Paging;
    loading: boolean = false;
    filtering: ComponentFramework.PropertyHelper.DataSetApi.Filtering = {} as never;
    linking: ComponentFramework.PropertyHelper.DataSetApi.Linking = {} as never;
    sorting: ComponentFramework.PropertyHelper.DataSetApi.SortStatus[] = [];
    error: boolean = false;
    errorMessage: string = "";

    private selected: Set<string> = new Set();
    private entityName: string;

    public openedItems: ComponentFramework.EntityReference[] = [];
    public refreshCount: number = 0;

    constructor(seed: DatasetSeed) {
        this.entityName = seed.entityName ?? "account";
        this.columns = seed.columns;
        seed.records.forEach((r, idx) => {
            const id = r.id ?? `record-${idx}`;
            this.sortedRecordIds.push(String(id));
            this.records[String(id)] = this.makeRecord(String(id), r);
        });
        this.paging = this.makePaging(seed.pageSize ?? 25);
    }

    addColumn?: (name: string, entityAlias?: string) => void;

    getSelectedRecordIds = (): string[] => Array.from(this.selected);
    setSelectedRecordIds = (ids: string[]): void => { this.selected = new Set(ids); };
    clearSelectedRecordIds = (): void => { this.selected.clear(); };
    getTargetEntityType = (): string => this.entityName;
    getTitle = (): string => "";
    getViewId = (): string => "";
    refresh = (): void => { this.refreshCount++; };
    openDatasetItem = (ref: ComponentFramework.EntityReference): void => { this.openedItems.push(ref); };

    private makeRecord(id: string, data: Record<string, unknown>): EntityRecord {
        return {
            getRecordId: () => id,
            getValue: (col: string) => data[col] as never,
            getFormattedValue: (col: string) => String(data[col] ?? ""),
            getNamedReference: () => ({
                id: { guid: id } as never,
                name: String(data.name ?? id),
                etn: this.entityName
            } as ComponentFramework.EntityReference),
            isDirty: () => false
        } as EntityRecord;
    }

    private makePaging(pageSize: number): ComponentFramework.PropertyHelper.DataSetApi.Paging {
        return {
            pageSize,
            totalResultCount: this.sortedRecordIds.length,
            hasNextPage: false,
            hasPreviousPage: false,
            firstPageNumber: 1,
            lastPageNumber: 1,
            loadNextPage: () => { /* no-op */ },
            loadPreviousPage: () => { /* no-op */ },
            loadExactPage: () => { /* no-op */ },
            reset: () => { /* no-op */ },
            setPageSize: (n: number) => { (this.paging as { pageSize: number }).pageSize = n; }
        };
    }
}
