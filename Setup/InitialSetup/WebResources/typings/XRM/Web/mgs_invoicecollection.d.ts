declare namespace XDT {
  interface mgs_InvoiceCollection_Base extends WebEntity {
    createdon?: Date | null;
    importsequencenumber?: number | null;
    mgs_invoicecollectionid?: string | null;
    mgs_invoiceuntil?: Date | null;
    mgs_name?: string | null;
    modifiedon?: Date | null;
    overriddencreatedon?: Date | null;
    statecode?: mgs_invoicecollection_statecode | null;
    statuscode?: mgs_invoicecollection_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
    versionnumber?: number | null;
  }
  interface mgs_InvoiceCollection_Relationships {
    mgs_invoice_InvoiceCollection_mgs_invoicecollection?: mgs_Invoice_Result[] | null;
  }
  interface mgs_InvoiceCollection extends mgs_InvoiceCollection_Base, mgs_InvoiceCollection_Relationships {
    ownerid_bind$systemusers?: string | null;
    ownerid_bind$teams?: string | null;
  }
  interface mgs_InvoiceCollection_Create extends mgs_InvoiceCollection {
  }
  interface mgs_InvoiceCollection_Update extends mgs_InvoiceCollection {
  }
  interface mgs_InvoiceCollection_Select {
    createdby_guid: WebAttribute<mgs_InvoiceCollection_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<mgs_InvoiceCollection_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<mgs_InvoiceCollection_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    importsequencenumber: WebAttribute<mgs_InvoiceCollection_Select, { importsequencenumber: number | null }, {  }>;
    mgs_invoicecollectionid: WebAttribute<mgs_InvoiceCollection_Select, { mgs_invoicecollectionid: string | null }, {  }>;
    mgs_invoiceuntil: WebAttribute<mgs_InvoiceCollection_Select, { mgs_invoiceuntil: Date | null }, { mgs_invoiceuntil_formatted?: string }>;
    mgs_name: WebAttribute<mgs_InvoiceCollection_Select, { mgs_name: string | null }, {  }>;
    modifiedby_guid: WebAttribute<mgs_InvoiceCollection_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<mgs_InvoiceCollection_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<mgs_InvoiceCollection_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    overriddencreatedon: WebAttribute<mgs_InvoiceCollection_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    ownerid_guid: WebAttribute<mgs_InvoiceCollection_Select, { ownerid_guid: string | null }, { ownerid_formatted?: string }>;
    owningbusinessunit_guid: WebAttribute<mgs_InvoiceCollection_Select, { owningbusinessunit_guid: string | null }, { owningbusinessunit_formatted?: string }>;
    owningteam_guid: WebAttribute<mgs_InvoiceCollection_Select, { owningteam_guid: string | null }, { owningteam_formatted?: string }>;
    owninguser_guid: WebAttribute<mgs_InvoiceCollection_Select, { owninguser_guid: string | null }, { owninguser_formatted?: string }>;
    statecode: WebAttribute<mgs_InvoiceCollection_Select, { statecode: mgs_invoicecollection_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<mgs_InvoiceCollection_Select, { statuscode: mgs_invoicecollection_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<mgs_InvoiceCollection_Select, { timezoneruleversionnumber: number | null }, {  }>;
    utcconversiontimezonecode: WebAttribute<mgs_InvoiceCollection_Select, { utcconversiontimezonecode: number | null }, {  }>;
    versionnumber: WebAttribute<mgs_InvoiceCollection_Select, { versionnumber: number | null }, {  }>;
  }
  interface mgs_InvoiceCollection_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    importsequencenumber: number;
    mgs_invoicecollectionid: XQW.Guid;
    mgs_invoiceuntil: Date;
    mgs_name: string;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    overriddencreatedon: Date;
    ownerid_guid: XQW.Guid;
    owningbusinessunit_guid: XQW.Guid;
    owningteam_guid: XQW.Guid;
    owninguser_guid: XQW.Guid;
    statecode: mgs_invoicecollection_statecode;
    statuscode: mgs_invoicecollection_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface mgs_InvoiceCollection_Expand {
    mgs_invoice_InvoiceCollection_mgs_invoicecollection: WebExpand<mgs_InvoiceCollection_Expand, mgs_Invoice_Select, mgs_Invoice_Filter, { mgs_invoice_InvoiceCollection_mgs_invoicecollection: mgs_Invoice_Result[] }>;
  }
  interface mgs_InvoiceCollection_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    mgs_invoiceuntil_formatted?: string;
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
  interface mgs_InvoiceCollection_Result extends mgs_InvoiceCollection_Base, mgs_InvoiceCollection_Relationships {
    "@odata.etag": string;
    createdby_guid: string | null;
    createdonbehalfby_guid: string | null;
    modifiedby_guid: string | null;
    modifiedonbehalfby_guid: string | null;
    ownerid_guid: string | null;
    owningbusinessunit_guid: string | null;
    owningteam_guid: string | null;
    owninguser_guid: string | null;
  }
  interface mgs_InvoiceCollection_RelatedOne {
  }
  interface mgs_InvoiceCollection_RelatedMany {
    mgs_invoice_InvoiceCollection_mgs_invoicecollection: WebMappingRetrieve<XDT.mgs_Invoice_Select,XDT.mgs_Invoice_Expand,XDT.mgs_Invoice_Filter,XDT.mgs_Invoice_Fixed,XDT.mgs_Invoice_Result,XDT.mgs_Invoice_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  mgs_invoicecollections: WebMappingRetrieve<XDT.mgs_InvoiceCollection_Select,XDT.mgs_InvoiceCollection_Expand,XDT.mgs_InvoiceCollection_Filter,XDT.mgs_InvoiceCollection_Fixed,XDT.mgs_InvoiceCollection_Result,XDT.mgs_InvoiceCollection_FormattedResult>;
}
interface WebEntitiesRelated {
  mgs_invoicecollections: WebMappingRelated<XDT.mgs_InvoiceCollection_RelatedOne,XDT.mgs_InvoiceCollection_RelatedMany>;
}
interface WebEntitiesCUDA {
  mgs_invoicecollections: WebMappingCUDA<XDT.mgs_InvoiceCollection_Create,XDT.mgs_InvoiceCollection_Update,XDT.mgs_InvoiceCollection_Select>;
}
