module ClientActor

open CommonUtil
open CommonTypes.CommonTypes
open DBHelper
open Akka.FSharp
open Akka
open System
open System.IO
open System.Reflection

let clientActor (user: UserEntity) (serverActor: Actor.ActorSelection) (usersCount: int) (loggingEnabled: bool) (mailbox: Actor<Message>) = 
    let mutable sessionId = ""
    let mutable followRequestsProcessed = 0
    let mutable sentTweetsCount = 0
    let mutable receivedNotificationCount = 0

    let rec loop() = actor {
        let! message = mailbox.Receive()

        match message with
        | INIT ->
            serverActor <! Message.LOGIN({USERNAME=user.USERNAME; PASSWORD=user.PASSWORD})


// Login Response 


        | LOGIN_RESPONSE(loginResponse) ->
            if loginResponse.Status = Status.SUCCESS then
                sessionId <- loginResponse.SessionID.Value

                if loggingEnabled then
                    insertLog (getLogDTO user.USERNAME LOG_LEVEL.INFO LOG_TYPE.LOGIN (String.Format("[info] Logged in successfully with session id {0}", sessionId)))

                mailbox.Self <! FOLLOW
            else
                if loggingEnabled then
                    insertLog (getLogDTO user.USERNAME LOG_LEVEL.ERROR LOG_TYPE.LOGIN (String.Format("[error] Login failed with error {0}", loginResponse.ErrorMessage.Value)))

// Following COde 

        | FOLLOW ->
            let users = getShuffledUsers()
            let mutable counter = 0
            while counter < user.RANK.Value do
                let requestId = Guid.NewGuid.ToString()
                serverActor <! Message.FOLLOW_REQUEST({sessionId=sessionId; followeeUsername=users.[counter].USERNAME; followerUsername=None;})

                if loggingEnabled then
                    insertLog (getLogDTO user.USERNAME LOG_LEVEL.INFO LOG_TYPE.FOLLOW (String.Format("[info] Started following the user {0}", users.[counter].USERNAME)))

                counter <- counter + 1

// Following Response Code

        | FOLLOW_RESPONSE ->
            followRequestsProcessed <- followRequestsProcessed + 1
            if followRequestsProcessed = user.RANK.Value then
                let interval = usersCount - user.RANK.Value
                mailbox.Context.System.Scheduler.ScheduleTellRepeatedly(
                    System.TimeSpan.FromMilliseconds(1000.0), System.TimeSpan.FromMilliseconds(float interval), mailbox.Self, TWEET, mailbox.Self
                )
                mailbox.Context.System.Scheduler.ScheduleTellRepeatedly(
                    System.TimeSpan.FromMilliseconds(1000.0), System.TimeSpan.FromMilliseconds(float 100), mailbox.Self, QUERY, mailbox.Self
                )


        | TWEET ->
            let tweet = generateTweet()

            if loggingEnabled then
                insertLog (getLogDTO user.USERNAME LOG_LEVEL.INFO LOG_TYPE.TWEET (String.Format("[info] Posting a tweet: {0}", tweet)))
            
            serverActor <! Message.TWEET_REQUEST({sessionId=sessionId; tweet=Some(tweet); userId=None; username=None; conversationId=None; tweetId=None; replyingTo=None})


        | TWEET_RESPONSE ->
            sentTweetsCount <- sentTweetsCount + 1


        | TWEET_NOTIFICATION(tweetNotificationDTO) ->
            if loggingEnabled then
                insertLog (getLogDTO user.USERNAME LOG_LEVEL.INFO LOG_TYPE.TWEETNOTIFICATION (String.Format("[info] Received tweet notification: [Tweet={0}, Posted By={1}, Retweet={2}]", tweetNotificationDTO.tweet, tweetNotificationDTO.postedBy, tweetNotificationDTO.isRetweet)))
            
            if not(tweetNotificationDTO.isRetweet) then
                receivedNotificationCount <- receivedNotificationCount + 1
                
                if receivedNotificationCount%10 = 0 then
                    serverActor <! Message.TWEET_REQUEST({sessionId=sessionId; tweet=Some(generateRetweet()); userId=None; username=None; conversationId=None; tweetId=Some(tweetNotificationDTO.id); replyingTo=None})
                    
                    if loggingEnabled then
                        insertLog (getLogDTO user.USERNAME LOG_LEVEL.INFO LOG_TYPE.RETWEET (String.Format("[info] Retweeting: {0}", tweetNotificationDTO.tweet)))

                if receivedNotificationCount%20 = 0 then
                    let reply = generateReply()
                    serverActor <! Message.TWEET_REQUEST({sessionId=sessionId; tweet=Some(reply); userId=None; username=None; conversationId=None; tweetId=None; replyingTo=Some(tweetNotificationDTO.id)})

                    if loggingEnabled then
                        insertLog (getLogDTO user.USERNAME LOG_LEVEL.INFO LOG_TYPE.REPLY (String.Format("[info] Replied to tweet \"{0}\" with \"{1}\"", tweetNotificationDTO.tweet, reply)))

        | QUERY ->
            let messageType = if Random().NextDouble() > 0.5 then "hashtags" else "mentioned";
            let mutable hashtags = None

            if messageType = "hashtags" then
                hashtags <- Some(getRandomHashTags())

            serverActor <! Message.QUERY_REQUEST({sessionId=sessionId; userId=null; searchFor=messageType; hashtags=hashtags})

        | QUERY_RESPONSE(responseDTO) ->
            if loggingEnabled then
                insertLog (getLogDTO user.USERNAME LOG_LEVEL.INFO LOG_TYPE.QUERY (String.Format("[info] Received {0} tweets for query of type {1}", responseDTO.tweets.Count, responseDTO.queryType)))

        
        | _ -> printfn "Invalid message received"
        
        return! loop()
    }
    loop()

