#load @"_Config.fsx"
#r @"System.ServiceModel"
open _Config
open System
open Microsoft.Xrm.Sdk
open Microsoft.Xrm.Sdk.Query
open Microsoft.Crm.Sdk.Messages

open DG.Daxif
open DG.Daxif.Common
open DG.Daxif.Common.Utility

let args = fsi.CommandLineArgs |> parseArgs

let clientId = args |> Map.tryFind "clientid"
let tenantId = args |> Map.tryFind "tenantid"
if clientId.IsNone || tenantId.IsNone then failwith "Missing 'clientid' or 'tenantid' argument needed to execute this script." else

let env =
  match args |> tryFindArg ["env"; "e"] with
  | Some arg -> Environment.Get arg
  | None     -> failwithf "Missing 'env' argument needed to execute this script."

let proxy = env.connect().GetService()
let appId = clientId.Value
let tenantId = tenantId.Value

let pluginAssembly =
    let query = QueryExpression("pluginassembly")
    query.Criteria <- FilterExpression()
    query.Criteria.AddCondition("name", ConditionOperator.Equal, Path.pluginDllName)
    query.ColumnSet <- ColumnSet("managedidentityid")
    CrmDataHelper.retrieveMultiple proxy query |> Seq.head

if (pluginAssembly.Contains("managedidentityid")) then () else

let managedIdentityRecord = Entity("managedidentity")
managedIdentityRecord.Attributes.Add("name", SolutionInfo.name + " Azure")
managedIdentityRecord.Attributes.Add("applicationid", Guid(appId))
managedIdentityRecord.Attributes.Add("credentialsource", OptionSetValue(2))
managedIdentityRecord.Attributes.Add("subjectscope", OptionSetValue(1))
managedIdentityRecord.Attributes.Add("tenantid", Guid(tenantId))
managedIdentityRecord.Id <- proxy.Create(managedIdentityRecord)

let pluginUpdate = Entity(pluginAssembly.LogicalName, pluginAssembly.Id)
pluginUpdate.Attributes.Add("managedidentityid", managedIdentityRecord.ToEntityReference())
proxy.Update(pluginUpdate)