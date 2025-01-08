declare namespace XDT {
  interface mgs_Transaction_Base extends WebEntity {
    createdon?: Date | null;
    exchangerate?: number | null;
    importsequencenumber?: number | null;
    mgs_amount?: number | null;
    mgs_amount_base?: number | null;
    mgs_end?: Date | null;
    mgs_start?: Date | null;
    mgs_transaction1?: string | null;
    mgs_transactionid?: string | null;
    mgs_type?: mgs_transactiontype | null;
    modifiedon?: Date | null;
    overriddencreatedon?: Date | null;
    statecode?: mgs_transaction_statecode | null;
    statuscode?: mgs_transaction_statuscode | null;
    timezoneruleversionnumber?: number | null;
    transactioncurrencyid_guid?: string | null;
    utcconversiontimezonecode?: number | null;
    versionnumber?: number | null;
  }
  interface mgs_Transaction_Relationships {
    mgs_Invoice?: mgs_Invoice_Result | null;
    mgs_Subscription?: mgs_Subscription_Result | null;
  }
  interface mgs_Transaction extends mgs_Transaction_Base, mgs_Transaction_Relationships {
    mgs_Invoice_bind$mgs_invoices?: string | null;
    mgs_Subscription_bind$mgs_subscriptions?: string | null;
    ownerid_bind$systemusers?: string | null;
    ownerid_bind$teams?: string | null;
    transactioncurrencyid_bind$transactioncurrencies?: string | null;
  }
  interface mgs_Transaction_Create extends mgs_Transaction {
  }
  interface mgs_Transaction_Update extends mgs_Transaction {
  }
  interface mgs_Transaction_Select {
    createdby_guid: WebAttribute<mgs_Transaction_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<mgs_Transaction_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<mgs_Transaction_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    exchangerate: WebAttribute<mgs_Transaction_Select, { exchangerate: number | null }, {  }>;
    importsequencenumber: WebAttribute<mgs_Transaction_Select, { importsequencenumber: number | null }, {  }>;
    mgs_amount: WebAttribute<mgs_Transaction_Select, { mgs_amount: number | null; transactioncurrencyid_guid: string | null }, { mgs_amount_formatted?: string; transactioncurrencyid_formatted?: string }>;
    mgs_amount_base: WebAttribute<mgs_Transaction_Select, { mgs_amount_base: number | null; transactioncurrencyid_guid: string | null }, { mgs_amount_base_formatted?: string; transactioncurrencyid_formatted?: string }>;
    mgs_end: WebAttribute<mgs_Transaction_Select, { mgs_end: Date | null }, { mgs_end_formatted?: string }>;
    mgs_invoice_guid: WebAttribute<mgs_Transaction_Select, { mgs_invoice_guid: string | null }, { mgs_invoice_formatted?: string }>;
    mgs_start: WebAttribute<mgs_Transaction_Select, { mgs_start: Date | null }, { mgs_start_formatted?: string }>;
    mgs_subscription_guid: WebAttribute<mgs_Transaction_Select, { mgs_subscription_guid: string | null }, { mgs_subscription_formatted?: string }>;
    mgs_transaction1: WebAttribute<mgs_Transaction_Select, { mgs_transaction1: string | null }, {  }>;
    mgs_transactionid: WebAttribute<mgs_Transaction_Select, { mgs_transactionid: string | null }, {  }>;
    mgs_type: WebAttribute<mgs_Transaction_Select, { mgs_type: mgs_transactiontype | null }, { mgs_type_formatted?: string }>;
    modifiedby_guid: WebAttribute<mgs_Transaction_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<mgs_Transaction_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<mgs_Transaction_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    overriddencreatedon: WebAttribute<mgs_Transaction_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    ownerid_guid: WebAttribute<mgs_Transaction_Select, { ownerid_guid: string | null }, { ownerid_formatted?: string }>;
    owningbusinessunit_guid: WebAttribute<mgs_Transaction_Select, { owningbusinessunit_guid: string | null }, { owningbusinessunit_formatted?: string }>;
    owningteam_guid: WebAttribute<mgs_Transaction_Select, { owningteam_guid: string | null }, { owningteam_formatted?: string }>;
    owninguser_guid: WebAttribute<mgs_Transaction_Select, { owninguser_guid: string | null }, { owninguser_formatted?: string }>;
    statecode: WebAttribute<mgs_Transaction_Select, { statecode: mgs_transaction_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<mgs_Transaction_Select, { statuscode: mgs_transaction_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<mgs_Transaction_Select, { timezoneruleversionnumber: number | null }, {  }>;
    transactioncurrencyid_guid: WebAttribute<mgs_Transaction_Select, { transactioncurrencyid_guid: string | null }, { transactioncurrencyid_formatted?: string }>;
    utcconversiontimezonecode: WebAttribute<mgs_Transaction_Select, { utcconversiontimezonecode: number | null }, {  }>;
    versionnumber: WebAttribute<mgs_Transaction_Select, { versionnumber: number | null }, {  }>;
  }
  interface mgs_Transaction_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    exchangerate: any;
    importsequencenumber: number;
    mgs_amount: number;
    mgs_amount_base: number;
    mgs_end: Date;
    mgs_invoice_guid: XQW.Guid;
    mgs_start: Date;
    mgs_subscription_guid: XQW.Guid;
    mgs_transaction1: string;
    mgs_transactionid: XQW.Guid;
    mgs_type: mgs_transactiontype;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    overriddencreatedon: Date;
    ownerid_guid: XQW.Guid;
    owningbusinessunit_guid: XQW.Guid;
    owningteam_guid: XQW.Guid;
    owninguser_guid: XQW.Guid;
    statecode: mgs_transaction_statecode;
    statuscode: mgs_transaction_statuscode;
    timezoneruleversionnumber: number;
    transactioncurrencyid_guid: XQW.Guid;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface mgs_Transaction_Expand {
    mgs_Invoice: WebExpand<mgs_Transaction_Expand, mgs_Invoice_Select, mgs_Invoice_Filter, { mgs_Invoice: mgs_Invoice_Result }>;
    mgs_Subscription: WebExpand<mgs_Transaction_Expand, mgs_Subscription_Select, mgs_Subscription_Filter, { mgs_Subscription: mgs_Subscription_Result }>;
  }
  interface mgs_Transaction_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    mgs_amount_base_formatted?: string;
    mgs_amount_formatted?: string;
    mgs_end_formatted?: string;
    mgs_invoice_formatted?: string;
    mgs_start_formatted?: string;
    mgs_subscription_formatted?: string;
    mgs_type_formatted?: string;
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
  interface mgs_Transaction_Result extends mgs_Transaction_Base, mgs_Transaction_Relationships {
    "@odata.etag": string;
    createdby_guid: string | null;
    createdonbehalfby_guid: string | null;
    mgs_invoice_guid: string | null;
    mgs_subscription_guid: string | null;
    modifiedby_guid: string | null;
    modifiedonbehalfby_guid: string | null;
    ownerid_guid: string | null;
    owningbusinessunit_guid: string | null;
    owningteam_guid: string | null;
    owninguser_guid: string | null;
    transactioncurrencyid_guid: string | null;
  }
  interface mgs_Transaction_RelatedOne {
    mgs_Invoice: WebMappingRetrieve<XDT.mgs_Invoice_Select,XDT.mgs_Invoice_Expand,XDT.mgs_Invoice_Filter,XDT.mgs_Invoice_Fixed,XDT.mgs_Invoice_Result,XDT.mgs_Invoice_FormattedResult>;
    mgs_Subscription: WebMappingRetrieve<XDT.mgs_Subscription_Select,XDT.mgs_Subscription_Expand,XDT.mgs_Subscription_Filter,XDT.mgs_Subscription_Fixed,XDT.mgs_Subscription_Result,XDT.mgs_Subscription_FormattedResult>;
  }
  interface mgs_Transaction_RelatedMany {
  }
}
interface WebEntitiesRetrieve {
  mgs_transactions: WebMappingRetrieve<XDT.mgs_Transaction_Select,XDT.mgs_Transaction_Expand,XDT.mgs_Transaction_Filter,XDT.mgs_Transaction_Fixed,XDT.mgs_Transaction_Result,XDT.mgs_Transaction_FormattedResult>;
}
interface WebEntitiesRelated {
  mgs_transactions: WebMappingRelated<XDT.mgs_Transaction_RelatedOne,XDT.mgs_Transaction_RelatedMany>;
}
interface WebEntitiesCUDA {
  mgs_transactions: WebMappingCUDA<XDT.mgs_Transaction_Create,XDT.mgs_Transaction_Update,XDT.mgs_Transaction_Select>;
}
