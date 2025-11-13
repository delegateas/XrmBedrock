declare namespace XDT {
  interface ActivityParty_Base extends WebEntity {
    activitypartyid?: string | null;
    addressused?: string | null;
    addressusedemailcolumnnumber?: number | null;
    donotemail?: boolean | null;
    donotfax?: boolean | null;
    donotphone?: boolean | null;
    donotpostalmail?: boolean | null;
    effort?: number | null;
    exchangeentryid?: string | null;
    externalid?: string | null;
    externalidtype?: string | null;
    instancetypecode?: activityparty_instancetypecode | null;
    ispartydeleted?: boolean | null;
    owningbusinessunit?: string | null;
    owninguser?: string | null;
    participationtypemask?: activityparty_participationtypemask | null;
    scheduledend?: Date | null;
    scheduledstart?: Date | null;
    unresolvedpartyname?: string | null;
    versionnumber?: number | null;
  }
  interface ActivityParty_Relationships {
    partyid_account?: Account_Result | null;
    partyid_contact?: Contact_Result | null;
  }
  interface ActivityParty extends ActivityParty_Base, ActivityParty_Relationships {
    activityid_activitypointer_bind$activitypointers?: string | null;
    activityid_adx_inviteredemption_activityparty_bind$adx_inviteredemptions?: string | null;
    activityid_adx_portalcomment_activityparty_bind$adx_portalcomments?: string | null;
    activityid_appointment_bind$appointments?: string | null;
    activityid_chat_activityparty_bind$chats?: string | null;
    activityid_email_bind$emails?: string | null;
    activityid_fax_bind$faxes?: string | null;
    activityid_letter_bind$letters?: string | null;
    activityid_phonecall_bind$phonecalls?: string | null;
    activityid_recurringappointmentmaster_bind$recurringappointmentmasters?: string | null;
    activityid_socialactivity_bind$socialactivities?: string | null;
    activityid_task_bind$tasks?: string | null;
    partyid_account_bind$accounts?: string | null;
    partyid_contact_bind$contacts?: string | null;
    partyid_knowledgearticle_bind$knowledgearticles?: string | null;
    partyid_queue_bind$queues?: string | null;
    partyid_systemuser_bind$systemusers?: string | null;
  }
  interface ActivityParty_Create extends ActivityParty {
  }
  interface ActivityParty_Update extends ActivityParty {
  }
  interface ActivityParty_Select {
    activityid_guid: WebAttribute<ActivityParty_Select, { activityid_guid: string | null }, { activityid_formatted?: string }>;
    activitypartyid: WebAttribute<ActivityParty_Select, { activitypartyid: string | null }, {  }>;
    addressused: WebAttribute<ActivityParty_Select, { addressused: string | null }, {  }>;
    addressusedemailcolumnnumber: WebAttribute<ActivityParty_Select, { addressusedemailcolumnnumber: number | null }, {  }>;
    donotemail: WebAttribute<ActivityParty_Select, { donotemail: boolean | null }, {  }>;
    donotfax: WebAttribute<ActivityParty_Select, { donotfax: boolean | null }, {  }>;
    donotphone: WebAttribute<ActivityParty_Select, { donotphone: boolean | null }, {  }>;
    donotpostalmail: WebAttribute<ActivityParty_Select, { donotpostalmail: boolean | null }, {  }>;
    effort: WebAttribute<ActivityParty_Select, { effort: number | null }, {  }>;
    exchangeentryid: WebAttribute<ActivityParty_Select, { exchangeentryid: string | null }, {  }>;
    externalid: WebAttribute<ActivityParty_Select, { externalid: string | null }, {  }>;
    externalidtype: WebAttribute<ActivityParty_Select, { externalidtype: string | null }, {  }>;
    instancetypecode: WebAttribute<ActivityParty_Select, { instancetypecode: activityparty_instancetypecode | null }, { instancetypecode_formatted?: string }>;
    ispartydeleted: WebAttribute<ActivityParty_Select, { ispartydeleted: boolean | null }, {  }>;
    ownerid_guid: WebAttribute<ActivityParty_Select, { ownerid_guid: string | null }, { ownerid_formatted?: string }>;
    owningbusinessunit: WebAttribute<ActivityParty_Select, { owningbusinessunit: string | null }, {  }>;
    owninguser: WebAttribute<ActivityParty_Select, { owninguser: string | null }, {  }>;
    participationtypemask: WebAttribute<ActivityParty_Select, { participationtypemask: activityparty_participationtypemask | null }, { participationtypemask_formatted?: string }>;
    partyid_guid: WebAttribute<ActivityParty_Select, { partyid_guid: string | null }, { partyid_formatted?: string }>;
    scheduledend: WebAttribute<ActivityParty_Select, { scheduledend: Date | null }, { scheduledend_formatted?: string }>;
    scheduledstart: WebAttribute<ActivityParty_Select, { scheduledstart: Date | null }, { scheduledstart_formatted?: string }>;
    unresolvedpartyname: WebAttribute<ActivityParty_Select, { unresolvedpartyname: string | null }, {  }>;
    versionnumber: WebAttribute<ActivityParty_Select, { versionnumber: number | null }, {  }>;
  }
  interface ActivityParty_Filter {
    activityid_guid: XQW.Guid;
    activitypartyid: XQW.Guid;
    addressused: string;
    addressusedemailcolumnnumber: number;
    donotemail: boolean;
    donotfax: boolean;
    donotphone: boolean;
    donotpostalmail: boolean;
    effort: number;
    exchangeentryid: string;
    externalid: string;
    externalidtype: string;
    instancetypecode: activityparty_instancetypecode;
    ispartydeleted: boolean;
    ownerid_guid: XQW.Guid;
    owningbusinessunit: XQW.Guid;
    owninguser: XQW.Guid;
    participationtypemask: activityparty_participationtypemask;
    partyid_guid: XQW.Guid;
    scheduledend: Date;
    scheduledstart: Date;
    unresolvedpartyname: string;
    versionnumber: number;
  }
  interface ActivityParty_Expand {
    partyid_account: WebExpand<ActivityParty_Expand, Account_Select, Account_Filter, { partyid_account: Account_Result }>;
    partyid_contact: WebExpand<ActivityParty_Expand, Contact_Select, Contact_Filter, { partyid_contact: Contact_Result }>;
  }
  interface ActivityParty_FormattedResult {
    activityid_formatted?: string;
    instancetypecode_formatted?: string;
    ownerid_formatted?: string;
    participationtypemask_formatted?: string;
    partyid_formatted?: string;
    scheduledend_formatted?: string;
    scheduledstart_formatted?: string;
  }
  interface ActivityParty_Result extends ActivityParty_Base, ActivityParty_Relationships {
    "@odata.etag": string;
    activityid_guid: string | null;
    ownerid_guid: string | null;
    partyid_guid: string | null;
  }
  interface ActivityParty_RelatedOne {
    partyid_account: WebMappingRetrieve<XDT.Account_Select,XDT.Account_Expand,XDT.Account_Filter,XDT.Account_Fixed,XDT.Account_Result,XDT.Account_FormattedResult>;
    partyid_contact: WebMappingRetrieve<XDT.Contact_Select,XDT.Contact_Expand,XDT.Contact_Filter,XDT.Contact_Fixed,XDT.Contact_Result,XDT.Contact_FormattedResult>;
  }
  interface ActivityParty_RelatedMany {
  }
}
interface WebEntitiesRetrieve {
  activityparties: WebMappingRetrieve<XDT.ActivityParty_Select,XDT.ActivityParty_Expand,XDT.ActivityParty_Filter,XDT.ActivityParty_Fixed,XDT.ActivityParty_Result,XDT.ActivityParty_FormattedResult>;
}
interface WebEntitiesRelated {
  activityparties: WebMappingRelated<XDT.ActivityParty_RelatedOne,XDT.ActivityParty_RelatedMany>;
}
interface WebEntitiesCUDA {
  activityparties: WebMappingCUDA<XDT.ActivityParty_Create,XDT.ActivityParty_Update,XDT.ActivityParty_Select>;
}
