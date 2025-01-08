declare namespace XDT {
  interface mgs_Product_Base extends WebEntity {
    createdon?: Date | null;
    exchangerate?: number | null;
    importsequencenumber?: number | null;
    mgs_billinginterval?: mgs_billinginterval | null;
    mgs_name?: string | null;
    mgs_price?: number | null;
    mgs_price_base?: number | null;
    mgs_productid?: string | null;
    modifiedon?: Date | null;
    overriddencreatedon?: Date | null;
    statecode?: mgs_product_statecode | null;
    statuscode?: mgs_product_statuscode | null;
    timezoneruleversionnumber?: number | null;
    transactioncurrencyid_guid?: string | null;
    utcconversiontimezonecode?: number | null;
    versionnumber?: number | null;
  }
  interface mgs_Product_Relationships {
    mgs_subscription_Product_mgs_product?: mgs_Subscription_Result[] | null;
  }
  interface mgs_Product extends mgs_Product_Base, mgs_Product_Relationships {
    ownerid_bind$systemusers?: string | null;
    ownerid_bind$teams?: string | null;
    transactioncurrencyid_bind$transactioncurrencies?: string | null;
  }
  interface mgs_Product_Create extends mgs_Product {
  }
  interface mgs_Product_Update extends mgs_Product {
  }
  interface mgs_Product_Select {
    createdby_guid: WebAttribute<mgs_Product_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<mgs_Product_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<mgs_Product_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    exchangerate: WebAttribute<mgs_Product_Select, { exchangerate: number | null }, {  }>;
    importsequencenumber: WebAttribute<mgs_Product_Select, { importsequencenumber: number | null }, {  }>;
    mgs_billinginterval: WebAttribute<mgs_Product_Select, { mgs_billinginterval: mgs_billinginterval | null }, { mgs_billinginterval_formatted?: string }>;
    mgs_name: WebAttribute<mgs_Product_Select, { mgs_name: string | null }, {  }>;
    mgs_price: WebAttribute<mgs_Product_Select, { mgs_price: number | null; transactioncurrencyid_guid: string | null }, { mgs_price_formatted?: string; transactioncurrencyid_formatted?: string }>;
    mgs_price_base: WebAttribute<mgs_Product_Select, { mgs_price_base: number | null; transactioncurrencyid_guid: string | null }, { mgs_price_base_formatted?: string; transactioncurrencyid_formatted?: string }>;
    mgs_productid: WebAttribute<mgs_Product_Select, { mgs_productid: string | null }, {  }>;
    modifiedby_guid: WebAttribute<mgs_Product_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<mgs_Product_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<mgs_Product_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    overriddencreatedon: WebAttribute<mgs_Product_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    ownerid_guid: WebAttribute<mgs_Product_Select, { ownerid_guid: string | null }, { ownerid_formatted?: string }>;
    owningbusinessunit_guid: WebAttribute<mgs_Product_Select, { owningbusinessunit_guid: string | null }, { owningbusinessunit_formatted?: string }>;
    owningteam_guid: WebAttribute<mgs_Product_Select, { owningteam_guid: string | null }, { owningteam_formatted?: string }>;
    owninguser_guid: WebAttribute<mgs_Product_Select, { owninguser_guid: string | null }, { owninguser_formatted?: string }>;
    statecode: WebAttribute<mgs_Product_Select, { statecode: mgs_product_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<mgs_Product_Select, { statuscode: mgs_product_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<mgs_Product_Select, { timezoneruleversionnumber: number | null }, {  }>;
    transactioncurrencyid_guid: WebAttribute<mgs_Product_Select, { transactioncurrencyid_guid: string | null }, { transactioncurrencyid_formatted?: string }>;
    utcconversiontimezonecode: WebAttribute<mgs_Product_Select, { utcconversiontimezonecode: number | null }, {  }>;
    versionnumber: WebAttribute<mgs_Product_Select, { versionnumber: number | null }, {  }>;
  }
  interface mgs_Product_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    exchangerate: any;
    importsequencenumber: number;
    mgs_billinginterval: mgs_billinginterval;
    mgs_name: string;
    mgs_price: number;
    mgs_price_base: number;
    mgs_productid: XQW.Guid;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    overriddencreatedon: Date;
    ownerid_guid: XQW.Guid;
    owningbusinessunit_guid: XQW.Guid;
    owningteam_guid: XQW.Guid;
    owninguser_guid: XQW.Guid;
    statecode: mgs_product_statecode;
    statuscode: mgs_product_statuscode;
    timezoneruleversionnumber: number;
    transactioncurrencyid_guid: XQW.Guid;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface mgs_Product_Expand {
    mgs_subscription_Product_mgs_product: WebExpand<mgs_Product_Expand, mgs_Subscription_Select, mgs_Subscription_Filter, { mgs_subscription_Product_mgs_product: mgs_Subscription_Result[] }>;
  }
  interface mgs_Product_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    mgs_billinginterval_formatted?: string;
    mgs_price_base_formatted?: string;
    mgs_price_formatted?: string;
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
  interface mgs_Product_Result extends mgs_Product_Base, mgs_Product_Relationships {
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
  interface mgs_Product_RelatedOne {
  }
  interface mgs_Product_RelatedMany {
    mgs_subscription_Product_mgs_product: WebMappingRetrieve<XDT.mgs_Subscription_Select,XDT.mgs_Subscription_Expand,XDT.mgs_Subscription_Filter,XDT.mgs_Subscription_Fixed,XDT.mgs_Subscription_Result,XDT.mgs_Subscription_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  mgs_products: WebMappingRetrieve<XDT.mgs_Product_Select,XDT.mgs_Product_Expand,XDT.mgs_Product_Filter,XDT.mgs_Product_Fixed,XDT.mgs_Product_Result,XDT.mgs_Product_FormattedResult>;
}
interface WebEntitiesRelated {
  mgs_products: WebMappingRelated<XDT.mgs_Product_RelatedOne,XDT.mgs_Product_RelatedMany>;
}
interface WebEntitiesCUDA {
  mgs_products: WebMappingCUDA<XDT.mgs_Product_Create,XDT.mgs_Product_Update,XDT.mgs_Product_Select>;
}
