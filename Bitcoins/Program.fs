module CoinMiner


open Akka
open Akka.FSharp
open Akka.Cluster
open System
open Cutils
open Configs

[<EntryPoint>]
let main argv =

    let zeroCount = (argv.[1] |> int)
    let nodeType = argv.[0]
    let seedHostName = if nodeType<>"myNode" then argv.[2] else ""
    let port = if nodeType<>"myNode" then argv.[3] else ""
    let hostName = if nodeType="remote" then argv.[4] else ""
    let systemName = if nodeType = "myNode" then "coin-miner" else "coin-mining-cluster"
    let nodeName = sprintf "cluster-node-%s" Environment.MachineName
    
    let zeroCount = (argv.[1] |> int)
    let nodeType = argv.[0]
    let seedHostName = if nodeType<>"myNode" then argv.[2] else ""
    let port = if nodeType<>"myNode" then argv.[3] else ""
    let hostName = if nodeType="remote" then argv.[4] else ""
    let systemName = if nodeType = "myNode" then "coin-miner" else "coin-mining-cluster"
    let nodeName = sprintf "cluster-node-%s" Environment.MachineName

    let minerName = 
        if nodeType = "myNode" then "akka://" + systemName + "/user/provideMiner" else "akka.tcp://" + systemName +  "@" + seedHostName + ":" + port + "/user/provideMiner"

// Defining Hash Generator Actor

    let genHash (mailBox: Actor<MessageType>) = 
        let rec loop() = actor {
            let! message = mailBox.Receive();
            match message with
            | GenerateRandomString ->
                let randomStr = ranStr 6
                select minerName mailBox.Context.System <! {Type=Mine; Value=randomStr; Nonce=0}
                mailBox.Self <! GenerateRandomString
            | _ ->
                printfn "%s : The message is Invalid" (mailBox.Self.Path.ToStringWithAddress())
            return! loop()
        }
        loop()

 // Defining the minor actor 
 
    let doMine (mailbox: Actor<Message>) =
        let rec loop() = actor {
            let! message = mailbox.Receive ()
            match message.Type with
            | Mine -> 
                let input = if message.Nonce > 0 then (message.Value + message.Nonce.ToString()) else message.Value
                let hash = SHA256 input
                if hashLeadingZerosCount hash = zeroCount then
                    printCoin zeroCount input hash (mailbox.Self.Path.ToStringWithAddress())
                    printStats()
                elif message.Nonce < Int32.MaxValue then
                    message.Nonce <- (message.Nonce+1)
                    select minerName mailbox.Context.System <! message
            | _ ->
                printfn " Invalid message received" 
            return! loop ()
        }
        loop()

 // Creating the BOSS Node

    if nodeType = "boss" then  
        printfn "Starting the Boss node"
        printfn "Remote Nodes are invited to join"
        printfn "-------------------------------------------------------------------------------------------------------"
        let seedSystem = seedAkkaConfig seedHostName port |> System.create systemName




        let cluster = Cluster.Get seedSystem
        cluster.RegisterOnMemberUp (fun () -> 
            spawnOpt seedSystem "provideMiner" doMine [ Router(Akka.Routing.FromConfig.Instance) ] |> ignore
            let genRouter = spawnOpt seedSystem "provideGenerator" genHash [ Router(Akka.Routing.FromConfig.Instance) ]
            genRouter <! GenerateRandomString
            initStatParams()
        )

        0 |> ignore

    
// For Local Node

    elif nodeType = "myNode" then
        printfn "Starting the My PC  node"
        let system = System.create systemName <| singleNodeConfig
        spawnOpt system "provideMiner" doMine [ Router(Akka.Routing.FromConfig.Instance) ] |> ignore
        let genRouter = spawnOpt system "provideGenerator" genHash [ Router(Akka.Routing.FromConfig.Instance) ]
        genRouter <! GenerateRandomString
        initStatParams()

// For remote Node 

    elif nodeType = "remote" then
        printfn "Starting the remote node"
        let actorSystemClient = clientAkkaConfig hostName port seedHostName |> System.create systemName
        initStatParams()
        
        let clientListenerRef =  
            spawn actorSystemClient "remoteListner"
            <| fun mailbox ->
                let cluster = Cluster.Get (mailbox.Context.System)
                cluster.Subscribe (mailbox.Self, [| typeof<ClusterEvent.IMemberEvent>|])
                mailbox.Defer <| fun () -> cluster.Unsubscribe (mailbox.Self)
                printfn "An actor node is created with [%A] with roles [%s]" cluster.SelfAddress (String.Join(",", cluster.SelfRoles))
                let rec loop () = actor {
                    let! (msg: obj) = mailbox.Receive ()
                    match msg with
                    | :? ClusterEvent.MemberRemoved as actor -> 
                            printfn "The actor was removed %A" msg
                    | :? ClusterEvent.IMemberEvent           -> 
                            printfn "Task recieved from cluster %A" msg
                    | _ -> 
                            printfn "Message recieved: %A" msg
                    return! loop () 
                }
                loop ()
        0 |> ignore

    readInput()