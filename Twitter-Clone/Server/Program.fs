// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open DBHelper
open Config
open ServerActor
open CommonTypes.CommonTypes
open StatsGenActor
open UserUtils

open Akka.FSharp

[<EntryPoint>]
let main argv =

    let systemName = "TwitterSystem"
    let serverActorName = "TwitterServerActor"

    let mutable loggingEnabled = true
    let mutable gatherStatsMode = false
    
    if argv.Length > 0 then
        loggingEnabled <- Boolean.Parse(argv.[0])
        setLoggingEnabled loggingEnabled

    if argv.Length > 1 then
        gatherStatsMode <- Boolean.Parse(argv.[1])
        setGatherStats gatherStatsMode
    
    //Initializing all the tables
    initDB()

    //Server system actor
    let serverSystem = serverConfig "localhost" "8001" |> System.create systemName

    let serverProvider = spawnOpt serverSystem "serverProvider" serverActor [ Router(Akka.Routing.FromConfig.Instance) ]
    

    spawn serverSystem "serverStatsGenerator" (statsGenActor serverSystem) |> ignore

    Console.ReadLine() |> ignore
    0 // return an integer exit code