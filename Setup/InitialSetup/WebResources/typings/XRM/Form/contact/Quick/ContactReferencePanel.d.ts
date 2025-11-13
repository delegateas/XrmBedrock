declare namespace Form.contact.Quick {
  namespace ContactReferencePanel {
    namespace Tabs {
      interface tab_1 extends Xrm.SectionCollectionBase {
        get(name: "tab_1_column_1_section_1"): Xrm.PageSection;
        get(name: string): undefined;
        get(): Xrm.PageSection[];
        get(index: number): Xrm.PageSection;
        get(chooser: (item: Xrm.PageSection, index: number) => boolean): Xrm.PageSection[];
      }
    }
    interface Controls extends Xrm.ControlCollectionBase {
      get(name: "address1_city"): Xrm.StringControl;
      get(name: "address1_line1"): Xrm.StringControl;
      get(name: "address1_line2"): Xrm.StringControl;
      get(name: "address1_postalcode"): Xrm.StringControl;
      get(name: "emailaddress1"): Xrm.StringControl;
      get(name: "fullname"): Xrm.StringControl | null;
      get(name: "jobtitle"): Xrm.StringControl;
      get(name: "ownerid"): Xrm.LookupControl<"systemuser" | "team">;
      get(name: "parentcustomerid"): Xrm.LookupControl<"account" | "contact">;
      get(name: "telephone1"): Xrm.StringControl;
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
  interface ContactReferencePanel extends Xrm.QuickViewForm<ContactReferencePanel.Tabs,ContactReferencePanel.Controls> {
    getAttribute(attributeName: "address1_city"): Xrm.Attribute<string>;
    getAttribute(attributeName: "address1_line1"): Xrm.Attribute<string>;
    getAttribute(attributeName: "address1_line2"): Xrm.Attribute<string>;
    getAttribute(attributeName: "address1_postalcode"): Xrm.Attribute<string>;
    getAttribute(attributeName: "emailaddress1"): Xrm.Attribute<string>;
    getAttribute(attributeName: "fullname"): Xrm.Attribute<string> | null;
    getAttribute(attributeName: "jobtitle"): Xrm.Attribute<string>;
    getAttribute(attributeName: "ownerid"): Xrm.LookupAttribute<"systemuser" | "team">;
    getAttribute(attributeName: "parentcustomerid"): Xrm.LookupAttribute<"account" | "contact">;
    getAttribute(attributeName: "telephone1"): Xrm.Attribute<string>;
    getAttribute(attributeName: string): undefined;
    getAttribute(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Attribute<any>>): Xrm.Attribute<any>[];
    getControl(controlName: "address1_city"): Xrm.StringControl;
    getControl(controlName: "address1_line1"): Xrm.StringControl;
    getControl(controlName: "address1_line2"): Xrm.StringControl;
    getControl(controlName: "address1_postalcode"): Xrm.StringControl;
    getControl(controlName: "emailaddress1"): Xrm.StringControl;
    getControl(controlName: "fullname"): Xrm.StringControl | null;
    getControl(controlName: "jobtitle"): Xrm.StringControl;
    getControl(controlName: "ownerid"): Xrm.LookupControl<"systemuser" | "team">;
    getControl(controlName: "parentcustomerid"): Xrm.LookupControl<"account" | "contact">;
    getControl(controlName: "telephone1"): Xrm.StringControl;
    getControl(controlName: string): undefined;
    getControl(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Control<any>>): Xrm.Control<any>[];
  }
}
