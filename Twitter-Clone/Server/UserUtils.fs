module UserUtils

open System
open DBHelper
open CommonUtil
open System.Collections.Generic
open CommonTypes.CommonTypes
open Akka.FSharp
open Akka.Actor

let mutable loggingEnabled: bool = true
let mutable gatherStatsMode: bool = true

let setLoggingEnabled value =
    loggingEnabled <- value


let setGatherStats value =
    gatherStatsMode <- value


let registerUser (registerDTO: REGISTER_DTO) = 
    let mutable response = Unchecked.defaultof<REGISTER_RESPONSE>

    if not(isUserPresent registerDTO.USERNAME) then
        if loggingEnabled then
            printfn "[info] Register request received for user %s" registerDTO.USERNAME

        let id = 
            insert 
                (string Entity.USERS) 
                (dict [ ("@username", registerDTO.USERNAME); ("@password", (hashString registerDTO.PASSWORD)) ]) true
        if loggingEnabled then
            printfn "[info] User %s registered successfully" registerDTO.USERNAME
        response <- { STATUS=Status.SUCCESS; MESSAGE=None; ID=Some(id) }
    else
        response <- { STATUS=Status.FAILED; MESSAGE=Some("User already exists"); ID=None }

    response



let loginUser (loginDTO: LoginRequestDTO) =
    if loggingEnabled then
        printfn "[info] Login request received for user %s" loginDTO.USERNAME
    let mutable loginResponse: LoginResponse = Unchecked.defaultof<LoginResponse>
    let user: UserEntity = getUserByName loginDTO.USERNAME

    if obj.ReferenceEquals(user, null) then
        if loggingEnabled then
            printfn "[error] Login failed for user %s" loginDTO.USERNAME
        loginResponse <- {
            Status=Status.FAILED;
            ErrorMessage=Some("User not found");
            SessionID=None
        }
    else
        if user.PASSWORD = (hashString loginDTO.PASSWORD) then
            if loggingEnabled then
                printfn "[info] Login was successful for user %s" loginDTO.USERNAME
            let sessionId = Guid.NewGuid().ToString()
            loginResponse <- {
                Status=Status.SUCCESS;
                ErrorMessage=None;
                SessionID=Some(sessionId)
            }
            addLoggedInUser {ID=user.ID; USERNAME=user.USERNAME} sessionId
        else
            if loggingEnabled then
                printfn "[error] Login failed for user %s" loginDTO.USERNAME
            loginResponse <- {
                Status=Status.FAILED;
                ErrorMessage=Some("Invalid password");
                SessionID=None
            }
    loginResponse



let followUser (followRequest: FollowRequestDTO) = 
    let mutable response = Unchecked.defaultof<GenericResponse>
    let userIds = getUserIdsByNames(new List<string>([ followRequest.followeeUsername; followRequest.followerUsername.Value ]))
    
    if not(userIds.ContainsKey(followRequest.followeeUsername)) then
        response <- { Status=Status.FAILED; ErrorMessage=Some("The user you want to follow does not exist") }
    else
        insert 
            (string Entity.FOLLOWERS) 
            (dict [ ("@followeeId", userIds.[followRequest.followeeUsername]); ("@followerId", userIds.[followRequest.followerUsername.Value]) ]) false |> ignore
        response <- { Status=Status.SUCCESS; ErrorMessage=None }
        if loggingEnabled then
            printfn "[info] %s started following %s" followRequest.followerUsername.Value followRequest.followeeUsername
    response



let processHashtagsAndUsers (tweet: string) (tweetId: string) (conversationId: string) (userId: string) (username: string) (serverSystem: ActorSystem) (isRetweet: bool) = 
    let hashtags = getHashTagsFromTweet tweet
    let usernames = getUsernamesFromTweet tweet
    let notifiedUsers: HashSet<string> = new HashSet<string>()

    insert 
        (string Entity.CONVERSATION_SUBSCRIPTIONS) 
        (dict [ ("@coversationId", conversationId); ("@userId", userId) ]) false |> ignore

    if hashtags.Count > 0 then
        let existingHashtags = getHashTagsByValues hashtags
        let newHashtags = getListsDiff hashtags (new List<string>(existingHashtags.Keys))
        let newHashtagsMapping = new Dictionary<string, string>()

        if newHashtags.Count > 0 then
            for hashtag in newHashtags do
                newHashtagsMapping.Add(hashtag, insert (string Entity.HASHTAGS) (dict [ ("@value", hashtag) ]) true)

        for hashtag in hashtags do
            let mutable hashtagId = ""

            if (existingHashtags.ContainsKey(hashtag)) then
                hashtagId <- existingHashtags.[hashtag]
            else
                hashtagId <- newHashtagsMapping.[hashtag]

            insert 
                (string Entity.TWEETS_HASHTAGS) 
                (dict [ ("@tweetId", tweetId); ("@hashtagId", hashtag) ]) false |> ignore

    if usernames.Count > 0 then
        let existingUsers = getUserIdsByNames usernames

        for KeyValue(username, id) in existingUsers do
            if not(isSubscriptionExists id conversationId) then
                insert 
                    (string Entity.CONVERSATION_SUBSCRIPTIONS) 
                    (dict [ ("@coversationId", conversationId); ("@userId", id) ]) false |> ignore

            insert 
                (string Entity.TWEETS_MENTIONS) 
                (dict [ ("@tweetId", tweetId); ("@userId", id) ]) false |> ignore

            if userRemoteAddresses.ContainsKey username then
                select userRemoteAddresses.[username] serverSystem <! Message.TWEET_NOTIFICATION({id=tweetId; tweet=tweet; tweetType=TWEET_TYPE.MENTION; isRetweet=isRetweet; isReply=false; postedBy=username})
                notifiedUsers.Add(username) |> ignore

    notifiedUsers
        


let notifyFollowers (userId: string) (username: string) (notifiedUsers: HashSet<String>) (serverSystem: ActorSystem) (tweetId: string) (tweet: string) (isRetweet: bool) (isReply: bool) =
    let followers = getFollowerUsernames userId
    
    if followers.Count > 0 then
        for follower in followers do
            if not(notifiedUsers.Contains follower) && userRemoteAddresses.ContainsKey follower then
                select userRemoteAddresses.[follower] serverSystem <! Message.TWEET_NOTIFICATION({id=tweetId; tweet=tweet; tweetType=TWEET_TYPE.TWEET; isRetweet=isRetweet; isReply=isReply; postedBy=username})



let postTweet (tweetRequest: TweetRequestDTO) (serverSystem: ActorSystem) =
    if tweetRequest.conversationId.IsNone then
        tweetRequest.conversationId <- Some(insert (string Entity.CONVERSATIONS) (new Dictionary<string, _>()) true)

    let tweetId = 
        insert 
            (string Entity.TWEETS) 
            (dict [ ("@conversationId", tweetRequest.conversationId.Value); ("@userId", tweetRequest.userId.Value); ("@tweet", tweetRequest.tweet.Value); ("@originTweetId", null); ("@postTime", (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString())); ("@replyingTo", null) ]) true

    let notifiedUsers = processHashtagsAndUsers tweetRequest.tweet.Value tweetId tweetRequest.conversationId.Value tweetRequest.userId.Value tweetRequest.username.Value serverSystem false
    notifyFollowers tweetRequest.userId.Value tweetRequest.username.Value notifiedUsers serverSystem tweetId tweetRequest.tweet.Value false false
    if loggingEnabled then
        printfn "[info] %s posted a tweet: %s" tweetRequest.userId.Value tweetRequest.tweet.Value



let reTweet (tweetRequest: TweetRequestDTO) (serverSystem: ActorSystem) = 
    let conversationId = insert (string Entity.CONVERSATIONS) (new Dictionary<string, _>()) true
    let tweetId = 
            insert 
                (string Entity.TWEETS) 
                (dict [ ("@conversationId", conversationId); ("@userId", tweetRequest.userId.Value); ("@tweet", tweetRequest.tweet.Value); ("@originTweetId", tweetRequest.tweetId.Value); ("@postTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()); ("@replyingTo", null) ])
                true

    let mutable notifiedUsers: HashSet<string> = new HashSet<string>()

    if tweetRequest.tweet.IsSome then
        notifiedUsers <- processHashtagsAndUsers tweetRequest.tweet.Value tweetId conversationId tweetRequest.userId.Value tweetRequest.username.Value serverSystem true

    notifyFollowers tweetRequest.userId.Value tweetRequest.username.Value notifiedUsers serverSystem tweetId tweetRequest.tweet.Value false false



let replyToTweet (tweetRequest: TweetRequestDTO) (serverSystem: ActorSystem) =
    let conversationId = getConversationIdByTweetId tweetRequest.replyingTo.Value

    let tweetId =
            insert
                (string Entity.TWEETS)
                (dict [ ("@conversationId", conversationId); ("@userId", tweetRequest.userId.Value); ("@tweet", tweetRequest.tweet.Value); ("@originTweetId", null); ("@postTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()); ("@replyingTo", tweetRequest.replyingTo.Value) ])
                true
    
    let subscribers = getSubscribers conversationId

    for subscriber in subscribers do
        if userRemoteAddresses.ContainsKey subscriber then
            select userRemoteAddresses.[subscriber] serverSystem <! Message.TWEET_NOTIFICATION({id=tweetId; tweet=tweetRequest.tweet.Value; tweetType=TWEET_TYPE.TWEET; isRetweet=false; isReply=false; postedBy=tweetRequest.username.Value})

