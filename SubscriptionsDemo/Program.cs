using System;
using System.Net;
using System.Threading.Tasks;
using EventSourceDemo;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace SubscriptionsDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            RunDemo().Wait();
        }

        public static async Task RunDemo()
        {
            // Connect to Event Store
            var settings = ConnectionSettings.Create();
            var userCredentials = new UserCredentials("admin", "changeit");
            var connection = EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Loopback, 1113));
            await connection.ConnectAsync();

            var report= new ReportReadModel();

            // Register our report read model's handlers with the event bus
            // Whenever the bus comes across one of these events, it will be passed to the ReportReadModel
            var bus = new EventBus(connection, userCredentials);
            bus.RegisterHandler<ItemAdded>(report);
            bus.RegisterHandler<ItemRemoved>(report);
            bus.RegisterHandler<OrderCompleted>(report);

            Console.WriteLine("Replaying all events");
            // Replay all the events in the Event Store.
            // This will publish them all to our report read model
            await bus.ReplayAllEvents();

            // Generate Tentative Items Report
            report.GenerateReport();
        }
    }
}
