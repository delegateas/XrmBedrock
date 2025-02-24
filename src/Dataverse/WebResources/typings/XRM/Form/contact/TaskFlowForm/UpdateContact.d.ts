declare namespace Form.contact.TaskFlowForm {
  namespace UpdateContact {
    namespace Tabs {
      interface step_2 extends Xrm.SectionCollectionBase {
        get(name: "step_2_section1"): Xrm.PageSection;
        get(name: "step_2_section2"): Xrm.PageSection;
        get(name: "step_2_section2_2"): Xrm.PageSection;
        get(name: string): undefined;
        get(): Xrm.PageSection[];
        get(index: number): Xrm.PageSection;
        get(chooser: (item: Xrm.PageSection, index: number) => boolean): Xrm.PageSection[];
      }
      interface step_25 extends Xrm.SectionCollectionBase {
        get(name: "step_25_section1"): Xrm.PageSection;
        get(name: "step_25_section1_3"): Xrm.PageSection;
        get(name: string): undefined;
        get(): Xrm.PageSection[];
        get(index: number): Xrm.PageSection;
        get(chooser: (item: Xrm.PageSection, index: number) => boolean): Xrm.PageSection[];
      }
    }
    interface Attributes extends Xrm.AttributeCollectionBase {
      get(name: "043dc848-5c9b-b5c1-9417-c4c55740395d_back"): Xrm.Attribute<any>;
      get(name: "043dc848-5c9b-b5c1-9417-c4c55740395d_next"): Xrm.Attribute<any>;
      get(name: "birthdate"): Xrm.DateAttribute;
      get(name: "c610debb-05fc-462e-9ec5-8ed646b3e585_next"): Xrm.Attribute<any>;
      get(name: "emailaddress1"): Xrm.Attribute<string>;
      get(name: "familystatuscode"): Xrm.OptionSetAttribute<contact_familystatuscode>;
      get(name: "firstname"): Xrm.Attribute<string>;
      get(name: "industrycode"): Xrm.OptionSetAttribute<number>;
      get(name: "lastname"): Xrm.Attribute<string>;
      get(name: "middlename"): Xrm.Attribute<string>;
      get(name: "mobilephone"): Xrm.Attribute<string>;
      get(name: "name"): Xrm.Attribute<string>;
      get(name: "parentaccountid"): Xrm.LookupAttribute<"accounts">;
      get(name: "spousesname"): Xrm.Attribute<string>;
      get(name: "telephone1"): Xrm.Attribute<string>;
      get(name: "websiteurl"): Xrm.Attribute<string>;
      get(name: string): undefined;
      get(): Xrm.Attribute<any>[];
      get(index: number): Xrm.Attribute<any>;
      get(chooser: (item: Xrm.Attribute<any>, index: number) => boolean): Xrm.Attribute<any>[];
    }
    interface Controls extends Xrm.ControlCollectionBase {
      get(name: "043dc848-5c9b-b5c1-9417-c4c55740395d_back"): Xrm.Control<Xrm.Attribute<any>>;
      get(name: "043dc848-5c9b-b5c1-9417-c4c55740395d_next"): Xrm.Control<Xrm.Attribute<any>>;
      get(name: "birthdate"): Xrm.DateControl;
      get(name: "c610debb-05fc-462e-9ec5-8ed646b3e585_next"): Xrm.Control<Xrm.Attribute<any>>;
      get(name: "contact_customer_accounts:industrycode"): Xrm.OptionSetControl<number>;
      get(name: "contact_customer_accounts:name"): Xrm.StringControl;
      get(name: "contact_customer_accounts:parentaccountid"): Xrm.LookupControl<"accounts">;
      get(name: "contact_customer_accounts:telephone1"): Xrm.StringControl;
      get(name: "contact_customer_accounts:websiteurl"): Xrm.StringControl;
      get(name: "emailaddress1"): Xrm.StringControl;
      get(name: "familystatuscode"): Xrm.OptionSetControl<contact_familystatuscode>;
      get(name: "firstname"): Xrm.StringControl;
      get(name: "header_process_birthdate"): Xrm.DateControl | null;
      get(name: "header_process_emailaddress1"): Xrm.StringControl | null;
      get(name: "header_process_familystatuscode"): Xrm.OptionSetControl<contact_familystatuscode> | null;
      get(name: "header_process_firstname"): Xrm.StringControl | null;
      get(name: "header_process_industrycode"): Xrm.OptionSetControl<number> | null;
      get(name: "header_process_lastname"): Xrm.StringControl | null;
      get(name: "header_process_middlename"): Xrm.StringControl | null;
      get(name: "header_process_mobilephone"): Xrm.StringControl | null;
      get(name: "header_process_name"): Xrm.StringControl | null;
      get(name: "header_process_parentaccountid"): Xrm.LookupControl<"account"> | null;
      get(name: "header_process_spousesname"): Xrm.StringControl | null;
      get(name: "header_process_telephone1"): Xrm.StringControl | null;
      get(name: "header_process_websiteurl"): Xrm.StringControl | null;
      get(name: "lastname"): Xrm.StringControl;
      get(name: "middlename"): Xrm.StringControl;
      get(name: "mobilephone"): Xrm.StringControl;
      get(name: "spousesname"): Xrm.StringControl;
      get(name: string): undefined;
      get(): Xrm.BaseControl[];
      get(index: number): Xrm.BaseControl;
      get(chooser: (item: Xrm.BaseControl, index: number) => boolean): Xrm.BaseControl[];
    }
    interface Tabs extends Xrm.TabCollectionBase {
      get(name: "step_2"): Xrm.PageTab<Tabs.step_2>;
      get(name: "step_25"): Xrm.PageTab<Tabs.step_25>;
      get(name: string): undefined;
      get(): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
      get(index: number): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>;
      get(chooser: (item: Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>, index: number) => boolean): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
    }
  }
  interface UpdateContact extends Xrm.PageBase<UpdateContact.Attributes,UpdateContact.Tabs,UpdateContact.Controls> {
    getAttribute(attributeName: "043dc848-5c9b-b5c1-9417-c4c55740395d_back"): Xrm.Attribute<any>;
    getAttribute(attributeName: "043dc848-5c9b-b5c1-9417-c4c55740395d_next"): Xrm.Attribute<any>;
    getAttribute(attributeName: "birthdate"): Xrm.DateAttribute;
    getAttribute(attributeName: "c610debb-05fc-462e-9ec5-8ed646b3e585_next"): Xrm.Attribute<any>;
    getAttribute(attributeName: "emailaddress1"): Xrm.Attribute<string>;
    getAttribute(attributeName: "familystatuscode"): Xrm.OptionSetAttribute<contact_familystatuscode>;
    getAttribute(attributeName: "firstname"): Xrm.Attribute<string>;
    getAttribute(attributeName: "industrycode"): Xrm.OptionSetAttribute<number>;
    getAttribute(attributeName: "lastname"): Xrm.Attribute<string>;
    getAttribute(attributeName: "middlename"): Xrm.Attribute<string>;
    getAttribute(attributeName: "mobilephone"): Xrm.Attribute<string>;
    getAttribute(attributeName: "name"): Xrm.Attribute<string>;
    getAttribute(attributeName: "parentaccountid"): Xrm.LookupAttribute<"accounts">;
    getAttribute(attributeName: "spousesname"): Xrm.Attribute<string>;
    getAttribute(attributeName: "telephone1"): Xrm.Attribute<string>;
    getAttribute(attributeName: "websiteurl"): Xrm.Attribute<string>;
    getAttribute(attributeName: string): undefined;
    getControl(controlName: "043dc848-5c9b-b5c1-9417-c4c55740395d_back"): Xrm.Control<Xrm.Attribute<any>>;
    getControl(controlName: "043dc848-5c9b-b5c1-9417-c4c55740395d_next"): Xrm.Control<Xrm.Attribute<any>>;
    getControl(controlName: "birthdate"): Xrm.DateControl;
    getControl(controlName: "c610debb-05fc-462e-9ec5-8ed646b3e585_next"): Xrm.Control<Xrm.Attribute<any>>;
    getControl(controlName: "contact_customer_accounts:industrycode"): Xrm.OptionSetControl<number>;
    getControl(controlName: "contact_customer_accounts:name"): Xrm.StringControl;
    getControl(controlName: "contact_customer_accounts:parentaccountid"): Xrm.LookupControl<"accounts">;
    getControl(controlName: "contact_customer_accounts:telephone1"): Xrm.StringControl;
    getControl(controlName: "contact_customer_accounts:websiteurl"): Xrm.StringControl;
    getControl(controlName: "emailaddress1"): Xrm.StringControl;
    getControl(controlName: "familystatuscode"): Xrm.OptionSetControl<contact_familystatuscode>;
    getControl(controlName: "firstname"): Xrm.StringControl;
    getControl(controlName: "header_process_birthdate"): Xrm.DateControl | null;
    getControl(controlName: "header_process_emailaddress1"): Xrm.StringControl | null;
    getControl(controlName: "header_process_familystatuscode"): Xrm.OptionSetControl<contact_familystatuscode> | null;
    getControl(controlName: "header_process_firstname"): Xrm.StringControl | null;
    getControl(controlName: "header_process_industrycode"): Xrm.OptionSetControl<number> | null;
    getControl(controlName: "header_process_lastname"): Xrm.StringControl | null;
    getControl(controlName: "header_process_middlename"): Xrm.StringControl | null;
    getControl(controlName: "header_process_mobilephone"): Xrm.StringControl | null;
    getControl(controlName: "header_process_name"): Xrm.StringControl | null;
    getControl(controlName: "header_process_parentaccountid"): Xrm.LookupControl<"account"> | null;
    getControl(controlName: "header_process_spousesname"): Xrm.StringControl | null;
    getControl(controlName: "header_process_telephone1"): Xrm.StringControl | null;
    getControl(controlName: "header_process_websiteurl"): Xrm.StringControl | null;
    getControl(controlName: "lastname"): Xrm.StringControl;
    getControl(controlName: "middlename"): Xrm.StringControl;
    getControl(controlName: "mobilephone"): Xrm.StringControl;
    getControl(controlName: "spousesname"): Xrm.StringControl;
    getControl(controlName: string): undefined;
  }
}
