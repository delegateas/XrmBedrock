(**
SolutionUpdateCustomContext
*)

#load @"_Config.fsx"
open _Config
open DG.Daxif
open DG.Daxif.Common.Utility
open System.IO

let xrmContext = Path.toolsFolder ++ @"XrmContext\XrmContext.exe"
let businessDomainFolder = Path.solutionRoot ++ "src" ++ "Shared" ++ "SharedContext"

Solution.GenerateCSharpContext(Env.dev, xrmContext, businessDomainFolder,
  solutions = [
    SolutionInfo.name
    ],
  entities = [
    "account"
    "annotation"
    "appnotification"
    "contact"
    "duplicaterule"
    "environmentvariablevalue"
    "environmentvariabledefinition"
    "queue"
    "savedquery"
    "systemuser"
    "task"
    "template"
    ],
  extraArguments = [
    "deprecatedprefix", "ZZ_"
    "ns","XrmBedrock.SharedContext"
    "labelMappings", "\u2714\uFE0F: checkmark, \u26D4\uFE0F: stopsign"
    "intersect", "ICustomer:account;contact"
    ]
)
