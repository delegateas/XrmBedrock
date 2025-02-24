(**
Playground
*)

#load @"_Config.fsx"
#r @"System.ServiceModel"
#r @"bin\Newtonsoft.Json.dll"
open _Config
open System
open System.IO
open System.Collections.Generic
open Microsoft.Xrm.Sdk
open Microsoft.Xrm.Sdk.Client
open Microsoft.Xrm.Sdk.Messages
open Microsoft.Xrm.Sdk.Query

open DG.Daxif
open DG.Daxif.Common
open DG.Daxif.Common.Utility
open Newtonsoft.Json

let proxy = Env.dev.connect().GetService()
