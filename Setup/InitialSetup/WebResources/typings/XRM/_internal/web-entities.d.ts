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
  interface ctx_Invoice_Base extends WebEntity {
  }
  interface ctx_Invoice_Fixed extends WebEntity_Fixed {
    ctx_invoiceid: string;
  }
  interface ctx_Invoice extends ctx_Invoice_Base, ctx_Invoice_Relationships {
  }
  interface ctx_Invoice_Relationships {
  }
  interface ctx_Invoice_Result extends ctx_Invoice_Base, ctx_Invoice_Relationships {
  }
  interface ctx_Invoice_FormattedResult {
  }
  interface ctx_Invoice_Select {
  }
  interface ctx_Invoice_Expand {
  }
  interface ctx_Invoice_Filter {
  }
  interface ctx_Invoice_Create extends ctx_Invoice {
  }
  interface ctx_Invoice_Update extends ctx_Invoice {
  }
  interface ctx_InvoiceCollection_Base extends WebEntity {
  }
  interface ctx_InvoiceCollection_Fixed extends WebEntity_Fixed {
    ctx_invoicecollectionid: string;
  }
  interface ctx_InvoiceCollection extends ctx_InvoiceCollection_Base, ctx_InvoiceCollection_Relationships {
  }
  interface ctx_InvoiceCollection_Relationships {
  }
  interface ctx_InvoiceCollection_Result extends ctx_InvoiceCollection_Base, ctx_InvoiceCollection_Relationships {
  }
  interface ctx_InvoiceCollection_FormattedResult {
  }
  interface ctx_InvoiceCollection_Select {
  }
  interface ctx_InvoiceCollection_Expand {
  }
  interface ctx_InvoiceCollection_Filter {
  }
  interface ctx_InvoiceCollection_Create extends ctx_InvoiceCollection {
  }
  interface ctx_InvoiceCollection_Update extends ctx_InvoiceCollection {
  }
  interface ctx_Product_Base extends WebEntity {
  }
  interface ctx_Product_Fixed extends WebEntity_Fixed {
    ctx_productid: string;
  }
  interface ctx_Product extends ctx_Product_Base, ctx_Product_Relationships {
  }
  interface ctx_Product_Relationships {
  }
  interface ctx_Product_Result extends ctx_Product_Base, ctx_Product_Relationships {
  }
  interface ctx_Product_FormattedResult {
  }
  interface ctx_Product_Select {
  }
  interface ctx_Product_Expand {
  }
  interface ctx_Product_Filter {
  }
  interface ctx_Product_Create extends ctx_Product {
  }
  interface ctx_Product_Update extends ctx_Product {
  }
  interface ctx_Subscription_Base extends WebEntity {
  }
  interface ctx_Subscription_Fixed extends WebEntity_Fixed {
    ctx_subscriptionid: string;
  }
  interface ctx_Subscription extends ctx_Subscription_Base, ctx_Subscription_Relationships {
  }
  interface ctx_Subscription_Relationships {
  }
  interface ctx_Subscription_Result extends ctx_Subscription_Base, ctx_Subscription_Relationships {
  }
  interface ctx_Subscription_FormattedResult {
  }
  interface ctx_Subscription_Select {
  }
  interface ctx_Subscription_Expand {
  }
  interface ctx_Subscription_Filter {
  }
  interface ctx_Subscription_Create extends ctx_Subscription {
  }
  interface ctx_Subscription_Update extends ctx_Subscription {
  }
  interface ctx_Transaction_Base extends WebEntity {
  }
  interface ctx_Transaction_Fixed extends WebEntity_Fixed {
    ctx_transactionid: string;
  }
  interface ctx_Transaction extends ctx_Transaction_Base, ctx_Transaction_Relationships {
  }
  interface ctx_Transaction_Relationships {
  }
  interface ctx_Transaction_Result extends ctx_Transaction_Base, ctx_Transaction_Relationships {
  }
  interface ctx_Transaction_FormattedResult {
  }
  interface ctx_Transaction_Select {
  }
  interface ctx_Transaction_Expand {
  }
  interface ctx_Transaction_Filter {
  }
  interface ctx_Transaction_Create extends ctx_Transaction {
  }
  interface ctx_Transaction_Update extends ctx_Transaction {
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
}
