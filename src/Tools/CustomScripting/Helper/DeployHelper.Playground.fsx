(**
Test
*)
#load @"..\..\Daxif\_Config.fsx"
#load @"DeployHelper.Main.fsx"

module DeployHelper = DeployHelper.DeployHelper

(** Open libraries for use *)
open System
open System.IO
open _Config


DeployHelper.publishAll Env.dev
