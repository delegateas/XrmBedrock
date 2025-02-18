(**
Assign records to owners by name
*)
#load @"..\Daxif\_Config.fsx"

#r @"..\..\BusinessDomain\bin\Debug\Microsoft.Xrm.Sdk.dll"
#r @"..\..\BusinessDomain\bin\Debug\Microsoft.Crm.Sdk.Proxy.dll"
#r @"..\..\BusinessDomain\bin\Debug\Delegate.XrmFramework.BusinessDomain.dll"
open DG.XrmFramework.BusinessDomain.ServiceContext

open System
open System.IO
open Microsoft.Xrm.Sdk
open Microsoft.Xrm.Sdk.Query
open Microsoft.Xrm.Sdk.Messages
open Microsoft.Crm.Sdk.Messages

open _Config
open DG.Daxif.Common
open System.Web

let env = Env.prod
let logicalName = SavedQuery.EntityLogicalName

let proxy = env.connect().GetProxy()
let query = QueryExpression(logicalName)
query.ColumnSet <- ColumnSet()

CrmDataHelper.retrieveMultiple proxy query
|> Seq.length
|> printfn "%s has %i records" logicalName
