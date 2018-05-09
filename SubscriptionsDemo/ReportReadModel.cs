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
                                   // Register handlers for the events we care about
                                   // We don't care about CartCreated, as that doesn't change this report
    public class ReportReadModel : IHandle<ItemAdded>,
                                   IHandle<ItemRemoved>,
                                   IHandle<OrderCompleted>
    {
        private Dictionary<string, Cart> Carts = new Dictionary<string, Cart>();

        // These handlers update our internal state as we receive events
        public void Handle(ItemAdded evnt)
        {
            if (!Carts.ContainsKey(evnt.CartId)) Carts.Add(evnt.CartId, new Cart{CartId = evnt.CartId});
            var cart = Carts[evnt.CartId];

            cart.BoughtItems.Add(evnt.ItemId);
            if(cart.RemovedItems.Contains(evnt.ItemId))
            {
                // The item was added, removed and then added again
                // This makes it a tentative item
                cart.TentativeItems.Add(evnt.ItemId);
                cart.RemovedItems.Remove(evnt.ItemId);
            }
        }

        public void Handle(ItemRemoved evnt)
        {
            if (!Carts.ContainsKey(evnt.CartId)) Carts.Add(evnt.CartId, new Cart{CartId = evnt.CartId});

            Carts[evnt.CartId].BoughtItems.Remove(evnt.ItemId);
            Carts[evnt.CartId].RemovedItems.Add(evnt.ItemId);
        }

        public void Handle(OrderCompleted evnt)
        {
            if (!Carts.ContainsKey(evnt.CartId)) Carts.Add(evnt.CartId, new Cart{CartId = evnt.CartId});
            Carts[evnt.CartId].Completed = true;
        }

        // Generate a report from the current state
        public void GenerateReport()
        {
            Console.WriteLine();
            foreach(var cart in Carts.Values.Where(x => x.Completed)) // Don't include incomplete carts
            {
                Console.WriteLine(new string('-', 40));
                Console.WriteLine($"Cart {cart.CartId}");
                Console.WriteLine(new string('-', 40));
                Console.WriteLine($"Items Bought: ");
                foreach(var itemId in cart.BoughtItems)
                {
                    PrintItem(itemId);
                }
                if(cart.RemovedItems.Count > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Items Removed from Cart: ");
                    foreach(var itemId in cart.RemovedItems)
                    {
                        PrintItem(itemId);
                    }
                }
                if(cart.TentativeItems.Count > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Tentative Items: ");

                    foreach(var itemId in cart.TentativeItems)
                    {
                        PrintItem(itemId);
                    }
                }
                Console.WriteLine();
            }
        }

        private void PrintItem(string itemId)
        {
            var item = Helper.Inventory.First(x => x.ItemId == itemId);
            Console.WriteLine($"{itemId} - {item.Description}");
        }

        public class Cart
        {
            public string CartId;
            public List<string> RemovedItems = new List<string>();
            public List<string> BoughtItems = new List<string>();
            public List<string> TentativeItems = new List<string>();
            public bool Completed;
        }
    }
}