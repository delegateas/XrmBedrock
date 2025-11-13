declare namespace XDT {
  interface ctx_Transaction_Base extends WebEntity {
    createdon?: Date | null;
    ctx_amount?: number | null;
    ctx_amount_base?: number | null;
    ctx_enddate?: Date | null;
    ctx_name?: string | null;
    ctx_startdate?: Date | null;
    ctx_transactionid?: string | null;
    exchangerate?: number | null;
    importsequencenumber?: number | null;
    modifiedon?: Date | null;
    overriddencreatedon?: Date | null;
    statecode?: ctx_transaction_statecode | null;
    statuscode?: ctx_transaction_statuscode | null;
    timezoneruleversionnumber?: number | null;
    transactioncurrencyid_guid?: string | null;
    utcconversiontimezonecode?: number | null;
    versionnumber?: number | null;
  }
  interface ctx_Transaction_Relationships {
    ctx_Invoice?: ctx_Invoice_Result | null;
    ctx_Subscription?: ctx_Subscription_Result | null;
  }
  interface ctx_Transaction extends ctx_Transaction_Base, ctx_Transaction_Relationships {
    ctx_Invoice_bind$ctx_invoices?: string | null;
    ctx_Subscription_bind$ctx_subscriptions?: string | null;
    ownerid_bind$systemusers?: string | null;
    ownerid_bind$teams?: string | null;
    transactioncurrencyid_bind$transactioncurrencies?: string | null;
  }
  interface ctx_Transaction_Create extends ctx_Transaction {
  }
  interface ctx_Transaction_Update extends ctx_Transaction {
  }
  interface ctx_Transaction_Select {
    createdby_guid: WebAttribute<ctx_Transaction_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<ctx_Transaction_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<ctx_Transaction_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    ctx_amount: WebAttribute<ctx_Transaction_Select, { ctx_amount: number | null; transactioncurrencyid_guid: string | null }, { ctx_amount_formatted?: string; transactioncurrencyid_formatted?: string }>;
    ctx_amount_base: WebAttribute<ctx_Transaction_Select, { ctx_amount_base: number | null; transactioncurrencyid_guid: string | null }, { ctx_amount_base_formatted?: string; transactioncurrencyid_formatted?: string }>;
    ctx_enddate: WebAttribute<ctx_Transaction_Select, { ctx_enddate: Date | null }, { ctx_enddate_formatted?: string }>;
    ctx_invoice_guid: WebAttribute<ctx_Transaction_Select, { ctx_invoice_guid: string | null }, { ctx_invoice_formatted?: string }>;
    ctx_name: WebAttribute<ctx_Transaction_Select, { ctx_name: string | null }, {  }>;
    ctx_startdate: WebAttribute<ctx_Transaction_Select, { ctx_startdate: Date | null }, { ctx_startdate_formatted?: string }>;
    ctx_subscription_guid: WebAttribute<ctx_Transaction_Select, { ctx_subscription_guid: string | null }, { ctx_subscription_formatted?: string }>;
    ctx_transactionid: WebAttribute<ctx_Transaction_Select, { ctx_transactionid: string | null }, {  }>;
    exchangerate: WebAttribute<ctx_Transaction_Select, { exchangerate: number | null }, {  }>;
    importsequencenumber: WebAttribute<ctx_Transaction_Select, { importsequencenumber: number | null }, {  }>;
    modifiedby_guid: WebAttribute<ctx_Transaction_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<ctx_Transaction_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<ctx_Transaction_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    overriddencreatedon: WebAttribute<ctx_Transaction_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    ownerid_guid: WebAttribute<ctx_Transaction_Select, { ownerid_guid: string | null }, { ownerid_formatted?: string }>;
    owningbusinessunit_guid: WebAttribute<ctx_Transaction_Select, { owningbusinessunit_guid: string | null }, { owningbusinessunit_formatted?: string }>;
    owningteam_guid: WebAttribute<ctx_Transaction_Select, { owningteam_guid: string | null }, { owningteam_formatted?: string }>;
    owninguser_guid: WebAttribute<ctx_Transaction_Select, { owninguser_guid: string | null }, { owninguser_formatted?: string }>;
    statecode: WebAttribute<ctx_Transaction_Select, { statecode: ctx_transaction_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<ctx_Transaction_Select, { statuscode: ctx_transaction_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<ctx_Transaction_Select, { timezoneruleversionnumber: number | null }, {  }>;
    transactioncurrencyid_guid: WebAttribute<ctx_Transaction_Select, { transactioncurrencyid_guid: string | null }, { transactioncurrencyid_formatted?: string }>;
    utcconversiontimezonecode: WebAttribute<ctx_Transaction_Select, { utcconversiontimezonecode: number | null }, {  }>;
    versionnumber: WebAttribute<ctx_Transaction_Select, { versionnumber: number | null }, {  }>;
  }
  interface ctx_Transaction_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    ctx_amount: number;
    ctx_amount_base: number;
    ctx_enddate: Date;
    ctx_invoice_guid: XQW.Guid;
    ctx_name: string;
    ctx_startdate: Date;
    ctx_subscription_guid: XQW.Guid;
    ctx_transactionid: XQW.Guid;
    exchangerate: any;
    importsequencenumber: number;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    overriddencreatedon: Date;
    ownerid_guid: XQW.Guid;
    owningbusinessunit_guid: XQW.Guid;
    owningteam_guid: XQW.Guid;
    owninguser_guid: XQW.Guid;
    statecode: ctx_transaction_statecode;
    statuscode: ctx_transaction_statuscode;
    timezoneruleversionnumber: number;
    transactioncurrencyid_guid: XQW.Guid;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface ctx_Transaction_Expand {
    ctx_Invoice: WebExpand<ctx_Transaction_Expand, ctx_Invoice_Select, ctx_Invoice_Filter, { ctx_Invoice: ctx_Invoice_Result }>;
    ctx_Subscription: WebExpand<ctx_Transaction_Expand, ctx_Subscription_Select, ctx_Subscription_Filter, { ctx_Subscription: ctx_Subscription_Result }>;
  }
  interface ctx_Transaction_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    ctx_amount_base_formatted?: string;
    ctx_amount_formatted?: string;
    ctx_enddate_formatted?: string;
    ctx_invoice_formatted?: string;
    ctx_startdate_formatted?: string;
    ctx_subscription_formatted?: string;
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
    transactioncurrencyid_formatted?: string;
  }
  interface ctx_Transaction_Result extends ctx_Transaction_Base, ctx_Transaction_Relationships {
    "@odata.etag": string;
    createdby_guid: string | null;
    createdonbehalfby_guid: string | null;
    ctx_invoice_guid: string | null;
    ctx_subscription_guid: string | null;
    modifiedby_guid: string | null;
    modifiedonbehalfby_guid: string | null;
    ownerid_guid: string | null;
    owningbusinessunit_guid: string | null;
    owningteam_guid: string | null;
    owninguser_guid: string | null;
    transactioncurrencyid_guid: string | null;
  }
  interface ctx_Transaction_RelatedOne {
    ctx_Invoice: WebMappingRetrieve<XDT.ctx_Invoice_Select,XDT.ctx_Invoice_Expand,XDT.ctx_Invoice_Filter,XDT.ctx_Invoice_Fixed,XDT.ctx_Invoice_Result,XDT.ctx_Invoice_FormattedResult>;
    ctx_Subscription: WebMappingRetrieve<XDT.ctx_Subscription_Select,XDT.ctx_Subscription_Expand,XDT.ctx_Subscription_Filter,XDT.ctx_Subscription_Fixed,XDT.ctx_Subscription_Result,XDT.ctx_Subscription_FormattedResult>;
  }
  interface ctx_Transaction_RelatedMany {
  }
}
interface WebEntitiesRetrieve {
  ctx_transactions: WebMappingRetrieve<XDT.ctx_Transaction_Select,XDT.ctx_Transaction_Expand,XDT.ctx_Transaction_Filter,XDT.ctx_Transaction_Fixed,XDT.ctx_Transaction_Result,XDT.ctx_Transaction_FormattedResult>;
}
interface WebEntitiesRelated {
  ctx_transactions: WebMappingRelated<XDT.ctx_Transaction_RelatedOne,XDT.ctx_Transaction_RelatedMany>;
}
interface WebEntitiesCUDA {
  ctx_transactions: WebMappingCUDA<XDT.ctx_Transaction_Create,XDT.ctx_Transaction_Update,XDT.ctx_Transaction_Select>;
}
