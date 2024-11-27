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


(** Configuration *)
let config = 
  [|
    Account.EntityLogicalName, @"AccountOwners.csv"
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
let loadData path = 
  File.ReadAllLines(Path.Combine(__SOURCE_DIRECTORY__, path), Text.Encoding.UTF8) 
  |> Array.map (fun s -> s.Trim().Trim(';'))
  |> Array.filter (fun s -> s.Length > 0)
  |> Array.map (fun s -> s.Split(';'))
  |> Array.map (fun arr -> arr.[0], arr.[1])

let makeAssignRequest logicalName ownerRef (guid:string) =
  let req = AssignRequest() 
  req.Target <- EntityReference(logicalName, Guid(guid))
  req.Assignee <- ownerRef
  req :> OrganizationRequest

let makeComparisonRetrieveRequest logicalName (attribute:string) (value:string) =
  let query = QueryExpression(logicalName)
  query.Criteria <- FilterExpression()
  query.Criteria.AddCondition(attribute, ConditionOperator.Equal, value)
  query.TopCount <- Nullable(1)
  let req = RetrieveMultipleRequest()
  req.Query <- query
  req :> OrganizationRequest
  


let getOwnerMapByName proxy names =
  let uniqueNames = names |> Array.distinct

  uniqueNames
  |> Array.map (makeComparisonRetrieveRequest SystemUser.EntityLogicalName "fullname")
  |> CrmDataHelper.performAsBulk proxy
  |> Array.map (fun x -> uniqueNames.[x.RequestIndex], x)

  |> Array.partition (fun (name,x) -> (x.Response :?> RetrieveMultipleResponse).EntityCollection.Entities.Count = 1)
  |> (fun (found,notfound) -> // Try Team instead of SystemUser
    notfound 
    |> Array.map (fst >> makeComparisonRetrieveRequest Team.EntityLogicalName "name")
    |> CrmDataHelper.performAsBulk proxy
    |> Array.map (fun x -> notfound.[x.RequestIndex] |> fst, x)
    |> Array.append found)

  |> Array.partition (fun (name,x) -> (x.Response :?> RetrieveMultipleResponse).EntityCollection.Entities.Count = 1)
  |> (fun (found,notfound) -> // Print owners that were not found, and make map of rest
    notfound 
    |> Array.iter (fun (name,x) -> printfn "Owner with name \"%s\" was not found in the CRM system" name)
    found 
    |> Array.map (fun (name,x) -> 
      name, (x.Response :?> RetrieveMultipleResponse).EntityCollection.Entities.[0].ToEntityReference())
    |> Map.ofArray)


let main (env:DG.Daxif.Environment) =
  let proxy = env.connect().GetProxy()
  let data = config |> Array.map (fun (entity, path) -> entity, loadData path)
  let ownerMap = 
    data 
    |> Array.map (fun (_, rows) -> rows |> Array.map snd) 
    |> Array.concat 
    |> getOwnerMapByName proxy
  
  data
  |> Array.map (fun (logicalName, rows) ->
    rows 
    |> Array.choose (fun (guid, owner) -> ownerMap.TryFind owner |> Option.bind (fun owner -> Some (guid,owner)))
    |> Array.map (fun (guid, owner) -> makeAssignRequest logicalName owner guid))
  |> Array.concat
  |> CrmDataHelper.performAsBulk proxy
  |> ignore



;;
printfn ""
printfn "Assigning records in given environments.."
printfn ""
envs |> Array.Parallel.iter main
printfn ""
