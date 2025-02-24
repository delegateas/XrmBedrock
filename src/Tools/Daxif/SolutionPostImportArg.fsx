(**
SolutionPostImportArg

Configurable import script, which is mainly intended for use by the build server.
This script will only perform extended solution post-import actions, and it meant to be used after the solution import process.

Arguments:
  
  * `env=<name of environment>` (required)
  * `dir=<path to solution folder>` (recommended for build server to point at artifacts)

*)

#load @"_Config.fsx"
open _Config
open DG.Daxif
open DG.Daxif.Common.Utility


let args = fsi.CommandLineArgs |> parseArgs

let env =
  match args |> tryFindArg ["env"; "e"] with
  | Some arg -> Environment.Get arg
  | None     -> failwithf "Missing 'env' argument needed to execute this script."

let solutionFolder =
  args |> tryFindArg ["dir"; "d"] ?| Path.Daxif.crmSolutionsFolder
  
let solutionZipPath = 
  sprintf "%s.zip" SolutionInfo.name
  |> (++) solutionFolder

ExtendedSolution.PostImport(env, solutionZipPath);