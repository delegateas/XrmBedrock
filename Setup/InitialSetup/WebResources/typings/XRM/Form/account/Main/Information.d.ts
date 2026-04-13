declare namespace Form.account.Main {
  namespace Information {
    namespace Tabs {
      interface administration extends Xrm.SectionCollectionBase {
        get(name: "contact methods"): Xrm.PageSection;
        get(name: "internal information"): Xrm.PageSection;
        get(name: string): undefined;
        get(): Xrm.PageSection[];
        get(index: number): Xrm.PageSection;
        get(chooser: (item: Xrm.PageSection, index: number) => boolean): Xrm.PageSection[];
      }
      interface contacts extends Xrm.SectionCollectionBase {
        get(name: "contacts"): Xrm.PageSection;
        get(name: string): undefined;
        get(): Xrm.PageSection[];
        get(index: number): Xrm.PageSection;
        get(chooser: (item: Xrm.PageSection, index: number) => boolean): Xrm.PageSection[];
      }
      interface details extends Xrm.SectionCollectionBase {
        get(name: "billing information"): Xrm.PageSection;
        get(name: "description_2"): Xrm.PageSection;
        get(name: "professional information"): Xrm.PageSection;
        get(name: string): undefined;
        get(): Xrm.PageSection[];
        get(index: number): Xrm.PageSection;
        get(chooser: (item: Xrm.PageSection, index: number) => boolean): Xrm.PageSection[];
      }
      interface general extends Xrm.SectionCollectionBase {
        get(name: "account information"): Xrm.PageSection;
        get(name: "address"): Xrm.PageSection;
        get(name: "description"): Xrm.PageSection;
        get(name: "shipping information"): Xrm.PageSection;
        get(name: string): undefined;
        get(): Xrm.PageSection[];
        get(index: number): Xrm.PageSection;
        get(chooser: (item: Xrm.PageSection, index: number) => boolean): Xrm.PageSection[];
      }
      interface notesandactivities extends Xrm.SectionCollectionBase {
        get(name: "activities"): Xrm.PageSection;
        get(name: "notes"): Xrm.PageSection;
        get(name: string): undefined;
        get(): Xrm.PageSection[];
        get(index: number): Xrm.PageSection;
        get(chooser: (item: Xrm.PageSection, index: number) => boolean): Xrm.PageSection[];
      }
    }
    interface Attributes extends Xrm.AttributeCollectionBase {
      get(name: "accountcategorycode"): Xrm.OptionSetAttribute<account_accountcategorycode>;
      get(name: "accountnumber"): Xrm.Attribute<string>;
      get(name: "address1_addresstypecode"): Xrm.OptionSetAttribute<account_address1_addresstypecode>;
      get(name: "address1_city"): Xrm.Attribute<string>;
      get(name: "address1_country"): Xrm.Attribute<string>;
      get(name: "address1_freighttermscode"): Xrm.OptionSetAttribute<account_address1_freighttermscode>;
      get(name: "address1_line1"): Xrm.Attribute<string>;
      get(name: "address1_line2"): Xrm.Attribute<string>;
      get(name: "address1_line3"): Xrm.Attribute<string>;
      get(name: "address1_name"): Xrm.Attribute<string>;
      get(name: "address1_postalcode"): Xrm.Attribute<string>;
      get(name: "address1_shippingmethodcode"): Xrm.OptionSetAttribute<account_address1_shippingmethodcode>;
      get(name: "address1_stateorprovince"): Xrm.Attribute<string>;
      get(name: "address1_telephone1"): Xrm.Attribute<string>;
      get(name: "creditlimit"): Xrm.NumberAttribute;
      get(name: "creditonhold"): Xrm.OptionSetAttribute<boolean>;
      get(name: "customertypecode"): Xrm.OptionSetAttribute<account_customertypecode>;
      get(name: "description"): Xrm.Attribute<string>;
      get(name: "donotbulkemail"): Xrm.OptionSetAttribute<boolean>;
      get(name: "donotemail"): Xrm.OptionSetAttribute<boolean>;
      get(name: "donotfax"): Xrm.OptionSetAttribute<boolean>;
      get(name: "donotphone"): Xrm.OptionSetAttribute<boolean>;
      get(name: "donotpostalmail"): Xrm.OptionSetAttribute<boolean>;
      get(name: "emailaddress1"): Xrm.Attribute<string>;
      get(name: "fax"): Xrm.Attribute<string>;
      get(name: "followemail"): Xrm.OptionSetAttribute<boolean>;
      get(name: "industrycode"): Xrm.OptionSetAttribute<account_industrycode>;
      get(name: "name"): Xrm.Attribute<string>;
      get(name: "numberofemployees"): Xrm.NumberAttribute;
      get(name: "ownerid"): Xrm.LookupAttribute<"systemuser" | "team">;
      get(name: "ownershipcode"): Xrm.OptionSetAttribute<account_ownershipcode>;
      get(name: "parentaccountid"): Xrm.LookupAttribute<"account">;
      get(name: "paymenttermscode"): Xrm.OptionSetAttribute<account_paymenttermscode>;
      get(name: "preferredcontactmethodcode"): Xrm.OptionSetAttribute<account_preferredcontactmethodcode>;
      get(name: "primarycontactid"): Xrm.LookupAttribute<"contact">;
      get(name: "revenue"): Xrm.NumberAttribute;
      get(name: "sic"): Xrm.Attribute<string>;
      get(name: "telephone1"): Xrm.Attribute<string>;
      get(name: "telephone2"): Xrm.Attribute<string>;
      get(name: "tickersymbol"): Xrm.Attribute<string>;
      get(name: "transactioncurrencyid"): Xrm.LookupAttribute<"transactioncurrency">;
      get(name: "websiteurl"): Xrm.Attribute<string>;
      get(name: string): undefined;
      get(): Xrm.Attribute<any>[];
      get(index: number): Xrm.Attribute<any>;
      get(chooser: (item: Xrm.Attribute<any>, index: number) => boolean): Xrm.Attribute<any>[];
    }
    interface Controls extends Xrm.ControlCollectionBase {
      get(name: "accountContactsGrid"): Xrm.SubGridControl<"contact">;
      get(name: "accountactivitiesgrid"): Xrm.SubGridControl<"activitypointer">;
      get(name: "accountcategorycode"): Xrm.OptionSetControl<account_accountcategorycode>;
      get(name: "accountnumber"): Xrm.StringControl;
      get(name: "address1_addresstypecode"): Xrm.OptionSetControl<account_address1_addresstypecode>;
      get(name: "address1_city"): Xrm.StringControl;
      get(name: "address1_country"): Xrm.StringControl;
      get(name: "address1_freighttermscode"): Xrm.OptionSetControl<account_address1_freighttermscode>;
      get(name: "address1_line1"): Xrm.StringControl;
      get(name: "address1_line2"): Xrm.StringControl;
      get(name: "address1_line3"): Xrm.StringControl;
      get(name: "address1_name"): Xrm.StringControl;
      get(name: "address1_postalcode"): Xrm.StringControl;
      get(name: "address1_shippingmethodcode"): Xrm.OptionSetControl<account_address1_shippingmethodcode>;
      get(name: "address1_stateorprovince"): Xrm.StringControl;
      get(name: "address1_telephone1"): Xrm.StringControl;
      get(name: "creditlimit"): Xrm.NumberControl;
      get(name: "creditonhold"): Xrm.OptionSetControl<boolean>;
      get(name: "customertypecode"): Xrm.OptionSetControl<account_customertypecode>;
      get(name: "description"): Xrm.StringControl;
      get(name: "donotbulkemail"): Xrm.OptionSetControl<boolean>;
      get(name: "donotemail"): Xrm.OptionSetControl<boolean>;
      get(name: "donotfax"): Xrm.OptionSetControl<boolean>;
      get(name: "donotphone"): Xrm.OptionSetControl<boolean>;
      get(name: "donotpostalmail"): Xrm.OptionSetControl<boolean>;
      get(name: "emailaddress1"): Xrm.StringControl;
      get(name: "fax"): Xrm.StringControl;
      get(name: "followemail"): Xrm.OptionSetControl<boolean>;
      get(name: "header_creditlimit"): Xrm.NumberControl;
      get(name: "header_ownerid"): Xrm.LookupControl<"systemuser" | "team">;
      get(name: "header_preferredcontactmethodcode"): Xrm.OptionSetControl<account_preferredcontactmethodcode>;
      get(name: "header_primarycontactid"): Xrm.LookupControl<"contact">;
      get(name: "header_revenue"): Xrm.NumberControl;
      get(name: "industrycode"): Xrm.OptionSetControl<account_industrycode>;
      get(name: "name"): Xrm.StringControl;
      get(name: "notescontrol"): Xrm.BaseControl;
      get(name: "numberofemployees"): Xrm.NumberControl;
      get(name: "ownerid"): Xrm.LookupControl<"systemuser" | "team">;
      get(name: "ownershipcode"): Xrm.OptionSetControl<account_ownershipcode>;
      get(name: "parentaccountid"): Xrm.LookupControl<"account">;
      get(name: "paymenttermscode"): Xrm.OptionSetControl<account_paymenttermscode>;
      get(name: "preferredcontactmethodcode"): Xrm.OptionSetControl<account_preferredcontactmethodcode>;
      get(name: "primarycontactid"): Xrm.LookupControl<"contact">;
      get(name: "revenue"): Xrm.NumberControl;
      get(name: "sic"): Xrm.StringControl;
      get(name: "telephone1"): Xrm.StringControl;
      get(name: "telephone2"): Xrm.StringControl;
      get(name: "tickersymbol"): Xrm.StringControl;
      get(name: "transactioncurrencyid"): Xrm.LookupControl<"transactioncurrency">;
      get(name: "websiteurl"): Xrm.StringControl;
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
      get(name: "administration"): Xrm.PageTab<Tabs.administration>;
      get(name: "contacts"): Xrm.PageTab<Tabs.contacts>;
      get(name: "details"): Xrm.PageTab<Tabs.details>;
      get(name: "general"): Xrm.PageTab<Tabs.general>;
      get(name: "notes and activities"): Xrm.PageTab<Tabs.notesandactivities>;
      get(name: string): undefined;
      get(): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
      get(index: number): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>;
      get(chooser: (item: Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>, index: number) => boolean): Xrm.PageTab<Xrm.Collection<Xrm.PageSection>>[];
    }
  }
  interface Information extends Xrm.PageBase<Information.Attributes,Information.Tabs,Information.Controls,Information.QuickViewForms> {
    getAttribute(attributeName: "accountcategorycode"): Xrm.OptionSetAttribute<account_accountcategorycode>;
    getAttribute(attributeName: "accountnumber"): Xrm.Attribute<string>;
    getAttribute(attributeName: "address1_addresstypecode"): Xrm.OptionSetAttribute<account_address1_addresstypecode>;
    getAttribute(attributeName: "address1_city"): Xrm.Attribute<string>;
    getAttribute(attributeName: "address1_country"): Xrm.Attribute<string>;
    getAttribute(attributeName: "address1_freighttermscode"): Xrm.OptionSetAttribute<account_address1_freighttermscode>;
    getAttribute(attributeName: "address1_line1"): Xrm.Attribute<string>;
    getAttribute(attributeName: "address1_line2"): Xrm.Attribute<string>;
    getAttribute(attributeName: "address1_line3"): Xrm.Attribute<string>;
    getAttribute(attributeName: "address1_name"): Xrm.Attribute<string>;
    getAttribute(attributeName: "address1_postalcode"): Xrm.Attribute<string>;
    getAttribute(attributeName: "address1_shippingmethodcode"): Xrm.OptionSetAttribute<account_address1_shippingmethodcode>;
    getAttribute(attributeName: "address1_stateorprovince"): Xrm.Attribute<string>;
    getAttribute(attributeName: "address1_telephone1"): Xrm.Attribute<string>;
    getAttribute(attributeName: "creditlimit"): Xrm.NumberAttribute;
    getAttribute(attributeName: "creditonhold"): Xrm.OptionSetAttribute<boolean>;
    getAttribute(attributeName: "customertypecode"): Xrm.OptionSetAttribute<account_customertypecode>;
    getAttribute(attributeName: "description"): Xrm.Attribute<string>;
    getAttribute(attributeName: "donotbulkemail"): Xrm.OptionSetAttribute<boolean>;
    getAttribute(attributeName: "donotemail"): Xrm.OptionSetAttribute<boolean>;
    getAttribute(attributeName: "donotfax"): Xrm.OptionSetAttribute<boolean>;
    getAttribute(attributeName: "donotphone"): Xrm.OptionSetAttribute<boolean>;
    getAttribute(attributeName: "donotpostalmail"): Xrm.OptionSetAttribute<boolean>;
    getAttribute(attributeName: "emailaddress1"): Xrm.Attribute<string>;
    getAttribute(attributeName: "fax"): Xrm.Attribute<string>;
    getAttribute(attributeName: "followemail"): Xrm.OptionSetAttribute<boolean>;
    getAttribute(attributeName: "industrycode"): Xrm.OptionSetAttribute<account_industrycode>;
    getAttribute(attributeName: "name"): Xrm.Attribute<string>;
    getAttribute(attributeName: "numberofemployees"): Xrm.NumberAttribute;
    getAttribute(attributeName: "ownerid"): Xrm.LookupAttribute<"systemuser" | "team">;
    getAttribute(attributeName: "ownershipcode"): Xrm.OptionSetAttribute<account_ownershipcode>;
    getAttribute(attributeName: "parentaccountid"): Xrm.LookupAttribute<"account">;
    getAttribute(attributeName: "paymenttermscode"): Xrm.OptionSetAttribute<account_paymenttermscode>;
    getAttribute(attributeName: "preferredcontactmethodcode"): Xrm.OptionSetAttribute<account_preferredcontactmethodcode>;
    getAttribute(attributeName: "primarycontactid"): Xrm.LookupAttribute<"contact">;
    getAttribute(attributeName: "revenue"): Xrm.NumberAttribute;
    getAttribute(attributeName: "sic"): Xrm.Attribute<string>;
    getAttribute(attributeName: "telephone1"): Xrm.Attribute<string>;
    getAttribute(attributeName: "telephone2"): Xrm.Attribute<string>;
    getAttribute(attributeName: "tickersymbol"): Xrm.Attribute<string>;
    getAttribute(attributeName: "transactioncurrencyid"): Xrm.LookupAttribute<"transactioncurrency">;
    getAttribute(attributeName: "websiteurl"): Xrm.Attribute<string>;
    getAttribute(attributeName: string): undefined;
    getAttribute(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Attribute<any>>): Xrm.Attribute<any>[];
    getControl(controlName: "accountContactsGrid"): Xrm.SubGridControl<"contact">;
    getControl(controlName: "accountactivitiesgrid"): Xrm.SubGridControl<"activitypointer">;
    getControl(controlName: "accountcategorycode"): Xrm.OptionSetControl<account_accountcategorycode>;
    getControl(controlName: "accountnumber"): Xrm.StringControl;
    getControl(controlName: "address1_addresstypecode"): Xrm.OptionSetControl<account_address1_addresstypecode>;
    getControl(controlName: "address1_city"): Xrm.StringControl;
    getControl(controlName: "address1_country"): Xrm.StringControl;
    getControl(controlName: "address1_freighttermscode"): Xrm.OptionSetControl<account_address1_freighttermscode>;
    getControl(controlName: "address1_line1"): Xrm.StringControl;
    getControl(controlName: "address1_line2"): Xrm.StringControl;
    getControl(controlName: "address1_line3"): Xrm.StringControl;
    getControl(controlName: "address1_name"): Xrm.StringControl;
    getControl(controlName: "address1_postalcode"): Xrm.StringControl;
    getControl(controlName: "address1_shippingmethodcode"): Xrm.OptionSetControl<account_address1_shippingmethodcode>;
    getControl(controlName: "address1_stateorprovince"): Xrm.StringControl;
    getControl(controlName: "address1_telephone1"): Xrm.StringControl;
    getControl(controlName: "creditlimit"): Xrm.NumberControl;
    getControl(controlName: "creditonhold"): Xrm.OptionSetControl<boolean>;
    getControl(controlName: "customertypecode"): Xrm.OptionSetControl<account_customertypecode>;
    getControl(controlName: "description"): Xrm.StringControl;
    getControl(controlName: "donotbulkemail"): Xrm.OptionSetControl<boolean>;
    getControl(controlName: "donotemail"): Xrm.OptionSetControl<boolean>;
    getControl(controlName: "donotfax"): Xrm.OptionSetControl<boolean>;
    getControl(controlName: "donotphone"): Xrm.OptionSetControl<boolean>;
    getControl(controlName: "donotpostalmail"): Xrm.OptionSetControl<boolean>;
    getControl(controlName: "emailaddress1"): Xrm.StringControl;
    getControl(controlName: "fax"): Xrm.StringControl;
    getControl(controlName: "followemail"): Xrm.OptionSetControl<boolean>;
    getControl(controlName: "header_creditlimit"): Xrm.NumberControl;
    getControl(controlName: "header_ownerid"): Xrm.LookupControl<"systemuser" | "team">;
    getControl(controlName: "header_preferredcontactmethodcode"): Xrm.OptionSetControl<account_preferredcontactmethodcode>;
    getControl(controlName: "header_primarycontactid"): Xrm.LookupControl<"contact">;
    getControl(controlName: "header_revenue"): Xrm.NumberControl;
    getControl(controlName: "industrycode"): Xrm.OptionSetControl<account_industrycode>;
    getControl(controlName: "name"): Xrm.StringControl;
    getControl(controlName: "notescontrol"): Xrm.BaseControl;
    getControl(controlName: "numberofemployees"): Xrm.NumberControl;
    getControl(controlName: "ownerid"): Xrm.LookupControl<"systemuser" | "team">;
    getControl(controlName: "ownershipcode"): Xrm.OptionSetControl<account_ownershipcode>;
    getControl(controlName: "parentaccountid"): Xrm.LookupControl<"account">;
    getControl(controlName: "paymenttermscode"): Xrm.OptionSetControl<account_paymenttermscode>;
    getControl(controlName: "preferredcontactmethodcode"): Xrm.OptionSetControl<account_preferredcontactmethodcode>;
    getControl(controlName: "primarycontactid"): Xrm.LookupControl<"contact">;
    getControl(controlName: "revenue"): Xrm.NumberControl;
    getControl(controlName: "sic"): Xrm.StringControl;
    getControl(controlName: "telephone1"): Xrm.StringControl;
    getControl(controlName: "telephone2"): Xrm.StringControl;
    getControl(controlName: "tickersymbol"): Xrm.StringControl;
    getControl(controlName: "transactioncurrencyid"): Xrm.LookupControl<"transactioncurrency">;
    getControl(controlName: "websiteurl"): Xrm.StringControl;
    getControl(controlName: string): undefined;
    getControl(delegateFunction: Xrm.Collection.MatchingDelegate<Xrm.Control<any>>): Xrm.Control<any>[];
  }
}
