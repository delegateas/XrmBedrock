declare namespace XDT {
  interface ctx_InvoiceCollection_Base extends WebEntity {
    createdon?: Date | null;
    ctx_invoicecollectionid?: string | null;
    ctx_invoiceuntil?: Date | null;
    ctx_name?: string | null;
    importsequencenumber?: number | null;
    modifiedon?: Date | null;
    overriddencreatedon?: Date | null;
    statecode?: ctx_invoicecollection_statecode | null;
    statuscode?: ctx_invoicecollection_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
    versionnumber?: number | null;
  }
  interface ctx_InvoiceCollection_Relationships {
    ctx_invoice_InvoiceCollection_ctx_invoicecollection?: ctx_Invoice_Result[] | null;
  }
  interface ctx_InvoiceCollection extends ctx_InvoiceCollection_Base, ctx_InvoiceCollection_Relationships {
    ownerid_bind$systemusers?: string | null;
    ownerid_bind$teams?: string | null;
  }
  interface ctx_InvoiceCollection_Create extends ctx_InvoiceCollection {
  }
  interface ctx_InvoiceCollection_Update extends ctx_InvoiceCollection {
  }
  interface ctx_InvoiceCollection_Select {
    createdby_guid: WebAttribute<ctx_InvoiceCollection_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<ctx_InvoiceCollection_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<ctx_InvoiceCollection_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    ctx_invoicecollectionid: WebAttribute<ctx_InvoiceCollection_Select, { ctx_invoicecollectionid: string | null }, {  }>;
    ctx_invoiceuntil: WebAttribute<ctx_InvoiceCollection_Select, { ctx_invoiceuntil: Date | null }, { ctx_invoiceuntil_formatted?: string }>;
    ctx_name: WebAttribute<ctx_InvoiceCollection_Select, { ctx_name: string | null }, {  }>;
    importsequencenumber: WebAttribute<ctx_InvoiceCollection_Select, { importsequencenumber: number | null }, {  }>;
    modifiedby_guid: WebAttribute<ctx_InvoiceCollection_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<ctx_InvoiceCollection_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<ctx_InvoiceCollection_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    overriddencreatedon: WebAttribute<ctx_InvoiceCollection_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    ownerid_guid: WebAttribute<ctx_InvoiceCollection_Select, { ownerid_guid: string | null }, { ownerid_formatted?: string }>;
    owningbusinessunit_guid: WebAttribute<ctx_InvoiceCollection_Select, { owningbusinessunit_guid: string | null }, { owningbusinessunit_formatted?: string }>;
    owningteam_guid: WebAttribute<ctx_InvoiceCollection_Select, { owningteam_guid: string | null }, { owningteam_formatted?: string }>;
    owninguser_guid: WebAttribute<ctx_InvoiceCollection_Select, { owninguser_guid: string | null }, { owninguser_formatted?: string }>;
    statecode: WebAttribute<ctx_InvoiceCollection_Select, { statecode: ctx_invoicecollection_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<ctx_InvoiceCollection_Select, { statuscode: ctx_invoicecollection_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<ctx_InvoiceCollection_Select, { timezoneruleversionnumber: number | null }, {  }>;
    utcconversiontimezonecode: WebAttribute<ctx_InvoiceCollection_Select, { utcconversiontimezonecode: number | null }, {  }>;
    versionnumber: WebAttribute<ctx_InvoiceCollection_Select, { versionnumber: number | null }, {  }>;
  }
  interface ctx_InvoiceCollection_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    ctx_invoicecollectionid: XQW.Guid;
    ctx_invoiceuntil: Date;
    ctx_name: string;
    importsequencenumber: number;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    overriddencreatedon: Date;
    ownerid_guid: XQW.Guid;
    owningbusinessunit_guid: XQW.Guid;
    owningteam_guid: XQW.Guid;
    owninguser_guid: XQW.Guid;
    statecode: ctx_invoicecollection_statecode;
    statuscode: ctx_invoicecollection_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface ctx_InvoiceCollection_Expand {
    ctx_invoice_InvoiceCollection_ctx_invoicecollection: WebExpand<ctx_InvoiceCollection_Expand, ctx_Invoice_Select, ctx_Invoice_Filter, { ctx_invoice_InvoiceCollection_ctx_invoicecollection: ctx_Invoice_Result[] }>;
  }
  interface ctx_InvoiceCollection_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    ctx_invoiceuntil_formatted?: string;
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
  interface ctx_InvoiceCollection_Result extends ctx_InvoiceCollection_Base, ctx_InvoiceCollection_Relationships {
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
  interface ctx_InvoiceCollection_RelatedOne {
  }
  interface ctx_InvoiceCollection_RelatedMany {
    ctx_invoice_InvoiceCollection_ctx_invoicecollection: WebMappingRetrieve<XDT.ctx_Invoice_Select,XDT.ctx_Invoice_Expand,XDT.ctx_Invoice_Filter,XDT.ctx_Invoice_Fixed,XDT.ctx_Invoice_Result,XDT.ctx_Invoice_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  ctx_invoicecollections: WebMappingRetrieve<XDT.ctx_InvoiceCollection_Select,XDT.ctx_InvoiceCollection_Expand,XDT.ctx_InvoiceCollection_Filter,XDT.ctx_InvoiceCollection_Fixed,XDT.ctx_InvoiceCollection_Result,XDT.ctx_InvoiceCollection_FormattedResult>;
}
interface WebEntitiesRelated {
  ctx_invoicecollections: WebMappingRelated<XDT.ctx_InvoiceCollection_RelatedOne,XDT.ctx_InvoiceCollection_RelatedMany>;
}
interface WebEntitiesCUDA {
  ctx_invoicecollections: WebMappingCUDA<XDT.ctx_InvoiceCollection_Create,XDT.ctx_InvoiceCollection_Update,XDT.ctx_InvoiceCollection_Select>;
}
