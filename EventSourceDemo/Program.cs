using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;

namespace EventSourceDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateSimpleCart().Wait();
            // CreateCartWithItemRemoved().Wait();
            // CreateCartWithTentativeItem().Wait();
        }

        private static async Task CreateSimpleCart()
        {
            // Connect to Event Store
            var settings = ConnectionSettings.Create();
            var connection = EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Loopback, 1113));
            await connection.ConnectAsync();

            var items = Helper.Inventory;

            var shoppingCartId = Guid.NewGuid().ToString();
            // This is the name of the stream all the events will be written to
            var streamId = $"ShoppingCart-{shoppingCartId}";
            var shoppingCart = ShoppingCart.Create(shoppingCartId);
            // We are going to save after each event so we can see them populate
            await SaveAggregate(connection, shoppingCart, streamId);
            Console.WriteLine($"Created shoping cart {shoppingCart.Id}");

            // Add some items to our shopping cart
            shoppingCart.AddItem(items[0].ItemId, items[0].Description, items[0].Price);
            await SaveAggregate(connection, shoppingCart, streamId);
            Console.WriteLine($"Added item {items[0].Description}");

            shoppingCart.AddItem(items[1].ItemId, items[1].Description, items[1].Price);
            await SaveAggregate(connection, shoppingCart, streamId);
            Console.WriteLine($"Added item {items[1].Description}");

            shoppingCart.AddItem(items[11].ItemId, items[11].Description, items[11].Price);
            await SaveAggregate(connection, shoppingCart, streamId);
            Console.WriteLine($"Added item {items[11].Description}");

            var userId = Guid.NewGuid().ToString();
            shoppingCart.CompleteOrder(userId);
            await SaveAggregate(connection, shoppingCart, streamId);
            Console.WriteLine($"Completed order for shopping cart {shoppingCart.Id}");
        }

        private static async Task CreateCartWithItemRemoved()
        {
            // Connect to Event Store
            var settings = ConnectionSettings.Create();
            var connection = EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Loopback, 1113));
            await connection.ConnectAsync();

            var items = Helper.Inventory;

            var shoppingCartId = Guid.NewGuid().ToString();
            // This is the name of the stream all the events will be written to
            var streamId = $"ShoppingCart-{shoppingCartId}";
            var shoppingCart = ShoppingCart.Create(shoppingCartId);
            Console.WriteLine($"Created shoping cart {shoppingCart.Id}");

            // Add some items to our shopping cart
            shoppingCart.AddItem(items[10].ItemId, items[10].Description, items[10].Price);
            Console.WriteLine($"Added item {items[10].Description}");

            shoppingCart.AddItem(items[8].ItemId, items[8].Description, items[8].Price);
            Console.WriteLine($"Added item {items[8].Description}");

            shoppingCart.AddItem(items[6].ItemId, items[6].Description, items[6].Price);
            Console.WriteLine($"Added item {items[6].Description}");

            // Remove an item
            shoppingCart.RemoveItem(items[8].ItemId);
            Console.WriteLine($"Removed item {items[8].Description}");

            var userId = Guid.NewGuid().ToString();
            shoppingCart.CompleteOrder(userId);
            Console.WriteLine($"Completed order for shopping cart {shoppingCart.Id}");

            // We're now just going to save after all the events, as we have seen how this works
            await SaveAggregate(connection, shoppingCart, streamId);
        }


        private static async Task CreateCartWithTentativeItem()
        {
            // Connect to Event Store
            var settings = ConnectionSettings.Create();
            var connection = EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Loopback, 1113));
            await connection.ConnectAsync();

            var items = Helper.Inventory;

            var shoppingCartId = Guid.NewGuid().ToString();
            // This is the name of the stream all the events will be written to
            var streamId = $"ShoppingCart-{shoppingCartId}";
            var shoppingCart = ShoppingCart.Create(shoppingCartId);
            Console.WriteLine($"Created shoping cart {shoppingCart.Id}");

            // Add some items to our shopping cart
            shoppingCart.AddItem(items[2].ItemId, items[2].Description, items[2].Price);
            Console.WriteLine($"Added item {items[2].Description}");

            shoppingCart.AddItem(items[3].ItemId, items[3].Description, items[3].Price);
            Console.WriteLine($"Added item {items[3].Description}");

            // Remove an item
            shoppingCart.RemoveItem(items[2].ItemId);
            Console.WriteLine($"Removed item {items[2].Description}");

            // Change our mind and add it back again
            shoppingCart.AddItem(items[2].ItemId, items[2].Description, items[2].Price);
            Console.WriteLine($"Added item {items[2].Description} again");

            var userId = Guid.NewGuid().ToString();
            shoppingCart.CompleteOrder(userId);
            Console.WriteLine($"Completed order for shopping cart {shoppingCart.Id}");

            // We're now just going to save after all the events, as we have seen how this works
            await SaveAggregate(connection, shoppingCart, streamId);
        }

        private static async Task SaveAggregate(IEventStoreConnection connection, ShoppingCart cart, string streamId)
        {
            // Write each event to the Event Store.
            // We could do this in batches, but saving each one demonstrates the expected version changes
            var expectedVersion = cart.ExpectedVersion;
            foreach (var @event in cart.GetChanges())
            {
                await WriteEvents(connection, streamId, expectedVersion, @event);
                // Increase expected version for the next write
                expectedVersion++;
            }
            // Finalise the changes so we don't end up writing them again
            cart.MarkChangesAsCommitted();
        }

        private static async Task WriteEvents(IEventStoreConnection connection, string streamId, int expectedVersion, Event @event)
        {
            await connection.AppendToStreamAsync(streamId, expectedVersion, new EventData[]
            {
                new EventData(@event.Id,
                    @event.GetType().ToString(),
                    true,
                    // Just serialise the data as json
                    System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                    new byte[]{})
            });
        }

        private static async Task<ShoppingCart> LoadAggregate(IEventStoreConnection connection, string streamId)
        {
            var shoppingCart = new ShoppingCart();

            // Read the events from Event Store
            // We are ignoring paging for this demo
            var result = await connection.ReadStreamEventsForwardAsync(streamId, 0, 4096, false);

            // Pass the events we've loaded into the shopping cart
            // And replay them
            shoppingCart.LoadFromHistory(result.Events);

            return shoppingCart;
        }
    }    
}
