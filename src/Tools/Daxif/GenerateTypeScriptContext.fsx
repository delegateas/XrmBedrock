(**
SolutionUpdateTsContext
*)

#load @"_Config.fsx"
open _Config
open DG.Daxif
open DG.Daxif.Common.Utility

let xrmDefinitelyTyped = Path.toolsFolder ++ @"XrmDefinitelyTyped\XrmDefinitelyTyped.exe"
let xrmTypings = Path.webResourceProject ++ @"typings\XRM"
let jsLib = Path.webResourceFolder ++ "lib"

Solution.GenerateTypeScriptContext(Env.dev, xrmDefinitelyTyped, xrmTypings,
  solutions = [
    SolutionInfo.name
    ],
  entities = [
    // Add solution entities here

    // Prerequisites for the solution to build
    "account"
    "contact"
    ],
  extraArguments = [
    "web", "XDT"
    "jsLib", jsLib
    "labelMappings", "\u2714\uFE0F: checkmark, \u26D4\uFE0F: stopsign"
    ])
