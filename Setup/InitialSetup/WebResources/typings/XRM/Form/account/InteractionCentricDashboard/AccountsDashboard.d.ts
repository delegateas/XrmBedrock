declare namespace Form.account.InteractionCentricDashboard {
  namespace AccountsDashboard {
    namespace Tabs {
      interface StreamsContainer extends Xrm.SectionCollectionBase {
        get(name: "Streams"): Xrm.PageSection;
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
      get(name: "Component2316e00"): Xrm.SubGridControl<"account">;
      get(name: "Component666c4a4"): Xrm.SubGridControl<"account">;
      get(name: "Componentc4e86d7"): Xrm.SubGridControl<"account">;
      get(name: "Component{c25c1ba2-33b6-0760-d1cc-7621224bdaa7}"): Xrm.BaseControl;
      get(name: string): undefined;
      get(): Xrm.BaseControl[];
      get(index: number): Xrm.BaseControl;
      get(chooser: (item: Xrm.BaseControl, index: number) => boolean): Xrm.BaseControl[];
    }
    interface Tabs extends Xrm.TabCollectionBase {
      get(name: "StreamsContainer"): Xrm.PageTab<Tabs.StreamsContainer>;
      get(name: string): undefined;
      get(): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
      get(index: number): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>;
      get(chooser: (item: Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>, index: number) => boolean): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
    }
  }
  interface AccountsDashboard extends Xrm.PageBase<AccountsDashboard.Attributes,AccountsDashboard.Tabs,AccountsDashboard.Controls> {
    getAttribute(attributeName: string): undefined;
    getControl(controlName: "Component2316e00"): Xrm.SubGridControl<"account">;
    getControl(controlName: "Component666c4a4"): Xrm.SubGridControl<"account">;
    getControl(controlName: "Componentc4e86d7"): Xrm.SubGridControl<"account">;
    getControl(controlName: "Component{c25c1ba2-33b6-0760-d1cc-7621224bdaa7}"): Xrm.BaseControl;
    getControl(controlName: string): undefined;
  }
}
