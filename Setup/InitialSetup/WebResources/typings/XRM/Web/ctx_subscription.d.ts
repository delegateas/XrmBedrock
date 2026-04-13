declare namespace XDT {
  interface ctx_Subscription_Base extends WebEntity {
    createdon?: Date | null;
    ctx_enddate?: Date | null;
    ctx_invoiceduntil?: Date | null;
    ctx_name?: string | null;
    ctx_startdate?: Date | null;
    ctx_subscriptionid?: string | null;
    importsequencenumber?: number | null;
    modifiedon?: Date | null;
    overriddencreatedon?: Date | null;
    statecode?: ctx_subscription_statecode | null;
    statuscode?: ctx_subscription_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
    versionnumber?: number | null;
  }
  interface ctx_Subscription_Relationships {
    ctx_Customer_account?: Account_Result | null;
    ctx_Customer_contact?: Contact_Result | null;
    ctx_Product?: ctx_Product_Result | null;
    ctx_Transaction_ctx_Subscription?: ctx_Transaction_Result[] | null;
  }
  interface ctx_Subscription extends ctx_Subscription_Base, ctx_Subscription_Relationships {
    ctx_Customer_account_bind$accounts?: string | null;
    ctx_Customer_contact_bind$contacts?: string | null;
    ctx_Product_bind$ctx_products?: string | null;
    ownerid_bind$systemusers?: string | null;
    ownerid_bind$teams?: string | null;
  }
  interface ctx_Subscription_Create extends ctx_Subscription {
  }
  interface ctx_Subscription_Update extends ctx_Subscription {
  }
  interface ctx_Subscription_Select {
    createdby_guid: WebAttribute<ctx_Subscription_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<ctx_Subscription_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<ctx_Subscription_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    ctx_customer_guid: WebAttribute<ctx_Subscription_Select, { ctx_customer_guid: string | null }, { ctx_customer_formatted?: string }>;
    ctx_enddate: WebAttribute<ctx_Subscription_Select, { ctx_enddate: Date | null }, { ctx_enddate_formatted?: string }>;
    ctx_invoiceduntil: WebAttribute<ctx_Subscription_Select, { ctx_invoiceduntil: Date | null }, { ctx_invoiceduntil_formatted?: string }>;
    ctx_name: WebAttribute<ctx_Subscription_Select, { ctx_name: string | null }, {  }>;
    ctx_product_guid: WebAttribute<ctx_Subscription_Select, { ctx_product_guid: string | null }, { ctx_product_formatted?: string }>;
    ctx_startdate: WebAttribute<ctx_Subscription_Select, { ctx_startdate: Date | null }, { ctx_startdate_formatted?: string }>;
    ctx_subscriptionid: WebAttribute<ctx_Subscription_Select, { ctx_subscriptionid: string | null }, {  }>;
    importsequencenumber: WebAttribute<ctx_Subscription_Select, { importsequencenumber: number | null }, {  }>;
    modifiedby_guid: WebAttribute<ctx_Subscription_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<ctx_Subscription_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<ctx_Subscription_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    overriddencreatedon: WebAttribute<ctx_Subscription_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    ownerid_guid: WebAttribute<ctx_Subscription_Select, { ownerid_guid: string | null }, { ownerid_formatted?: string }>;
    owningbusinessunit_guid: WebAttribute<ctx_Subscription_Select, { owningbusinessunit_guid: string | null }, { owningbusinessunit_formatted?: string }>;
    owningteam_guid: WebAttribute<ctx_Subscription_Select, { owningteam_guid: string | null }, { owningteam_formatted?: string }>;
    owninguser_guid: WebAttribute<ctx_Subscription_Select, { owninguser_guid: string | null }, { owninguser_formatted?: string }>;
    statecode: WebAttribute<ctx_Subscription_Select, { statecode: ctx_subscription_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<ctx_Subscription_Select, { statuscode: ctx_subscription_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<ctx_Subscription_Select, { timezoneruleversionnumber: number | null }, {  }>;
    utcconversiontimezonecode: WebAttribute<ctx_Subscription_Select, { utcconversiontimezonecode: number | null }, {  }>;
    versionnumber: WebAttribute<ctx_Subscription_Select, { versionnumber: number | null }, {  }>;
  }
  interface ctx_Subscription_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    ctx_customer_guid: XQW.Guid;
    ctx_enddate: Date;
    ctx_invoiceduntil: Date;
    ctx_name: string;
    ctx_product_guid: XQW.Guid;
    ctx_startdate: Date;
    ctx_subscriptionid: XQW.Guid;
    importsequencenumber: number;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    overriddencreatedon: Date;
    ownerid_guid: XQW.Guid;
    owningbusinessunit_guid: XQW.Guid;
    owningteam_guid: XQW.Guid;
    owninguser_guid: XQW.Guid;
    statecode: ctx_subscription_statecode;
    statuscode: ctx_subscription_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface ctx_Subscription_Expand {
    ctx_Customer_account: WebExpand<ctx_Subscription_Expand, Account_Select, Account_Filter, { ctx_Customer_account: Account_Result }>;
    ctx_Customer_contact: WebExpand<ctx_Subscription_Expand, Contact_Select, Contact_Filter, { ctx_Customer_contact: Contact_Result }>;
    ctx_Product: WebExpand<ctx_Subscription_Expand, ctx_Product_Select, ctx_Product_Filter, { ctx_Product: ctx_Product_Result }>;
    ctx_Transaction_ctx_Subscription: WebExpand<ctx_Subscription_Expand, ctx_Transaction_Select, ctx_Transaction_Filter, { ctx_Transaction_ctx_Subscription: ctx_Transaction_Result[] }>;
  }
  interface ctx_Subscription_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    ctx_customer_formatted?: string;
    ctx_enddate_formatted?: string;
    ctx_invoiceduntil_formatted?: string;
    ctx_product_formatted?: string;
    ctx_startdate_formatted?: string;
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
  interface ctx_Subscription_Result extends ctx_Subscription_Base, ctx_Subscription_Relationships {
    "@odata.etag": string;
    createdby_guid: string | null;
    createdonbehalfby_guid: string | null;
    ctx_customer_guid: string | null;
    ctx_product_guid: string | null;
    modifiedby_guid: string | null;
    modifiedonbehalfby_guid: string | null;
    ownerid_guid: string | null;
    owningbusinessunit_guid: string | null;
    owningteam_guid: string | null;
    owninguser_guid: string | null;
  }
  interface ctx_Subscription_RelatedOne {
    ctx_Customer_account: WebMappingRetrieve<XDT.Account_Select,XDT.Account_Expand,XDT.Account_Filter,XDT.Account_Fixed,XDT.Account_Result,XDT.Account_FormattedResult>;
    ctx_Customer_contact: WebMappingRetrieve<XDT.Contact_Select,XDT.Contact_Expand,XDT.Contact_Filter,XDT.Contact_Fixed,XDT.Contact_Result,XDT.Contact_FormattedResult>;
    ctx_Product: WebMappingRetrieve<XDT.ctx_Product_Select,XDT.ctx_Product_Expand,XDT.ctx_Product_Filter,XDT.ctx_Product_Fixed,XDT.ctx_Product_Result,XDT.ctx_Product_FormattedResult>;
  }
  interface ctx_Subscription_RelatedMany {
    ctx_Transaction_ctx_Subscription: WebMappingRetrieve<XDT.ctx_Transaction_Select,XDT.ctx_Transaction_Expand,XDT.ctx_Transaction_Filter,XDT.ctx_Transaction_Fixed,XDT.ctx_Transaction_Result,XDT.ctx_Transaction_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  ctx_subscriptions: WebMappingRetrieve<XDT.ctx_Subscription_Select,XDT.ctx_Subscription_Expand,XDT.ctx_Subscription_Filter,XDT.ctx_Subscription_Fixed,XDT.ctx_Subscription_Result,XDT.ctx_Subscription_FormattedResult>;
}
interface WebEntitiesRelated {
  ctx_subscriptions: WebMappingRelated<XDT.ctx_Subscription_RelatedOne,XDT.ctx_Subscription_RelatedMany>;
}
interface WebEntitiesCUDA {
  ctx_subscriptions: WebMappingCUDA<XDT.ctx_Subscription_Create,XDT.ctx_Subscription_Update,XDT.ctx_Subscription_Select>;
}
