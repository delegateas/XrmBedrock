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
open Microsoft.Xrm.Sdk.Messages
open Microsoft.Xrm.Sdk.Query
open Microsoft.Crm.Sdk.Messages

open _Config
open DG.Daxif.Common




(** Configuration *)
let envs = 
  [|
    // Test
    Env.test 

    // Production
    Env.prod 
  |]



(** Implementation *)
let businessUnitNames proxy =
  let req = RetrieveMultipleRequest()
  let query = QueryExpression("businessunit")
  query.ColumnSet <- ColumnSet("name")
  query.Criteria <- FilterExpression()
  req.Query <- query
  let resp = CrmDataHelper.getResponse<RetrieveMultipleResponse> proxy req
  resp.EntityCollection.Entities |> Seq.map (fun e -> e.Attributes.["name"] :?> string) |> Set.ofSeq
  
let existingViews proxy =
  let req = RetrieveMultipleRequest()
  let query = QueryExpression(SavedQuery.EntityLogicalName)
  query.ColumnSet <- ColumnSet(true)
  query.Criteria <- FilterExpression()
  query.Criteria.AddCondition("returnedtypecode", ConditionOperator.Equal, "report")
  query.Criteria.AddCondition("ismanaged", ConditionOperator.Equal, false)
  query.Criteria.AddCondition("name", ConditionOperator.BeginsWith, "ISS ")
  query.Criteria.AddCondition("querytype", ConditionOperator.Equal, 0)
  req.Query <- query
  let resp = CrmDataHelper.getResponse<RetrieveMultipleResponse> proxy req
  resp.EntityCollection.Entities 
  |> Seq.map (fun e -> (e.Attributes.["name"] :?> string).Substring(4)) |> Set.ofSeq



let layoutxml = "<grid name=\"resultset\" object=\"9100\" jump=\"name\" select=\"1\" preview=\"1\" icon=\"1\"><row name=\"result\" id=\"reportid\" layoutstyle=\"ReportList\"><cell name=\"name\" width=\"300\" /><cell name=\"reporttypecode\" width=\"100\" /><cell name=\"modifiedon\" width=\"100\" /><cell name=\"description\" width=\"100\" /></row></grid>"
let fetchxml name = 
  sprintf "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\"><entity name=\"report\"><attribute name=\"reporttypecode\" /><attribute name=\"modifiedon\" /><attribute name=\"description\" /><order attribute=\"name\" descending=\"false\" /><filter type=\"and\"><condition attribute=\"name\" operator=\"like\" value=\"%%%s%%\" /></filter><attribute name=\"reportid\" /></entity></fetch>" name



let makeViewCreate name =
  let view = Entity(SavedQuery.EntityLogicalName)
  view.Attributes.["name"] <- "ISS " + name
  view.Attributes.["returnedtypecode"] <- "report"
  view.Attributes.["layoutxml"] <- layoutxml
  view.Attributes.["fetchxml"] <- fetchxml name
  view.Attributes.["querytype"] <- 0
  let req = CreateRequest()
  req.Target <- view
  req :> OrganizationRequest


let main (env:DG.Daxif.Environment) =
  let proxy = env.connect().GetProxy()
  let toCreate = Set.difference (businessUnitNames proxy) (existingViews proxy) |> Set.toArray
  if Array.isEmpty toCreate then
    printfn "All views already exist in %O" env
  else
    let resp = 
      toCreate
      |> Array.map makeViewCreate
      |> CrmDataHelper.performAsBulk proxy

    resp |> Array.iter (fun r -> if r.Fault <> null then eprintfn "Error when creating view: %s" r.Fault.Message)
    resp |> Array.filter (fun x -> x.Fault = null)
    |> Array.length
    |> fun count -> printfn "Succesfully created %d/%d views in %O" count toCreate.Length env



;;
printfn ""
printfn "Creating report views in given environments.."
printfn ""
envs |> Array.Parallel.iter main
printfn ""
