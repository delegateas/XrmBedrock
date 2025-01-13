declare namespace XDT {
  interface PostFollow_Base extends WebEntity {
    createdon?: Date | null;
    postfollowid?: string | null;
    posttoyammer?: boolean | null;
    timezoneruleversionnumber?: number | null;
    utcconversiontimezonecode?: number | null;
    versionnumber?: number | null;
    yammerpoststate?: number | null;
    yammerretrycount?: number | null;
  }
  interface PostFollow_Relationships {
    regardingobjectid_account?: Account_Result | null;
    regardingobjectid_contact?: Contact_Result | null;
  }
  interface PostFollow extends PostFollow_Base, PostFollow_Relationships {
  }
  interface PostFollow_Create extends PostFollow {
    ownerid_bind$systemusers?: string | null;
    ownerid_bind$teams?: string | null;
    regardingobjectid_account_bind$accounts?: string | null;
    regardingobjectid_appointment_bind$appointments?: string | null;
    regardingobjectid_competitor_bind$competitors?: string | null;
    regardingobjectid_contact_bind$contacts?: string | null;
    regardingobjectid_incident_bind$incidents?: string | null;
    regardingobjectid_knowledgearticle_bind$knowledgearticles?: string | null;
    regardingobjectid_lead_bind$leads?: string | null;
    regardingobjectid_msdyn_salessuggestion_bind$msdyn_salessuggestions?: string | null;
    regardingobjectid_msdyn_swarm_bind$msdyn_swarms?: string | null;
    regardingobjectid_opportunity_bind$opportunities?: string | null;
    regardingobjectid_phonecall_bind$phonecalls?: string | null;
    regardingobjectid_processsession_bind$processsessions?: string | null;
    regardingobjectid_queue_bind$queues?: string | null;
    regardingobjectid_recurringappointmentmaster_bind$recurringappointmentmasters?: string | null;
    regardingobjectid_systemuser_bind$systemusers?: string | null;
    regardingobjectid_task_bind$tasks?: string | null;
  }
  interface PostFollow_Update extends PostFollow {
  }
  interface PostFollow_Select {
    createdby_guid: WebAttribute<PostFollow_Select, { createdby_guid: string | null }, { createdby_formatted?: string }>;
    createdon: WebAttribute<PostFollow_Select, { createdon: Date | null }, { createdon_formatted?: string }>;
    createdonbehalfby_guid: WebAttribute<PostFollow_Select, { createdonbehalfby_guid: string | null }, { createdonbehalfby_formatted?: string }>;
    ownerid_guid: WebAttribute<PostFollow_Select, { ownerid_guid: string | null }, { ownerid_formatted?: string }>;
    owningbusinessunit_guid: WebAttribute<PostFollow_Select, { owningbusinessunit_guid: string | null }, { owningbusinessunit_formatted?: string }>;
    owningteam_guid: WebAttribute<PostFollow_Select, { owningteam_guid: string | null }, { owningteam_formatted?: string }>;
    owninguser_guid: WebAttribute<PostFollow_Select, { owninguser_guid: string | null }, { owninguser_formatted?: string }>;
    postfollowid: WebAttribute<PostFollow_Select, { postfollowid: string | null }, {  }>;
    posttoyammer: WebAttribute<PostFollow_Select, { posttoyammer: boolean | null }, {  }>;
    regardingobjectid_guid: WebAttribute<PostFollow_Select, { regardingobjectid_guid: string | null }, { regardingobjectid_formatted?: string }>;
    timezoneruleversionnumber: WebAttribute<PostFollow_Select, { timezoneruleversionnumber: number | null }, {  }>;
    utcconversiontimezonecode: WebAttribute<PostFollow_Select, { utcconversiontimezonecode: number | null }, {  }>;
    versionnumber: WebAttribute<PostFollow_Select, { versionnumber: number | null }, {  }>;
    yammerpoststate: WebAttribute<PostFollow_Select, { yammerpoststate: number | null }, {  }>;
    yammerretrycount: WebAttribute<PostFollow_Select, { yammerretrycount: number | null }, {  }>;
  }
  interface PostFollow_Filter {
    createdby_guid: XQW.Guid;
    createdon: Date;
    createdonbehalfby_guid: XQW.Guid;
    ownerid_guid: XQW.Guid;
    owningbusinessunit_guid: XQW.Guid;
    owningteam_guid: XQW.Guid;
    owninguser_guid: XQW.Guid;
    postfollowid: XQW.Guid;
    posttoyammer: boolean;
    regardingobjectid_guid: XQW.Guid;
    timezoneruleversionnumber: number;
    utcconversiontimezonecode: number;
    versionnumber: number;
    yammerpoststate: number;
    yammerretrycount: number;
  }
  interface PostFollow_Expand {
    regardingobjectid_account: WebExpand<PostFollow_Expand, Account_Select, Account_Filter, { regardingobjectid_account: Account_Result }>;
    regardingobjectid_contact: WebExpand<PostFollow_Expand, Contact_Select, Contact_Filter, { regardingobjectid_contact: Contact_Result }>;
  }
  interface PostFollow_FormattedResult {
    createdby_formatted?: string;
    createdon_formatted?: string;
    createdonbehalfby_formatted?: string;
    ownerid_formatted?: string;
    owningbusinessunit_formatted?: string;
    owningteam_formatted?: string;
    owninguser_formatted?: string;
    regardingobjectid_formatted?: string;
  }
  interface PostFollow_Result extends PostFollow_Base, PostFollow_Relationships {
    "@odata.etag": string;
    createdby_guid: string | null;
    createdonbehalfby_guid: string | null;
    ownerid_guid: string | null;
    owningbusinessunit_guid: string | null;
    owningteam_guid: string | null;
    owninguser_guid: string | null;
    regardingobjectid_guid: string | null;
  }
  interface PostFollow_RelatedOne {
    regardingobjectid_account: WebMappingRetrieve<XDT.Account_Select,XDT.Account_Expand,XDT.Account_Filter,XDT.Account_Fixed,XDT.Account_Result,XDT.Account_FormattedResult>;
    regardingobjectid_contact: WebMappingRetrieve<XDT.Contact_Select,XDT.Contact_Expand,XDT.Contact_Filter,XDT.Contact_Fixed,XDT.Contact_Result,XDT.Contact_FormattedResult>;
  }
  interface PostFollow_RelatedMany {
  }
}
interface WebEntitiesRetrieve {
  postfollows: WebMappingRetrieve<XDT.PostFollow_Select,XDT.PostFollow_Expand,XDT.PostFollow_Filter,XDT.PostFollow_Fixed,XDT.PostFollow_Result,XDT.PostFollow_FormattedResult>;
}
interface WebEntitiesRelated {
  postfollows: WebMappingRelated<XDT.PostFollow_RelatedOne,XDT.PostFollow_RelatedMany>;
}
interface WebEntitiesCUDA {
  postfollows: WebMappingCUDA<XDT.PostFollow_Create,XDT.PostFollow_Update,XDT.PostFollow_Select>;
}
