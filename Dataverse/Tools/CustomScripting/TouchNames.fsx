
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

let env = Env.prod
let proxy = env.connect().GetProxy()


let contactQuery = QueryExpression("contact")
contactQuery.ColumnSet <- ColumnSet("firstname", "middlename", "lastname", "fullname")

let userQuery = QueryExpression("systemuser")
userQuery.ColumnSet <- ColumnSet("firstname", "middlename", "lastname", "fullname")

let contacts () = 
  CrmDataHelper.retrieveMultiple proxy contactQuery 
let users () = 
  CrmDataHelper.retrieveMultiple proxy userQuery


let updates = 
  [ contacts() ]
  |> Seq.concat
  |> Seq.choose (fun e ->
    let fn = e.GetAttributeValue<string>("fullname")
    if isNull fn || (fn.Contains(",") |> not) then None
    else

    let ne = Entity(e.LogicalName, e.Id)
    ne.["firstname"] <- e.GetAttributeValue("firstname") + " TEMP"
    ne.["middlename"] <- e.GetAttributeValue("middlename") + " TEMP"
    ne.["lastname"] <- e.GetAttributeValue("lastname") + " TEMP"

    let oe = Entity(e.LogicalName, e.Id)
    oe.["firstname"] <- e.GetAttributeValue("firstname")
    oe.["middlename"] <- e.GetAttributeValue("middlename")
    oe.["lastname"] <- e.GetAttributeValue("lastname")

    let req1 = UpdateRequest()
    req1.Target <- ne
    let req2 = UpdateRequest()
    req2.Target <- oe
    
    Some [req1 :> OrganizationRequest; req2 :> OrganizationRequest])
  |> Seq.concat
  |> Array.ofSeq

let perform () =
  CrmDataHelper.performAsBulk proxy updates
