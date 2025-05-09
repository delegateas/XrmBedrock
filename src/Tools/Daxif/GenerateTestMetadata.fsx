(**
SolutionUpdateCustomContext
*)

#load @"_Config.fsx"
open _Config
open DG.Daxif
open DG.Daxif.Common.Utility
open System.IO

let xrmMockupMetadataGenerator = Path.toolsFolder ++ @"MetadataGenerator\MetadataGenerator365.exe"
Solution.GenerateXrmMockupMetadata(Env.dev, xrmMockupMetadataGenerator, Path.solutionRoot ++ "test" ++ "SharedTest" ++ "MetadataGenerated",
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
  ]
)
