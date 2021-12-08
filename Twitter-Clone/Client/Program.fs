open Config
open CommonTypes.CommonTypes
open ClientCoordinatorActor
open DBHelper

open System
open Akka.FSharp
open Akka.Cluster

[<EntryPoint>]
let main argv =
    
    let systemName = "TwitterSystem"
    let serverActorName = "serverProvider"
    let serverStatsGenActor = "serverStatsGenerator"
    let clientCoordinatorActorName = "TwitterClientCoordinator"

    let clientSystem = clientConfig "localhost" "8001" "localhost" |> System.create systemName
    let serverActor = select ("akka.tcp://" + systemName + "@localhost:8001/user/" + serverActorName) clientSystem
    let serverStatsActor = select ("akka.tcp://" + systemName + "@localhost:8001/user/" + serverStatsGenActor) clientSystem

    let usersCount = (argv.[0] |> int)
    let mutable loggingEnabled = true

    if argv.Length > 1 then
        loggingEnabled <- Boolean.Parse(argv.[1])

    initDB()
    
    let clientCoordinatorRef = spawn clientSystem clientCoordinatorActorName (clientActorsCoordinator usersCount serverActor loggingEnabled)

    let cluster = Cluster.Get clientSystem
    cluster.RegisterOnMemberUp (fun () -> 
        printfn "[info] Successfully connected to the server. Starting the simulator with %d users" usersCount
        clientCoordinatorRef <! INIT
    )

    let rec readInput() =
        let command = System.Console.ReadLine()
        
        match command with
        |   "generateStats" ->
                serverStatsActor <! GENERATE_STATS
                readInput()
        | _ -> readInput()

    readInput()

    Console.ReadLine() |> ignore

    0 // return an integer exit code