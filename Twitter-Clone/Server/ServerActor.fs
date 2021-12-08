module ServerActor

open CommonUtil
open UserUtils
open DBHelper
open CommonTypes.CommonTypes

open Akka
open Akka.FSharp
open System

let serverActor (mailbox: Actor<Message>) = 
    let mutable numOfRequests = 1
    let mutable numOfTweetRequests = 1
    let mutable tweetResponseTime = 1
    let mutable numOfhashtagQueryRequests = 1
    let mutable hashtagQueryResponseTime = 1
    let mutable numOfmentionedQueryRequests = 1
    let mutable mentionedQueryResponseTime = 1
    let globalStopwatch = System.Diagnostics.Stopwatch()
    globalStopwatch.Start()

    let rec loop() = actor {
        let! message = mailbox.Receive()
        
        match message with
        | REGISTER(registerDTO) ->
            
            if gatherStatsMode then
                numOfRequests <- numOfRequests + 1

            let response = registerUser registerDTO
            mailbox.Sender() <! response


        | LOGIN(loginDTO) ->
            let response = loginUser loginDTO
            addUserAddress loginDTO.USERNAME (mailbox.Sender().Path.ToStringWithAddress())
            mailbox.Sender() <! Message.LOGIN_RESPONSE(response)

            if gatherStatsMode then
                numOfRequests <- numOfRequests + 1


        | FOLLOW_REQUEST(followRequestDTO) ->
            let loggedInUsername = getLoggedInUsername followRequestDTO.sessionId
            if loggedInUsername.Length > 0 then
                followRequestDTO.followerUsername <- Some(loggedInUsername)
                followUser followRequestDTO |> ignore
                mailbox.Sender() <! Message.FOLLOW_RESPONSE

                if gatherStatsMode then
                    numOfRequests <- numOfRequests + 1


        | TWEET_REQUEST(tweetRequest) ->
            let loggedInUserId = getLoggedInUserId tweetRequest.sessionId
            if loggedInUserId.Length > 0 then
                tweetRequest.userId <- Some(loggedInUserId)
                tweetRequest.username <- Some(getLoggedInUsername tweetRequest.sessionId)
                
                let stopwatch = System.Diagnostics.Stopwatch()
                if gatherStatsMode then
                    stopwatch.Start()
                    
                if tweetRequest.tweetId.IsSome then
                    reTweet tweetRequest mailbox.Context.System
                elif tweetRequest.replyingTo.IsSome then
                    replyToTweet tweetRequest mailbox.Context.System
                else
                    postTweet tweetRequest mailbox.Context.System
                mailbox.Sender() <! Message.TWEET_RESPONSE

                if gatherStatsMode then
                    numOfRequests <- numOfRequests + 1
                    numOfTweetRequests <- numOfTweetRequests + 1
                    tweetResponseTime <- tweetResponseTime + ((int) stopwatch.ElapsedMilliseconds)


        | QUERY_REQUEST(queryRequest) ->
            let loggedInUserId = getLoggedInUserId queryRequest.sessionId
            if loggedInUserId.Length > 0 then 
                
                
                let stopwatch = System.Diagnostics.Stopwatch()
                if gatherStatsMode then
                    stopwatch.Start()   

                match queryRequest.searchFor with
                | "mentioned" ->
                    let tweets = getMentionedTweets loggedInUserId
                    mailbox.Sender() <! Message.QUERY_RESPONSE({tweets=tweets; queryType="mentioned"})
                | "hashtags" ->
                    let tweets = getTweetsByHashtags queryRequest.hashtags.Value
                    mailbox.Sender() <! Message.QUERY_RESPONSE({tweets=tweets; queryType="hashtags"})
                | _ -> printfn "Invalid message received"

                if gatherStatsMode then
                    numOfRequests <- numOfRequests + 1
                    if queryRequest.searchFor = "mentioned" then
                        numOfmentionedQueryRequests <- numOfmentionedQueryRequests + 1
                        mentionedQueryResponseTime <- mentionedQueryResponseTime + ((int) stopwatch.ElapsedMilliseconds)
                    else
                        numOfhashtagQueryRequests <- numOfhashtagQueryRequests + 1
                        hashtagQueryResponseTime <- hashtagQueryResponseTime + ((int) stopwatch.ElapsedMilliseconds)
                    stopwatch.Stop()

        | GATHER_STATS ->
            printfn "%d %d %d %d" (globalStopwatch.ElapsedMilliseconds)  numOfTweetRequests numOfTweetRequests numOfhashtagQueryRequests
            select ("akka://TwitterSystem/user/serverStatsGenerator") mailbox.Context.System <! STATS_RESPONSE({
                numOfRequestsPerSecond = (numOfRequests/((int)globalStopwatch.ElapsedMilliseconds/1000));
                avgTweetResponseTime =  (tweetResponseTime/numOfTweetRequests);
                avgMentionedQueryResponseTime = (mentionedQueryResponseTime/numOfTweetRequests);
                avghashtagQueryResponseTime = (hashtagQueryResponseTime/numOfhashtagQueryRequests);
            })
        | _ -> printfn "Invalid message received"
        return! loop()
    }
    loop()

