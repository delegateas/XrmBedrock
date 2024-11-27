(**
Updates the exchange rate of found records by setting a money value on the entity to the same as it was previously
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
open Microsoft.Xrm.Sdk.Messages
open Microsoft.Xrm.Sdk.Query


(** Configuration *)
let entities = 
  [| // logical names of entities
  |]


let envs = 
  [|
    Env.dev
    Env.test
    Env.prod
  |]


(** Implementation *)
let makeUpdateRequest transform (entity:Entity) =
  let updEntity = Entity(entity.LogicalName)
  updEntity.Id <- entity.Id
  transform entity updEntity
  let req = UpdateRequest()
  req.Target <- updEntity
  req :> OrganizationRequest

let getDefaultCurrency proxy =
  let query = QueryExpression("organization")
  query.ColumnSet <- ColumnSet("basecurrencyid")
  query.TopCount <- Nullable(1)

  match CrmDataHelper.retrieveMultiple proxy query |> Seq.tryHead with
  | Some org -> org.GetAttributeValue<EntityReference>("basecurrencyid")
  | None -> failwith "Unable to retrieve organization"


let getRecords proxy logicalName =
  let query = QueryExpression(logicalName)
  query.ColumnSet <- ColumnSet("transactioncurrencyid")
  query.Criteria <- FilterExpression()
  query.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.Null)
  
  CrmDataHelper.retrieveMultiple proxy query |> Array.ofSeq


let main (env:DG.Daxif.Environment) =
  let proxy = env.connect().GetProxy()
  let baseCurrency = getDefaultCurrency proxy
  entities 
  |> Array.map (getRecords proxy) 
  |> Array.map (fun es -> 
    printfn "Found %d entities that needs to be updated" (Seq.length es)
    es |> Seq.map (makeUpdateRequest (fun _ upd -> 
      upd.Attributes.["transactioncurrencyid"] <- baseCurrency
    )))
  |> Seq.concat
  |> Array.ofSeq
  |> CrmDataHelper.performAsBulk proxy
  |> ignore


;;
printfn ""
printfn "Setting missing transactioncurrencyid fields.."
printfn ""
envs |> Array.Parallel.iter main
printfn ""
