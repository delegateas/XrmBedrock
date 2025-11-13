declare namespace XDT {
  interface ctx_Product_Base extends WebEntity {
    createdon?: Date | null;
    ctx_billinginterval?: ctx_billinginterval | null;
    ctx_name?: string | null;
    ctx_price?: number | null;
    ctx_price_base?: number | null;
    ctx_productid?: string | null;
    exchangerate?: number | null;
    importsequencenumber?: number | null;
    modifiedon?: Date | null;
    overriddencreatedon?: Date | null;
    statecode?: ctx_product_statecode | null;
    statuscode?: ctx_product_statuscode | null;
    timezoneruleversionnumber?: number | null;
    transactioncurrencyid_guid?: string | null;
    utcconversiontimezonecode?: number | null;
    versionnumber?: number | null;
  }
  interface ctx_Product_Relationships {
    ctx_Subscription_ctx_Product?: ctx_Subscription_Result[] | null;
  }
  interface ctx_Product extends ctx_Product_Base, ctx_Product_Relationships {
    ownerid_bind$systemusers?: string | null;
    ownerid_bind$teams?: string | null;
    transactioncurrencyid_bind$transactioncurrencies?: string | null;
  }
  interface ctx_Product_Create extends ctx_Product {
  }
  interface ctx_Product_Update extends ctx_Product {
  }
  interface ctx_Product_Select {
    createdby_guid: WebAttribute<ctx_Product_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<ctx_Product_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<ctx_Product_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    ctx_billinginterval: WebAttribute<ctx_Product_Select, { ctx_billinginterval: ctx_billinginterval | null }, { ctx_billinginterval_formatted?: string }>;
    ctx_name: WebAttribute<ctx_Product_Select, { ctx_name: string | null }, {  }>;
    ctx_price: WebAttribute<ctx_Product_Select, { ctx_price: number | null; transactioncurrencyid_guid: string | null }, { ctx_price_formatted?: string; transactioncurrencyid_formatted?: string }>;
    ctx_price_base: WebAttribute<ctx_Product_Select, { ctx_price_base: number | null; transactioncurrencyid_guid: string | null }, { ctx_price_base_formatted?: string; transactioncurrencyid_formatted?: string }>;
    ctx_productid: WebAttribute<ctx_Product_Select, { ctx_productid: string | null }, {  }>;
    exchangerate: WebAttribute<ctx_Product_Select, { exchangerate: number | null }, {  }>;
    importsequencenumber: WebAttribute<ctx_Product_Select, { importsequencenumber: number | null }, {  }>;
    modifiedby_guid: WebAttribute<ctx_Product_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<ctx_Product_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<ctx_Product_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    overriddencreatedon: WebAttribute<ctx_Product_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    ownerid_guid: WebAttribute<ctx_Product_Select, { ownerid_guid: string | null }, { ownerid_formatted?: string }>;
    owningbusinessunit_guid: WebAttribute<ctx_Product_Select, { owningbusinessunit_guid: string | null }, { owningbusinessunit_formatted?: string }>;
    owningteam_guid: WebAttribute<ctx_Product_Select, { owningteam_guid: string | null }, { owningteam_formatted?: string }>;
    owninguser_guid: WebAttribute<ctx_Product_Select, { owninguser_guid: string | null }, { owninguser_formatted?: string }>;
    statecode: WebAttribute<ctx_Product_Select, { statecode: ctx_product_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<ctx_Product_Select, { statuscode: ctx_product_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<ctx_Product_Select, { timezoneruleversionnumber: number | null }, {  }>;
    transactioncurrencyid_guid: WebAttribute<ctx_Product_Select, { transactioncurrencyid_guid: string | null }, { transactioncurrencyid_formatted?: string }>;
    utcconversiontimezonecode: WebAttribute<ctx_Product_Select, { utcconversiontimezonecode: number | null }, {  }>;
    versionnumber: WebAttribute<ctx_Product_Select, { versionnumber: number | null }, {  }>;
  }
  interface ctx_Product_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    ctx_billinginterval: ctx_billinginterval;
    ctx_name: string;
    ctx_price: number;
    ctx_price_base: number;
    ctx_productid: XQW.Guid;
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
    statecode: ctx_product_statecode;
    statuscode: ctx_product_statuscode;
    timezoneruleversionnumber: number;
    transactioncurrencyid_guid: XQW.Guid;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface ctx_Product_Expand {
    ctx_Subscription_ctx_Product: WebExpand<ctx_Product_Expand, ctx_Subscription_Select, ctx_Subscription_Filter, { ctx_Subscription_ctx_Product: ctx_Subscription_Result[] }>;
  }
  interface ctx_Product_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    ctx_billinginterval_formatted?: string;
    ctx_price_base_formatted?: string;
    ctx_price_formatted?: string;
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
  interface ctx_Product_Result extends ctx_Product_Base, ctx_Product_Relationships {
    "@odata.etag": string;
    createdby_guid: string | null;
    createdonbehalfby_guid: string | null;
    modifiedby_guid: string | null;
    modifiedonbehalfby_guid: string | null;
    ownerid_guid: string | null;
    owningbusinessunit_guid: string | null;
    owningteam_guid: string | null;
    owninguser_guid: string | null;
    transactioncurrencyid_guid: string | null;
  }
  interface ctx_Product_RelatedOne {
  }
  interface ctx_Product_RelatedMany {
    ctx_Subscription_ctx_Product: WebMappingRetrieve<XDT.ctx_Subscription_Select,XDT.ctx_Subscription_Expand,XDT.ctx_Subscription_Filter,XDT.ctx_Subscription_Fixed,XDT.ctx_Subscription_Result,XDT.ctx_Subscription_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  ctx_products: WebMappingRetrieve<XDT.ctx_Product_Select,XDT.ctx_Product_Expand,XDT.ctx_Product_Filter,XDT.ctx_Product_Fixed,XDT.ctx_Product_Result,XDT.ctx_Product_FormattedResult>;
}
interface WebEntitiesRelated {
  ctx_products: WebMappingRelated<XDT.ctx_Product_RelatedOne,XDT.ctx_Product_RelatedMany>;
}
interface WebEntitiesCUDA {
  ctx_products: WebMappingCUDA<XDT.ctx_Product_Create,XDT.ctx_Product_Update,XDT.ctx_Product_Select>;
}
