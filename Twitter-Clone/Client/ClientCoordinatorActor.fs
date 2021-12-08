module ClientCoordinatorActor

open CommonUtil
open CommonTypes.CommonTypes
open ClientActor
open DBHelper
open Akka.FSharp
open Akka
open System
open System.IO
open System.Reflection

let usernamePrefix = "TwitterSimulatorUser"

let clientActorsCoordinator (usersCount: int) (serverActor: Akka.Actor.ActorSelection) (loggingEnabled: bool) (mailbox: Actor<Message>) = 
    let logPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/logs/client/coordinator/coordinator.log"

    let rec loop() = actor {
        let! message = mailbox.Receive()

        match message with
        | INIT ->
            printfn "[info ]Instantiating the actors of the User"
            let mutable rankDivider = 1;

            for i in 1..usersCount do
                let password = Guid.NewGuid().ToString()
                let username = usernamePrefix + i.ToString()
                let response: REGISTER_RESPONSE = (serverActor <? Message.REGISTER({USERNAME=username; PASSWORD=password}) |> Async.RunSynchronously)
                
                if response.STATUS = SUCCESS then
                    if loggingEnabled then
                        insertLog (getLogDTO username LOG_LEVEL.INFO LOG_TYPE.REGISTER (String.Format("[info] User {0} is registered successfully", username)))
                        mailbox.Self <! LOG(logPath, String.Format("[info] User {0} is registered successfully", username))
                    
                    let rank = (usersCount - 1)/rankDivider
                    rankDivider <- rankDivider + 1
                    addUserDetails({ID=response.ID.Value; USERNAME=username; PASSWORD=password; RANK=Some(rank)})
                else
                    if loggingEnabled then
                        insertLog (getLogDTO username LOG_LEVEL.ERROR LOG_TYPE.REGISTER (String.Format("[error] Registration failed for user {0} with error {1}", username, response.MESSAGE.Value)))
                        mailbox.Self <! LOG(logPath, String.Format("[error] Registration failed for user {0} with error {1}", username, response.MESSAGE.Value))
                
            for user in userDetails.Values do
                let clientActorRef = spawn mailbox.Context user.USERNAME (clientActor user serverActor usersCount loggingEnabled)
                clientActorRef <! INIT

            printfn "[info] All user actors has been instantiated successfully and the simulation has begun."

        | _ -> printfn "Invalid Response"
        return! loop()
    }
    loop()