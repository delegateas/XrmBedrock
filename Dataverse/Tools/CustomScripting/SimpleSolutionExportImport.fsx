
#load @"..\Daxif\_Config.fsx"


open System
open DG.Daxif.Modules
open _Config
open DG.Daxif.Common
module cfg = LF.LF.Medlemssystem.Config


(** Configuration *)
cfg.ensureFolder cfg.solutions

let solution = "PluginDLL"
let zip = cfg.solutions + solution + @".zip"

let fromEnv = devEnv

let toEnvs = 
  [|
    // Test
//    testEnv


    // Production
//    prodEnv
    
  |]
;;


(** Implementation *)
Solution.export
    fromEnv.wsdl solution cfg.solutions false 
        cfg.authType fromEnv.usr fromEnv.pwd fromEnv.dmn
            cfg.log
;;

toEnvs
|> Array.iter (fun (toEnv: Environment) ->
  Solution.import
        toEnv.wsdl solution zip false
          cfg.authType toEnv.usr toEnv.pwd toEnv.dmn
              cfg.log

  Solution.pluginSteps
      toEnv.wsdl solution true
          cfg.authType toEnv.usr toEnv.pwd toEnv.dmn
              cfg.log
)

