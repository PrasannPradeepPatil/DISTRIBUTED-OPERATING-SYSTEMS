module Config

open Akka.FSharp

let clientConfig hostName port seedHostName = Configuration.parse("""
akka {  
	actor {
		provider = "Akka.Cluster.ClusterActorRefProvider, Akka.Cluster"
		serializers {
			json = "Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion"
		}
		serialization-bindings {
			"System.Object" = json
		}
	}
	remote {
		log-remote-lifecycle-events = off
		helios.tcp {
			hostname = """ + hostName + """
			port = 9999       
		}
	}
	cluster {
		roles = ["client"]  # custom node roles
		seed-nodes = ["akka.tcp://TwitterSystem@""" + seedHostName + """:""" + port + """"]
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

