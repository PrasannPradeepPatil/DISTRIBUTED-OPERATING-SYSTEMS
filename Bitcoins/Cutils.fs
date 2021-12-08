module Cutils

open System
open System.Security.Cryptography
open System.Text.RegularExpressions
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open Microsoft.FSharp.Core

open Akka



let proc = System.Diagnostics.Process.GetCurrentProcess()
let mutable cpu_time_stamp = proc.TotalProcessorTime
let timer = System.Diagnostics.Stopwatch()

let gatorId = "pnarkar"
let mutable actorSystem = Unchecked.defaultof<Actor.ActorSystem>

type MessageType =
    | Mine
    | GenerateRandomString

type LogType = 
    | RandomStringLog
    | MineLog

type Message = {Type: MessageType; Value: String; mutable Nonce: int}


//GENERATE RANDOM STRING
let ranStr n = 
    let r = Random()
    let chars = Array.concat([[|'a' .. 'z'|];[|'A' .. 'Z'|];[|'0' .. '9'|]])
    let sz = Array.length chars in
    "ppatil1" + String(Array.init n (fun _ -> chars.[r.Next sz]))
    
 //GENERATE HASH KEY FROM RANDOM STRING
let SHA256 (text:string) =
    System.Text.Encoding.ASCII.GetBytes(text)
    |>(new SHA256Managed()).ComputeHash 
    |> Array.map (fun (x : byte) -> System.String.Format("{0:X2}", x))
    |> String.concat System.String.Empty

//COUNT NO OF LEADING 0'S FOR HASH KEY
let hashLeadingZerosCount (hashkey :string) = 
    let mutable count =0
    let mutable flag = 0
    for elem in hashkey do
        if elem = '0' && flag <> 1 then
            count <- count+1
        elif elem <> '0'  then
             flag <- 1
    count    
 //GENERATE HASH KEY (STOP ON FINDING THE 1ST COIN)
let generateHash (n:int) = 
    let mutable leadingZeroCount = 1;
    let mutable randomString = ""
    let mutable  hashKey = ""
    while(leadingZeroCount <> n) do 
        randomString <- ranStr(30)
        hashKey <- randomString |> SHA256
        leadingZeroCount <- hashKey|>hashLeadingZerosCount
    printfn $"%s{randomString}"
    printfn $"%s{hashKey}"

//GENERATE HASH KEY (FIND COINS INFINITELY)
let generateHashh (n:int, randomString:string) = 
    let mutable leadingZeroCount = 1;
    while(true) do 
        //let randomString = ranStr(30)
        let hashKey = randomString|>SHA256
        leadingZeroCount <- hashKey|>hashLeadingZerosCount
        if(leadingZeroCount = n) then 
            printfn $"%s{randomString}"
            printfn $"%s{hashKey}"
            


let printStats() =
    let cpu_time = (proc.TotalProcessorTime-cpu_time_stamp).TotalMilliseconds
    printfn "\n\n======================================================================\n\
    Current Stats\n\
    CPU time = %dms\n\
    Absolute time = %dms\n\
    ======================================================================\n" (int64 cpu_time) timer.ElapsedMilliseconds

let printCoin input coin hash nodeAddress= 
    printfn "\n\n======================================================================\n\
    Hurray !!! New Coin found\n\
    Leading Zeros = %d\n\
    Coin = %s\n\
    Hash = %s\n\
    Actor = %s\n\
    ======================================================================\n" input coin hash nodeAddress

let initStatParams() = 
    cpu_time_stamp <- proc.TotalProcessorTime
    timer.Start()

let rec readInput() =
    let command = System.Console.ReadLine()
    
    match command with
    |   "printStats" ->
            printStats()
            readInput()
    | _ -> readInput()