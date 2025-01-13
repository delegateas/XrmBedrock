declare namespace XDT {
  interface mgs_Invoice_Base extends WebEntity {
    createdon?: Date | null;
    exchangerate?: number | null;
    importsequencenumber?: number | null;
    mgs_invoice1?: string | null;
    mgs_invoicedate?: Date | null;
    mgs_invoiceid?: string | null;
    mgs_totalamount?: number | null;
    mgs_totalamount_base?: number | null;
    modifiedon?: Date | null;
    overriddencreatedon?: Date | null;
    statecode?: mgs_invoice_statecode | null;
    statuscode?: mgs_invoice_statuscode | null;
    timezoneruleversionnumber?: number | null;
    transactioncurrencyid_guid?: string | null;
    utcconversiontimezonecode?: number | null;
    versionnumber?: number | null;
  }
  interface mgs_Invoice_Relationships {
    mgs_Contact?: Contact_Result | null;
    mgs_InvoiceCollection?: mgs_InvoiceCollection_Result | null;
    mgs_transaction_Invoice_mgs_invoice?: mgs_Transaction_Result[] | null;
  }
  interface mgs_Invoice extends mgs_Invoice_Base, mgs_Invoice_Relationships {
    mgs_Contact_bind$contacts?: string | null;
    mgs_InvoiceCollection_bind$mgs_invoicecollections?: string | null;
    ownerid_bind$systemusers?: string | null;
    ownerid_bind$teams?: string | null;
    transactioncurrencyid_bind$transactioncurrencies?: string | null;
  }
  interface mgs_Invoice_Create extends mgs_Invoice {
  }
  interface mgs_Invoice_Update extends mgs_Invoice {
  }
  interface mgs_Invoice_Select {
    createdby_guid: WebAttribute<mgs_Invoice_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<mgs_Invoice_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<mgs_Invoice_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    exchangerate: WebAttribute<mgs_Invoice_Select, { exchangerate: number | null }, {  }>;
    importsequencenumber: WebAttribute<mgs_Invoice_Select, { importsequencenumber: number | null }, {  }>;
    mgs_contact_guid: WebAttribute<mgs_Invoice_Select, { mgs_contact_guid: string | null }, { mgs_contact_formatted?: string }>;
    mgs_invoice1: WebAttribute<mgs_Invoice_Select, { mgs_invoice1: string | null }, {  }>;
    mgs_invoicecollection_guid: WebAttribute<mgs_Invoice_Select, { mgs_invoicecollection_guid: string | null }, { mgs_invoicecollection_formatted?: string }>;
    mgs_invoicedate: WebAttribute<mgs_Invoice_Select, { mgs_invoicedate: Date | null }, { mgs_invoicedate_formatted?: string }>;
    mgs_invoiceid: WebAttribute<mgs_Invoice_Select, { mgs_invoiceid: string | null }, {  }>;
    mgs_totalamount: WebAttribute<mgs_Invoice_Select, { mgs_totalamount: number | null; transactioncurrencyid_guid: string | null }, { mgs_totalamount_formatted?: string; transactioncurrencyid_formatted?: string }>;
    mgs_totalamount_base: WebAttribute<mgs_Invoice_Select, { mgs_totalamount_base: number | null; transactioncurrencyid_guid: string | null }, { mgs_totalamount_base_formatted?: string; transactioncurrencyid_formatted?: string }>;
    modifiedby_guid: WebAttribute<mgs_Invoice_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<mgs_Invoice_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<mgs_Invoice_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    overriddencreatedon: WebAttribute<mgs_Invoice_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    ownerid_guid: WebAttribute<mgs_Invoice_Select, { ownerid_guid: string | null }, { ownerid_formatted?: string }>;
    owningbusinessunit_guid: WebAttribute<mgs_Invoice_Select, { owningbusinessunit_guid: string | null }, { owningbusinessunit_formatted?: string }>;
    owningteam_guid: WebAttribute<mgs_Invoice_Select, { owningteam_guid: string | null }, { owningteam_formatted?: string }>;
    owninguser_guid: WebAttribute<mgs_Invoice_Select, { owninguser_guid: string | null }, { owninguser_formatted?: string }>;
    statecode: WebAttribute<mgs_Invoice_Select, { statecode: mgs_invoice_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<mgs_Invoice_Select, { statuscode: mgs_invoice_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<mgs_Invoice_Select, { timezoneruleversionnumber: number | null }, {  }>;
    transactioncurrencyid_guid: WebAttribute<mgs_Invoice_Select, { transactioncurrencyid_guid: string | null }, { transactioncurrencyid_formatted?: string }>;
    utcconversiontimezonecode: WebAttribute<mgs_Invoice_Select, { utcconversiontimezonecode: number | null }, {  }>;
    versionnumber: WebAttribute<mgs_Invoice_Select, { versionnumber: number | null }, {  }>;
  }
  interface mgs_Invoice_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    exchangerate: any;
    importsequencenumber: number;
    mgs_contact_guid: XQW.Guid;
    mgs_invoice1: string;
    mgs_invoicecollection_guid: XQW.Guid;
    mgs_invoicedate: Date;
    mgs_invoiceid: XQW.Guid;
    mgs_totalamount: number;
    mgs_totalamount_base: number;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    overriddencreatedon: Date;
    ownerid_guid: XQW.Guid;
    owningbusinessunit_guid: XQW.Guid;
    owningteam_guid: XQW.Guid;
    owninguser_guid: XQW.Guid;
    statecode: mgs_invoice_statecode;
    statuscode: mgs_invoice_statuscode;
    timezoneruleversionnumber: number;
    transactioncurrencyid_guid: XQW.Guid;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface mgs_Invoice_Expand {
    mgs_Contact: WebExpand<mgs_Invoice_Expand, Contact_Select, Contact_Filter, { mgs_Contact: Contact_Result }>;
    mgs_InvoiceCollection: WebExpand<mgs_Invoice_Expand, mgs_InvoiceCollection_Select, mgs_InvoiceCollection_Filter, { mgs_InvoiceCollection: mgs_InvoiceCollection_Result }>;
    mgs_transaction_Invoice_mgs_invoice: WebExpand<mgs_Invoice_Expand, mgs_Transaction_Select, mgs_Transaction_Filter, { mgs_transaction_Invoice_mgs_invoice: mgs_Transaction_Result[] }>;
  }
  interface mgs_Invoice_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    mgs_contact_formatted?: string;
    mgs_invoicecollection_formatted?: string;
    mgs_invoicedate_formatted?: string;
    mgs_totalamount_base_formatted?: string;
    mgs_totalamount_formatted?: string;
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
  interface mgs_Invoice_Result extends mgs_Invoice_Base, mgs_Invoice_Relationships {
    "@odata.etag": string;
    createdby_guid: string | null;
    createdonbehalfby_guid: string | null;
    mgs_contact_guid: string | null;
    mgs_invoicecollection_guid: string | null;
    modifiedby_guid: string | null;
    modifiedonbehalfby_guid: string | null;
    ownerid_guid: string | null;
    owningbusinessunit_guid: string | null;
    owningteam_guid: string | null;
    owninguser_guid: string | null;
    transactioncurrencyid_guid: string | null;
  }
  interface mgs_Invoice_RelatedOne {
    mgs_Contact: WebMappingRetrieve<XDT.Contact_Select,XDT.Contact_Expand,XDT.Contact_Filter,XDT.Contact_Fixed,XDT.Contact_Result,XDT.Contact_FormattedResult>;
    mgs_InvoiceCollection: WebMappingRetrieve<XDT.mgs_InvoiceCollection_Select,XDT.mgs_InvoiceCollection_Expand,XDT.mgs_InvoiceCollection_Filter,XDT.mgs_InvoiceCollection_Fixed,XDT.mgs_InvoiceCollection_Result,XDT.mgs_InvoiceCollection_FormattedResult>;
  }
  interface mgs_Invoice_RelatedMany {
    mgs_transaction_Invoice_mgs_invoice: WebMappingRetrieve<XDT.mgs_Transaction_Select,XDT.mgs_Transaction_Expand,XDT.mgs_Transaction_Filter,XDT.mgs_Transaction_Fixed,XDT.mgs_Transaction_Result,XDT.mgs_Transaction_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  mgs_invoices: WebMappingRetrieve<XDT.mgs_Invoice_Select,XDT.mgs_Invoice_Expand,XDT.mgs_Invoice_Filter,XDT.mgs_Invoice_Fixed,XDT.mgs_Invoice_Result,XDT.mgs_Invoice_FormattedResult>;
}
interface WebEntitiesRelated {
  mgs_invoices: WebMappingRelated<XDT.mgs_Invoice_RelatedOne,XDT.mgs_Invoice_RelatedMany>;
}
interface WebEntitiesCUDA {
  mgs_invoices: WebMappingCUDA<XDT.mgs_Invoice_Create,XDT.mgs_Invoice_Update,XDT.mgs_Invoice_Select>;
}
