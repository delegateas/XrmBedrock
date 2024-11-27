(**
Sets up all the necessary variables and functions to be used for the other scripts. 
*)
#r @"bin\Microsoft.Xrm.Sdk.dll"
#r @"bin\Microsoft.Crm.Sdk.Proxy.dll"
#r @"bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll"
#r @"bin\Microsoft.Xrm.Tooling.Connector.dll"
#r @"bin\Delegate.Daxif.dll"
open System
open System.IO
open Microsoft.Xrm.Sdk.Client
open DG.Daxif
open DG.Daxif.Common.Utility

let args = fsi.CommandLineArgs |> parseArgs

let authMethod, authSecret =
 match args |> tryFindArg ["mfaClientSecret"; "s"] with
 | Some arg -> ConnectionType.ClientSecret, arg
 | None -> ConnectionType.ConnectionString, ""

let appId =
 match args |> tryFindArg ["mfaAppId"; "a"] with
 | Some arg -> arg
 | None -> "51f81489-12ee-4a9e-aaae-a2591f45987d" // OAuth AppId

let defaultUsername =
    try
      let lines = File.ReadAllLines(@"username.txt") in
        if (lines.Length > 0) then
          let usernameFromFile = lines |> Seq.tryHead in
            match usernameFromFile with
            | Some str -> sprintf @"username=%s" str
            | None -> ""
        else
          ""
    with
        | :? System.IO.FileNotFoundException -> ""

module Env =

  let dev = 
    Environment.Create(
      name = "Dev",
      url = "https://msys-udv.crm4.dynamics.com",
      method = authMethod,
      args = fsi.CommandLineArgs,
      ap = AuthenticationProviderType.OnlineFederation,
      mfaClientSecret = authSecret,
      mfaAppId = appId,
      mfaReturnUrl = "https://login.microsoftonline.com/common/oauth2/nativeclient",
      connectionString = sprintf @"AuthType=OAuth; url=https://msys-udv.crm4.dynamics.com; %s; LoginPrompt=Always; AppId=51f81489-12ee-4a9e-aaae-a2591f45987d; RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97" defaultUsername
    )
    
  let test = 
    Environment.Create(
      name = "Test",
      url = "https://xxxx-test.crm4.dynamics.com",
      method = authMethod,
      args = fsi.CommandLineArgs,
      ap = AuthenticationProviderType.OnlineFederation,
      mfaClientSecret = authSecret,
      mfaAppId = appId,
      mfaReturnUrl = "https://login.microsoftonline.com/common/oauth2/nativeclient",
      connectionString = sprintf @"AuthType=OAuth; url=https://xxxx-test.crm4.dynamics.com; %s; LoginPrompt=Always; AppId=51f81489-12ee-4a9e-aaae-a2591f45987d; RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97" defaultUsername
    )

  let uat = 
    Environment.Create(
      name = "UAT",
      url = "https://xxxx-uat.crm4.dynamics.com",
      method = authMethod,
      args = fsi.CommandLineArgs,
      ap = AuthenticationProviderType.OnlineFederation,
      mfaClientSecret = authSecret,
      mfaAppId = appId,
      mfaReturnUrl = "https://login.microsoftonline.com/common/oauth2/nativeclient",
      connectionString = sprintf @"AuthType=OAuth; url=https://xxxx-uat.crm4.dynamics.com; %s; LoginPrompt=Always; AppId=51f81489-12ee-4a9e-aaae-a2591f45987d; RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97" defaultUsername
    )

  let prod = 
    Environment.Create(
      name = "Production",
      url = "https://xxxx.crm4.dynamics.com",
      method = authMethod,
      args = fsi.CommandLineArgs,
      ap = AuthenticationProviderType.OnlineFederation,
      mfaClientSecret = authSecret,
      mfaAppId = appId,
      mfaReturnUrl = "https://login.microsoftonline.com/common/oauth2/nativeclient",
      connectionString = sprintf @"AuthType=OAuth; url=https://xxxx.crm4.dynamics.com; %s; LoginPrompt=Always; AppId=51f81489-12ee-4a9e-aaae-a2591f45987d; RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97" defaultUsername
    )

(** 
CRM Solution Setup 
------------------
*)
module SolutionInfo =
  let name = @"XrmBedrock"
  let displayName = @"XrmBedrock"

module PublisherInfo =
  let prefix = @"xb"
  let name = @"XrmBedrock"
  let displayName = @"XrmBedrock"

(** 
Path and project setup 
----------------------
*)
Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

module Path =
  let daxifRoot = __SOURCE_DIRECTORY__
  let solutionRoot = daxifRoot ++ @"..\..\.."
  let dataverseRoot = solutionRoot ++ @"Dataverse"
  let toolsFolder = daxifRoot ++ @".."
  
  let webResourceProject = dataverseRoot ++ "src" ++ @"WebResources"
  let webResourceFolder = webResourceProject ++ @"src" ++ (sprintf "%s_%s" PublisherInfo.prefix SolutionInfo.name)
  
  let pluginDllName = "ILMerged.XrmBedrock.Plugins"

  /// Path information used by the SolutionPackager scripts
  module SolutionPack =
    let projName = "SolutionBlueprint"
    let projFolder = solutionRoot ++ projName
    let xmlMappingFile = projFolder ++ (sprintf "%s.xml" SolutionInfo.name)
    let customizationsFolder = projFolder ++ @"customizations"
    let projFile = projFolder ++ (sprintf @"%s.csproj" projName)

  /// Paths Daxif uses to store/load files
  module Daxif =
    let crmSolutionsFolder = daxifRoot ++ "solutions"
    let unmanagedSolution = crmSolutionsFolder ++ (sprintf "%s.zip" SolutionInfo.name)
    let managedSolution = crmSolutionsFolder ++ (sprintf "%s_managed.zip" SolutionInfo.name)
    let translationsFolder = daxifRoot ++ "translations"
    let metadataFolder = daxifRoot ++ "metadata"
    let dataFolder = daxifRoot ++ "data"
    let stateFolder = daxifRoot ++ "state"
    let associationsFolder = daxifRoot ++ "associations"
    let mappingFolder = daxifRoot ++ "mapping"
    let importedFolder = daxifRoot ++ "imported"