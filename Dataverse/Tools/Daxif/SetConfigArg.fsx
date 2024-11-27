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

let configEntityName = "msys_configuration"

let args = fsi.CommandLineArgs |> parseArgs

let env =
  match args |> tryFindArg ["env"; "e"] with
  | Some arg -> Environment.Get arg
  | None     -> failwithf "Missing 'env' argument needed to execute this script."

let proxy = env.connect().GetService()
let tenantId = args.TryFind "tenantid" ?| ""
let clientId = args.TryFind "clientid" ?| ""
let clientSecret = args.TryFind "clientsecret" ?| ""
let storageAccountName = args.TryFind "storageaccountname" ?| ""

let existingConfigId = 
  let query = QueryExpression(configEntityName)
  query.Criteria <- FilterExpression()
  query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0)
  query.ColumnSet <- ColumnSet()
  CrmDataHelper.retrieveMultiple proxy query |> Seq.tryHead

let entity = 
  match existingConfigId with
  | None -> Entity(configEntityName)
  | Some e -> e
entity.Attributes.Add("msys_azuretenantid", tenantId)
entity.Attributes.Add("msys_azureclientid", clientId)
entity.Attributes.Add("msys_azureclientsecret", clientSecret)
entity.Attributes.Add("msys_azurestorageaccountname", storageAccountName)

let req = UpsertRequest()
req.Target <- entity
proxy.Execute req
