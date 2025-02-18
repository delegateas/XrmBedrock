namespace DeployHelper

(** 
PluginsHelper
*)
#load "..\..\Daxif\_Config.fsx"

(** Open libraries for use *)
open System
open DG.Daxif.Common
open Microsoft.Xrm.Sdk
open Microsoft.Xrm.Sdk.Query
open Microsoft.Crm.Sdk.Messages

module DeployHelper =   
  open System.Text

  let outputfn format =
    printf "%A: " DateTime.Now
    printfn format

  let publishAll (env: DG.Daxif.Environment) =
    outputfn "==================================="
    outputfn "DeployHelper: Performing PublishAll"
    outputfn "==================================="

    outputfn "Connecting to CRM"
    let proxy = env.connect().GetProxy()

    outputfn "Publishing all customizations"
    proxy.Timeout <- new TimeSpan(0,59,0)
    CrmDataHelper.publishAll proxy
    outputfn "Done publishing all customizations!"
    outputfn "***********************************"

  let retrieve (env: DG.Daxif.Environment) logicalname guid =
    outputfn "==================================="
    outputfn "DeployHelper: Retrieve record      "
    outputfn "==================================="

    outputfn "Connecting to CRM"
    let proxy = env.connect().GetProxy()
    
    outputfn "Retrieving record"
    let res = CrmDataHelper.retrieve proxy logicalname guid
    outputfn "Done retrieving the record!"
    outputfn "***********************************"
    res

  let getImportJob (env: DG.Daxif.Environment) guid =
    outputfn "==================================="
    outputfn "DeployHelper: Get ImportJob        "
    outputfn "==================================="

    outputfn "Connecting to CRM"
    let proxy = env.connect().GetProxy()
    
    outputfn "Retrieving job"
    let req = new RetrieveFormattedImportJobResultsRequest()
    req.ImportJobId <- guid
    let resp = proxy.Execute(req) :?> RetrieveFormattedImportJobResultsResponse
    let xml = resp.FormattedResults
    let bytes = Encoding.UTF8.GetBytes(xml)
    let res = Encoding.UTF8.GetString(bytes)
    outputfn "Done retrieving the job!"
    outputfn "***********************************"
    res

  module DuplicateDetectionRules = 
    let makePublishDupRuleRequest guid =
          let req = PublishDuplicateRuleRequest()
          req.DuplicateRuleId <- guid
          req :> OrganizationRequest

    let retrieveDR proxy = 
      let f = FilterExpression()
      f.AddCondition
        (@"statecode", ConditionOperator.Equal, 0)
      f.AddCondition
        (@"statuscode", ConditionOperator.Equal, 0)
      let q = QueryExpression("duplicaterule")
      q.ColumnSet <- ColumnSet([|"duplicateruleid";"name"|])
      q.Criteria <- f
      CrmDataHelper.retrieveMultiple proxy  q

    let publish (env: DG.Daxif.Environment) dupRules =
      outputfn "==================================="
      outputfn "DeployHelper: Publishing duplicate detection rules"
      outputfn "==================================="
      let proxy = env.connect().GetProxy()
      let drToPublish = 
        dupRules
        |> Array.filter (snd >> Array.contains env.url)
        |> Array.map fst 

      retrieveDR proxy
      |> Seq.filter (fun x -> drToPublish |> Array.contains (x.Attributes.["name"].ToString()))
      |> Array.ofSeq
      |> Array.map (fun x ->
        outputfn "Publishing rule '%s' " (x.Attributes.["name"].ToString())
        makePublishDupRuleRequest (Guid(x.Attributes.["duplicateruleid"].ToString())))
      |> CrmDataHelper.performAsBulk proxy
      |> Array.filter (fun x -> x.Fault = null)
      |> Array.length
      |> outputfn "Done publishing %d duplicate detection rules"
      outputfn "*************************************"
