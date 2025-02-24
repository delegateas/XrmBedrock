(**
PluginSyncDev
*)

#load @"_Config.fsx"
open _Config
open DG.Daxif
open DG.Daxif.Common.Utility

let pluginProjFile = Path.srcRoot ++ "Dataverse" ++ @"Plugins\Plugins.csproj"
let pluginDll = Path.srcRoot ++ "Dataverse" ++ @"Plugins\bin\Release\net462\" ++ Path.pluginDllName + ".dll"

Plugin.Sync(Env.dev, pluginDll, pluginProjFile, SolutionInfo.name)
