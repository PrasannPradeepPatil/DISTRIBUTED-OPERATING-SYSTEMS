module CommonUtil

open System
open System.Security.Cryptography
open System.Collections.Generic
open CommonTypes.CommonTypes

type Entity =
    | USERS
    | FOLLOWERS
    | HASHTAGS
    | TWEETS
    | CONVERSATIONS
    | CONVERSATION_SUBSCRIPTIONS
    | TWEETS_HASHTAGS
    | TWEETS_MENTIONS


type UserDetails = {
    ID: string;
    USERNAME: string
}

type GenericResponse = {
    Status: Status;
    ErrorMessage: option<string>
}

type Message = 
    | REGISTER of (REGISTER_DTO)

let loggedInUsers: Dictionary<string, UserDetails> = new Dictionary<string, UserDetails>()
let userRemoteAddresses: Dictionary<string, string> = new Dictionary<string, string>()

let hashString (input: string) = 
    use sha256hash = SHA256Managed.Create();
    let inputBytes = System.Text.Encoding.ASCII.GetBytes(input)
    sha256hash.ComputeHash(inputBytes) |> Array.map (sprintf "%02X") |> String.concat ""


let addLoggedInUser (userDetails: UserDetails) (sessionId: string) = 
    loggedInUsers.Add(sessionId, userDetails)


let extractDataFromTweet (tweet: string) (delimiter: string) = 
    let data = new List<string>()
    if tweet.Contains(delimiter) then
        let words = tweet.Split(" ")
        for word in words do
            if word.StartsWith(delimiter) && word.Length > 1 then
                data.Add(word.Substring(1, word.Length-1))
    data


let getHashTagsFromTweet (tweet: string) = 
    extractDataFromTweet tweet "#"


let getUsernamesFromTweet (tweet: string) = 
    extractDataFromTweet tweet "@"


let getListsDiff (list1: List<string>) (list2: List<string>) = 
    let diff = new List<string>()
    for ele in list1 do
        if not(list2.Contains ele) then
            diff.Add(ele)
    diff


let IDictionaryToDictionary (data: IDictionary<string, Object>) =
    let output = new Dictionary<string, Object>()
    for KeyValue(key, value) in data do
        output.Add(key, value)
    output


let getLoggedInUsername (sessionId: string) = 
    let mutable loggedInUsername = ""
    if loggedInUsers.ContainsKey sessionId then
        loggedInUsername <- loggedInUsers.[sessionId].USERNAME
    loggedInUsername


let getLoggedInUserId (sessionId: string) = 
    let mutable loggedInUserId = ""
    if loggedInUsers.ContainsKey sessionId then
        loggedInUserId <- loggedInUsers.[sessionId].ID
    loggedInUserId


let addUserAddress (username: string) (address: string) = 
    printfn "%s %s" username address
    userRemoteAddresses.Add(username, address)