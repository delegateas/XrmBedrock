(**
Playground
*)

#load @"_Config.fsx"
#r @"System.ServiceModel"
#r @"bin\Newtonsoft.Json.dll"
open _Config
open System
open System.IO
open System.Collections.Generic
open Microsoft.Xrm.Sdk
open Microsoft.Xrm.Sdk.Client
open Microsoft.Xrm.Sdk.Messages
open Microsoft.Xrm.Sdk.Query

open DG.Daxif
open DG.Daxif.Common
open DG.Daxif.Common.Utility
open Newtonsoft.Json

let argMap = fsi.CommandLineArgs |> parseArgs

let env =
  match argMap |> tryFindArg ["env"; "e"] with
  | Some arg -> Environment.Get arg
  | None     -> failwithf "Missing 'env' argument needed to execute this script."

let proxy = env.connect().GetService()

type AzureConfig = {
  ConfigType: string
  TenantId: string
  SubscriptionId: string
  ClientId: string
  ClientSecret: string
  KeyvaultUrl: string
  KeyvaultName: string
}

let secureConfig: AzureConfig = {
  ConfigType = "AzureConfig"
  TenantId = argMap.TryFind "tenantid" ?| ""
  SubscriptionId = argMap.TryFind "subscriptionid" ?| ""
  ClientId = argMap.TryFind "clientid" ?| ""
  ClientSecret = argMap.TryFind "clientsecret" ?| ""
  KeyvaultUrl = argMap.TryFind "keyvaulturl" ?| ""
  KeyvaultName = argMap.TryFind "keyvaulturlname" ?| ""
}

let existingConfigId = 
  let query = QueryExpression("sdkmessageprocessingstepsecureconfig")
  query.Criteria <- FilterExpression()
  query.Criteria.AddCondition("secureconfig", ConditionOperator.BeginsWith, """{"ConfigType":"AzureConfig""")
  query.ColumnSet <- ColumnSet()
  CrmDataHelper.retrieveMultiple proxy query |> Seq.tryHead

let entity = 
  match existingConfigId with
  | None -> Entity("sdkmessageprocessingstepsecureconfig")
  | Some e -> e
entity.Attributes.Add("secureconfig", secureConfig |> JsonConvert.SerializeObject)

let req = UpsertRequest()
req.Target <- entity
proxy.Execute req

let solutionId = 
  let query = QueryExpression("solution")
  query.Criteria <- FilterExpression()
  query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, SolutionInfo.name)
  query.ColumnSet <- ColumnSet()
  CrmDataHelper.retrieveMultiple proxy query
  |> Seq.head
  |> fun x -> x.Id

let pluginSteps =
  let query = QueryExpression("solutioncomponent")
  query.Criteria <- FilterExpression()
  query.Criteria.AddCondition("solutionid", ConditionOperator.Equal, solutionId)
  query.Criteria.AddCondition("componenttype", ConditionOperator.Equal, 92)
  query.ColumnSet <- ColumnSet("objectid")
  CrmDataHelper.retrieveMultiple proxy query 
  |> Array.ofSeq
  |> Array.map (fun x -> x.GetAttributeValue<Guid>("objectid"))

pluginSteps
|> Array.map (fun id -> 
  let e = Entity("sdkmessageprocessingstep")
  e.Id <- id
  e.Attributes.Add("sdkmessageprocessingstepid", id)
  e.Attributes.Add("sdkmessageprocessingstepsecureconfigid", EntityReference("sdkmessageprocessingstepsecureconfig", entity.Id))
  CrmDataHelper.makeUpdateReq e :> OrganizationRequest
)
|> Array.chunkBySize 100
|> Array.map (CrmDataHelper.performAsBulk proxy)
