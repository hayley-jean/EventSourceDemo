## Set up Event Store

1. Download the latest version of (Event Store)[https://eventstore.org/downloads/].
2. Unpack the zip file to the location you want to run Event Store from.
3. Start Event Store with :

Windows admin command prompt :

```
.\EventStore.ClusterNode.exe --mem-db
```

Linux :

 ```
 .\run-node.sh --mem-db
 ```
 
 **The --mem-db switch means the database is running in memory. Any data will be lost when the node is stopped**
 
 
## Run the demo

1. Download the sample application
2. Build and run it with `dotnet run`
3. The console should log out the different events that are being written by the demo. For example :

```
Received EventSourceDemo.ShoppingCartCreated event : {"CartId":"1d76f0ff-3060-4606-a46d-7f804c2aefec"}
Received EventSourceDemo.ItemAdded event : {"CartId":"1d76f0ff-3060-4606-a46d-7f804c2aefec","ItemId":"a3ab3459-391d-4976-8de3-9f858c30896a","Description":"External Hard Drive","Price":2000.0}
Received EventSourceDemo.ItemAdded event : {"CartId":"1d76f0ff-3060-4606-a46d-7f804c2aefec","ItemId":"c439b352-abc1-4140-b9ee-8998f75b138d","Description":"Oculus Go","Price":4000.0}
Received EventSourceDemo.ItemRemoved event : {"CartId":"1d76f0ff-3060-4606-a46d-7f804c2aefec","ItemId":"a3ab3459-391d-4976-8de3-9f858c30896a"}
Received EventSourceDemo.ItemRemoved event : {"CartId":"1d76f0ff-3060-4606-a46d-7f804c2aefec","ItemId":"c439b352-abc1-4140-b9ee-8998f75b138d"}
Received EventSourceDemo.ItemAdded event : {"CartId":"1d76f0ff-3060-4606-a46d-7f804c2aefec","ItemId":"a1a45d0d-c96b-4db6-a9df-6a756165445d","Description":"Xbox One","Price":6000.0}
```

4. Browse to the admin UI at `localhost:2113`. The default credentials are `admin:changeit`
5. Browse to the streams in the top menu bar. Click on the relevant stream.
6. You should see all the events written by the demo