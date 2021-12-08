module Config

open Akka
open Akka.FSharp

let serverConfig hostName port = Configuration.parse("""
akka {  
	actor {
		provider = "Akka.Cluster.ClusterActorRefProvider, Akka.Cluster"
		serializers {
			json = "Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion"
		}
		serialization-bindings {
			"System.Object" = json
		}
		deployment {
			/serverProvider = {
				router = round-robin-pool
				metrics-selector = cpu
				nr-of-instances = 10
				cluster {
					enabled = on
					use-role = server
					allow-local-routees = on
					max-nr-of-instances-per-node = 10
				}
			}
		}
	}
	remote {
		log-remote-lifecycle-events = off
		helios.tcp {
			hostname = """ + hostName + """
			port = """ + port + """       
		}
	}
	cluster {
		min-nr-of-members = 0
		roles = [server]
		role {
			server.min-nr-of-members = 1
			client.min-nr-of-members = 0
		}
		seed-nodes = ["akka.tcp://TwitterSystem@""" + hostName + """:""" + port + """"]
		# when node cannot be reached within 10 sec, mark is as down
		auto-down-unreachable-after = 20 s
		failure-detector {
			heartbeat-interval = 1 s
			threshold = 8.0
			max-sample-size = 1000
			min-std-deviation = 100 ms
			acceptable-heartbeat-pause = 20 s
			expected-response-after = 1 s
		}
	}
	log-dead-letters = 0
	log-dead-letters-during-shutdown = off
}
""")