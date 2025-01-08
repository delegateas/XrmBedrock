declare namespace Form.mgs_subscription.Main {
  namespace Information {
    namespace Tabs {
    }
    interface Attributes extends Xrm.AttributeCollectionBase {
      get(name: "mgs_enddate"): Xrm.DateAttribute;
      get(name: "mgs_invoiceduntil"): Xrm.DateAttribute;
      get(name: "mgs_startdate"): Xrm.DateAttribute;
      get(name: "mgs_subscriptionname"): Xrm.Attribute<string>;
      get(name: "ownerid"): Xrm.LookupAttribute<"systemuser" | "team">;
      get(name: string): undefined;
      get(): Xrm.Attribute<any>[];
      get(index: number): Xrm.Attribute<any>;
      get(chooser: (item: Xrm.Attribute<any>, index: number) => boolean): Xrm.Attribute<any>[];
    }
    interface Controls extends Xrm.ControlCollectionBase {
      get(name: "mgs_enddate"): Xrm.DateControl;
      get(name: "mgs_invoiceduntil"): Xrm.DateControl;
      get(name: "mgs_startdate"): Xrm.DateControl;
      get(name: "mgs_subscriptionname"): Xrm.StringControl;
      get(name: "ownerid"): Xrm.LookupControl<"systemuser" | "team">;
      get(name: string): undefined;
      get(): Xrm.BaseControl[];
      get(index: number): Xrm.BaseControl;
      get(chooser: (item: Xrm.BaseControl, index: number) => boolean): Xrm.BaseControl[];
    }
    interface Tabs extends Xrm.TabCollectionBase {
      get(name: string): undefined;
      get(): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
      get(index: number): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>;
      get(chooser: (item: Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>, index: number) => boolean): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
    }
  }
  interface Information extends Xrm.PageBase<Information.Attributes,Information.Tabs,Information.Controls> {
    getAttribute(attributeName: "mgs_enddate"): Xrm.DateAttribute;
    getAttribute(attributeName: "mgs_invoiceduntil"): Xrm.DateAttribute;
    getAttribute(attributeName: "mgs_startdate"): Xrm.DateAttribute;
    getAttribute(attributeName: "mgs_subscriptionname"): Xrm.Attribute<string>;
    getAttribute(attributeName: "ownerid"): Xrm.LookupAttribute<"systemuser" | "team">;
    getAttribute(attributeName: string): undefined;
    getControl(controlName: "mgs_enddate"): Xrm.DateControl;
    getControl(controlName: "mgs_invoiceduntil"): Xrm.DateControl;
    getControl(controlName: "mgs_startdate"): Xrm.DateControl;
    getControl(controlName: "mgs_subscriptionname"): Xrm.StringControl;
    getControl(controlName: "ownerid"): Xrm.LookupControl<"systemuser" | "team">;
    getControl(controlName: string): undefined;
  }
}
