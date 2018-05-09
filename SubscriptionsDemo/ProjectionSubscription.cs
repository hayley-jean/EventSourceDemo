using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventSourceDemo;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace SubscriptionsDemo
{
    public class ProjectionSubscription
    {
        private IEventStoreConnection _connection;
        private UserCredentials _credentials;

        public ProjectionSubscription(IEventStoreConnection connection, UserCredentials credentials)
        {
            _connection = connection;
            _credentials = credentials;
        }

        public async Task SubscribeToOrdersFromAll()
        {
            await _connection.SubscribeToAllAsync(false, (sub, evnt) => 
                {
                    // Subscribe to OrderCompleted events and write to console
                    Console.WriteLine($"Received Event {evnt.Event.EventNumber}@{evnt.Event.EventStreamId} - {evnt.Event.EventType}");
                    if (evnt.Event.EventType == typeof(OrderCompleted).FullName)
                    {
                        var @event = (OrderCompleted)Helper.ConstructEvent(evnt);
                        Console.WriteLine($"Received Order for {@event.UserId} {@event.CartId}");
                        Console.WriteLine();
                    }
                },
                (sub, reason, ex) => {
                    Console.WriteLine($"Subscription dropped because {reason}. Exception: {ex}");
                }, _credentials);
            Console.WriteLine("Subscribed to all events");
        }

        public async Task SubscribeToOrderTypeStream()
        {
            string streamId = "$et-EventSourceDemo.OrderCompleted";
            // Subscribe to the OrderCompleted type stream. This requires the event type projection to be running
            await _connection.SubscribeToStreamAsync(streamId, true, (sub, evnt) => 
                {
                    var @event = (OrderCompleted)Helper.ConstructEvent(evnt);
                    Console.WriteLine($"Received Order for {@event.UserId} {@event.CartId}");
                },
                (sub, reason, ex) => {
                Console.WriteLine($"Subscription dropped because {reason}. Exception: {ex}");
            }, _credentials);

            Console.WriteLine($"Subscribed to order type stream {streamId}");
        }
    }
}