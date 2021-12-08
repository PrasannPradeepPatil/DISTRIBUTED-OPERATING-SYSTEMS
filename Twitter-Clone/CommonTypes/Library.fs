namespace CommonTypes

open System.Collections.Generic

module CommonTypes =

    type Status =
        | SUCCESS
        | FAILED

    type TWEET_TYPE = 
        | TWEET
        | MENTION

    type LOG_LEVEL =
        | INFO
        | ERROR

    type LOG_TYPE = 
        | LOGIN
        | REGISTER
        | TWEET
        | RETWEET
        | FOLLOW
        | TWEETNOTIFICATION
        | REPLY
        | QUERY

    type REGISTER_DTO = {
        USERNAME: string;
        PASSWORD: string
    }

    type REGISTER_RESPONSE = {
        STATUS: Status;
        ID: option<string>;
        MESSAGE: option<string>;
    }

    type UserEntity = {
        ID: string;
        USERNAME: string;
        PASSWORD: string;
        RANK: option<int>
    }

    type LoginRequestDTO = {
        USERNAME: string;
        PASSWORD: string;
    }
    
    type LoginResponse = {
        Status: Status;
        ErrorMessage: option<string>;
        SessionID: option<string>
    }

    type FollowRequestDTO = {
        sessionId: string;
        mutable followerUsername: option<string>;
        followeeUsername: string;
    }

    type TweetRequestDTO = {
        sessionId: string;
        tweet: option<string>;
        tweetId: option<string>;
        replyingTo: option<string>;
        mutable userId: option<string>;
        mutable username: option<string>;
        mutable conversationId: option<string>;
    }

    type TweetNotificationDTO = {
        id: string;
        tweet: string;
        tweetType: TWEET_TYPE;
        isRetweet: bool;
        postedBy: string;
        isReply: bool;
    }

    type LogDTO = {
        ID: string;
        logLevel: LOG_LEVEL;
        logType: LOG_TYPE;
        logTime: int64;
        username: string;
        log: string;
    }

    type QueryDTO = {
        sessionId: string;
        mutable userId: string;
        searchFor: string;
        hashtags: option<List<string>>
    }

    type TweetDTO = {
        ID: string;
        tweet: string;
        postTime: int64;
        postedBy: string;
    }

    type QueryResponseDTO = {
        tweets: List<TweetDTO>;
        queryType: string;
    }

    type StatsResponseDTO = {
        numOfRequestsPerSecond: int;
        avgTweetResponseTime: int;
        avgMentionedQueryResponseTime: int;
        avghashtagQueryResponseTime: int;
    }

    type Message = 
        | INIT
        | REGISTER of (REGISTER_DTO)
        | LOGIN of (LoginRequestDTO)
        | LOGIN_RESPONSE of (LoginResponse)
        | FOLLOW
        | FOLLOW_REQUEST of (FollowRequestDTO)
        | FOLLOW_RESPONSE
        | TWEET
        | TWEET_REQUEST of (TweetRequestDTO)
        | TWEET_RESPONSE
        | TWEET_NOTIFICATION of (TweetNotificationDTO)
        | LOG of (string*string)
        | QUERY
        | QUERY_REQUEST of (QueryDTO)
        | QUERY_RESPONSE of (QueryResponseDTO)
        | GENERATE_STATS
        | GATHER_STATS
        | STATS_RESPONSE of (StatsResponseDTO)