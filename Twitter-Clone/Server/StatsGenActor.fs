module StatsGenActor

open CommonTypes.CommonTypes
open DBHelper
open Akka
open Akka.FSharp
open System
open System.IO
open System.Reflection


let statsGenActor (serverSystem: Actor.ActorSystem) (mailbox: Actor<Message>) = 
    let reportPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/reports/"
    let followersReportFile = "followers.csv"
    let tweetsReportFile = "tweets.csv"
    let generalReportFile = "report2.txt"

    let rec loop() = actor {
        let! message = mailbox.Receive()

        match message with
        | GENERATE_STATS ->
            let followesCount = getNumberOfFollowersPerUser()
            let tweetsCount = getTweetsCountPerUser()

            printfn "Users and their follower counts"
            printfn "Username,Follower Count"
            for KeyValue(key, value) in followesCount do
                printfn "%s" (String.Format("{0}, {1}", key, value))


            printfn "\n\nUsers and their tweet counts"
            printfn "Username,Tweet Count"
            for KeyValue(key, value) in tweetsCount do
                printfn "%s" (String.Format("{0}, {1}", key, value))

            
            select ("akka://TwitterSystem/user/serverProvider") serverSystem <! GATHER_STATS

        | STATS_RESPONSE(responseDTO) ->
            printfn "\n\n Performance Stats"
            printfn "Number of requests per second: %d" responseDTO.numOfRequestsPerSecond
            printfn "Average response time for Tweet requests: %d" responseDTO.avgTweetResponseTime
            printfn "Average response time for Query to fetch mentioned tweets: %d" responseDTO.avgMentionedQueryResponseTime
            printfn "Average response time for Query to fetch hashtag tweets: %d" responseDTO.avghashtagQueryResponseTime

        | _ -> printfn "Invalid message received"

        return! loop()
    }
    loop()
