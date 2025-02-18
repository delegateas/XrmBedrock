(**
Deactive given views
*)

#load @"..\Daxif\_Config.fsx"

#r @"..\..\BusinessDomain\bin\Debug\Microsoft.Xrm.Sdk.dll"
#r @"..\..\BusinessDomain\bin\Debug\Microsoft.Crm.Sdk.Proxy.dll"
#r @"..\..\BusinessDomain\bin\Debug\Delegate.XrmFramework.BusinessDomain.dll"
open DG.XrmFramework.BusinessDomain.ServiceContext

open System
open System.IO
open Microsoft.Xrm.Sdk
open Microsoft.Crm.Sdk.Messages

open _Config
open DG.Daxif.Common


(** Configuration *)
let config = 
  [|
    Account.EntityLogicalName, int(AccountState.Inactive), @"Accounts.csv"
    Contact.EntityLogicalName, int(ContactState.Inactive), @"Contacts.csv"
  |]


let envs = 
  [|
//    devEnv

    // Test
//    testEnv 

    // Production
//    prodEnv 
  |]





(** Implementation *)
let loadGuids path = 
  File.ReadAllLines(Path.Combine(__SOURCE_DIRECTORY__, path)) 
  |> Array.map (fun s -> s.Trim(';'))
  |> Array.filter (fun s -> s.Length > 0)

let makeDeactiveRequest logicalName state (guid:string) =
  let req = SetStateRequest() 
  req.EntityMoniker <- EntityReference(logicalName, Guid(guid))
  req.State <- OptionSetValue(state)
  req.Status <- OptionSetValue(-1)
  req :> OrganizationRequest

let main (env:DG.Daxif.Environment) =
  let proxy = env.connect().GetProxy()
  config
  |> Array.map (fun (logicalName, state, path) ->
    loadGuids path |> Array.map (makeDeactiveRequest logicalName state))
  |> Array.concat
  |> CrmDataHelper.performAsBulk proxy
  |> ignore
  


;;
printfn ""
printfn "Deactivating records in given environments.."
printfn ""
envs |> Array.Parallel.iter main
printfn ""
