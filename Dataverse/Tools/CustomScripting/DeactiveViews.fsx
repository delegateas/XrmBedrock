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
let viewGuids = 
  [| // View GUIDs
    
  |]


let envs = 
  [|
    Env.dev

    // Test
    Env.test 

    // Production
    Env.prod
  |]


(** Implementation *)
let makeDeactiveRequest (guid:string) =
  let req = SetStateRequest() 
  req.EntityMoniker <- EntityReference(SavedQuery.EntityLogicalName, Guid(guid))
  req.State <- OptionSetValue(SavedQueryState.Inactive |> int)
  req.Status <- OptionSetValue(-1)
  req :> OrganizationRequest


let main (env:DG.Daxif.Environment) =
  let proxy = env.connect().GetProxy()
  viewGuids 
  |> Array.map makeDeactiveRequest 
  |> CrmDataHelper.performAsBulk proxy
  |> Array.filter (fun x -> x.Fault = null)
  |> Array.length
  |> fun count -> printfn "Succesfully deactivated %d/%d possible views in %O" count viewGuids.Length env



;;
printfn ""
printfn "Deactivating views in given environments.."
printfn ""
envs |> Array.Parallel.iter main
printfn ""
