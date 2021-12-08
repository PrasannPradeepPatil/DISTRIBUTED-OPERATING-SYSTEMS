# Distributed operating system Projects

## Bitcoins
The goal of this first project is to use F# and the actor model to build a Simple Bitcoin model usin SHA-256, and create multiple actors to mine the coins using AKKA.net.

## Gossip- Algorithm
The goal of this project is to determinethe convergence of such algorithms through a simulator based on actors writtenin F#.  
Since actors in F# are fully asynchronous, the particular type of Gossipimplemented is the so called Asynchronous Gossip.


## Chord - Protocol
In this project, we will be simulating chord protocol using FSharp Akka.net framework. We will be simulating the protocol using the algorithm mentioned in this paper - https://pdos.csail.mit.edu/papers/ton:chord/paper-ton.pd


## Twitter clone
Implement a Twitter-like engine with following funtionalities
- Register account
- Send tweet. Tweets can have hashtags (e.g. #COP5615isgreat) and mentions (@bestuser)
- Subscribe to user's tweets
- Re-tweets (so that your subscribers get an interesting tweet you got by other means)
- Allow querying tweets subscribed to, tweets with specific hashtags, tweets in which the user is mentioned (my mentions)
- If the user is connected, deliver the above types of tweets live (without querying)