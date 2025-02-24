declare namespace Xrm {
  /**
   * Enum which corresponds to the values of Xrm.Page.ui.getFormType()
   */
  const enum FormType {
    Undefined = 0,
    Create = 1,
    Update = 2,
    ReadOnly = 3,
    Disabled = 4,
    QuickCreate = 5,
    BulkEdit = 6,
  }

  /**
   * Interface for an option set value.
   */
  interface Option<T> {
    /**
     * Label for the option.
     */
    text: string;
    /**
     * Value for the option.
     */
    value: T;
  }

  /**
   * Interface for an user privileges for an attribute.
   */
  interface UserPrivilege {
    /**
     * Specificies if the user can read data values for the attribute.
     */
    canRead: boolean;
    /**
     * Specificies if the user can update data values for the attribute.
     */
    canUpdate: boolean;
    /**
     * Specificies if the user can create data values for the attribute.
     */
    canCreate: boolean;
  }

  /**
   * Interface for an entity reference for the Xrm.Page context.
   */
  interface EntityReference<T extends string> {
    id: string;
    entityType: T;
    name?: string | null;
  }

  /**
   * Interface of the base functionality of a collection without the 'get' function.
   */
  interface CollectionBase<T> {
    /**
     * Apply an action in a delegate function to each object in the collection.
     *
     * @param delegate The delegate function which iterates over the collection.
     */
    forEach(delegate: ForEach<T>): void;

    /**
     * Get the number of items in the collection.
     */
    getLength(): number;
  }

  interface ForEach<T> {
    /**
     * Iterates over the collection.
     *
     * @param item The current object.
     * @param index The index of the current object.
     */
    (item: T, index: number): any;
  }

  /**
   * A collection of a certain type.
   */
  interface Collection<T> extends CollectionBase<T> {
    /**
     * Get all the objects from the collection.
     */
    get(): T[];

    /**
     * Gets the object with the given index in the collection.
     *
     * @param index The index of the desired object.
     */
    get(index: number): T;

    /**
     * Gets the object with the given name in the collection.
     *
     * @param name The name of the desired object.
     */
    get(name: string): T;

    /**
     * Get the objects from the collection which make the delegate function return true.
     *
     * @param chooser The delegate function that filters the objects.
     */
    get(chooser: CollectionChooser<T>): T[];
  }

  interface CollectionChooser<T> {
    /**
     * Delegate function to choose which objects from the collections should be returned.
     *
     * @param item Current object
     * @parem index Index of the current object
     */
    (item: T, index: number): boolean;
  }

  /**
   * A collection of attributes.
   */
  interface AttributeCollection extends Collection<Attribute<any>> {}

  /**
   * A collection of controls.
   */

  interface ControlCollection extends Collection<AnyControl> {}

  /**
   * A collection of sections.
   */
  interface SectionCollection extends Collection<PageSection> {}

  /**
   * A collection of tabs.
   */
  interface TabCollection extends Collection<PageTab<SectionCollection>> {}

  /**
   * A collection of attributes.
   */
  interface AttributeCollectionBase extends CollectionBase<Attribute<any>> {}

  /**
   * A collection of controls.
   */
  interface ControlCollectionBase extends CollectionBase<AnyControl> {}

  /**
   * A collection of sections.
   */
  interface SectionCollectionBase extends CollectionBase<PageSection> {}

  /**
   * A collection of tabs.
   */
  interface TabCollectionBase extends CollectionBase<PageTab<SectionCollectionBase>> {}

  type AttributeType = "boolean" | "datetime" | "decimal" | "double" | "integer" | "lookup" | "memo" | "money" | "optionset" | "string" | "multiselectoptionset";

  type AttributeFormat = "date" | "datetime" | "duration" | "email" | "language" | "none" | "phone" | "text" | "textarea" | "tickersymbol" | "timezone" | "url";

  type AttributeRequiredLevel = "none" | "required" | "recommended";

  type AttributeSubmitMode = "always" | "never" | "dirty";

  /**
   * Interface for an standard entity attribute.
   */
  interface Attribute<T> {
    /**
     * Collection of controls associated with the attribute.
     */
    controls: Collection<Control<Attribute<T>>>;

    /**
     * Retrieves the data value for an attribute.
     */
    getValue(): T | null;

    /**
     * Sets the data value for an attribute.
     *
     * @param val The new value for the attribute.
     */
    setValue(val?: T | null): void;

    /**
     * Get the type of attribute.
     */
    getAttributeType(): AttributeType;

    /**
     * Get the attribute format.
     */
    getFormat(): AttributeFormat;

    /**
     * Determine whether the value of an attribute has changed since it was last saved.
     */
    getIsDirty(): boolean;

    /**
     * Get the maximum length of string which an attribute that stores string data can have.
     */
    getMaxLength(): number;

    /**
     * Get the name of the attribute.
     */
    getName(): string;

    /**
     * Get a reference to the Xrm.Page.data.entity object that is the parent to all attributes.
     */
    getParent(): PageEntity<Collection<Attribute<any>>>;

    /**
     * Returns an object with three Boolean properties corresponding to privileges indicating if the user can create,
     * read or update data values for an attribute. This function is intended for use when Field Level Security
     * modifies a user's privileges for a particular attribute.
     */
    getUserPrivilege(): UserPrivilege;

    /**
     * Sets a function to be called when the attribute value is changed.
     *
     * @param functionRef The event handler for the on change event.
     */
    addOnChange(functionRef: (context?: ExecutionContext<this, undefined>) => any): void;

    /**
     * Removes a function from the OnChange event hander for an attribute.
     *
     * @param functionRef The event handler for the on change event.
     */
    removeOnChange(functionRef: Function): void;

    /**
     * Causes the OnChange event to occur on the attribute so that any script associated to that event can execute.
     */
    fireOnChange(): void;

    /**
     * Returns a string value indicating whether a value for the attribute is required or recommended.
     */
    getRequiredLevel(): AttributeRequiredLevel;

    /**
     * Sets whether data is required, recommended or optional for the attribute before the record can be saved.
     */
    setRequiredLevel(level: AttributeRequiredLevel): void;

    /**
     * Returns a string indicating when data from the attribute will be submitted when the record is saved.
     */
    getSubmitMode(): AttributeSubmitMode;

    /**
     * Sets when data from the attribute will be submitted when the record is saved.
     */
    setSubmitMode(mode: AttributeSubmitMode): void;
  }

  /**
   * Interface for a numerical attribute.
   */
  interface NumberAttribute extends Attribute<number> {
    /**
     * Collection of controls associated with the attribute.
     */
    controls: Collection<NumberControl>;

    /**
     * Returns a number indicating the maximum allowed value for an attribute.
     */
    getMax(): number;

    /**
     * Returns a number indicating the minimum allowed value for an attribute.
     */
    getMin(): number;

    /**
     * Returns the number of digits allowed to the right of the decimal point.
     */
    getPrecision(): number;
  }

  /**
   * Interface for a lookup attribute.
   */
  interface LookupAttribute<T extends string> extends Attribute<EntityReference<T>[]> {
    /**
     * Collection of controls associated with the attribute.
     */
    controls: Collection<LookupControl<T>>;
  }

  /**
   * Interface for a date attribute.
   */
  interface DateAttribute extends Attribute<Date> {
    /**
     * Collection of controls associated with the attribute.
     */
    controls: Collection<DateControl>;
  }

  /**
   * Interface for an OptionSet attribute.
   */
  interface OptionSetAttribute<T> extends Attribute<T> {
    /**
     * Collection of controls associated with the attribute.
     */
    controls: Collection<OptionSetControl<T>>;

    /**
     * Returns a value that represents the value set for an OptionSet or Boolean attribute when the form opened.
     */
    getInitialValue(): T | null;

    /**
     * Returns a string value of the text for the currently selected option for an optionset attribute.
     */
    getText(): string;

    /**
     * Returns an option object with the value matching the argument passed to the method.
     */
    getOption(value: string): Option<T> | null;

    /**
     * Returns an option object with the value matching the argument passed to the method.
     */
    getOption(value: T): Option<T> | null;

    /**
     * Returns an array of option objects representing the valid options for an option-set attribute.
     */
    getOptions(): Option<T>[];

    /**
     * Returns the option object that is selected in an optionset attribute.
     */
    getSelectedOption(): Option<T> | null;
  }

  type ControlType = "standard" | "iframe" | "lookup" | "optionset" | "subgrid" | "webresource" | "notes" | "timercontrol" | "kbsearch" | "multiselectoptionset";

  /**
   * Interface for a standard form control.
   */
  interface BaseControl {
    /**
     * Get information about the type of control.
     */
    getControlType(): ControlType;

    /**
     * Sets the focus on the control.
     */
    setFocus(): void;

    /**
     * Get the section object that the control is in.
     */
    getParent(): PageSection;

    /**
     * Get the name of the control.
     */
    getName(): string;

    /**
     * Returns the label for the control.
     */
    getLabel(): string;

    /**
     * Sets the label for the control.
     *
     * @param label The new label for the control.
     */
    setLabel(label: string): void;

    /**
     * Returns a value that indicates whether the control is currently visible.
     */
    getVisible(): boolean;

    /**
     * Sets a value that indicates whether the control is visible.
     *
     * @param visible True if the control should be visible; otherwise, false.
     */
    setVisible(visible: boolean): void;
  }

  interface Control<T extends Xrm.Attribute<any>> extends BaseControl {
    /**
     * Get the attribute this control is bound to.
     */
    getAttribute(): T;

    /**
     * Returns whether the control is disabled.
     */
    getDisabled(): boolean;

    /**
     * Sets whether the control is disabled.
     *
     * @param disable True if the control should be disabled, otherwise false.
     */
    setDisabled(disable: boolean): void;
  }

  /**
   * Interface for an OptionSet form control.
   */
  interface OptionSetControl<T> extends Control<OptionSetAttribute<T>> {
    /**
     * Adds an option to an option set control.
     *
     * @param option An option object to add to the OptionSet.
     * @param index The index position to place the new option in. If not provided, the option will be added to the end.
     */
    addOption(option: Option<T>, index?: number): void;

    /**
     * Clears all options from an option set control.
     */
    clearOptions(): void;

    /**
     * Removes an option from an option set control.
     *
     * @param number The value of the option you want to remove.
     */
    removeOption(number: number): void;
  }

  /**
   * Interface for an external form control.
   */
  interface ExternalControl extends BaseControl {
    /**
     * Returns the object in the form that represents an IFRAME or WebResource.
     */
    getObject(): any;

    /**
     * Returns the current URL being displayed in an IFRAME or WebResource.
     */
    getSrc(): string;

    /**
     * Sets the URL to be displayed in an IFRAME or WebResource.
     *
     * @param url The URL.
     */
    setSrc(url?: string): void;
  }

  /**
   * Interface for a WebResource form control.
   */
  interface WebResourceControl extends ExternalControl {
    /**
     * Returns the value of the data query string parameter passed to a web resource.
     */
    getData(): string;

    /**
     * Sets the value of the data query string parameter passed to a web resource.
     *
     * @param dataQuery The data value to pass to the web resource.
     */
    setData(dataQuery?: string): void;
  }

  /**
   * Interface for an IFrame form control.
   */
  interface IFrameControl extends ExternalControl {
    /**
     * Returns the default URL that an IFRAME control is configured to display. This method is not available for web resources.
     */
    getInitialUrl(): string;
  }

  /**
   * Interface for a DateTime form control.
   */
  interface DateControl extends Control<Attribute<Date>> {}

  /**
   * Interface for a Lookup form control.
   */
  interface LookupControl<T extends string> extends Control<LookupAttribute<T>> {
    /**
     * Adds a new view for the lookup dialog box.
     *
     * @param viewId The string representation of a GUID for a view.
     * @param entityName The name of the entity.
     * @param viewDisplayName The name of the view.
     * @param fetchXml The fetchXml query for the view.
     * @param layoutXml The XML that defines the layout of the view.
     * @param isDefault Whether the view should be the default view.
     */
    addCustomView(viewId: string, entityName: string, viewDisplayName: string, fetchXml: string, layoutXml: string, isDefault: boolean): void;

    /**
     * Returns the ID value of the default lookup dialog view.
     */
    getDefaultView(): string;

    /**
     * Sets the default view for the lookup control dialog box.
     */
    setDefaultView(guid: string): void;
  }

  /**
   * Interface for a SubGrid form control.
   */
  interface SubGridControl<T extends string> extends BaseControl {
    /**
     * Refreshes the data displayed in a subgrid.
     */
    refresh(): void;
  }

  /**
   * Type to be be used for iterating over a list of controls and being able to interact with all of them with precursory checks for undefined
   */
  type AnyControl = BaseControl & Partial<Control<any> & WebResourceControl & IFrameControl & LookupControl<string> & SubGridControl<string> & DateControl & OptionSetControl<any>>;

  /**
   * Remarks:
   * If the subgrid control is not configured to display the view selector, calling this method on the ViewSelector returned by the GridControl.getViewSelector will throw an error.
   */
  interface ViewSelector {
    /**
     * Use this method to get a reference to the current view.
     */
    getCurrentView(): Xrm.EntityReference<string>;

    /**
     * Use this method to determine whether the view selector is visible.
     */
    isVisible(): boolean;

    /**
     * Use this method to set the current view.
     */
    setCurrentView(reference: Xrm.EntityReference<string>): void;
  }

  /**
   * Interface for a string form control.
   */
  interface StringControl extends Control<Attribute<string>> {}

  /**
   * Interface for a number form control.
   */
  interface NumberControl extends Control<NumberAttribute> {}

  /**
   * Interface for the entity on a form.
   */
  interface PageEntity<T extends AttributeCollectionBase> {
    /**
     * The collection of attributes for the entity.
     */
    attributes: T;

    /**
     * Adds a function to be called when the record is saved.
     *
     * @param functionRef Reference to a function. It will be added to the bottom of the event handler pipeline.
     *                  The execution context is automatically set to be passed as the first parameter passed to event handlers set using this method.
     */
    addOnSave(functionRef: (context?: SaveEventContext<this>) => any): void;

    /**
     * Removes a function to be called when the record is saved.
     *
     * @param functionRef Reference to a function that was added to the OnSave event.
     */
    removeOnSave(functionRef: Function): void;

    /**
     * Returns a string representing the GUID id value for the record.
     */
    getId(): string;

    /**
     * Returns a string representing the xml that will be sent to the server when the record is saved.
     */
    getDataXml(): string;

    /**
     * Returns a string representing the logical name of the entity for the record.
     */
    getEntityName(): string;

    /**
     * Returns a Boolean value that indicates if any fields in the form have been modified.
     */
    getIsDirty(): boolean;
  }

  interface ExecutionContext<TSource, TArgs> {
    /**
     * Method that returns the Client-side context object
     */
    getContext(): context;

    /**
     * Method that returns a value that indicates the order in which this handler is executed.
     */
    getDepth(): number;

    /**
     * Method that returns an object with methods to manage the Save event.
     */
    getEventArgs(): TArgs;

    /**
     * Method that returns a reference to the object that the event occurred on.
     */
    getEventSource(): TSource;

    /**
     * Sets the value of a variable to be used by a handler after the current handler completes.
     *
     * @param key Key for the value
     * @param value The value to be stored
     */
    setSharedVariable(key: string, value: any): void;

    /**
     * Retrieves a variable set using setSharedVariable.
     *
     * @param key Key for the desired value
     */
    getSharedVariable(key: string): any;
  }

  interface SaveEventContext<T> extends ExecutionContext<T, SaveEventArgs> { }

  interface SaveEventArgs {
    /**
     * Returns a value indicating how the save event was initiated by the user.
     */
    getSaveMode(): SaveMode;

    /**
     * Returns a value indicating whether the save event has been canceled because the preventDefault method was used in this event hander or a previous event handler.
     */
    isDefaultPrevented(): boolean;

    /**
     * Cancels the save operation, but all remaining handlers for the event will still be executed.
     */
    preventDefault(): void;
  }

  /**
   * Supported values returned to detect different ways entity records may be saved by the user.
   */
  const enum SaveMode {
    Save = 1,
    SaveAndClose = 2,
    SaveAndNew = 59,
    AutoSave = 70,
    SaveAsCompleted = 58,
    Deactivate = 5,
    Reactivate = 6,
    Assign = 47,
    Send = 7,
    Qualify = 16,
    Disqualify = 15,
  }

  /**
   * Interface for the data of a form.
   */
  interface DataModule<T extends AttributeCollectionBase> {
    /**
     * Contains information about the entity of the page.
     */
    entity: PageEntity<T>;
  }

  interface ProcessContainer {
    [id: string]: string;
  }

  interface Process {
    /**
     * Returns the unique identifier of the process.
     */
    getId(): string;

    /**
     * Returns the name of the process.
     */
    getName(): string;

    /**
     * Returns an collection of stages in the process.
     */
    getStages(): Collection<Stage>;

    /**
     * Returns true if the process is rendered, false if not.
     */
    isRendered(): boolean;
  }

  type StageStatus = "active" | "inactive";

  interface Stage {
    /**
     * Returns an object with a getValue method which will return the integer value of the business process flow category.
     */
    getCategory(): IStageCategory;

    /**
     * Returns the logical name of the entity associated with the stage.
     */
    getEntityName(): string;

    /**
     * Returns the unique identifier of the stage.
     */
    getId(): string;

    /**
     * Returns the name of the stage.
     */
    getName(): string;

    /**
     * Returns the status of the stage.
     */
    getStatus(): StageStatus;

    /**
     * Returns a collection of steps in the stage.
     */
    getSteps(): StageStep[];
  }

  const enum StageCategory {
    Qualify = 0,
    Develop = 1,
    Propose = 2,
    Close = 3,
    Identify = 4,
    Research = 5,
    Resolve = 6,
  }

  interface IStageCategory {
    /**
     * Returns the stage category.
     */
    getValue(): StageCategory;
  }

  interface StageStep {
    /**
     * Returns the logical name of the attribute associated to the step.
     */
    getAttribute(): string;

    /**
     * Returns the name of the step.
     */
    getName(): string;

    /**
     * Returns whether the step is required in the business process flow.
     */
    isRequired(): boolean;
  }

  interface SaveOptions {
    /**
     * Indicates whether to use the "Book" or "Reschedule" messages, rather than the "Create" or "Update" messages.
     */
    UseSchedulingEngine: boolean;
  }

  interface Then<T> {
    /**
     * A function which can add callback handlers after it has finished.
     *
     * @param successCallback A function to call when the operation succeeds.
     * @param errorCallback A function to call when the operation fails.
     */
    then(successCallback?: (result: T) => void, errorCallback?: ErrorCallback): void;
  }

  interface ErrorCallback {
    /**
     * A function to call when the operation fails.
     *
     * @param messageObject Object containing information about the error.
     */
    (messageObject: ErrorCallbackObject): void;

    /**
     * A function to call when the operation fails.
     *
     * @param errorCode The error code.
     * @param message A localized error message.
     */
    (errorCode: number, message: string): void;
  }

  interface ErrorCallbackObject {
    errorCode: number;
    message: string;
  }

  /**
   * Interface for a section on a form.
   */
  interface PageSection {
    /**
     * A collection of controls in the section.
     */
    controls: Collection<AnyControl>;

    /**
     * Method to return the name of the section.
     */
    getName(): string;

    /**
     * Method to return the tab containing the section.
     */
    getParent(): PageTab<Collection<PageSection>>;

    /**
     * Returns the label for the section.
     */
    getLabel(): string;

    /**
     * Sets the label for the section.
     *
     * @param label The label text to set.
     */
    setLabel(label: string): void;

    /**
     * Sets a value to show or hide the section.
     */
    setVisible(visibility: boolean): void;

    /**
     * Returns true if the section is visible, otherwise returns false.
     */
    getVisible(): boolean;
  }

  type CollapsableDisplayState = "expanded" | "collapsed";

  /**
   * Interface for a tab on a form.
   */
  interface PageTab<T extends SectionCollectionBase> {
    /**
     * Collection of sections within this tab.
     */
    sections: T;

    /**
     * Method to get the name of the tab.
     */
    getName(): string;

    /**
     * Returns a value that indicates whether the tab is collapsed or expanded.
     */
    getDisplayState(): CollapsableDisplayState;

    /**
     * Sets the tab to be collapsed or expanded.
     */
    setDisplayState(state: CollapsableDisplayState): void;

    /**
     * Returns the Xrm.Page.ui object.
     */
    getParent(): UiModule<Collection<PageTab<Collection<PageSection>>>, Collection<BaseControl>>;

    /**
     * Returns the tab label.
     */
    getLabel(): string;

    /**
     * Sets the label for the tab.
     *
     * @param label The new label for the tab.
     */
    setLabel(label: string): void;

    /**
     * Sets the focus on the tab.
     */
    setFocus(): void;

    /**
     * Sets a value that indicates whether the control is visible.
     */
    setVisible(visibility: boolean): void;

    /**
     * Returns a value that indicates whether the tab is visible.
     */
    getVisible(): boolean;
  }

  type NotificationLevel = "INFO" | "WARNING" | "ERROR";

  /**
   * Interface for the ui of a form.
   */
  interface UiModule<T extends TabCollectionBase, U extends ControlCollectionBase> {
    /**
     * Collection of tabs on the page.
     */
    tabs: T;

    /**
     * Collection of controls on the page.
     */
    controls: U;

    /**
     * Navigation for the page.
     */
    navigation: navigation;

    /**
     * Method to get the form context for the record.
     * Matches the values found in the Xrm.FormType enum.
     */
    getFormType(): FormType;

    /**
     * Method to close the form.
     */
    close(): void;

    /**
     * Use the formSelector.getCurrentItem method to retrieve information about the form currently in use and the formSelector.items
     * collection containing information about all the forms available for the user.
     */
    formSelector: FormSelector;

    /**
     * Method to get the control object that currently has focus on the form. Web Resource and IFRAME controls are not returned by this method.
     * This method was deprecated in Microsoft Dynamics CRM 2013 Update Rollup 2.
     */
    getCurrentControl(): AnyControl;

    /**
     * Method to cause the ribbon to re-evaluate data that controls what is displayed in it.
     */
    refreshRibbon(): void;

    /**
     * Method to get the height of the viewport in pixels.
     */
    getViewPortHeight(): number;

    /**
     * Method to get the width of the viewport in pixels.
     */
    getViewPortWidth(): number;
  }

  interface FormSelector {
    /**
     * Method to return a reference to the form currently being shown.
     */
    getCurrentItem(): FormItem;

    /**
     * Method to return a reference to the form currently being shown.
     */
    items: Collection<FormItem>;
  }

  interface FormItem {
    /**
     * Returns the GUID ID of the form.
     */
    getId(): string;

    /**
     * Returns the label for the form.
     */
    getLabel(): string;

    /**
     * Opens the specified form.
     */
    navigate(): void;
  }

  interface navigation {
    /**
     * Navigation items for the page.
     */
    items: Collection<NavigationItem>;
  }

  interface NavigationItem {
    /**
     * Returns the name of the item.
     */
    getId(): string;

    /**
     * Returns the label for the item.
     */
    getLabel(): string;

    /**
     * Sets the label for the item.
     */
    setLabel(label: string): void;

    /**
     * Sets the focus on the item.
     */
    setFocus(): string;

    /**
     * Returns a value that indicates whether the item is currently visible.
     */
    getVisible(): boolean;

    /**
     * Sets a value that indicates whether the item is visible.
     */
    setVisible(visible: boolean): void;
  }

  /**
   * Interface for the context of a form.
   */
  interface context {
    /**
     * Returns the base URL that was used to access the application.
     */
    getClientUrl(): string;

    /**
     * Returns a string representing the current Microsoft Office Outlook theme chosen by the user.
     */
    getCurrentTheme(): string;

    /**
     * Prepends the organization name to the specified path.
     */
    prependOrgName(sPath: string): string;
  }

  /**
   * Interface for the base of an Xrm.Page
   */
  interface PageBase<T extends AttributeCollectionBase, U extends TabCollectionBase, V extends ControlCollectionBase> {
    /**
     * Data on the page.
     */
    data: Xrm.DataModule<T>;

    /**
     * UI of the page.
     */
    ui: Xrm.UiModule<U, V>;

    /**
     * Returns string with current page URL.
     */
    getUrl(): string;
  }

  /**
   * Interface for a generic Xrm.Page
   */
  interface BasicPage extends PageBase<AttributeCollection, TabCollection, ControlCollection> {
    /**
     * Generic getAttribute
     */
    getAttribute(attrName: string): Xrm.Attribute<any> | undefined;

    /**
     * Generic getControl
     */
    getControl(ctrlName: string): Xrm.AnyControl | undefined;
  }
}

type BaseXrm = typeof Xrm;
/**
 * Client-side xRM object model.
 */
interface Xrm<T extends Xrm.PageBase<Xrm.AttributeCollectionBase, Xrm.TabCollectionBase, Xrm.ControlCollectionBase>> extends BaseXrm {
  /**
   * Various utility functions can be found here.
   */
  Utility: Xrm.Utility;
}

declare namespace Xrm {
  var Utility: Utility;

  /**
   * Interface for a Lookup which is used by some Xrm.Utility functions.
   */
  interface Lookup {
    /**
     * Entity type (logical name) of the lookup.
     */
    entityType: string;

    /**
     * GUID of the lookup.
     */
    id: string;

    /**
     * Record name of the lookup.
     */
    name?: string;
  }

  /**
   * Interface for a WindowOption object.
   */
  interface WindowOptions {
    /**
     * Specifies if it should open in a new window.
     */
    openInNewWindow: boolean;
  }
}
declare namespace Xrm {
  interface BaseControl {
    /**
     * Display a message near the control to indicate that data isn?t valid. When this method is used on Microsoft Dynamics CRM for tablets a red "X" icon appears next to the control. Tapping on the icon will display the message.
     *
     * @param message The message to display.
     * @param uniqueId The ID to use to clear just this message when using clearNotification.
     */
    setNotification(message: string, uniqueId?: string): boolean;

    /**
     * Remove a message already displayed for a control.
     *
     * @param uniqueId The ID to use to clear a specific message set using setNotification.
     */
    clearNotification(uniqueId?: string | null): boolean;
  }

  interface LookupControl<T extends string> extends Control<LookupAttribute<T>> {
    /**
     * Use to add filters to the results displayed in the lookup. Each filter will be combined with any previously added filters as an 'AND' condition.
     *
     * @param fetchXml The fetchXml filter element to apply.
     * @param entityType If this is set, the filter only applies to that entity type. Otherwise, it applies to all types of entities returned.
     */
    addCustomFilter(fetchXml: string, entityType?: string): void;

    /**
     * Use this method to apply changes to lookups based on values current just as the user is about to view results for the lookup.
     */
    addPreSearch(handler: Function): void;

    /**
     * Use this method to remove event handler functions that have previously been set for the PreSearch event.
     */
    removePreSearch(handler: Function): void;
  }

  interface DateControl extends Control<Attribute<Date>> {
    /**
     * Specify whether a date control should show the time portion of the date.
     */
    setShowTime(doShow: boolean): void;
  }

  interface PageEntity<T extends AttributeCollectionBase> {
    /**
     * Gets a string for the value of the primary attribute of the entity.
     */
    getPrimaryAttributeValue(): string;
  }

  interface BaseAttribute<T> {
    /**
     * Determine whether a lookup attribute represents a partylist lookup.
     */
    getIsPartyList(): boolean;
  }

  interface UiModule<T extends TabCollectionBase, U extends ControlCollectionBase> {
    /**
     * Use this method to remove form level notifications.
     *
     * @param uniqueId Id of the notification to remove.
     */
    clearFormNotification(uniqueId: string): boolean;

    /**
     * Use this method to display form level notifications. You can display any number of notifications and they will be displayed until
     * they are removed using clearFormNotification. The height of the notification area is limited so each new message will be added to the top.
     * Users can scroll down to view older messages that have not yet been removed.
     *
     * @param message The text of the message.
     * @param level The level of the message.
     * @param uniqueId A unique identifier for the message used with clearFormNotification to remove the notification.
     */
    setFormNotification(message: string, level: NotificationLevel, uniqueId: string): boolean;
  }

  type ClientType = "Web" | "Outlook" | "Mobile";
  type ClientState = "Online" | "Offline";

  interface client {
    /**
     * Returns a value to indicate which client the script is executing in.
     */
    getClient(): ClientType;

    /**
     * Use this instead of the removed isOutlookOnline method.
     */
    getClientState(): ClientState;
  }

  interface context {
    /**
     * Provides access to the getClient and getClientState methods you can use to determine which client is being used and whether the client is connected to the server.
     */
    client: client;
  }
}
declare namespace Xrm {
  /**
   * Interface for the ui of a form.
   */
  interface UiModule<T extends TabCollectionBase, U extends ControlCollectionBase> {
    /**
     * Access UI controls for the business process flow on the form.
     */
    process: UiProcessModule;
  }

  interface UiProcessModule {
    /**
     * Use this method to retrieve the display state for the business process control.
     */
    getDisplayState(): CollapsableDisplayState;

    /**
     * Use this method to expand or collapse the business process flow control.
     */
    setDisplayState(val: CollapsableDisplayState): void;

    /**
     * Use getVisible to retrieve whether the business process control is visible.
     */
    getVisible(): boolean;

    /**
     * Use setVisible to show or hide the business process control.
     */
    setVisible(visible: boolean): void;
  }
}
declare namespace Xrm {
  interface SubGridControl<T extends string> extends BaseControl {
    /**
     * Add event handlers to this event to run every time the subgrid refreshes.
     * This includes when users sort the values by clicking the column headings.
     */
    addOnLoad(functionRef: (context?: ExecutionContext<this, any>) => any): void;

    /**
     * Use this method to get the logical name of the entity data displayed in the grid.
     */
    getEntityName(): string;

    /**
     * Use this method to get access to the Grid available in the GridControl.
     */
    getGrid(): Grid<T>;

    /**
     * Use this method to get access to the ViewSelector available for the GridControl.
     */
    getViewSelector(): ViewSelector;

    /**
     * Use this method to remove event handlers from the GridControl.OnLoad event.
     */
    removeOnLoad(reference: Function): void;

    /**
     * Use this method to get the logical name of the relationship used for the data displayed in the grid.
     */
    getRelationshipName(): string;
  }

  interface Grid<T extends string> {
    /**
     * Returns a collection of every GridRow in the Grid.
     */
    getRows(): Collection<GridRow<T>>;

    /**
     * Returns a collection of every selected GridRow in the Grid.
     */
    getSelectedRows(): Collection<GridRow<T>>;

    /**
     * In the web application or the Dynamics CRM for Outlook client while connected to the server, this method returns the total number of records that match the filter criteria of the view, not limited by the number visible in a single page.
     * When the Dynamics CRM for Outlook client isn’t connected to the server, this number is limited to those records that the user has selected to take offline.
     * For Microsoft Dynamics CRM for tablets and Microsoft Dynamics CRM for phones this method will return the number of records in the subgrid.
     */
    getTotalRecordCount(): number;
  }

  interface GridRow<T extends string> {
    /**
     * Returns the GridRowData for the GridRow.
     */
    getData(): GridRowData<T>;
  }

  interface GridRowData<T extends string> {
    /**
     * Returns the GridEntity for the GridRowData.
     */
    getEntity(): GridEntity<T>;
  }

  interface GridEntity<T extends string> {
    /**
     * Returns the logical name for the record in the row.
     */
    getEntityName(): string;

    /**
     * Returns an entity reference for the record in the row.
     */
    getEntityReference(): Xrm.EntityReference<T>;

    /**
     * Returns the id for the record in the row.
     */
    getId(): string;

    /**
     * Returns the primary attribute value for the record in the row.
     */
    getPrimaryAttributeValue(): string;
  }

  /**
   * Interface for a DateTime form control.
   */
  interface DateControl extends Control<Attribute<Date>> {
    /**
     * Get whether a date control shows the time portion of the date.
     */
    getShowTime(): boolean;
  }
}
declare namespace Xrm {
  interface PageTab<T extends SectionCollectionBase> {
    /**
     * Add an event handler on tab state change.
     *
     * @param reference Event handler for tab state change.
     */
    addTabStateChange(reference: Function): void;
  }

  interface GridEntity<T extends string> {
    /**
     * Returns a collection of the attributes on this record.
     */
    getAttributes(): GridCollection<GridEntityAttribute<T>>;
  }

  interface GridCollection<T> {
    /**
     * Returns a list of all attributes on this record.
     */
    getAll(): T[];
    /**
     * Returns a list of all attributes on this record which matches the filter.
     */
    getByFilter(filter: (a: T) => boolean): T[];
    /**
     * Returns the first attribute on this record which matches the filter.
     */
    getFirst(filter: (a: T) => boolean): T | null;
    /**
     * Returns the attribute on this record with the given name, if any.
     */
    getByName(name: string): T | null;
    /**
     * Returns the attribute on this record with the given index, if any.
     */
    getByIndex(idx: number): T | undefined;
    /**
     * Returns the amount of attributes in this entity grid.
     */
    getLength(): number;
    /**
     * Iterator function for the attributes.
     */
    forEach(delegate: ForEach<T>): void;
  }

  interface GridEntityAttribute<T extends string> {
    /**
     * Returns the logical name of the attribute.
     */
    getKey(): string;
    /**
     * Returns the logical name of the attribute.
     */
    getName(): string;
    /**
     * Returns the value of the attribute on this record.
     */
    getValue(): string | null;
    /**
     * Returns the parent entity of this attribute.
     */
    getParent(): GridEntity<T>;
  }

  /**
   * Interface for the data of a form.
   */
  interface DataModule<T extends AttributeCollectionBase> {
    /**
     * Access various functionality for a business process flow.
     */
    process: ProcessModule;
  }

  type ProcessStageMoveAnswer = "success" | "crossEntity" | "end" | "invalid" | "dirtyForm";
  type ProcessStageSetAnswer = "crossEntity" | "unreachable" | "dirtyForm" | "invalid";
  type ProcessStageChangeDirection = "Next" | "Previous";

  interface StageSelectedEventArguments {
    getStage(): Stage;
  }

  interface StageChangeEventArguments extends StageSelectedEventArguments {
    getDirection(): ProcessStageChangeDirection;
  }

  interface StageSelectedContext extends ExecutionContext<Stage, StageSelectedEventArguments> { }

  interface StageChangeContext extends ExecutionContext<Stage, StageChangeEventArguments> { }

  /**
   * Interface for the business process flow on a form.
   */
  interface ProcessModule {
    /**
     * Returns a Process object representing the active process.
     */
    getActiveProcess(): Process;

    /**
     * Returns a Stage object representing the active stage.
     */
    getActiveStage(): Stage;

    /**
     * Set a completed stage as the active stage.
     * This method can only be used when the selected stage and the active stage are the same.
     *
     * @param stageId The ID of the completed stage for the entity to make the active stage.
     * @param callback The callback function will be passed a string value of “success” if the operation completes successfully.
     *    If the stageId represents a stage that isn't valid, the stage won't be made active and the callback function will be passed a string value indicating the reason.
     *    "crossEntity": The stage must be one for the current entity.
     *    "unreachable": The stage exists on a different path.
     *    "dirtyForm": This value will be returned if the data in the page is not saved.
     *    "invalid":
     *      - The stageId parameter is a non-existent stage ID value
     *                    OR
     *      - The active stage isn’t the selected stage.
     *                    OR
     *      - The record hasn’t been saved yet.
     */
    setActiveStage(stageId: string, callback?: (stringVal: ProcessStageSetAnswer) => any): void;

    /**
     * Use this method to get a collection of stages currently in the active path with methods to interact with the stages displayed in the business process flow control.
     * The active path represents stages currently rendered in the process control based on the branching rules and current data in the record.
     */
    getActivePath(): Collection<Stage>;

    /**
     * Use this method to asynchronously retrieve the enabled business process flows that the user can switch to for an entity.
     *
     * @param callback The callback function must accept a parameter that contains an object with dictionary properties where the name of the property is the Id of the
     *      business process flow and the value of the property is the name of the business process flow.
     *      The enabled processes are filtered according to the user’s privileges. The list of enabled processes is the same ones a user can see in the UI
     *      if they want to change the process manually.
     */
    getEnabledProcesses(callback: (enabledProcesses: ProcessContainer) => any): void;

    /**
     * Use this method to get the currently selected stage.
     */
    getSelectedStage(): Stage;

    /**
     * Use this to add a function as an event handler for the OnStageChange event so that it will be called when the business process flow stage changes.
     * You should use a reference to a named function rather than an anonymous function if you may later want to remove the event handler.
     *
     * @param handler The function will be added to the bottom of the event handler pipeline.
     */
    addOnStageChange(handler: (context?: StageChangeContext) => any): void;

    /**
     * Use this to remove a function as an event handler for the OnStageChange event.
     *
     * @param handler If an anonymous function is set using the addOnStageChange method it cannot be removed using this method.
     */
    removeOnStageChange(handler: (context?: StageChangeContext) => any): void;

    /**
     * Use this to add a function as an event handler for the OnStageSelected event so that it will be called when a business process flow stage is selected.
     * You should use a reference to a named function rather than an anonymous function if you may later want to remove the event handler.
     *
     * @param handler The function will be added to the bottom of the event handler pipeline.
     */
    addOnStageSelected(handler: (context?: StageSelectedContext) => any): void;

    /**
     * Use this to remove a function as an event handler for the OnStageSelected event.
     *
     * @param handler If an anonymous function is set using the addOnStageSelected method it cannot be removed using this method.
     */
    removeOnStageSelected(handler: (context?: StageSelectedContext) => any): void;

    /**
     * Progresses to the next stage.
     * Will cause the OnStageChange event to occur.
     * This method can only be used when the selected stage and the active stage are the same.
     *
     * @param callback An optional function to call when the operation is complete. This callback function is passed one of the following string values to indicate whether the operation succeeded:
     *      "success": The operation succeeded.
     *      "crossEntity": The next stage is for a different entity.
     *      "end": The active stage is the last stage of the active path.
     *      "invalid": The operation failed because the selected stage isn’t the same as the active stage.
     *      "dirtyForm": This value will be returned if the data in the page is not saved.
     */
    moveNext(callback?: (stringVal: ProcessStageMoveAnswer) => any): void;

    /**
     * Moves to the previous stage.
     * Will cause the OnStageChange event to occur.
     * This method can only be used when the selected stage and the active stage are the same.
     *
     * @param callback An optional function to call when the operation is complete. This callback function is passed one of the following string values to indicate whether the operation succeeded:
     *      "success": The operation succeeded.
     *      "crossEntity": The previous stage is for a different entity.
     *      "end": The active stage is the last stage of the active path.
     *      "invalid": The operation failed because the selected stage isn’t the same as the active stage.
     *      "dirtyForm": This value will be returned if the data in the page is not saved.
     */
    movePrevious(callback?: (stringVal: ProcessStageMoveAnswer) => any): void;
  }

  /**
   * Interface for an OptionSet form control.
   */
  interface OptionSetControl<T> extends Control<OptionSetAttribute<T>> {
    /**
     * Returns an array of option objects representing the valid options for an option-set control.
     */
    getOptions(): Option<T>[];
  }
}
//Function helper type for a function that can be set to be called by a view column to show an image with a tooltip instead of the ordinary data
type TooltipFunc = (rowData: string, lcid: LCID) => [WebResourceImage, string];

declare namespace Xrm {
  const enum ProcessStatus {
    Active = "active",
    Aborted = "aborted",
    Finished = "finished",
  }

  /**
   * Interface for a single result in the auto-completion list.
   */
  interface AutoCompleteResult {
    /**
     * Unique id for the result
     */
    id: string;
    /**
     * Result icon defined by an image url
     */
    icon: string;
    /**
     * Values to display in the result. Support up to three values
     */
    fields: string[];
  }

  /**
   * Interface for a command button in lower right corner of the auto-completion drop-down list.
   */
  interface AutoCompleteCommand {
    /**
     * Unique id for the command
     */
    id: string;
    /**
     * Command icon defined by an image url
     */
    icon: string;
    /**
     * Label for the command
     */
    label: string;
    /**
     * Command action
     */
    action: () => any;
  }

  /**
   * Interface for the result set to be shown in auto-completion list.
   */
  interface AutoCompleteResultSet {
    /**
     * List of returned results
     */
    results: AutoCompleteResult[];
    /**
     * Command at the bottom of the auto-completion drop-down list (optional)
     */
    commands: AutoCompleteCommand;
  }

  interface StringControl extends Control<Attribute<string>> {
    /**
     * Gets the latest value in a control as the user types characters in a specific text or number field.
     * The getValue method is different from the attribute getValue method because the control method retrieves the value from the control as the user is typing in the control as opposed to the attribute getValue method that retrieves the value after the user commits (saves) the field.
     */
    getValue(): string;

    /**
     * Use this to manually fire an event handler that you created for a specific text field to be executed on the keypress event.
     */
    fireOnKeyPress(): void;
  }

  interface GridEntity<T extends string> {
    /**
     * Returns the GUID of the record.
     */
    getKey(): string;
    getIsDirty(): boolean;
    getDataXml(): string | null;

    /**
     * Returns a collection of the related entities for this record.
     * TODO: Unsure as to what type of elements are returned in the collection.
     */
    getRelatedEntities(): GridCollection<any>;

    isInHierarchy(): boolean;
  }

  interface ProcessStatusChangeContext extends ExecutionContext<Process, any> { }

  interface ProcessModule {
    /**
     * Use this to add a function as an event handler for the OnProcessStatusChange event so that it will be called when the
     * business process flow status changes.
     * @param handler The function will be added to the bottom of the event
     *                handler pipeline. The execution context is automatically
     *                set to be the first parameter passed to the event handler.
     *                Use a reference to a named function rather than an
     *                anonymous function if you may later want to remove the
     *                event handler.
     */
    addOnProcessStatusChange(handler: (context?: ProcessStatusChangeContext) => any): void;

    /**
     * Use this to remove a function as an event handler for the OnProcessStatusChange event.
     * @param handler If an anonymous function is set using the addOnProcessStatusChange method it
     *                cannot be removed using this method.
     */
    removeOnProcessStatusChange(handler: (context?: ProcessStatusChangeContext) => any): void;

    /**
     * Returns the unique identifier of the process instance.
     */
    getInstanceId(): string;

    /**
     * Returns the name of the process instance.
     */
    getInstanceName(): string;

    /**
     * Returns the current status of the process instance.
     */
    getStatus(): ProcessStatus;

    /**
     * Sets the current status of the active process instance.
     * @param status The new status. The values can be active, aborted, or finished.
     * @param callbackFunction A function to call when the operation is complete. This callback function is passed the new status as a string value.
     */
    setStatus(status: ProcessStatus, callbackFunction?: (status: ProcessStatus) => any): ProcessStatus;
  }
}
declare namespace Xrm {
    var Device: Device;
    var Encoding: Encoding;
    var Navigation: Navigation;
    //var UI: UI;
    var WebApi: WebApi;

    interface ImageOptions {
        /**
         * Indicates whether to edit the image before saving
         */
        allowEdit: boolean;

        /**
         * Height of the image to capture
         */
        height: number;

        /**
         * Indicates whether to capture image using the front camera of the device
         */
        preferFrontCamera: boolean;

        /**
         * Quality of the image file in percentage
         */
        quality: number;

        /**
         * Width of the image to capture
         */
        width: number;
    }

    type PickFileFileType = "audio" | "video" | "image";

    interface PickFileOptions {
        /**
         *  Image file types to select.
         */
        accept: PickFileFileType;

        /**
         * Indicates whether to allow selecting multiple files.
         */
        allowMultipleFiles: boolean;

        /**
         * Maximum size of the files(s) to be selected.
         */
        maximumAllowedFileSize: number;
    }

    /**
     * Interface for a File object
     */
    interface File {
        /**
         * Contents of the file. Base64 Encoded
         */
        fileContent: string;

        /**
         * Name of the file.
         */
        fileName: string;

        /**
         * Size of the file in KB.
         */
        fileSize: string;

        /**
         * MIME type of the file.
         */
        mimeType: string;
    }

    /**
     * Interface for geo location object acquired through Xrm.Device.getCurrentPosition
     */
    interface GeoObject {
        coords: any;
        timestamp: number;
    }

    /**
     * Contains methods to use the device capabilities of mobile devices.
     */
    interface Device {
        /**
         * Invokes the device microphone to record audio.
         */
        captureAudio(): Then<File>;

        /**
         * Invokes the device camera to capture an image.
         * @param imageOptions Options for capturing the image.
         */
        captureImage(imageOptions: ImageOptions): Then<File>;

        /**
         * Invokes the device camera to record video.
         */
        captureVideo(): Then<File>;

        /**
         * Invokes the device camera to scan the barcode information, such as a product number.
         */
        getBarcodeValue(): Then<string>;

        /**
         * Returns the current location using the device geolocation capability.
         */
        getCurrentPosition(): Then<GeoObject>;

        /**
         * Opens a dialog box to select files from your computer (web client) or mobile device (mobile clients).
         * @param pickFileOptions Options for picking file(s)
         */
        pickFile(pickFileOptions: PickFileOptions): Then<File[]>;
    }

    /**
     * Contains methods related to applying attribute and XML encoding to strings.
     */
    interface Encoding {
        /**
         * Encodes the specified string so that it can be used in an HTML attribute.
         * @param arg String to be encoded.
         */
        htmlAttributeEncode(arg: string): string;

        /**
         * Converts a string that has been HTML-encoded into a decoded string.
         * @param arg HTML-encoded string to be decoded.
         */
        htmlDecode(arg: string): string;

        /**
         * Converts a string to an HTML-encoded string.
         * @param arg String to be encoded.
         */
        htmlEncode(arg: string): string;
        /**
         * Applies attribute encoding to a string.
         * @param arg String to be encoded.
         */
        xmlAttributeEncode(arg: string): string;

        /**
         * Applies XML encoding to a string.
         * @param arg String to be encoded.
         */
        xmlEncode(arg: string): string;
    }

    /**
     * Interface for an AlertStrings object
     */
    interface AlertStrings {
        /**
         * The confirm button label. If you do not specify the button label, OK is used as the button label.
         */
        confirmButtonLabel?: string;

        /**
         * The message to be displayed in the alert dialog.
         */
        text: string;

        /**
         * The title of the alert dialog.
         */
        title?: string;
    }

    /**
     * Interface for a ConfirmationStrings object
     */
    interface ConfirmStrings {
        /**
         * The cancel button label.If you do not specify the cancel button label, Cancel is used as the button label.
         */
        cancelButtonLabel?: string;

        /**
         * The confirm button label.If you do not specify the confirm button label, OK is used as the button label.
         */
        confirmButtonLabel?: string;

        /**
         * The subtitle to be displayed in the confirmation dialog.
         */
        subtitle?: string;

        /**
         * The message to be displyed in the confirmation dialog.
         */
        text: string;

        /**
         * The title to be displyed in the confirmation dialog.
         */
        title?: string;
    }

    /**
     * Interface for a SizeOptions object
     */
    interface SizeOptions {
        /**
         * Height of the alert dialog in pixels
         */
        height?: number;

        /**
         * Width of the alert dialog in pixels
         */
        width?: number;
    }

    /**
     * Interface for an ErrorOptions object
     */
    interface ErrorOptions {
        /**
         * Details about the error.
         * When you specify this, the Download Log File button is available in the error message,
         * and clicking it will let users download a text file with the content specified in this attribute.
         */
        details?: string;

        /**
         * The error code.
         * If you just set errorCode, the message for the error code is automatically retrieved from the server and displayed in the error dialog.
         * If you specify an invalid errorCode value, an error dialog with a default error message is displyed.
         */
        errorCode?: number;

        /**
         * The message to be displayed in the error dialog.
         */
        message?: string;
    }

    const enum OpenFileOptions {
        Open = 1,
        Save = 2,
    }

    type EntityFormNavBar = "on" | "off" | "entity";

    const enum EntityFormWindowPosition {
        Center = 1,
        Side = 2,
    }

    const enum EntityFormRelationshipType {
        OneToMany = 1,
        ManyToMany = 2,
    }

    const enum EntityFormRelationshipRoleType {
        Referencing = 1,
        AssociationEntity = 2,
    }

    interface EntityFormRelationship {
        /**
         * Name of the attribute used for relationship.
         */
        attributeName: string;

        /**
         * Name of the relationship.
         */
        name: string;

        /**
         * Name for the navigation property for this relationship.
         */
        navigationPropertyName: string;

        /**
         * Relationship type.
         */
        relationshipType: EntityFormRelationshipType;

        /**
         * Role type in relationship.
         */
        roleType: EntityFormRelationshipRoleType;
    }

    /**
     * Interface for an EntityFormOptions object
     */
    interface EntityFormOptions {
        /**
         * Indicates whether to display the command bar. If you do not specify this parameter, the command bar is displayed by default.
         */
        cmdbar?: boolean;

        /**
         * Designates a record that will provide default values based on mapped attribute values.
         */
        createFromEntity?: Lookup;

        /**
         * ID of the entity record to display the form for.
         */
        entityId?: string;

        /**
         * Logical name of the entity to display the form for.
         */
        entityName?: string;

        /**
         * ID of the form instance to be displayed.
         */
        formId?: string;

        /**
         * Height of the form window to be displayed in pixels.
         */
        height?: number;

        /**
         *
         */
        isCrossEntityNavigate?: boolean;

        /**
         *
         */
        isOfflineSyncError?: boolean;

        /**
         * Controls whether the navigation bar is displayed and whether application navigation is available using the areas and subareas defined in the sitemap.
         * on: The navigation bar is displayed. This is the default behavior if the navBar parameter is not used.
         * off: The navigation bar is not displayed. People can navigate using other user interface elements or the back and forward buttons.
         * entity: On an entity form, only the navigation options for related entities are available. After navigating to a related entity, a back button is displayed in the navigation bar to allow returning to the original record.
         */
        navBar?: EntityFormNavBar;

        /**
         * Indicates whether to display form in a new window.
         */
        openInNewWindow?: boolean;

        /**
         * Window Position
         */
        windowPosition?: EntityFormWindowPosition;

        /**
         * ID of the business process to be displayed on the form.
         */
        processId?: string;

        /**
         * ID of the business process instance to be displayed on the form.
         */
        processInstanceId?: string;

        /**
         * A relationship object to display the related records on the form.
         */
        relationship?: EntityFormRelationship;

        /**
         * ID of the selected stage in business process instance.
         */
        selectedStageId?: string;

        /**
         * Indicates whether to open a quick create form.
         */
        useQuickCreateForm?: boolean;

        /**
         * Width of the form window to be displayed in pixels.
         */
        width?: number;
    }

    interface WindowOptions {
        /**
         * Height of the window to open in pixels.
         */
        height?: number;

        /**
         * Width of the window to open in pixels.
         */
        width?: number;
    }

    interface ConfirmDialogResult {
        confirmed: boolean;
    }

    interface OpenFormResult {
        /**
         * Identifies the record displayed or created
         */
        savedEntityReference: Lookup[];
    }

    type ViewType = "savedquery" | "userquery";

    interface EntityList {
        /**
         * The type of the page.
         */
        pageType: "entitylist";

        /**
         *  The logical name of the entity to load in the list control.
         */
        entityName: string;

        /**
         * The ID of the view to load. If you don't specify it, navigates to the default main view for the entity.
         */
        viewId?: string;

        /**
         * Type of view to load. Specify "savedquery" or "userquery".
         */
        viewType?: ViewType;
    }

    interface EntityRecord {
        /**
         * The type of the page.
         */
        pageType: "entityrecord";

        /**
         *  Logical name of the entity to display the form for.
         */
        entityName: string;

        /**
         * ID of the entity record to display the form for. If you don't specify this value, the form will be opened in create mode.
         */
        entityId?: string;

        /**
         * Designates a record that will provide default values based on mapped attribute values.
         */
        createFromEntity?: Lookup;

        /**
         * A dictionary object that passes extra parameters to the form.
         */
        data?: object;

        /**
         * ID of the form instance to be displayed.
         */
        formId?: string;

        /**
         * Indicates whether the form is navigated to from a different entity using cross-entity business process flow.
         */
        isCrossEntityNavigate?: boolean;

        /**
         * Indicates whether there are any offline sync errors.
         */
        isOfflineSyncError?: boolean;

        /**
         * ID of the business process to be displayed on the form.
         */
        processId?: string;

        /**
         * ID of the business process instance to be displayed on the form.
         */
        processInstanceId?: string;

        /**
         * Define a relationship object to display the related records on the form
         */
        relationship?: EntityFormRelationship;

        /**
         * ID of the selected stage in business process instance.
         */
        selectedStageId?: string;
    }

    interface WebResource {
        /**
         * The type of the page.
         */
        pageType: "webresource";

        /**
         * The name of the web resource to load.
         */
        webresourceName: string;

        /**
         *  The data to pass to the web resource.
         */
        data?: string;
    }

    const enum NavigationOptionsTarget {
        PageInline = 1,
        Dialog = 2,
    }

    type SizeValueUnit = "%" | "px";

    interface SizeValue {
        /**
         * The numerical value.
         */
        value: number;

        /**
         * The unit of measurement.Specify "%" or "px".Default value is "px".
         */
        unit: SizeValueUnit;
    }

    const enum NavigationOptionsPosition {
        Center = 1,
        Side = 2,
    }

    interface NavigationOptions {
        /**
         * Specify 1 to open the page inline; 2 to open the page in a dialog. Entity lists can only be opened inline; web resources can be opened either inline or in a dialog
         */
        target: NavigationOptionsTarget;

        /**
         * The width of dialog. To specify the width in pixels, just type a numeric value. To specify the width in percentage, specify an object of type SizeValue
         */
        width?: number | SizeValue;

        /**
         * The height of dialog. To specify the width in pixels, just type a numeric value. To specify the width in percentage, specify an object of type SizeValue
         */
        height?: number | SizeValue;

        /**
         * Number. Specify 1 to open the dialog in center; 2 to open the dialog on the side. Default is 1 (center).
         */
        position?: NavigationOptionsPosition;

        /**
         * String. The dialog title on top of the center or side dialog.
         */
        title?: string;
    }

    /**
     * Contains methods for multi-page dialogs and task flow, and some methods moved from the Xrm.Utility namespace.
     */
    interface Navigation {
        /**
         * Navigates to the specified page.
         * @param pageInput Input about the page to navigate to. The object definition changes depending on the type of page to navigate to: entity list or HTML web resource.
         * @param navigationOptions Options for navigating to a page: whether to open inline or in a dialog. If you don't specify this parameter, page is opened inline by default.
         */
        navigateTo(pageInput: EntityRecord | EntityList | WebResource, navigationOptions?: NavigationOptions): Promise<undefined>;

        /**
         * Displays an alert dialog containing a message and a button.
         * @param alertStrings The string to be used in the alert dialog.
         * @param alertOptions The height and width options for alert dialog.
         */
        openAlertDialog(alertStrings: AlertStrings, alertOptions?: SizeOptions): Promise<undefined>;

        /**
         * Displays a confirmation dialog box containing a message and two buttons.
         * @param confirmStrings The strings to be used in the confirmation dialog.
         * @param confirmOptions The height and width options for confirmation dialog.
         */
        openConfirmDialog(confirmStrings: ConfirmStrings, confirmOptions?: SizeOptions): Promise<ConfirmDialogResult>;

        /**
         * Displays an error dialog.
         * @param errorOptions An object to specify the options for error dialog.
         * Either errorCode or message must be sat.
         */
        openErrorDialog(errorOptions: ErrorOptions): Promise<undefined>;

        /**
         * Opens a file.
         * @param file An object describing the file to open.
         * @param openFileOptions Specify whether to open or save the file.
         */
        openFile(file: Xrm.File, openFileOptions?: OpenFileOptions): void;

        /**
         * Opens an entity form or a quick create form.
         * @param entityFormOptions Entity form options for opening the form.
         * @param formParameters A dictionary object that passes extra parameters to the form.
         * See examples at: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/set-field-values-using-parameters-passed-form
         */
        openForm(entityFormOptions: EntityFormOptions, formParameters?: any): Then<OpenFormResult | undefined>;

        /**
         * Opens a URL, including file URLs.
         * @param url URL to open.
         * @param openUrlOptions Options to open the URL.
         */
        openUrl(url: string, openUrlOptions: SizeOptions): void;

        /**
         * Opens an HTML web resource.
         *
         * @param webResourceName The name of the HTML web resource to open.
         * @param windowOptions Window options for opening web resource.
         * @param data Data to be passed into the data parameter.
         */
        openWebResource(webResourceName: string, windowOptions?: WindowOptions, data?: string): void;
    }

    /**
     * Contains methods for displaying and hiding app-level global notifications.
     */
    interface UI {
        /**
         * addGlobalNotification();
         * clearGlobalNotification();
         */
    }

    interface WebApiBase {
        /**
         * Creates an entity record.
         * We recommend using XrmQuery instead of this interface.
         * @param entityLogicalName Logical name of the entity you want to create. For example: "account".
         * @param data A JSON object defining the attributes and values for the new entity record.
         * See examples at: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/clientapi/reference/xrm-webapi/createrecord
         */
        createRecord(entityLogicalName: string, data: object): Promise<Lookup>;

        /**
         * Deletes an entity record.
         * We recommend using XrmQuery instead of this interface.
         * @param entityLogicalName The entity logical name of the record you want to delete. For example "account".
         * @param id GUID of the entity record you want to delete.
         */
        deleteRecord(entityLogicalName: string, id: string): Promise<Lookup>;

        /**
         * Retrieves an entity record.
         * We recommend using XrmQuery instead of this interface.
         * @param entityLogicalName The entity logical name of the records you want to retrieve. For example: "account".
         * @param id GUID of the entity record you want to retrieve.
         * @param options OData system query options, $select and $expand, to retrieve your data.
         * See examples at: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/clientapi/reference/xrm-webapi/retrieverecord
         */
        retrieveRecord(entityLogicalName: string, id: string, options?: string): Promise<any>;

        /**
         * Retrieves a collection of entity records.
         * We recommend using XrmQuery instead of this interface.
         * @param entityLogicalName The entity logical name of the records you want to retrieve. For example: "account".
         * @param options OData system query options or FetchXML query to retrieve your data.
         * See examples at: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/clientapi/reference/xrm-webapi/retrievemultiplerecords
         * @param maxPageSize Specify a positive number that indicates the number of entity records to be returned per page. If you do not specify this parameter, the default value is passed as 5000.
         * If the number of records being retrieved is more than the specified maxPageSize value, nextLink attribute in the returned promise object will contain a link to retrieve the next set of entities.
         */
        retrieveMultipleRecords(entityLogicalName: string, options?: string, maxPageSize?: number): Promise<any>;

        /**
         * Updates an entity record.
         * We recommend using XrmQuery instead of this interface.
         * @param entityLogicalName The entity logical anem of the record you want to update. For example "account".
         * @param id GUID of the entity record you want to update.
         * @param data A JSON object containing key, value pairs where key is the property of the entity and value is the value of the property you want to update.
         * See examples at: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/clientapi/reference/xrm-webapi/updaterecord
         */
        updateRecord(entityLogicalName: string, id: string, data: object): Promise<Lookup>;
    }

    interface WebApiOffline extends WebApiBase {
        /**
         * Returns a boolean value indicating whether an entity is offline enabled.
         * We recommend using XrmQuery instead of this interface.
         * @param entityLogicalName Logical name of the entity. For example: "account".
         */
        isAvailableOffline(entityLogicalName: string): boolean;
    }

    interface WebApiResponse extends Response { }

    interface WebApiOnline extends WebApiBase {
        /**
         * Execute a single action, function, or CRUD operation.
         * We recommend using XrmQuery instead of this interface.
         * @param request Object that will be passed to the Web API endpoint to execute an action, function, or CRUD request.
         * The object exposes a getMetadata method that lets you define the metadata for the action, function or CRUD request you want to execute.
         */
        execute(request: any): Promise<WebApiResponse>;

        /**
         * Execute a collection of action, function, or CRUD operations.
         * If you want to execute multiple requests in a transaction, you must pass in a change set as a parameter to this method.
         * Change sets represent a collection of operations that are executed in a transaction.
         * You can also pass in individual requests and change sets together as parameters to this method.
         * We recommend using XrmQuery instead of this interface.
         * @param requests An array of requests and changesets. Requests are the same as for execute. Changesets are arrays of requests that will be executed in transaction.
         */
        executeMultiple(requests: any): Promise<WebApiResponse[]>;
    }

    /**
     * Contains methods for performing CRUD operations on records; automatically switches between online and offline mode.
     * We recommend using XrmQuery instead of this interface
     */
    interface WebApi extends WebApiBase {
        /**
         * We recommend using XrmQuery instead of this interface
         */
        online: WebApiOnline;

        /**
         * We recommend using XrmQuery instead of this interface
         */
        offline: WebApiOffline;
    }

    interface LookupOptionsFilter {
        /**
         * The FetchXML filter element to apply.
         */
        filterXml: string;

        /**
         * The entity type to which to apply this filter.
         */
        entityLogicalName: string
    }

    interface LookupOptions {
        /**
         * Indicates whether the lookup allows more than one item to be selected.
         */
        allowMultiSelect?: boolean;

        /**
         * The default entity type to use.
         */
        defaultEntityType?: string;

        /**
         * The default view to use.
         */
        defaultViewId?: string;

        /**
         * Decides whether to display the most recently used(MRU) item. Available only for Unified Interface.
         */
        disableMru?: boolean;

        /**
         * The entity types to display.
         */
        entityTypes?: string[];

        /**
         * Used to filter the results. Each object in the array contains the following attributes.
         */
        filters?: LookupOptionsFilter[];

        /**
         * Indicates whether the lookup control should show the barcode scanner in mobile clients.
         */
        showBarcodeScanner?: boolean;

        /**
         * The views to be available in the view picker. Only system views are supported.
         */
        viewIds?: string[];
    }

    /**
     * Provides a container for useful methods.
     */
    interface Utility {
        /**
         * Closes a progress dialog box.
         * If no progress dialog is displayed currently, this method will do nothing.
         */
        closeProgressIndicator(): void;

        /**
         * Returns the valid state transitions for the specified entity type and state code.
         * @param entityName The logical name of the entity.
         * @param stateCode The state code to find out the allowed status transition values
         */
        getAllowedStatusTransitions(entityName: string, stateCode: number): Promise<number[] | null>;

        /**
         * Returns the entity metadata for the specified entity
         * @param entityName The logical name of the entity.
         * @param attributes The attributes to get metadata for.
         */
        getEntityMetadata(entityName: string, attributes?: string[]): Promise<any>;

        /**
         * Gets the global context.
         */
        getGlobalContext(): context;

        /**
         * Returns the name of the DOM attribute expected by the Learning Path (guided help) Content Designer for identifying UI controls in the Dynamics 365 Customer Engagement forms.
         * An attribute by this name must be added to the UI element that needs to be exposed to Learning Path (guided help).
         */
        getLearningPathAttributeName(): string;

        /**
         * Returns the localized string for a given key associated with the specified web resource.
         * @param webResourceName The name of the web resource.
         * @param key The key for the localized string.
         */
        getResourceString(webResourceName: string, key: string): string;

        /**
         * Invokes an action based on the specified parameters.
         * @param name of the process action to invoke.
         * @param parameters An object containing input parameters for the action. You define an object using key:value pairs of items, where key is of String type.
         */
        invokeProcessAction(name: string, parameters?: any): Promise<undefined>;

        /**
         * Opens a lookup control to select one or more items.
         * @param lookupOptions Defines the options for opening the lookup dialog.
         */
        lookupObjects(lookupOptions: LookupOptions): Promise<Lookup[]>;

        /**
         * Refreshes the parent grid containing the specified record.
         * @param lookupOptions The record who's parent's grid to refresh.
         */
        refreshParentGrid(lookupOptions: Lookup): void;

        /**
         * Displays a progress dialog with the specified message.
         * Any subsequent call to this method will update the displayed message in the existing progress dialog with the message specified in the latest method call.
         * @param message The message to be displayed in the progress dialog.
         */
        showProgressIndicator(message: string): void;
    }

    interface PreProcessStatusChangeContext extends ExecutionContext<Process, any> { }

    interface PreStageChangeContext extends ExecutionContext<Stage, StageChangeEventArguments> { }

    interface ProcessInstanceContext {
        CreatedOnDate: Date,
        ProccessDefinitionID: string,
        ProccessDefinitionName: string,
        ProcessInstanceID: string,
        ProcessInstanceName: string,
        StatusCodeName: string
    }

    interface ProcessModule {
        /**
         * Adds a function as an event handler for the OnPreProcessStatusChange event so that it will be called before the business process flow status changes.
         * @param handler The function to be executed when the business process flow status changes.
         *                The function will be added to the start of the event handler pipeline.
         *                The execution context is automatically passed as the first parameter to
         *                the function. See Execution context for more information.
         *                Use a reference to a named function rather than an
         *                anonymous function if you may later want to remove the
         *                event handler.
         * This client API is only supported on the Unified Client. The legacy web client does not support this client API.
         */
        addOnPreProcessStatusChange(handler: (context?: PreProcessStatusChangeContext) => any): void;

        /**
         * Removes an event handler from the OnPreProcessStatusChange event.
         * @param handler The function to be removed from the OnPreProcessStatusChange event.
         */
        removeOnPreProcessStatusChange(handler: (context?: PreProcessStatusChangeContext) => any): void;

        /**
         * Adds a function as an event handler for the OnPreStageChange event so that it will be called before the business process flow stage changes.
         * @param handler The function that runs before the business process flow stage changes.
         *                The function will be added to the start of the event handler pipeline.
         *                The execution context is automatically passed as the first parameter
         *                to the function. See Execution context for more information.
         *                Use a reference to a named function rather than an
         *                anonymous function if you may later want to remove the
         *                event handler.
         * This client API is only supported on the Unified Client. The legacy web client does not support this client API.
         */
        addOnPreStageChange(handler: (context?: PreStageChangeContext) => any): void;

        /**
         * Removes an event handler from the OnPreStageChange event.
         * @param handler The function to be removed from the OnPreStageChange event.
         */
        removeOnPreStageChange(handler: (context?: PreStageChangeContext) => any): void;

        /**
         * Returns all the process instances for the entity record that the calling user has access to.
         * @param handler The callback function is passed an object with the following attributes
         *                and their corresponding values as the key: value pair.
         */
        getProcessInstances(callbackFunction: (context?: ProcessInstanceContext) => any): void;

        /**
         * Sets a process instance as the active instance.
         * @param processInstanceid The Id of the process instance to set as the active instance.
         * @param callbackFunction A function to call when the operation is complete. This callback function is passed either string "succes" or "invalid" to indicate whether the operation succeeded:
         */
        setActiveProcessInstance(processInstanceId: string, callbackFunction?: (succesOrInvalid: "success" | "invalid") => any): void;

        /**
         * Set a Process as the active process.
         *
         * @param processId The Id of the process to make the active process.
         * @param callback A function to call when the operation is complete. This callback function is passed one of the following string
         *    values to indicate whether the operation succeeded. Is "success" or "invalid".
         */
        setActiveProcess(processId: string, callback?: (successOrInvalid: "success" | "invalid") => any): void;
    }

    /**
     * Form executionContext
     */
    interface ExecutionContext<TSource, TArgs> {
        getFormContext(): Xrm.PageBase<Xrm.AttributeCollectionBase, Xrm.TabCollectionBase, Xrm.ControlCollectionBase>;
    }

    interface SaveOptions {
        /**
         * Specify a value indicating how the save event was initiated.
         * Note that setting the saveMode does not actually take the corresponding action; it is just to provide information to the OnSave event handlers about the reason for the save operation.
         */
        saveMode?: SaveMode;
    }

    /**
     * Interface for the data of a form.
     */
    interface DataModule<T extends AttributeCollectionBase> {
        /**
         * Collection of non-entity data on the form. Items in this collection are of the same type as the attributes collection, but they are not attributes of the form entity.
         */
        attributes: T;

        /**
         * Adds a function to be called when form data is loaded.
         */
        addOnLoad(myFunction: (context?: OnLoadEventContext) => any): void;

        /**
         * Gets a boolean value indicating whether the form data has been modified.
         */
        getIsDirty(): boolean;

        /**
         * Gets a boolean value indicating whether all of the form data is valid. This includes the main entity and any unbound attributes.
         */
        isValid(): boolean;

        /**
         * Removes a function to be called when form data is loaded.
         */
        removeOnLoad(myFunction: Function): void;

        /**
         * Asynchronously refreshes and optionally saves all the data of the form without reloading the page.
         *
         * @param save true if the data should be saved after it is refreshed, otherwise false.
         */
        refresh(save?: boolean): Promise<undefined>;

        /**
         * Saves the record asynchronously with the option to set callback functions to be executed after the save operation is completed.
         *
         * @param saveOptions This option is only applicable when used with appointment, recurring appointment, or service activity records.
         */
        save(saveOptions?: SaveOptions): Promise<undefined>;
    }

    /**
     * Specify options for saving the record. If no parameter is included in the method, the record will simply be saved. This is the equivalent of using the Save command.
     * You can specify one of the following values:
     * - saveandclose: This is the equivalent of using the Save and Close command.
     * - saveandnew: This is the equivalent of the using the Save and New command
     */
    type saveOption = "saveandclose" | "saveandnew";

    /**
     * Interface for the entity on a form.
     */
    interface PageEntity<T extends AttributeCollectionBase> {
        /**
         * Returns a lookup value that references the record.
         */
        getEntityReference(): Lookup;

        /**
         * Gets a boolean value indicating whether all of the entity data is valid.
         */
        isValid(): boolean;

        /**
         * Saves the record synchronously with the options to close the form or open a new form after the save is completed.
         */
        save(saveOptions?: saveOption): void;
    }

    /**
     * Interface for an standard entity attribute.
     */
    interface Attribute<T> {

        /**
         * Returns a boolean value to indicate whether the value of an attribute is valid.
         */
        isValid(): boolean;
    }

    /**
     * Interface for the ui of a form.
     */
    interface UiModule<T extends TabCollectionBase, U extends ControlCollectionBase> {
        /**
         * Adds a function to be called on the form OnLoad event.
         * @param myFunction The function to be executed on the form OnLoad event. The function will be added to the bottom of the event handler pipeline.
         * The execution context is automatically passed as the first parameter to the function. See Execution context for more information.
         */
        addOnLoad(myFunction: (context?: OnLoadEventContext) => any): void;

        /**
         * Removes a function from the form OnLoad event.
         * @param myFunction The function to be removed from the form OnLoad event.
         */
        removeOnLoad(myFunction: Function): void;
    }

    interface dateFormattingInfo {
        AMDesignator: string;
        Calendar: Calendar;
        DateSeparator: string;
        FirstDayOfWeek: number;
        CalendarWeekRule: number;
        FullDateTimePattern: string;
        LongDatePattern: string;
        LongTimePattern: string;
        MonthDayPattern: string;
        PMDesignator: string;
        RFC1123Pattern: string;
        ShortDatePattern: string;
        ShortTimePattern: string;
        SortableDateTimePattern: string;
        TimeSeparator: string;
        UniversalSortableDateTimePattern: string;
        YearMonthPattern: string;
        AbbreviatedDayNames: string[];
        ShortestDayNames: string[];
        DayNames: string[];
        AbbreviatedMonthNames: string[];
        MonthNames: string[];
        IsReadOnly: boolean;
        NativeCalendarName: string;
        AbbreviatedMonthGenitiveNames: string[];
        MonthGenitiveNames: string[];
        eras: (null | number | string)[];
    }

    interface Calendar {
        MinSupportedDateTime: string;
        MaxSupportedDateTime: string;
        AlgorithmType: number;
        CalendarType: number;
        Eras: number[];
        TwoDigitYearMax: number;
        IsReadOnly: boolean;
    }

    interface userSettings {
        /**
         * The name of the current user.
         */
        dateFormattingInfo: dateFormattingInfo;

        /**
         * Returns the ID of the default dashboard for the current user.
         */
        defaultDashboardId: string;

        /**
         * Indicates whether high contrast is enabled for the current user.
         */
        isGuidedHelpEnabled: boolean;

        /**
         * Indicates whether guided help is enabled for the current user..
         */
        isHighContrastEnabled: boolean;

        /**
         * Indicates whether the language for the current user is a right-to-left (RTL) language.
         */
        isRTL: boolean;

        /**
         * The LCID value that represents the provisioned language that the user selected as their preferred language.
         */
        languageId: number;

        /**
         * The name of the current user.
         */
        userName: string;

        /**
         * Returns the difference between the local time and Coordinated Universal Time (UTC).
         */
        getTimeZoneOffsetMinutes(): number;

        /**
         * The GUID of the SystemUser.Id value for the current user.
         */
        userId: string;
    }

    interface organizationSettings {
        /**
         * Returns whether Autosave is enabled for the organization.
         */
        getIsAutoSaveEnabled(): boolean;

        /**
         * The language code identifier (LCID) value that represents the base language for the organization.
         */
        languageId: number;

        /**
         * The unique text value of the organization?s name.
         */
        uniqueName: string;
    }

    interface appProperties {
        appId: string;
        displayName: string;
        uniqueName: string;
        url: string;
        webResourceId: string;
        webResourceName: string;
        welcomePageId: string;
        welcomePageName: string;
    }

    interface context {
        /**
         * Returns information about the current user settings.
         */
        userSettings: userSettings;

        /**
         * Returns information about the current organization settings.
         */
        organizationSettings: organizationSettings;

        /**
         * Returns the URL of the current business app in Customer Engagement.
         */
        getCurrentAppUrl(): string;

        /**
         * Returns the name of the current business app in model-driven apps.
         */
        getCurrentAppName(): Promise<string>;

        /**
         * Returns the properties of the current business app in model-driven apps.
         */
        getCurrentAppProperties(): Promise<appProperties>;

        /**
         * Returns the version number of the model-driven apps instance.
         */
        getVersion(): string;

        /**
         * Returns the relative URL with the caching token for the specified web resource.
         */
        getWebResourceUrl(webResourceName: string): string;

        /**
         * Returns a boolean value indicating if the model-driven apps instance is hosted on-premises or online.
         */
        isOnPremises(): boolean;
    }

    /**
     * Interface for an MultiSelectOptionSet attribute.
     */
    interface MultiSelectOptionSetAttribute<T> extends Attribute<T[]> {
        /**
         * Collection of controls associated with the attribute.
         */
        controls: Collection<MultiSelectOptionSetControl<T>>;

        /**
         * Returns a value that represents the value set for a MultiSelectOptionSet when the form opened.
         */
        getInitialValue(): T | null;

        /**
         * Returns an array of string values of the text for the currently selected options for a multiselectoptionset attribute.
         */
        getText(): string[] | null;

        /**
         * Returns an option object with the value matching the argument passed to the method.
         */
        getOption(value: string): Option<T> | null;

        /**
         * Returns an option object with the value matching the argument passed to the method.
         */
        getOption(value: T): Option<T> | null;

        /**
         * Returns an array of option objects representing the valid options for an option-set attribute.
         */
        getOptions(): Option<T>[];

        /**
         * Returns the option object that is selected in an optionset attribute.
         */
        getSelectedOption(): Option<T>[] | null;
    }

    /**
     * Interface for an MultiSelectOptionSet form control.
     */
    interface MultiSelectOptionSetControl<T> extends Control<MultiSelectOptionSetAttribute<T>> {
        /**
         * Adds an option to an option set control.
         *
         * @param option An option object to add to the OptionSet.
         * @param index The index position to place the new option in. If not provided, the option will be added to the end.
         */
        addOption(option: Option<T>, index?: number): void;

        /**
         * Clears all options from an option set control.
         */
        clearOptions(): void;

        /**
         * Removes an option from an option set control.
         *
         * @param number The value of the option you want to remove.
         */
        removeOption(number: number): void;
    }

    interface LookupControl<T extends string> extends Control<LookupAttribute<T>> {
        /**
         * Use this method to set which entity types the lookup control will show the user
         */
        setEntityTypes(entityTypes: string[]): void;

        /**
         * Use this method to get which entity types the lookup control will show the user
         */
        getEntityTypes(): string[];
    }

    const enum GridType {
        HomePageGrid = 1,
        Subgrid = 2,
    }

    const enum SubGridControlClientType {
        Browser = 0,
        MobileApplication = 1,
    }

    interface SubGridControl<T extends string> extends BaseControl {
        /**
         * Gets the FetchXML query that represents the current data, including filtered and sorted data, in the grid control.
         */
        getFetchXml(): string;

        /**
         * Gets the FetchXML query that represents the current data, including filtered and sorted data, in the grid control.
         */
        getRelationship(): EntityFormRelationship;

        /**
         * Gets information about the relationship used to filter the subgrid.
         */
        getGridType(): GridType;

        /**
         * Gets the URL of the current grid control.
         */
        getUrl(client?: SubGridControlClientType): string;

        /**
         * Gets the URL of the current grid control.
         */
        openRelatedGrid(): void;

        /**
         * Refreshes the ribbon rules for the grid control.
         */
        refreshRibbon(): void;
    }

    interface GridRow<T extends string> {
        /**
         * A collection containing the GridRowData for the GridRow.
         */
        data: GridRowData<T>;
    }

    interface GridRowData<T extends string> {
        /**
         * Returns the GridEntity for the GridRowData.
         */
        entity: GridEntity<T>;
    }

    interface GridEntity<T extends string> {
        /**
         * Each attribute (GridAttribute) represents the data in the cell of an editable grid,
         * and contains a reference to all the cells associated with the attribute.
         */
        attributes: GridCollection<GridEntityAttribute<T>>;
    }

    interface GridCollection<T> {
    }

    interface GridEntityAttribute<T extends string> {
    }

    type AddNotificationLevel = "RECOMMENDATION" | "ERROR";

    interface actionsObject {
        message?: string;
        actions?: Function[];
    }

    interface AddNotificationObject {
        actions?: actionsObject[];
        messages: string[];
        notificationLevel: AddNotificationLevel;
        uniqueId: string;
    }

    interface BaseControl {
        addNotification(notification: AddNotificationObject): void;
    }

    interface NavigationBehaviorObject {
        allowCreateNew(): boolean
    }

    interface Stage {
        /**
         * Returns a navigation behavior object for a stage that can be used to define whether the Create button is available for users to create other entity record in a cross-entity business process flow navigation scenario.
         */
        getNavigationBehavior(): NavigationBehaviorObject;
    }

    interface StageStep {
        /**
         * Returns the progress of the action step.
         */
        getProgress(): number;

        /**
         * Updates the progress of the action step.
         * @param stepProgress number value specifying the step progress:
         * 0: None
         * 1: Processing
         * 2: Completed
         * 3: Failure
         * 4: Invalid
         * @param message An optional message that is set as the Alt text on the icon for the step.
         */
        setProgress(stepProgress: number, message?: string): string;
    }
}


interface Xrm<T extends Xrm.PageBase<Xrm.AttributeCollectionBase, Xrm.TabCollectionBase, Xrm.ControlCollectionBase>> extends BaseXrm {
    Device: Xrm.Device;
    Encoding: Xrm.Encoding;
    Navigation: Xrm.Navigation;
    //UI: Xrm.UI;
    WebApi: Xrm.WebApi;
}
declare namespace Xrm {
    /**
     * Lookup-like type object for userSettings.roles.
     */
    interface Role {
        id: string;
        name: string;
    }

    interface userSettings {
        /**
         * Collection of lookup-like objects containing the GUID and display name of each of the security role or teams that the user is associated with.
         */
        roles: Collection<Role>;

        /**
         * Returns an array of strings that represent the GUID values of each of the security role privilege that the user is associated with or any teams that the user is associated with.
         */
        securityRolePrivileges: string[]
    }

    /**
     * Interface for an standard entity attribute.
     */
    interface Attribute<T> {

        /**
         * Sets a value for an attribute to determine whether it is valid or invalid with a message.
         */
        setIsValid(bool: boolean, message?: string): void;
    }

    const enum LoadState {
        InitialLoad = 1,
        Save = 2,
        Refresh = 3,
    }

    interface LoadEventArgs {
        /**
         * Gets the state of the data load.
         */
        getDataLoadState(): LoadState;
    }

    interface OnLoadEventContext extends ExecutionContext<UiModule<TabCollectionBase, ControlCollectionBase>, LoadEventArgs> { }

    interface LookupTagValue extends Lookup {
        /**
         * The originating lookup field that raised the event.
         */
        fieldName: string;
    }

    interface OnLookupTagClickEventArgs {
        /**
         * Gets the selected tag value.
         */
        getTagValue(): LookupTagValue;

        /**
         * Returns a value indicating whether the lookup tag click event has been canceled because the preventDefault method was used in this event hander or a previous event handler.
         */
        isDefaultPrevented(): boolean;

        /**
         * Cancels the lookup tag click event, but all remaining handlers for the event will still be executed.
         */
        preventDefault(): void;
    }

    interface OnLookupTagClickContext extends ExecutionContext<any, OnLookupTagClickEventArgs> { }

    interface LookupControl<T extends string> extends Control<LookupAttribute<T>> {
        /**
         * Adds an event handler to the OnLookupTagClick event.
         */
        addOnLookupTagClick(myFunction: (context?: OnLookupTagClickContext) => any): void;

        /**
         * Removes an event handler from the OnLookupTagClick event.
         */
        removeOnLookupTagClick(functionRef: Function): void;
    }

    interface FormItem {
        /**
         * Sets a value that indicates whether the control is visible.
         */
        setVisible(visibility: boolean): void;
    }

    type KBSeachControlMode = "Inline" | "Popout";

    /**
     * Interface for a KBSearchResult returned on the method getSelectedResults from the KBSearchControl.
     * All Date objects will be in the current user's time zone and format.
     */
    interface KBSearchResult {
        /**
         * The HTML markup containing the content of the article.
         */
        answer(): string;

        /**
         * The article ID in Dynamics 365.
         */
        articleId(): string;

        /**
         * The unique article ID in Dynamics 365.
         */
        articleUid(): string;

        /**
         * The date the article was created.
         */
        createdOn(): Date;

        /**
         * Indicates whether the article is associated with the parent record or not.
         */
        isAssociated(): boolean;

        /**
         * Date on which the article was last modified.
         */
        lastModifiedOn(): Date;

        /**
         * Support Portal URL of the article; blank if Portal URL option is turned off.
         */
        publicUrl(): string;

        /**
         * Indicates whether the article is in published state. True if published; otherwise False.
         */
        published(): boolean;

        /**
         * The title of the article.
         */
        question(): string;

        /**
         * The rating of the article.
         */
        rating(): number;

        /**
         * A short snippet of article content that contains the areas where the search query was hit.
         */
        searchBlurb(): string;

        /**
         * Link to the article in the Dynamics 365.
         */
        serviceDeskUri(): string;

        /**
         * The number of times an article is viewed on the portal by customers.
         */
        timesViewved(): number;
    }

    /**
     * Interface for a knowledge base search control.
     */
    interface KBSearchControl extends BaseControl {
        /**
         * Add an event handler to the PostSearch event.
         * @param functionRef The function to add.
         */
        addOnPostSearch(functionRef: (context?: ExecutionContext<this, undefined>) => any): void;

        /**
         * Add an event handler to the OnResultOpened event.
         * @param functionRef The function to add.
         */
        addOnResultOpened(functionRef: (context?: ExecutionContext<this, undefined>) => any): void;

        /**
         * Add an event handler to the OnSelection event.
         * @param functionRef The function to add.
         */
        addOnSelection(functionRef: (context?: ExecutionContext<this, undefined>) => any): void;

        /**
         * Get the text used as the search criteria for the knowledge base management control.
         */
        getSearchQuery(): string;

        /**
         * Get the currently selected result of the search control; the one that is currently open.
         * @return The currently selected result.
         */
        getSelectedResults(): KBSearchResult;

        /**
         * Gets the count of results found in the search control.
         * @return The count of the search result.
         */
        getTotalResultCount(): number;

        /**
         * Opens a search result in the search control by specifying the result number.
         * @param resultNumber Numerical value specifying the result number to be opened. Required.
         * @param mode Specify "Inline" or "Popout". Optional. Defaults to "Inline".
         * @return Returns 1 if successful; 0 if unsuccessful; -1 if the specified resultNumber is not present, or if the specified mode is invalid.
         */
        openSearchResult(resultNumber: number, mode?: KBSeachControlMode): number;

        /**
         * Use this method to remove an event handler from the PostSearch event.
         * @param functionRef The function to remove.
         */
        removeOnPostSearch(functionRef: Function): void;

        /**
         * Remove an event handler from the OnResultOpened event.
         * @param functionRef The function to remove.
         */
        removeOnResultOpened(functionRef: Function): void;

        /**
         * Remove an event handler from the OnSelection event.
         * @param functionRef The function to remove.
         */
        removeOnSelection(functionRef: Function): void;

        /**
         * Set the text used as the search criteria for the knowledge base management control.
         * @param text The text for the search query.
         */
        setSearchQuery(text: string): void;
    }

    interface PageEntity<T extends AttributeCollectionBase> {
        /**
         * Adds a function to be called when save event has completed; either successfully or with a failure.
         * @param functionRef The function to add to the PostSave event.
         * The execution context is automatically passed as the first parameter to this function.
         */
        addOnPostSave(functionRef: (context?: SaveEventContext<this>) => any): void;
    }
}
