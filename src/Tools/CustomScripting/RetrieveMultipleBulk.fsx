
#load @"..\Daxif\_Config.fsx"


open System
open System.Collections.Generic
open System.Text
open System.Text.RegularExpressions
open DG.Daxif.Modules
open _Config
open DG.Daxif.Common
open Microsoft.Xrm.Sdk
open Microsoft.Xrm.Sdk.Messages
open Microsoft.Crm.Sdk.Messages
open Microsoft.Xrm.Sdk.Query
open Microsoft.Xrm.Sdk.Metadata

(** Configuration *)

let env = Env.dev
let proxy = env.connect().GetProxy()

let guid = Guid("01AB2EA2-1653-E611-80F0-5065F38A3A41")

let query = QueryExpression("sharepointdocumentlocation")
//query.ColumnSet <- ColumnSet()
query.Criteria <- FilterExpression()
query.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, guid)

let queries = 
  [|
    query
  |]

let result = CrmDataHelper.bulkRetrieveMultiple proxy queries
