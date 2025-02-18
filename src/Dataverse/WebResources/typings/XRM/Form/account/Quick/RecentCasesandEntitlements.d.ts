declare namespace Form.account.Quick {
  namespace RecentCasesandEntitlements {
    namespace Tabs {
      interface general extends Xrm.SectionCollectionBase {
        get(name: "Cases"): Xrm.PageSection;
        get(name: "Entitlements"): Xrm.PageSection;
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
      get(name: "accountcasessgrid"): Xrm.SubGridControl<"incident">;
      get(name: "subgrid_Entitlement"): Xrm.SubGridControl<"entitlement">;
      get(name: string): undefined;
      get(): Xrm.BaseControl[];
      get(index: number): Xrm.BaseControl;
      get(chooser: (item: Xrm.BaseControl, index: number) => boolean): Xrm.BaseControl[];
    }
    interface Tabs extends Xrm.TabCollectionBase {
      get(name: "general"): Xrm.PageTab<Tabs.general>;
      get(name: string): undefined;
      get(): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
      get(index: number): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>;
      get(chooser: (item: Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>, index: number) => boolean): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
    }
  }
  interface RecentCasesandEntitlements extends Xrm.PageBase<RecentCasesandEntitlements.Attributes,RecentCasesandEntitlements.Tabs,RecentCasesandEntitlements.Controls> {
    getAttribute(attributeName: string): undefined;
    getControl(controlName: "accountcasessgrid"): Xrm.SubGridControl<"incident">;
    getControl(controlName: "subgrid_Entitlement"): Xrm.SubGridControl<"entitlement">;
    getControl(controlName: string): undefined;
  }
}
