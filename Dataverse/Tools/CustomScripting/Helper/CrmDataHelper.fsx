
(** 
CrmDataHelper
*)
#r @"System.ServiceModel"
#r @"..\..\Daxif\Microsoft.Xrm.Sdk.dll"
#r @"..\..\Daxif\Microsoft.Crm.Sdk.Proxy.dll"

open System
open System.Net
open Microsoft.Crm.Sdk
open Microsoft.Xrm.Sdk
open Microsoft.Xrm.Sdk.Client
open Microsoft.Xrm.Sdk.Query
open Microsoft.Xrm.Sdk.Metadata
open Microsoft.Xrm.Sdk.Messages


module CrmDataHelper =

  let getResponse<'T when 'T :> OrganizationResponse> (proxy:OrganizationServiceProxy) request =
    proxy.Timeout <- TimeSpan(1,0,0)
    (proxy.Execute(request)) :?> 'T


  let performAsBulk proxy reqs = 
    reqs
    |> Array.chunkBySize 1000
    |> Array.map (fun splitReqs ->
      let req = ExecuteMultipleRequest()
      req.Requests <- OrganizationRequestCollection()
      req.Requests.AddRange(splitReqs)
      req.Settings <- ExecuteMultipleSettings()
      req.Settings.ContinueOnError <- true
      req.Settings.ReturnResponses <- true
      (getResponse<ExecuteMultipleResponse> proxy req).Responses
    ) |> Seq.concat
    |> Array.ofSeq

  let performAsBulkWithOutput proxy reqs =
    let resp = performAsBulk proxy reqs
    resp |> Array.iter (fun r -> if r.Fault <> null then eprintfn "Error when performing %s: %s" reqs.[r.RequestIndex].RequestName r.Fault.Message)
    resp |> Array.filter (fun x -> x.Fault = null)
    |> Array.length
    |> fun count -> printfn "Succesfully performed %d/%d actions in %A" count reqs.Length proxy.ServiceConfiguration.CurrentServiceEndpoint.Address

  let performAsParallelBulkHandle proxyGetter reqs handleResponses = 
    reqs
    |> Array.chunkBySize 1000
    |> Array.map (fun splitReqs -> 
      async {
        let (proxy : OrganizationServiceProxy) = proxyGetter()
        proxy.Timeout <- TimeSpan(1, 0, 0)
        let req = ExecuteMultipleRequest()
        req.Requests <- OrganizationRequestCollection()
        req.Requests.AddRange(splitReqs)
        req.Settings <- ExecuteMultipleSettings()
        req.Settings.ContinueOnError <- true
        req.Settings.ReturnResponses <- true
        let resp = (getResponse<ExecuteMultipleResponse> proxy req).Responses
        handleResponses resp
        return resp
      }) 
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Seq.concat
    |> Array.ofSeq

  let performAsParallelBulk proxyGetter reqs = 
    performAsParallelBulkHandle proxyGetter reqs ignore

  let performAsParallelBulkWithOutput proxyGetter reqs =
    let resp = performAsParallelBulkHandle proxyGetter reqs (fun resps -> printfn "Finished %d requests." resps.Count)
    let proxy = proxyGetter()
    proxy.Timeout <- TimeSpan(1, 0, 0)
    resp |> Array.iter (fun r -> if r.Fault <> null then eprintfn "Error when performing %s: %s" reqs.[r.RequestIndex].RequestName r.Fault.Message)
    resp |> Array.filter (fun x -> x.Fault = null)
    |> Array.length
    |> fun count -> printfn "Succesfully performed %d/%d actions in %A" count reqs.Length proxy.ServiceConfiguration.CurrentServiceEndpoint.Address

  module internal Metadata = 
    let private entityHelper proxy logicalName filter = 
      let (proxy : OrganizationServiceProxy) = proxy
      let (filter : EntityFilters) = filter
      let req = OrganizationRequest()
      let param = ParameterCollection()
      param.Add(@"LogicalName", logicalName)
      param.Add(@"EntityFilters", filter)
      param.Add(@"MetadataId", Guid.Empty)
      param.Add(@"RetrieveAsIfPublished", true)
      req.RequestName <- @"RetrieveEntity"
      req.Parameters.AddRange(param)
      let resp = proxy.Execute(req)
      (Seq.head resp.Results).Value :?> EntityMetadata
    
    let entity proxy logicalName = 
      entityHelper proxy logicalName EntityFilters.Entity

  module CRUD =
    open System.Text

    let retrieveMultiple proxy query = 
      let (proxy : OrganizationServiceProxy) = proxy
      let (query : QueryExpression) = query
      if query.TopCount.HasValue |> not then
        query.PageInfo <- PagingInfo()
        query.PageInfo.ReturnTotalRecordCount <- true
        query.PageInfo.PageNumber <- 1
        query.PageInfo.Count <- 1000 // ItemsInEachCall 
      seq { 
        let resp = proxy.RetrieveMultiple(query)
        yield! resp.Entities
        let rec retrieveMultiple' (ec : EntityCollection) pn = 
          seq { 
            match ec.MoreRecords with
            | true -> 
              query.PageInfo.PageNumber <- (pn + 1)
              query.PageInfo.PagingCookie <- ec.PagingCookie
              let resp' = proxy.RetrieveMultiple(query)
              yield! resp'.Entities
              yield! retrieveMultiple' resp' (pn + 1)
            | false -> ()
          }
        yield! retrieveMultiple' resp 1
      }

    let retrieve (proxy:OrganizationServiceProxy) logicalname guid =
      proxy.Retrieve(logicalname, guid, ColumnSet(true))

    let retrieveImportJob proxy guid = 
      let (proxy : OrganizationServiceProxy) = proxy
      let req = new Messages.RetrieveFormattedImportJobResultsRequest()
      req.ImportJobId <- guid
      let resp = proxy.Execute(req) :?> Messages.RetrieveFormattedImportJobResultsResponse
      let xml = resp.FormattedResults
      let bytes = Encoding.UTF8.GetBytes(xml)
      Encoding.UTF8.GetString(bytes)

    let makeDeactivate logicalName guid state = 
      let req = Messages.SetStateRequest()
      req.EntityMoniker <- EntityReference(logicalName, id = guid)
      req.State <- OptionSetValue(state)
      req.Status <- OptionSetValue(-1)
      req :> OrganizationRequest

    let makeDelete logicalName guid = 
      let req = Messages.DeleteRequest()
      req.Target <- EntityReference(logicalName, id = guid)
      req :> OrganizationRequest

    let delete proxy logicalName guid = 
      let (proxy : OrganizationServiceProxy) = proxy
      proxy.Execute(makeDelete logicalName guid) :?> Messages.DeleteResponse |> ignore

    let publish proxy = 
      let (proxy : OrganizationServiceProxy) = proxy
      let req = Messages.PublishAllXmlRequest()
      proxy.Execute(req) :?> Messages.PublishAllXmlResponse |> ignore

  module Entities =
    let retrieveSolution proxy uniqueName = 
      let (uniqueName : string) = uniqueName
      let ln = @"solution"
      let an = @"uniquename"
      let f = FilterExpression()
      f.AddCondition
        (ConditionExpression(an, ConditionOperator.Equal, uniqueName))
      let q = QueryExpression(ln)
      q.ColumnSet <- ColumnSet(an)
      q.Criteria <- f
      CRUD.retrieveMultiple proxy q |> Seq.head

    let retrievePluginAssemblies proxy solutionId = 
      let (solutionId : Guid) = solutionId
      let ln = @"pluginassembly"
      let an = @"solutionid"
      let em = Metadata.entity proxy ln
      let le = LinkEntity()
      le.JoinOperator <- JoinOperator.Inner
      le.LinkFromAttributeName <- @"pluginassemblyid"
      le.LinkFromEntityName <- @"pluginassembly"
      le.LinkToAttributeName <- @"objectid"
      le.LinkToEntityName <- @"solutioncomponent"
      le.LinkCriteria.Conditions.Add
        (ConditionExpression(an, ConditionOperator.Equal, solutionId))
      let q = QueryExpression(ln)
      q.ColumnSet <- ColumnSet(em.PrimaryIdAttribute, em.PrimaryNameAttribute)
      q.LinkEntities.Add(le)
      CRUD.retrieveMultiple proxy q

    let retrievePluginProcessingSteps proxy typeId = 
      let (typeId : Guid) = typeId
      let ln = @"sdkmessageprocessingstep"
      let an = @"solutionid"
      let an' = @"plugintypeid"
      let f = FilterExpression()
      f.AddCondition(ConditionExpression(an', ConditionOperator.Equal, typeId))
      let q = QueryExpression(ln)
      q.ColumnSet <- ColumnSet(true)
      q.Criteria <- f
      CRUD.retrieveMultiple proxy q

    let retrievePluginTypes proxy assemblyId = 
      let (assemblyId : Guid) = assemblyId
      let ln = @"plugintype"
      let an = @"pluginassemblyid"
      let em = Metadata.entity proxy ln
      let f = FilterExpression()
      f.AddCondition
        (ConditionExpression(an, ConditionOperator.Equal, assemblyId))
      let q = QueryExpression(ln)
      q.ColumnSet <- ColumnSet(em.PrimaryIdAttribute, em.PrimaryNameAttribute)
      q.Criteria <- f
      CRUD.retrieveMultiple proxy q

    let retrievePluginProcessingStepImages proxy stepId = 
      let (stepId : Guid) = stepId
      let ln = @"sdkmessageprocessingstepimage"
      let an' = @"sdkmessageprocessingstepid"
      let f = FilterExpression()
      f.AddCondition(ConditionExpression(an', ConditionOperator.Equal, stepId))
      let q = QueryExpression(ln)
      q.ColumnSet <- ColumnSet(true)
      q.Criteria <- f
      CRUD.retrieveMultiple proxy q

    let retrieveBusinessRules proxy = 
      let logicalName = @"workflow"
      let q = QueryExpression(logicalName)
      let f = FilterExpression()
      f.AddCondition(ConditionExpression("category", ConditionOperator.Equal, 2))
      f.AddCondition(ConditionExpression("ismanaged", ConditionOperator.Equal, false))
      q.ColumnSet <- ColumnSet(true)
      q.Criteria <- f
      CRUD.retrieveMultiple proxy q


module Auth = 
  let getOrganizationServiceProxy (serviceManagement : IServiceManagement<IOrganizationService>) 
      (authCredentials : AuthenticationCredentials) = 
    let ac = authCredentials
    match serviceManagement.AuthenticationType with
    | AuthenticationProviderType.ActiveDirectory -> 
      new OrganizationServiceProxy(serviceManagement, ac.ClientCredentials)
    | _ -> 
      new OrganizationServiceProxy(serviceManagement, ac.SecurityTokenResponse) 
    |> fun osp -> 
        osp.Timeout <- new TimeSpan(0, 59, 0)
        osp // Almost 1-hour timeout
    
  let getCredentials provider username password domain = 
    let (password_ : string) = password
    let ac = AuthenticationCredentials()
    match provider with
    | AuthenticationProviderType.ActiveDirectory -> 
      ac.ClientCredentials.Windows.ClientCredential <- 
        new NetworkCredential(username, password_, domain)
    //      | AuthenticationProviderType.LiveId -> // CRM Online using Live Id
    //        ac.ClientCredentials.UserName.UserName <- username
    //        ac.ClientCredentials.UserName.Password <- password_
    //        ac.SupportingCredentials <- new AuthenticationCredentials()
    //        ac.SupportingCredentials.ClientCredentials <-
    //          DeviceIdManager.LoadOrRegisterDevice
    //                                                        ()
    | AuthenticationProviderType.OnlineFederation -> // CRM Online using Office 365 
      ac.ClientCredentials.UserName.UserName <- username
      ac.ClientCredentials.UserName.Password <- password_
    | AuthenticationProviderType.Federation -> // Local Federation
      ac.ClientCredentials.UserName.UserName <- username
      ac.ClientCredentials.UserName.Password <- password_
    | _ -> failwith "No valid authentification provider was used."
    ac

  let authenticate uri provider username password domain =
    let m = ServiceConfigurationFactory.CreateManagement<IOrganizationService>(uri)
    let ac = m.Authenticate(getCredentials provider username password domain)
    let proxy = getOrganizationServiceProxy m ac
    proxy.ServiceConfiguration.CurrentServiceEndpoint.Behaviors.Add(new ProxyTypesBehavior());
    proxy
