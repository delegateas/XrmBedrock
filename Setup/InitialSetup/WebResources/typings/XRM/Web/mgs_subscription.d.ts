declare namespace XDT {
  interface mgs_Subscription_Base extends WebEntity {
    createdon?: Date | null;
    importsequencenumber?: number | null;
    mgs_enddate?: Date | null;
    mgs_invoiceduntil?: Date | null;
    mgs_startdate?: Date | null;
    mgs_subscriptionid?: string | null;
    mgs_subscriptionname?: string | null;
    modifiedon?: Date | null;
    overriddencreatedon?: Date | null;
    statecode?: mgs_subscription_statecode | null;
    statuscode?: mgs_subscription_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
    versionnumber?: number | null;
  }
  interface mgs_Subscription_Relationships {
    mgs_Contact?: Contact_Result | null;
    mgs_Product?: mgs_Product_Result | null;
    mgs_transaction_Subscription_mgs_subscription?: mgs_Transaction_Result[] | null;
  }
  interface mgs_Subscription extends mgs_Subscription_Base, mgs_Subscription_Relationships {
    mgs_Contact_bind$contacts?: string | null;
    mgs_Product_bind$mgs_products?: string | null;
    ownerid_bind$systemusers?: string | null;
    ownerid_bind$teams?: string | null;
  }
  interface mgs_Subscription_Create extends mgs_Subscription {
  }
  interface mgs_Subscription_Update extends mgs_Subscription {
  }
  interface mgs_Subscription_Select {
    createdby_guid: WebAttribute<mgs_Subscription_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<mgs_Subscription_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<mgs_Subscription_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    importsequencenumber: WebAttribute<mgs_Subscription_Select, { importsequencenumber: number | null }, {  }>;
    mgs_contact_guid: WebAttribute<mgs_Subscription_Select, { mgs_contact_guid: string | null }, { mgs_contact_formatted?: string }>;
    mgs_enddate: WebAttribute<mgs_Subscription_Select, { mgs_enddate: Date | null }, { mgs_enddate_formatted?: string }>;
    mgs_invoiceduntil: WebAttribute<mgs_Subscription_Select, { mgs_invoiceduntil: Date | null }, { mgs_invoiceduntil_formatted?: string }>;
    mgs_product_guid: WebAttribute<mgs_Subscription_Select, { mgs_product_guid: string | null }, { mgs_product_formatted?: string }>;
    mgs_startdate: WebAttribute<mgs_Subscription_Select, { mgs_startdate: Date | null }, { mgs_startdate_formatted?: string }>;
    mgs_subscriptionid: WebAttribute<mgs_Subscription_Select, { mgs_subscriptionid: string | null }, {  }>;
    mgs_subscriptionname: WebAttribute<mgs_Subscription_Select, { mgs_subscriptionname: string | null }, {  }>;
    modifiedby_guid: WebAttribute<mgs_Subscription_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<mgs_Subscription_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<mgs_Subscription_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    overriddencreatedon: WebAttribute<mgs_Subscription_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    ownerid_guid: WebAttribute<mgs_Subscription_Select, { ownerid_guid: string | null }, { ownerid_formatted?: string }>;
    owningbusinessunit_guid: WebAttribute<mgs_Subscription_Select, { owningbusinessunit_guid: string | null }, { owningbusinessunit_formatted?: string }>;
    owningteam_guid: WebAttribute<mgs_Subscription_Select, { owningteam_guid: string | null }, { owningteam_formatted?: string }>;
    owninguser_guid: WebAttribute<mgs_Subscription_Select, { owninguser_guid: string | null }, { owninguser_formatted?: string }>;
    statecode: WebAttribute<mgs_Subscription_Select, { statecode: mgs_subscription_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<mgs_Subscription_Select, { statuscode: mgs_subscription_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<mgs_Subscription_Select, { timezoneruleversionnumber: number | null }, {  }>;
    utcconversiontimezonecode: WebAttribute<mgs_Subscription_Select, { utcconversiontimezonecode: number | null }, {  }>;
    versionnumber: WebAttribute<mgs_Subscription_Select, { versionnumber: number | null }, {  }>;
  }
  interface mgs_Subscription_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    importsequencenumber: number;
    mgs_contact_guid: XQW.Guid;
    mgs_enddate: Date;
    mgs_invoiceduntil: Date;
    mgs_product_guid: XQW.Guid;
    mgs_startdate: Date;
    mgs_subscriptionid: XQW.Guid;
    mgs_subscriptionname: string;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    overriddencreatedon: Date;
    ownerid_guid: XQW.Guid;
    owningbusinessunit_guid: XQW.Guid;
    owningteam_guid: XQW.Guid;
    owninguser_guid: XQW.Guid;
    statecode: mgs_subscription_statecode;
    statuscode: mgs_subscription_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface mgs_Subscription_Expand {
    mgs_Contact: WebExpand<mgs_Subscription_Expand, Contact_Select, Contact_Filter, { mgs_Contact: Contact_Result }>;
    mgs_Product: WebExpand<mgs_Subscription_Expand, mgs_Product_Select, mgs_Product_Filter, { mgs_Product: mgs_Product_Result }>;
    mgs_transaction_Subscription_mgs_subscription: WebExpand<mgs_Subscription_Expand, mgs_Transaction_Select, mgs_Transaction_Filter, { mgs_transaction_Subscription_mgs_subscription: mgs_Transaction_Result[] }>;
  }
  interface mgs_Subscription_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    mgs_contact_formatted?: string;
    mgs_enddate_formatted?: string;
    mgs_invoiceduntil_formatted?: string;
    mgs_product_formatted?: string;
    mgs_startdate_formatted?: string;
    modifiedby_formatted?: string;
    modifiedon_formatted?: string;
    modifiedonbehalfby_formatted?: string;
    overriddencreatedon_formatted?: string;
    ownerid_formatted?: string;
    owningbusinessunit_formatted?: string;
    owningteam_formatted?: string;
    owninguser_formatted?: string;
    statecode_formatted?: string;
    statuscode_formatted?: string;
  }
  interface mgs_Subscription_Result extends mgs_Subscription_Base, mgs_Subscription_Relationships {
    "@odata.etag": string;
    createdby_guid: string | null;
    createdonbehalfby_guid: string | null;
    mgs_contact_guid: string | null;
    mgs_product_guid: string | null;
    modifiedby_guid: string | null;
    modifiedonbehalfby_guid: string | null;
    ownerid_guid: string | null;
    owningbusinessunit_guid: string | null;
    owningteam_guid: string | null;
    owninguser_guid: string | null;
  }
  interface mgs_Subscription_RelatedOne {
    mgs_Contact: WebMappingRetrieve<XDT.Contact_Select,XDT.Contact_Expand,XDT.Contact_Filter,XDT.Contact_Fixed,XDT.Contact_Result,XDT.Contact_FormattedResult>;
    mgs_Product: WebMappingRetrieve<XDT.mgs_Product_Select,XDT.mgs_Product_Expand,XDT.mgs_Product_Filter,XDT.mgs_Product_Fixed,XDT.mgs_Product_Result,XDT.mgs_Product_FormattedResult>;
  }
  interface mgs_Subscription_RelatedMany {
    mgs_transaction_Subscription_mgs_subscription: WebMappingRetrieve<XDT.mgs_Transaction_Select,XDT.mgs_Transaction_Expand,XDT.mgs_Transaction_Filter,XDT.mgs_Transaction_Fixed,XDT.mgs_Transaction_Result,XDT.mgs_Transaction_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  mgs_subscriptions: WebMappingRetrieve<XDT.mgs_Subscription_Select,XDT.mgs_Subscription_Expand,XDT.mgs_Subscription_Filter,XDT.mgs_Subscription_Fixed,XDT.mgs_Subscription_Result,XDT.mgs_Subscription_FormattedResult>;
}
interface WebEntitiesRelated {
  mgs_subscriptions: WebMappingRelated<XDT.mgs_Subscription_RelatedOne,XDT.mgs_Subscription_RelatedMany>;
}
interface WebEntitiesCUDA {
  mgs_subscriptions: WebMappingCUDA<XDT.mgs_Subscription_Create,XDT.mgs_Subscription_Update,XDT.mgs_Subscription_Select>;
}
