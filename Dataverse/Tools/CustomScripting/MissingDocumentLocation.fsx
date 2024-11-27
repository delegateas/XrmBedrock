#load @"..\Daxif\_Config.fsx"

open _Config
open DG.Daxif.Common
open Microsoft.Xrm.Sdk
open Microsoft.Xrm.Sdk.Messages
open Microsoft.Xrm.Sdk.Metadata

let env = Env.test
let proxy = env.connect().GetProxy()

let req = RetrieveAllEntitiesRequest () in
req.EntityFilters <- EntityFilters.Relationships;
req.RetrieveAsIfPublished <- true;
let resp = proxy.Execute req :?> RetrieveAllEntitiesResponse in
resp.EntityMetadata
|> Array.filter (fun em ->
  em.OneToManyRelationships
//  |> Array.iter (fun r -> printfn "%A" r.ReferencingEntity)
  |> Array.exists (fun r -> 
    r.ReferencingEntity = "activitypointer" || 
    r.ReferencingEntity = "annotation") && not em.IsDocumentManagementEnabled.Value
)
|> Array.iter (fun em -> printfn "%A" em.LogicalName) 
