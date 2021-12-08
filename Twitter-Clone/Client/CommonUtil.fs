module CommonUtil


open Quotes
open CommonTypes.CommonTypes
open System.Collections.Generic
open System


let userDetails: Dictionary<string, UserEntity> = new Dictionary<string, UserEntity>()

let addUserDetails (user: UserEntity) = 
    userDetails.Add(user.USERNAME, user)


let shuffleArray (array: UserEntity[]) =
    let random = Random()
    for i in 0 .. array.Length - 1 do
        let j = random.Next(i, array.Length)
        let pom = array.[i]
        array.[i] <- array.[j]
        array.[j] <- pom
    array


let getRandomUsername() = 
    let users = userDetails.Values |> Seq.toList
    "@" + users.[Random().Next(0, users.Length)].USERNAME


let randomHashtag n =
    let r = Random()
    let chars = Array.concat([[|'a' .. 'z'|];[|'A' .. 'Z'|];[|'0' .. '9'|]])
    let sz = Array.length chars in
    "#" + String(Array.init n (fun _ -> chars.[r.Next sz]))


let getShuffledUsers() =
    userDetails.Values |> Seq.toArray |> shuffleArray


let generateTweet() =
    let mutable quote = getRandomQuote()
    let hashtag = getRandomHashtag()
    let taggedUser = getRandomUsername()
    quote <- quote + " " + hashtag
    quote <- quote + " " + taggedUser
    quote


let generateReply() =
    getRandomReply()


let generateRetweet() = 
    "Hey guys, check out this amazing quote."


let getLogDTO (username: string) (level: LOG_LEVEL) (logType: LOG_TYPE) (log: string)  = 
    {ID=(Guid.NewGuid().ToString()); username=username; logLevel=level; logType=logType; log=log; logTime=DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}


let getLogType (value: string) =
    match value with
    | "LOGIN" -> LOG_TYPE.LOGIN
    | "REGISTER" -> LOG_TYPE.REGISTER
    | "TWEET" -> LOG_TYPE.TWEET
    | "RETWEET" -> LOG_TYPE.RETWEET
    | "TWEETNOTIFICATION" -> LOG_TYPE.TWEETNOTIFICATION
    | "REPLY" -> LOG_TYPE.REPLY
    | "QUERY" -> LOG_TYPE.QUERY
    | _ -> LOG_TYPE.TWEET


let getLogLevel (value: string) = 
   match value with 
   | "INFO" -> LOG_LEVEL.INFO
   | "ERROR" -> LOG_LEVEL.ERROR
   | _ -> LOG_LEVEL.INFO


let getRandomHashTags() =
    let hashtags = List<String>();
    for i in 0..5 do
        hashtags.Add(getRandomHashtag())
    hashtags

