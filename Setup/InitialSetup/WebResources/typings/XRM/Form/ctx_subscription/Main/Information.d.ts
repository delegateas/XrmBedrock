declare namespace Form.ctx_subscription.Main {
  namespace Information {
    namespace Tabs {
    }
    interface Attributes extends Xrm.AttributeCollectionBase {
      get(name: "ctx_enddate"): Xrm.DateAttribute;
      get(name: "ctx_name"): Xrm.Attribute<string>;
      get(name: "ctx_product"): Xrm.LookupAttribute<"ctx_product">;
      get(name: "ctx_startdate"): Xrm.DateAttribute;
      get(name: "ownerid"): Xrm.LookupAttribute<"systemuser" | "team">;
      get(name: string): undefined;
      get(): Xrm.Attribute<any>[];
      get(index: number): Xrm.Attribute<any>;
      get(chooser: (item: Xrm.Attribute<any>, index: number) => boolean): Xrm.Attribute<any>[];
    }
    interface Controls extends Xrm.ControlCollectionBase {
      get(name: "ctx_enddate"): Xrm.DateControl;
      get(name: "ctx_name"): Xrm.StringControl;
      get(name: "ctx_product"): Xrm.LookupControl<"ctx_product">;
      get(name: "ctx_startdate"): Xrm.DateControl;
      get(name: "ownerid"): Xrm.LookupControl<"systemuser" | "team">;
      get(name: string): undefined;
      get(): Xrm.BaseControl[];
      get(index: number): Xrm.BaseControl;
      get(chooser: (item: Xrm.BaseControl, index: number) => boolean): Xrm.BaseControl[];
    }
    interface QuickViewForms extends Xrm.QuickViewFormCollectionBase {
      get(name: string): undefined;
      get(): Xrm.QuickViewFormBase[];
      get(index: number): Xrm.QuickViewFormBase;
      get(chooser: (item: Xrm.QuickViewFormBase, index: number) => boolean): Xrm.QuickViewFormBase[];
    }
    interface Tabs extends Xrm.TabCollectionBase {
      get(name: string): undefined;
      get(): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
      get(index: number): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>;
      get(chooser: (item: Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>, index: number) => boolean): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
    }
  }
  interface Information extends Xrm.PageBase<Information.Attributes,Information.Tabs,Information.Controls,Information.QuickViewForms> {
    getAttribute(attributeName: "ctx_enddate"): Xrm.DateAttribute;
    getAttribute(attributeName: "ctx_name"): Xrm.Attribute<string>;
    getAttribute(attributeName: "ctx_product"): Xrm.LookupAttribute<"ctx_product">;
    getAttribute(attributeName: "ctx_startdate"): Xrm.DateAttribute;
    getAttribute(attributeName: "ownerid"): Xrm.LookupAttribute<"systemuser" | "team">;
    getAttribute(attributeName: string): undefined;
    getAttribute(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Attribute<any>>): Xrm.Attribute<any>[];
    getControl(controlName: "ctx_enddate"): Xrm.DateControl;
    getControl(controlName: "ctx_name"): Xrm.StringControl;
    getControl(controlName: "ctx_product"): Xrm.LookupControl<"ctx_product">;
    getControl(controlName: "ctx_startdate"): Xrm.DateControl;
    getControl(controlName: "ownerid"): Xrm.LookupControl<"systemuser" | "team">;
    getControl(controlName: string): undefined;
    getControl(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Control<any>>): Xrm.Control<any>[];
  }
}
