Note that this demo is incredibly simple, and is by no means an example of good code or practices.
For a better idea of what you should be aiming for, check out https://github.com/gregoryyoung/m-r

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

1. Download the sample applications
2. Run the EventSourceDemo application with `dotnet run`.
3. Browse to the admin UI at `localhost:2113`. The default credentials are `admin:changeit`
4. To view the streams, click "Streams" in the top menu bar. You should see a shopping cart stream with events in it.
5. Run the SubscriptionsDemo with `dotnet run`. This should print out the contents of the shopping cart.
