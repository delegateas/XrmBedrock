declare namespace XDT {
  interface BulkOperationLog_Base extends WebEntity {
    additionalinfo?: string | null;
    bulkoperationlogid?: string | null;
    errordescriptionformatted?: string | null;
    errornumber?: number | null;
    errornumberformatted?: string | null;
    importsequencenumber?: number | null;
    name?: string | null;
    overriddencreatedon?: Date | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
    versionnumber?: number | null;
  }
  interface BulkOperationLog_Relationships {
    BulkOperation_Logs_Accounts?: Account_Result[] | null;
    BulkOperation_Logs_Contacts?: Contact_Result[] | null;
    CampaignActivity_Logs_Accounts?: Account_Result[] | null;
    CampaignActivity_Logs_Contacts?: Contact_Result[] | null;
    createdobjectid_account?: Account_Result | null;
    createdobjectid_contact?: Contact_Result | null;
    regardingobjectid_account?: Account_Result | null;
    regardingobjectid_contact?: Contact_Result | null;
  }
  interface BulkOperationLog extends BulkOperationLog_Base, BulkOperationLog_Relationships {
  }
  interface BulkOperationLog_Create extends BulkOperationLog {
    CampaignActivityId_Logs_bind$campaignactivities?: string | null;
    bulkoperationid_activitypointer_bind$activitypointers?: string | null;
    bulkoperationid_bind$bulkoperations?: string | null;
    createdobjectid_account_bind$accounts?: string | null;
    createdobjectid_activitypointer_bind$activitypointers?: string | null;
    createdobjectid_contact_bind$contacts?: string | null;
    createdobjectid_lead_bind$leads?: string | null;
    createdobjectid_opportunity_bind$opportunities?: string | null;
    regardingobjectid_account_bind$accounts?: string | null;
    regardingobjectid_contact_bind$contacts?: string | null;
    regardingobjectid_lead_bind$leads?: string | null;
  }
  interface BulkOperationLog_Update extends BulkOperationLog {
  }
  interface BulkOperationLog_Select {
    additionalinfo: WebAttribute<BulkOperationLog_Select, { additionalinfo: string | null }, {  }>;
    bulkoperationid_guid: WebAttribute<BulkOperationLog_Select, { bulkoperationid_guid: string | null }, { bulkoperationid_formatted?: string }>;
    bulkoperationlogid: WebAttribute<BulkOperationLog_Select, { bulkoperationlogid: string | null }, {  }>;
    campaignactivityid_guid: WebAttribute<BulkOperationLog_Select, { campaignactivityid_guid: string | null }, { campaignactivityid_formatted?: string }>;
    createdobjectid_guid: WebAttribute<BulkOperationLog_Select, { createdobjectid_guid: string | null }, { createdobjectid_formatted?: string }>;
    errordescriptionformatted: WebAttribute<BulkOperationLog_Select, { errordescriptionformatted: string | null }, {  }>;
    errornumber: WebAttribute<BulkOperationLog_Select, { errornumber: number | null }, {  }>;
    errornumberformatted: WebAttribute<BulkOperationLog_Select, { errornumberformatted: string | null }, {  }>;
    importsequencenumber: WebAttribute<BulkOperationLog_Select, { importsequencenumber: number | null }, {  }>;
    name: WebAttribute<BulkOperationLog_Select, { name: string | null }, {  }>;
    overriddencreatedon: WebAttribute<BulkOperationLog_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    ownerid_guid: WebAttribute<BulkOperationLog_Select, { ownerid_guid: string | null }, { ownerid_formatted?: string }>;
    owningbusinessunit_guid: WebAttribute<BulkOperationLog_Select, { owningbusinessunit_guid: string | null }, { owningbusinessunit_formatted?: string }>;
    owningteam_guid: WebAttribute<BulkOperationLog_Select, { owningteam_guid: string | null }, { owningteam_formatted?: string }>;
    owninguser_guid: WebAttribute<BulkOperationLog_Select, { owninguser_guid: string | null }, { owninguser_formatted?: string }>;
    regardingobjectid_guid: WebAttribute<BulkOperationLog_Select, { regardingobjectid_guid: string | null }, { regardingobjectid_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<BulkOperationLog_Select, { timezoneruleversionnumber: number | null }, {  }>;
    utcconversiontimezonecode: WebAttribute<BulkOperationLog_Select, { utcconversiontimezonecode: number | null }, {  }>;
    versionnumber: WebAttribute<BulkOperationLog_Select, { versionnumber: number | null }, {  }>;
  }
  interface BulkOperationLog_Filter {
    additionalinfo: string;
    bulkoperationid_guid: XQW.Guid;
    bulkoperationlogid: XQW.Guid;
    campaignactivityid_guid: XQW.Guid;
    createdobjectid_guid: XQW.Guid;
    errordescriptionformatted: string;
    errornumber: number;
    errornumberformatted: string;
    importsequencenumber: number;
    name: string;
    overriddencreatedon: Date;
    ownerid_guid: XQW.Guid;
    owningbusinessunit_guid: XQW.Guid;
    owningteam_guid: XQW.Guid;
    owninguser_guid: XQW.Guid;
    regardingobjectid_guid: XQW.Guid;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface BulkOperationLog_Expand {
    BulkOperation_Logs_Accounts: WebExpand<BulkOperationLog_Expand, Account_Select, Account_Filter, { BulkOperation_Logs_Accounts: Account_Result[] }>;
    BulkOperation_Logs_Contacts: WebExpand<BulkOperationLog_Expand, Contact_Select, Contact_Filter, { BulkOperation_Logs_Contacts: Contact_Result[] }>;
    CampaignActivity_Logs_Accounts: WebExpand<BulkOperationLog_Expand, Account_Select, Account_Filter, { CampaignActivity_Logs_Accounts: Account_Result[] }>;
    CampaignActivity_Logs_Contacts: WebExpand<BulkOperationLog_Expand, Contact_Select, Contact_Filter, { CampaignActivity_Logs_Contacts: Contact_Result[] }>;
    createdobjectid_account: WebExpand<BulkOperationLog_Expand, Account_Select, Account_Filter, { createdobjectid_account: Account_Result }>;
    createdobjectid_contact: WebExpand<BulkOperationLog_Expand, Contact_Select, Contact_Filter, { createdobjectid_contact: Contact_Result }>;
    regardingobjectid_account: WebExpand<BulkOperationLog_Expand, Account_Select, Account_Filter, { regardingobjectid_account: Account_Result }>;
    regardingobjectid_contact: WebExpand<BulkOperationLog_Expand, Contact_Select, Contact_Filter, { regardingobjectid_contact: Contact_Result }>;
  }
  interface BulkOperationLog_FormattedResult {
    bulkoperationid_formatted?: string;
    campaignactivityid_formatted?: string;
    createdobjectid_formatted?: string;
    overriddencreatedon_formatted?: string;
    ownerid_formatted?: string;
    owningbusinessunit_formatted?: string;
    owningteam_formatted?: string;
    owninguser_formatted?: string;
    regardingobjectid_formatted?: string;
  }
  interface BulkOperationLog_Result extends BulkOperationLog_Base, BulkOperationLog_Relationships {
    "@odata.etag": string;
    bulkoperationid_guid: string | null;
    campaignactivityid_guid: string | null;
    createdobjectid_guid: string | null;
    ownerid_guid: string | null;
    owningbusinessunit_guid: string | null;
    owningteam_guid: string | null;
    owninguser_guid: string | null;
    regardingobjectid_guid: string | null;
  }
  interface BulkOperationLog_RelatedOne {
    createdobjectid_account: WebMappingRetrieve<XDT.Account_Select,XDT.Account_Expand,XDT.Account_Filter,XDT.Account_Fixed,XDT.Account_Result,XDT.Account_FormattedResult>;
    createdobjectid_contact: WebMappingRetrieve<XDT.Contact_Select,XDT.Contact_Expand,XDT.Contact_Filter,XDT.Contact_Fixed,XDT.Contact_Result,XDT.Contact_FormattedResult>;
    regardingobjectid_account: WebMappingRetrieve<XDT.Account_Select,XDT.Account_Expand,XDT.Account_Filter,XDT.Account_Fixed,XDT.Account_Result,XDT.Account_FormattedResult>;
    regardingobjectid_contact: WebMappingRetrieve<XDT.Contact_Select,XDT.Contact_Expand,XDT.Contact_Filter,XDT.Contact_Fixed,XDT.Contact_Result,XDT.Contact_FormattedResult>;
  }
  interface BulkOperationLog_RelatedMany {
    BulkOperation_Logs_Accounts: WebMappingRetrieve<XDT.Account_Select,XDT.Account_Expand,XDT.Account_Filter,XDT.Account_Fixed,XDT.Account_Result,XDT.Account_FormattedResult>;
    BulkOperation_Logs_Contacts: WebMappingRetrieve<XDT.Contact_Select,XDT.Contact_Expand,XDT.Contact_Filter,XDT.Contact_Fixed,XDT.Contact_Result,XDT.Contact_FormattedResult>;
    CampaignActivity_Logs_Accounts: WebMappingRetrieve<XDT.Account_Select,XDT.Account_Expand,XDT.Account_Filter,XDT.Account_Fixed,XDT.Account_Result,XDT.Account_FormattedResult>;
    CampaignActivity_Logs_Contacts: WebMappingRetrieve<XDT.Contact_Select,XDT.Contact_Expand,XDT.Contact_Filter,XDT.Contact_Fixed,XDT.Contact_Result,XDT.Contact_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  bulkoperationlogs: WebMappingRetrieve<XDT.BulkOperationLog_Select,XDT.BulkOperationLog_Expand,XDT.BulkOperationLog_Filter,XDT.BulkOperationLog_Fixed,XDT.BulkOperationLog_Result,XDT.BulkOperationLog_FormattedResult>;
}
interface WebEntitiesRelated {
  bulkoperationlogs: WebMappingRelated<XDT.BulkOperationLog_RelatedOne,XDT.BulkOperationLog_RelatedMany>;
}
interface WebEntitiesCUDA {
  bulkoperationlogs: WebMappingCUDA<XDT.BulkOperationLog_Create,XDT.BulkOperationLog_Update,XDT.BulkOperationLog_Select>;
}
