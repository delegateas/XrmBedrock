
#load @"..\Daxif\_Config.fsx"


open System
open System.Collections.Generic
open System.Text
open System.Text.RegularExpressions
open System.IO
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

let logicalName = "account"
let attributesToMatch = [| "accountnumber" |]


(** Logic *)

let query = QueryExpression(logicalName)
query.ColumnSet <- ColumnSet(attributesToMatch)
query.Criteria <- FilterExpression(LogicalOperator.And)
attributesToMatch |> Array.iter (fun a -> query.Criteria.AddCondition(a, ConditionOperator.NotNull))

let dups = 
  CrmDataHelper.retrieveMultiple proxy query
  |> Seq.groupBy (fun e -> 
    attributesToMatch 
    |> Array.map (fun a -> e.GetAttributeValue(a).ToString().Trim()))
  |> Seq.filter (fun group -> group |> snd |> Seq.length |> fun l -> l > 1)
  |> Array.ofSeq



let writeFile () =
  File.WriteAllLines(sprintf "C:\Temp\%s_duplicates.txt" logicalName,
    dups |> Seq.map (fun (attrs,es) ->
      sprintf "%s: %d" (String.Join(",", attrs)) (es |> Seq.length)
    ))
