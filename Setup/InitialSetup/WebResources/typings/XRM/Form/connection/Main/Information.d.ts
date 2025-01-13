declare namespace Form.connection.Main {
  namespace Information {
    namespace Tabs {
      interface details extends Xrm.SectionCollectionBase {
        get(name: "connect_from"): Xrm.PageSection;
        get(name: "details"): Xrm.PageSection;
        get(name: string): undefined;
        get(): Xrm.PageSection[];
        get(index: number): Xrm.PageSection;
        get(chooser: (item: Xrm.PageSection, index: number) => boolean): Xrm.PageSection[];
      }
      interface info extends Xrm.SectionCollectionBase {
        get(name: "description"): Xrm.PageSection;
        get(name: "info_s"): Xrm.PageSection;
        get(name: string): undefined;
        get(): Xrm.PageSection[];
        get(index: number): Xrm.PageSection;
        get(chooser: (item: Xrm.PageSection, index: number) => boolean): Xrm.PageSection[];
      }
    }
    interface Attributes extends Xrm.AttributeCollectionBase {
      get(name: "description"): Xrm.Attribute<string>;
      get(name: "effectiveend"): Xrm.DateAttribute;
      get(name: "effectivestart"): Xrm.DateAttribute;
      get(name: "ownerid"): Xrm.LookupAttribute<"systemuser" | "team">;
      get(name: "record1id"): Xrm.Attribute<any>;
      get(name: "record1roleid"): Xrm.LookupAttribute<"connectionrole">;
      get(name: "record2id"): Xrm.Attribute<any>;
      get(name: "record2roleid"): Xrm.LookupAttribute<"connectionrole">;
      get(name: "statecode"): Xrm.OptionSetAttribute<connection_statecode>;
      get(name: string): undefined;
      get(): Xrm.Attribute<any>[];
      get(index: number): Xrm.Attribute<any>;
      get(chooser: (item: Xrm.Attribute<any>, index: number) => boolean): Xrm.Attribute<any>[];
    }
    interface Controls extends Xrm.ControlCollectionBase {
      get(name: "description"): Xrm.StringControl;
      get(name: "effectiveend"): Xrm.DateControl;
      get(name: "effectivestart"): Xrm.DateControl;
      get(name: "footer_statecode"): Xrm.OptionSetControl<connection_statecode>;
      get(name: "header_record1id"): Xrm.Control<Xrm.Attribute<any>>;
      get(name: "ownerid"): Xrm.LookupControl<"systemuser" | "team">;
      get(name: "record1id"): Xrm.Control<Xrm.Attribute<any>>;
      get(name: "record1roleid"): Xrm.LookupControl<"connectionrole">;
      get(name: "record2id"): Xrm.Control<Xrm.Attribute<any>>;
      get(name: "record2roleid"): Xrm.LookupControl<"connectionrole">;
      get(name: string): undefined;
      get(): Xrm.BaseControl[];
      get(index: number): Xrm.BaseControl;
      get(chooser: (item: Xrm.BaseControl, index: number) => boolean): Xrm.BaseControl[];
    }
    interface Tabs extends Xrm.TabCollectionBase {
      get(name: "details"): Xrm.PageTab<Tabs.details>;
      get(name: "info"): Xrm.PageTab<Tabs.info>;
      get(name: string): undefined;
      get(): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
      get(index: number): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>;
      get(chooser: (item: Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>, index: number) => boolean): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
    }
  }
  interface Information extends Xrm.PageBase<Information.Attributes,Information.Tabs,Information.Controls> {
    getAttribute(attributeName: "description"): Xrm.Attribute<string>;
    getAttribute(attributeName: "effectiveend"): Xrm.DateAttribute;
    getAttribute(attributeName: "effectivestart"): Xrm.DateAttribute;
    getAttribute(attributeName: "ownerid"): Xrm.LookupAttribute<"systemuser" | "team">;
    getAttribute(attributeName: "record1id"): Xrm.Attribute<any>;
    getAttribute(attributeName: "record1roleid"): Xrm.LookupAttribute<"connectionrole">;
    getAttribute(attributeName: "record2id"): Xrm.Attribute<any>;
    getAttribute(attributeName: "record2roleid"): Xrm.LookupAttribute<"connectionrole">;
    getAttribute(attributeName: "statecode"): Xrm.OptionSetAttribute<connection_statecode>;
    getAttribute(attributeName: string): undefined;
    getControl(controlName: "description"): Xrm.StringControl;
    getControl(controlName: "effectiveend"): Xrm.DateControl;
    getControl(controlName: "effectivestart"): Xrm.DateControl;
    getControl(controlName: "footer_statecode"): Xrm.OptionSetControl<connection_statecode>;
    getControl(controlName: "header_record1id"): Xrm.Control<Xrm.Attribute<any>>;
    getControl(controlName: "ownerid"): Xrm.LookupControl<"systemuser" | "team">;
    getControl(controlName: "record1id"): Xrm.Control<Xrm.Attribute<any>>;
    getControl(controlName: "record1roleid"): Xrm.LookupControl<"connectionrole">;
    getControl(controlName: "record2id"): Xrm.Control<Xrm.Attribute<any>>;
    getControl(controlName: "record2roleid"): Xrm.LookupControl<"connectionrole">;
    getControl(controlName: string): undefined;
  }
}
