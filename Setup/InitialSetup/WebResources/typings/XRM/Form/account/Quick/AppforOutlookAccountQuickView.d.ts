declare namespace Form.account.Quick {
  namespace AppforOutlookAccountQuickView {
    namespace Tabs {
      interface tab_1 extends Xrm.SectionCollectionBase {
        get(name: "tab_1_column_1_section_1"): Xrm.PageSection;
        get(name: string): undefined;
        get(): Xrm.PageSection[];
        get(index: number): Xrm.PageSection;
        get(chooser: (item: Xrm.PageSection, index: number) => boolean): Xrm.PageSection[];
      }
    }
    interface Attributes extends Xrm.AttributeCollectionBase {
      get(name: string): undefined;
      get(): Xrm.Attribute<any>[];
      get(index: number): Xrm.Attribute<any>;
      get(chooser: (item: Xrm.Attribute<any>, index: number) => boolean): Xrm.Attribute<any>[];
    }
    interface Controls extends Xrm.ControlCollectionBase {
      get(name: "AppforOutlook_Accoount_QV_Subgrid_2"): Xrm.SubGridControl<"activitypointer">;
      get(name: "AppforOutlook_Account_QV_Subgrid_1"): Xrm.SubGridControl<"activitypointer">;
      get(name: "AppforOutlook_Account_QV_Subgrid_3"): Xrm.SubGridControl<"opportunity">;
      get(name: "AppforOutlook_Account_QV_Subgrid_4"): Xrm.SubGridControl<"incident">;
      get(name: string): undefined;
      get(): Xrm.BaseControl[];
      get(index: number): Xrm.BaseControl;
      get(chooser: (item: Xrm.BaseControl, index: number) => boolean): Xrm.BaseControl[];
    }
    interface Tabs extends Xrm.TabCollectionBase {
      get(name: "tab_1"): Xrm.PageTab<Tabs.tab_1>;
      get(name: string): undefined;
      get(): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
      get(index: number): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>;
      get(chooser: (item: Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>, index: number) => boolean): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
    }
  }
  interface AppforOutlookAccountQuickView extends Xrm.PageBase<AppforOutlookAccountQuickView.Attributes,AppforOutlookAccountQuickView.Tabs,AppforOutlookAccountQuickView.Controls> {
    getAttribute(attributeName: string): undefined;
    getControl(controlName: "AppforOutlook_Accoount_QV_Subgrid_2"): Xrm.SubGridControl<"activitypointer">;
    getControl(controlName: "AppforOutlook_Account_QV_Subgrid_1"): Xrm.SubGridControl<"activitypointer">;
    getControl(controlName: "AppforOutlook_Account_QV_Subgrid_3"): Xrm.SubGridControl<"opportunity">;
    getControl(controlName: "AppforOutlook_Account_QV_Subgrid_4"): Xrm.SubGridControl<"incident">;
    getControl(controlName: string): undefined;
  }
}
