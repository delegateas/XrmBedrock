declare namespace XDT {
  interface ctx_Invoice_Base extends WebEntity {
    createdon?: Date | null;
    ctx_invoiceid?: string | null;
    ctx_invoicenumber?: string | null;
    importsequencenumber?: number | null;
    modifiedon?: Date | null;
    overriddencreatedon?: Date | null;
    statecode?: ctx_invoice_statecode | null;
    statuscode?: ctx_invoice_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
    versionnumber?: number | null;
  }
  interface ctx_Invoice_Relationships {
    ctx_Customer_account?: Account_Result | null;
    ctx_Customer_contact?: Contact_Result | null;
    ctx_InvoiceCollection?: ctx_InvoiceCollection_Result | null;
    ctx_Transaction_ctx_Invoice?: ctx_Transaction_Result[] | null;
  }
  interface ctx_Invoice extends ctx_Invoice_Base, ctx_Invoice_Relationships {
    ctx_Customer_account_bind$accounts?: string | null;
    ctx_Customer_contact_bind$contacts?: string | null;
    ctx_InvoiceCollection_bind$ctx_invoicecollections?: string | null;
    ownerid_bind$systemusers?: string | null;
    ownerid_bind$teams?: string | null;
  }
  interface ctx_Invoice_Create extends ctx_Invoice {
  }
  interface ctx_Invoice_Update extends ctx_Invoice {
  }
  interface ctx_Invoice_Select {
    createdby_guid: WebAttribute<ctx_Invoice_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<ctx_Invoice_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<ctx_Invoice_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    ctx_customer_guid: WebAttribute<ctx_Invoice_Select, { ctx_customer_guid: string | null }, { ctx_customer_formatted?: string }>;
    ctx_invoicecollection_guid: WebAttribute<ctx_Invoice_Select, { ctx_invoicecollection_guid: string | null }, { ctx_invoicecollection_formatted?: string }>;
    ctx_invoiceid: WebAttribute<ctx_Invoice_Select, { ctx_invoiceid: string | null }, {  }>;
    ctx_invoicenumber: WebAttribute<ctx_Invoice_Select, { ctx_invoicenumber: string | null }, {  }>;
    importsequencenumber: WebAttribute<ctx_Invoice_Select, { importsequencenumber: number | null }, {  }>;
    modifiedby_guid: WebAttribute<ctx_Invoice_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<ctx_Invoice_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<ctx_Invoice_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    overriddencreatedon: WebAttribute<ctx_Invoice_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    ownerid_guid: WebAttribute<ctx_Invoice_Select, { ownerid_guid: string | null }, { ownerid_formatted?: string }>;
    owningbusinessunit_guid: WebAttribute<ctx_Invoice_Select, { owningbusinessunit_guid: string | null }, { owningbusinessunit_formatted?: string }>;
    owningteam_guid: WebAttribute<ctx_Invoice_Select, { owningteam_guid: string | null }, { owningteam_formatted?: string }>;
    owninguser_guid: WebAttribute<ctx_Invoice_Select, { owninguser_guid: string | null }, { owninguser_formatted?: string }>;
    statecode: WebAttribute<ctx_Invoice_Select, { statecode: ctx_invoice_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<ctx_Invoice_Select, { statuscode: ctx_invoice_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<ctx_Invoice_Select, { timezoneruleversionnumber: number | null }, {  }>;
    utcconversiontimezonecode: WebAttribute<ctx_Invoice_Select, { utcconversiontimezonecode: number | null }, {  }>;
    versionnumber: WebAttribute<ctx_Invoice_Select, { versionnumber: number | null }, {  }>;
  }
  interface ctx_Invoice_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    ctx_customer_guid: XQW.Guid;
    ctx_invoicecollection_guid: XQW.Guid;
    ctx_invoiceid: XQW.Guid;
    ctx_invoicenumber: string;
    importsequencenumber: number;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    overriddencreatedon: Date;
    ownerid_guid: XQW.Guid;
    owningbusinessunit_guid: XQW.Guid;
    owningteam_guid: XQW.Guid;
    owninguser_guid: XQW.Guid;
    statecode: ctx_invoice_statecode;
    statuscode: ctx_invoice_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface ctx_Invoice_Expand {
    ctx_Customer_account: WebExpand<ctx_Invoice_Expand, Account_Select, Account_Filter, { ctx_Customer_account: Account_Result }>;
    ctx_Customer_contact: WebExpand<ctx_Invoice_Expand, Contact_Select, Contact_Filter, { ctx_Customer_contact: Contact_Result }>;
    ctx_InvoiceCollection: WebExpand<ctx_Invoice_Expand, ctx_InvoiceCollection_Select, ctx_InvoiceCollection_Filter, { ctx_InvoiceCollection: ctx_InvoiceCollection_Result }>;
    ctx_Transaction_ctx_Invoice: WebExpand<ctx_Invoice_Expand, ctx_Transaction_Select, ctx_Transaction_Filter, { ctx_Transaction_ctx_Invoice: ctx_Transaction_Result[] }>;
  }
  interface ctx_Invoice_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    ctx_customer_formatted?: string;
    ctx_invoicecollection_formatted?: string;
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
  interface ctx_Invoice_Result extends ctx_Invoice_Base, ctx_Invoice_Relationships {
    "@odata.etag": string;
    createdby_guid: string | null;
    createdonbehalfby_guid: string | null;
    ctx_customer_guid: string | null;
    ctx_invoicecollection_guid: string | null;
    modifiedby_guid: string | null;
    modifiedonbehalfby_guid: string | null;
    ownerid_guid: string | null;
    owningbusinessunit_guid: string | null;
    owningteam_guid: string | null;
    owninguser_guid: string | null;
  }
  interface ctx_Invoice_RelatedOne {
    ctx_Customer_account: WebMappingRetrieve<XDT.Account_Select,XDT.Account_Expand,XDT.Account_Filter,XDT.Account_Fixed,XDT.Account_Result,XDT.Account_FormattedResult>;
    ctx_Customer_contact: WebMappingRetrieve<XDT.Contact_Select,XDT.Contact_Expand,XDT.Contact_Filter,XDT.Contact_Fixed,XDT.Contact_Result,XDT.Contact_FormattedResult>;
    ctx_InvoiceCollection: WebMappingRetrieve<XDT.ctx_InvoiceCollection_Select,XDT.ctx_InvoiceCollection_Expand,XDT.ctx_InvoiceCollection_Filter,XDT.ctx_InvoiceCollection_Fixed,XDT.ctx_InvoiceCollection_Result,XDT.ctx_InvoiceCollection_FormattedResult>;
  }
  interface ctx_Invoice_RelatedMany {
    ctx_Transaction_ctx_Invoice: WebMappingRetrieve<XDT.ctx_Transaction_Select,XDT.ctx_Transaction_Expand,XDT.ctx_Transaction_Filter,XDT.ctx_Transaction_Fixed,XDT.ctx_Transaction_Result,XDT.ctx_Transaction_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  ctx_invoices: WebMappingRetrieve<XDT.ctx_Invoice_Select,XDT.ctx_Invoice_Expand,XDT.ctx_Invoice_Filter,XDT.ctx_Invoice_Fixed,XDT.ctx_Invoice_Result,XDT.ctx_Invoice_FormattedResult>;
}
interface WebEntitiesRelated {
  ctx_invoices: WebMappingRelated<XDT.ctx_Invoice_RelatedOne,XDT.ctx_Invoice_RelatedMany>;
}
interface WebEntitiesCUDA {
  ctx_invoices: WebMappingCUDA<XDT.ctx_Invoice_Create,XDT.ctx_Invoice_Update,XDT.ctx_Invoice_Select>;
}
