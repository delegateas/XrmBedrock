interface WebMappingRetrieve<ISelect, IExpand, IFilter, IFixed, Result, FormattedResult> {
}
interface WebMappingCUDA<ICreate, IUpdate, ISelect> {
}
interface WebMappingRelated<ISingle, IMultiple> {
}
declare namespace XDT {
  interface WebEntity {
  }
  interface WebEntity_Fixed {
    "@odata.etag": string;
  }
  interface Account_Base extends WebEntity {
  }
  interface Account_Fixed extends WebEntity_Fixed {
    accountid: string;
  }
  interface Account extends Account_Base, Account_Relationships {
  }
  interface Account_Relationships {
  }
  interface Account_Result extends Account_Base, Account_Relationships {
  }
  interface Account_FormattedResult {
  }
  interface Account_Select {
  }
  interface Account_Expand {
  }
  interface Account_Filter {
  }
  interface Account_Create extends Account {
  }
  interface Account_Update extends Account {
  }
  interface Contact_Base extends WebEntity {
  }
  interface Contact_Fixed extends WebEntity_Fixed {
    contactid: string;
  }
  interface Contact extends Contact_Base, Contact_Relationships {
  }
  interface Contact_Relationships {
  }
  interface Contact_Result extends Contact_Base, Contact_Relationships {
  }
  interface Contact_FormattedResult {
  }
  interface Contact_Select {
  }
  interface Contact_Expand {
  }
  interface Contact_Filter {
  }
  interface Contact_Create extends Contact {
  }
  interface Contact_Update extends Contact {
  }
  interface mgs_Invoice_Base extends WebEntity {
  }
  interface mgs_Invoice_Fixed extends WebEntity_Fixed {
    mgs_invoiceid: string;
  }
  interface mgs_Invoice extends mgs_Invoice_Base, mgs_Invoice_Relationships {
  }
  interface mgs_Invoice_Relationships {
  }
  interface mgs_Invoice_Result extends mgs_Invoice_Base, mgs_Invoice_Relationships {
  }
  interface mgs_Invoice_FormattedResult {
  }
  interface mgs_Invoice_Select {
  }
  interface mgs_Invoice_Expand {
  }
  interface mgs_Invoice_Filter {
  }
  interface mgs_Invoice_Create extends mgs_Invoice {
  }
  interface mgs_Invoice_Update extends mgs_Invoice {
  }
  interface mgs_InvoiceCollection_Base extends WebEntity {
  }
  interface mgs_InvoiceCollection_Fixed extends WebEntity_Fixed {
    mgs_invoicecollectionid: string;
  }
  interface mgs_InvoiceCollection extends mgs_InvoiceCollection_Base, mgs_InvoiceCollection_Relationships {
  }
  interface mgs_InvoiceCollection_Relationships {
  }
  interface mgs_InvoiceCollection_Result extends mgs_InvoiceCollection_Base, mgs_InvoiceCollection_Relationships {
  }
  interface mgs_InvoiceCollection_FormattedResult {
  }
  interface mgs_InvoiceCollection_Select {
  }
  interface mgs_InvoiceCollection_Expand {
  }
  interface mgs_InvoiceCollection_Filter {
  }
  interface mgs_InvoiceCollection_Create extends mgs_InvoiceCollection {
  }
  interface mgs_InvoiceCollection_Update extends mgs_InvoiceCollection {
  }
  interface mgs_Product_Base extends WebEntity {
  }
  interface mgs_Product_Fixed extends WebEntity_Fixed {
    mgs_productid: string;
  }
  interface mgs_Product extends mgs_Product_Base, mgs_Product_Relationships {
  }
  interface mgs_Product_Relationships {
  }
  interface mgs_Product_Result extends mgs_Product_Base, mgs_Product_Relationships {
  }
  interface mgs_Product_FormattedResult {
  }
  interface mgs_Product_Select {
  }
  interface mgs_Product_Expand {
  }
  interface mgs_Product_Filter {
  }
  interface mgs_Product_Create extends mgs_Product {
  }
  interface mgs_Product_Update extends mgs_Product {
  }
  interface mgs_Subscription_Base extends WebEntity {
  }
  interface mgs_Subscription_Fixed extends WebEntity_Fixed {
    mgs_subscriptionid: string;
  }
  interface mgs_Subscription extends mgs_Subscription_Base, mgs_Subscription_Relationships {
  }
  interface mgs_Subscription_Relationships {
  }
  interface mgs_Subscription_Result extends mgs_Subscription_Base, mgs_Subscription_Relationships {
  }
  interface mgs_Subscription_FormattedResult {
  }
  interface mgs_Subscription_Select {
  }
  interface mgs_Subscription_Expand {
  }
  interface mgs_Subscription_Filter {
  }
  interface mgs_Subscription_Create extends mgs_Subscription {
  }
  interface mgs_Subscription_Update extends mgs_Subscription {
  }
  interface mgs_Transaction_Base extends WebEntity {
  }
  interface mgs_Transaction_Fixed extends WebEntity_Fixed {
    mgs_transactionid: string;
  }
  interface mgs_Transaction extends mgs_Transaction_Base, mgs_Transaction_Relationships {
  }
  interface mgs_Transaction_Relationships {
  }
  interface mgs_Transaction_Result extends mgs_Transaction_Base, mgs_Transaction_Relationships {
  }
  interface mgs_Transaction_FormattedResult {
  }
  interface mgs_Transaction_Select {
  }
  interface mgs_Transaction_Expand {
  }
  interface mgs_Transaction_Filter {
  }
  interface mgs_Transaction_Create extends mgs_Transaction {
  }
  interface mgs_Transaction_Update extends mgs_Transaction {
  }
  interface BulkOperationLog_Base extends WebEntity {
  }
  interface BulkOperationLog_Fixed extends WebEntity_Fixed {
    bulkoperationlogid: string;
  }
  interface BulkOperationLog extends BulkOperationLog_Base, BulkOperationLog_Relationships {
  }
  interface BulkOperationLog_Relationships {
  }
  interface BulkOperationLog_Result extends BulkOperationLog_Base, BulkOperationLog_Relationships {
  }
  interface BulkOperationLog_FormattedResult {
  }
  interface BulkOperationLog_Select {
  }
  interface BulkOperationLog_Expand {
  }
  interface BulkOperationLog_Filter {
  }
  interface BulkOperationLog_Create extends BulkOperationLog {
  }
  interface BulkOperationLog_Update extends BulkOperationLog {
  }
  interface ActivityParty_Base extends WebEntity {
  }
  interface ActivityParty_Fixed extends WebEntity_Fixed {
    activitypartyid: string;
  }
  interface ActivityParty extends ActivityParty_Base, ActivityParty_Relationships {
  }
  interface ActivityParty_Relationships {
  }
  interface ActivityParty_Result extends ActivityParty_Base, ActivityParty_Relationships {
  }
  interface ActivityParty_FormattedResult {
  }
  interface ActivityParty_Select {
  }
  interface ActivityParty_Expand {
  }
  interface ActivityParty_Filter {
  }
  interface ActivityParty_Create extends ActivityParty {
  }
  interface ActivityParty_Update extends ActivityParty {
  }
  interface Connection_Base extends WebEntity {
  }
  interface Connection_Fixed extends WebEntity_Fixed {
    connectionid: string;
  }
  interface Connection extends Connection_Base, Connection_Relationships {
  }
  interface Connection_Relationships {
  }
  interface Connection_Result extends Connection_Base, Connection_Relationships {
  }
  interface Connection_FormattedResult {
  }
  interface Connection_Select {
  }
  interface Connection_Expand {
  }
  interface Connection_Filter {
  }
  interface Connection_Create extends Connection {
  }
  interface Connection_Update extends Connection {
  }
  interface msdyn_accountkpiitem_Base extends WebEntity {
  }
  interface msdyn_accountkpiitem_Fixed extends WebEntity_Fixed {
    msdyn_accountkpiitemid: string;
  }
  interface msdyn_accountkpiitem extends msdyn_accountkpiitem_Base, msdyn_accountkpiitem_Relationships {
  }
  interface msdyn_accountkpiitem_Relationships {
  }
  interface msdyn_accountkpiitem_Result extends msdyn_accountkpiitem_Base, msdyn_accountkpiitem_Relationships {
  }
  interface msdyn_accountkpiitem_FormattedResult {
  }
  interface msdyn_accountkpiitem_Select {
  }
  interface msdyn_accountkpiitem_Expand {
  }
  interface msdyn_accountkpiitem_Filter {
  }
  interface msdyn_accountkpiitem_Create extends msdyn_accountkpiitem {
  }
  interface msdyn_accountkpiitem_Update extends msdyn_accountkpiitem {
  }
  interface msdyn_contactkpiitem_Base extends WebEntity {
  }
  interface msdyn_contactkpiitem_Fixed extends WebEntity_Fixed {
    msdyn_contactkpiitemid: string;
  }
  interface msdyn_contactkpiitem extends msdyn_contactkpiitem_Base, msdyn_contactkpiitem_Relationships {
  }
  interface msdyn_contactkpiitem_Relationships {
  }
  interface msdyn_contactkpiitem_Result extends msdyn_contactkpiitem_Base, msdyn_contactkpiitem_Relationships {
  }
  interface msdyn_contactkpiitem_FormattedResult {
  }
  interface msdyn_contactkpiitem_Select {
  }
  interface msdyn_contactkpiitem_Expand {
  }
  interface msdyn_contactkpiitem_Filter {
  }
  interface msdyn_contactkpiitem_Create extends msdyn_contactkpiitem {
  }
  interface msdyn_contactkpiitem_Update extends msdyn_contactkpiitem {
  }
  interface PostFollow_Base extends WebEntity {
  }
  interface PostFollow_Fixed extends WebEntity_Fixed {
    postfollowid: string;
  }
  interface PostFollow extends PostFollow_Base, PostFollow_Relationships {
  }
  interface PostFollow_Relationships {
  }
  interface PostFollow_Result extends PostFollow_Base, PostFollow_Relationships {
  }
  interface PostFollow_FormattedResult {
  }
  interface PostFollow_Select {
  }
  interface PostFollow_Expand {
  }
  interface PostFollow_Filter {
  }
  interface PostFollow_Create extends PostFollow {
  }
  interface PostFollow_Update extends PostFollow {
  }
}
