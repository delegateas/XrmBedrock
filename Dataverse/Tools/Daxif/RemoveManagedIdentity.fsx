#load @"_Config.fsx"
#r @"System.ServiceModel"
open _Config
open Microsoft.Xrm.Sdk
open Microsoft.Xrm.Sdk.Query

open DG.Daxif
open DG.Daxif.Common
open DG.Daxif.Common.Utility

let args = fsi.CommandLineArgs |> parseArgs

let env =
  match args |> tryFindArg ["env"; "e"] with
  | Some arg -> Environment.Get arg
  | None     -> failwithf "Missing 'env' argument needed to execute this script."

let proxy = env.connect().GetService()

let pluginAssembly =
    let query = QueryExpression("pluginassembly")
    query.Criteria <- FilterExpression()
    query.Criteria.AddCondition("name", ConditionOperator.Equal, Path.pluginDllName)
    query.ColumnSet <- ColumnSet("managedidentityid")
    CrmDataHelper.retrieveMultiple proxy query |> Seq.head

if (pluginAssembly.Contains("managedidentityid")) |> not then () else

let managedIdentityRef = pluginAssembly.GetAttributeValue<EntityReference>("managedidentityid")
proxy.Delete(managedIdentityRef.LogicalName, managedIdentityRef.Id)