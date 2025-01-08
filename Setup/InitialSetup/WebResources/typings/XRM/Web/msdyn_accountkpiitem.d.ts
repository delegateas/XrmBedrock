declare namespace XDT {
  interface msdyn_accountkpiitem_Base extends WebEntity {
    createdon?: Date | null;
    importsequencenumber?: number | null;
    modifiedon?: Date | null;
    msdyn_accountkpiitemid?: string | null;
    msdyn_acilastupdatetimestamp?: Date | null;
    msdyn_averagefirstresponsetimebytheminhrs?: number | null;
    msdyn_averagefirstresponsetimebyusinhrs?: number | null;
    msdyn_computationaccuracy?: msdyn_kpicomputationaccuracy | null;
    msdyn_date?: Date | null;
    msdyn_emailattachmentopens?: number | null;
    msdyn_emaillinksclicked?: number | null;
    msdyn_emailsreceived?: number | null;
    msdyn_emailssent?: number | null;
    msdyn_facetimewithcustomer?: number | null;
    msdyn_liemailssent?: number | null;
    msdyn_meetingsreceived?: number | null;
    msdyn_meetingssent?: number | null;
    msdyn_name?: string | null;
    msdyn_openedemails?: number | null;
    msdyn_phonecallsmade?: number | null;
    msdyn_phonecallsreceived?: number | null;
    msdyn_relationshiphealthscorestate?: msdyn_msdyn_accountkpiitem_msdyn_relationshiphealthscorestate | null;
    msdyn_relationshiphealthscorevalue?: number | null;
    msdyn_relationshiphealthtrend?: msdyn_msdyn_accountkpiitem_msdyn_relationshiphealthtrend | null;
    msdyn_responseratebythem?: number | null;
    msdyn_responseratebyus?: number | null;
    msdyn_timespentbycustomer?: number | null;
    msdyn_timespentbycustomer_calculated?: number | null;
    msdyn_timespentbyteam?: number | null;
    msdyn_timespentbyteam_calculated?: number | null;
    overriddencreatedon?: Date | null;
    statecode?: msdyn_accountkpiitem_statecode | null;
    statuscode?: msdyn_accountkpiitem_statuscode | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
    versionnumber?: number | null;
  }
  interface msdyn_accountkpiitem_Relationships {
    msdyn_msdyn_accountkpiitem_account_accountkpiid?: Account_Result[] | null;
  }
  interface msdyn_accountkpiitem extends msdyn_accountkpiitem_Base, msdyn_accountkpiitem_Relationships {
    msdyn_accountid_bind$accounts?: string | null;
  }
  interface msdyn_accountkpiitem_Create extends msdyn_accountkpiitem {
  }
  interface msdyn_accountkpiitem_Update extends msdyn_accountkpiitem {
  }
  interface msdyn_accountkpiitem_Select {
    createdby_guid: WebAttribute<msdyn_accountkpiitem_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<msdyn_accountkpiitem_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<msdyn_accountkpiitem_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    importsequencenumber: WebAttribute<msdyn_accountkpiitem_Select, { importsequencenumber: number | null }, {  }>;
    modifiedby_guid: WebAttribute<msdyn_accountkpiitem_Select, { modifiedby_guid: string | null }, { modifiedby_formatted?: string }>;
    modifiedon: WebAttribute<msdyn_accountkpiitem_Select, { modifiedon: Date | null }, { modifiedon_formatted?: string }>;
    modifiedonbehalfby_guid: WebAttribute<msdyn_accountkpiitem_Select, { modifiedonbehalfby_guid: string | null }, { modifiedonbehalfby_formatted?: string }>;
    msdyn_accountid_guid: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_accountid_guid: string | null }, { msdyn_accountid_formatted?: string }>;
    msdyn_accountkpiitemid: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_accountkpiitemid: string | null }, {  }>;
    msdyn_acilastupdatetimestamp: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_acilastupdatetimestamp: Date | null }, { msdyn_acilastupdatetimestamp_formatted?: string }>;
    msdyn_averagefirstresponsetimebytheminhrs: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_averagefirstresponsetimebytheminhrs: number | null }, {  }>;
    msdyn_averagefirstresponsetimebyusinhrs: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_averagefirstresponsetimebyusinhrs: number | null }, {  }>;
    msdyn_computationaccuracy: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_computationaccuracy: msdyn_kpicomputationaccuracy | null }, { msdyn_computationaccuracy_formatted?: string }>;
    msdyn_date: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_date: Date | null }, { msdyn_date_formatted?: string }>;
    msdyn_emailattachmentopens: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_emailattachmentopens: number | null }, {  }>;
    msdyn_emaillinksclicked: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_emaillinksclicked: number | null }, {  }>;
    msdyn_emailsreceived: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_emailsreceived: number | null }, {  }>;
    msdyn_emailssent: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_emailssent: number | null }, {  }>;
    msdyn_facetimewithcustomer: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_facetimewithcustomer: number | null }, {  }>;
    msdyn_liemailssent: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_liemailssent: number | null }, {  }>;
    msdyn_meetingsreceived: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_meetingsreceived: number | null }, {  }>;
    msdyn_meetingssent: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_meetingssent: number | null }, {  }>;
    msdyn_name: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_name: string | null }, {  }>;
    msdyn_openedemails: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_openedemails: number | null }, {  }>;
    msdyn_phonecallsmade: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_phonecallsmade: number | null }, {  }>;
    msdyn_phonecallsreceived: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_phonecallsreceived: number | null }, {  }>;
    msdyn_relationshiphealthscorestate: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_relationshiphealthscorestate: msdyn_msdyn_accountkpiitem_msdyn_relationshiphealthscorestate | null }, { msdyn_relationshiphealthscorestate_formatted?: string }>;
    msdyn_relationshiphealthscorevalue: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_relationshiphealthscorevalue: number | null }, {  }>;
    msdyn_relationshiphealthtrend: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_relationshiphealthtrend: msdyn_msdyn_accountkpiitem_msdyn_relationshiphealthtrend | null }, { msdyn_relationshiphealthtrend_formatted?: string }>;
    msdyn_responseratebythem: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_responseratebythem: number | null }, {  }>;
    msdyn_responseratebyus: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_responseratebyus: number | null }, {  }>;
    msdyn_timespentbycustomer: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_timespentbycustomer: number | null }, {  }>;
    msdyn_timespentbycustomer_calculated: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_timespentbycustomer_calculated: number | null }, {  }>;
    msdyn_timespentbyteam: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_timespentbyteam: number | null }, {  }>;
    msdyn_timespentbyteam_calculated: WebAttribute<msdyn_accountkpiitem_Select, { msdyn_timespentbyteam_calculated: number | null }, {  }>;
    organizationid_guid: WebAttribute<msdyn_accountkpiitem_Select, { organizationid_guid: string | null }, { organizationid_formatted?: string }>;
    overriddencreatedon: WebAttribute<msdyn_accountkpiitem_Select, { overriddencreatedon: Date | null }, { overriddencreatedon_formatted?: string }>;
    statecode: WebAttribute<msdyn_accountkpiitem_Select, { statecode: msdyn_accountkpiitem_statecode | null }, { statecode_formatted?: string }>;
    statuscode: WebAttribute<msdyn_accountkpiitem_Select, { statuscode: msdyn_accountkpiitem_statuscode | null }, { statuscode_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<msdyn_accountkpiitem_Select, { timezoneruleversionnumber: number | null }, {  }>;
    utcconversiontimezonecode: WebAttribute<msdyn_accountkpiitem_Select, { utcconversiontimezonecode: number | null }, {  }>;
    versionnumber: WebAttribute<msdyn_accountkpiitem_Select, { versionnumber: number | null }, {  }>;
  }
  interface msdyn_accountkpiitem_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    importsequencenumber: number;
    modifiedby_guid: XQW.Guid;
    modifiedon: Date;
    modifiedonbehalfby_guid: XQW.Guid;
    msdyn_accountid_guid: XQW.Guid;
    msdyn_accountkpiitemid: XQW.Guid;
    msdyn_acilastupdatetimestamp: Date;
    msdyn_averagefirstresponsetimebytheminhrs: any;
    msdyn_averagefirstresponsetimebyusinhrs: any;
    msdyn_computationaccuracy: msdyn_kpicomputationaccuracy;
    msdyn_date: Date;
    msdyn_emailattachmentopens: any;
    msdyn_emaillinksclicked: any;
    msdyn_emailsreceived: number;
    msdyn_emailssent: number;
    msdyn_facetimewithcustomer: number;
    msdyn_liemailssent: number;
    msdyn_meetingsreceived: number;
    msdyn_meetingssent: number;
    msdyn_name: string;
    msdyn_openedemails: any;
    msdyn_phonecallsmade: number;
    msdyn_phonecallsreceived: number;
    msdyn_relationshiphealthscorestate: msdyn_msdyn_accountkpiitem_msdyn_relationshiphealthscorestate;
    msdyn_relationshiphealthscorevalue: number;
    msdyn_relationshiphealthtrend: msdyn_msdyn_accountkpiitem_msdyn_relationshiphealthtrend;
    msdyn_responseratebythem: any;
    msdyn_responseratebyus: any;
    msdyn_timespentbycustomer: number;
    msdyn_timespentbycustomer_calculated: any;
    msdyn_timespentbyteam: number;
    msdyn_timespentbyteam_calculated: any;
    organizationid_guid: XQW.Guid;
    overriddencreatedon: Date;
    statecode: msdyn_accountkpiitem_statecode;
    statuscode: msdyn_accountkpiitem_statuscode;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
  }
  interface msdyn_accountkpiitem_Expand {
    msdyn_accountid: WebExpand<msdyn_accountkpiitem_Expand, Account_Select, Account_Filter, { msdyn_accountid: Account_Result }>;
    msdyn_msdyn_accountkpiitem_account_accountkpiid: WebExpand<msdyn_accountkpiitem_Expand, Account_Select, Account_Filter, { msdyn_msdyn_accountkpiitem_account_accountkpiid: Account_Result[] }>;
  }
  interface msdyn_accountkpiitem_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    modifiedby_formatted?: string;
    modifiedon_formatted?: string;
    modifiedonbehalfby_formatted?: string;
    msdyn_accountid_formatted?: string;
    msdyn_acilastupdatetimestamp_formatted?: string;
    msdyn_computationaccuracy_formatted?: string;
    msdyn_date_formatted?: string;
    msdyn_relationshiphealthscorestate_formatted?: string;
    msdyn_relationshiphealthtrend_formatted?: string;
    organizationid_formatted?: string;
    overriddencreatedon_formatted?: string;
    statecode_formatted?: string;
    statuscode_formatted?: string;
  }
  interface msdyn_accountkpiitem_Result extends msdyn_accountkpiitem_Base, msdyn_accountkpiitem_Relationships {
    "@odata.etag": string;
    createdby_guid: string | null;
    createdonbehalfby_guid: string | null;
    modifiedby_guid: string | null;
    modifiedonbehalfby_guid: string | null;
    msdyn_accountid_guid: string | null;
    organizationid_guid: string | null;
  }
  interface msdyn_accountkpiitem_RelatedOne {
    msdyn_accountid: WebMappingRetrieve<XDT.Account_Select,XDT.Account_Expand,XDT.Account_Filter,XDT.Account_Fixed,XDT.Account_Result,XDT.Account_FormattedResult>;
  }
  interface msdyn_accountkpiitem_RelatedMany {
    msdyn_msdyn_accountkpiitem_account_accountkpiid: WebMappingRetrieve<XDT.Account_Select,XDT.Account_Expand,XDT.Account_Filter,XDT.Account_Fixed,XDT.Account_Result,XDT.Account_FormattedResult>;
  }
}
interface WebEntitiesRetrieve {
  msdyn_accountkpiitems: WebMappingRetrieve<XDT.msdyn_accountkpiitem_Select,XDT.msdyn_accountkpiitem_Expand,XDT.msdyn_accountkpiitem_Filter,XDT.msdyn_accountkpiitem_Fixed,XDT.msdyn_accountkpiitem_Result,XDT.msdyn_accountkpiitem_FormattedResult>;
}
interface WebEntitiesRelated {
  msdyn_accountkpiitems: WebMappingRelated<XDT.msdyn_accountkpiitem_RelatedOne,XDT.msdyn_accountkpiitem_RelatedMany>;
}
interface WebEntitiesCUDA {
  msdyn_accountkpiitems: WebMappingCUDA<XDT.msdyn_accountkpiitem_Create,XDT.msdyn_accountkpiitem_Update,XDT.msdyn_accountkpiitem_Select>;
}
