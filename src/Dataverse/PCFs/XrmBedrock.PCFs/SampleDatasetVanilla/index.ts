import { IInputs, IOutputs } from "./generated/ManifestTypes";

export class SampleDatasetVanilla implements ComponentFramework.StandardControl<IInputs, IOutputs> {
    private container: HTMLDivElement;
    private context: ComponentFramework.Context<IInputs>;

    public init(
        context: ComponentFramework.Context<IInputs>,
        _notifyOutputChanged: () => void,
        _state: ComponentFramework.Dictionary,
        container: HTMLDivElement
    ): void {
        this.container = container;
        this.context = context;
    }

    public updateView(context: ComponentFramework.Context<IInputs>): void {
        this.context = context;
        this.render();
    }

    private render(): void {
        const ds = this.context.parameters.records;
        this.container.innerHTML = "";

        const table = document.createElement("table");
        table.classList.add("xrmbedrock-dataset-vanilla");
        table.setAttribute("data-testid", "dataset-vanilla-table");

        const thead = document.createElement("thead");
        const headRow = document.createElement("tr");
        ds.columns.forEach(col => {
            const th = document.createElement("th");
            th.textContent = col.displayName;
            th.dataset.columnName = col.name;
            headRow.appendChild(th);
        });
        thead.appendChild(headRow);
        table.appendChild(thead);

        const tbody = document.createElement("tbody");
        ds.sortedRecordIds.forEach(id => {
            const record = ds.records[id];
            const tr = document.createElement("tr");
            tr.setAttribute("data-record-id", id);
            tr.addEventListener("click", () => {
                ds.openDatasetItem(record.getNamedReference());
            });
            ds.columns.forEach(col => {
                const td = document.createElement("td");
                td.textContent = String(record.getFormattedValue(col.name) ?? "");
                tr.appendChild(td);
            });
            tbody.appendChild(tr);
        });
        table.appendChild(tbody);
        this.container.appendChild(table);

        if (ds.paging?.hasNextPage) {
            const next = document.createElement("button");
            next.textContent = "Next page";
            next.setAttribute("data-testid", "dataset-vanilla-next");
            next.addEventListener("click", () => ds.paging.loadNextPage());
            this.container.appendChild(next);
        }
    }

    public getOutputs(): IOutputs {
        return {};
    }

    public destroy(): void {
        this.container.innerHTML = "";
    }
}
