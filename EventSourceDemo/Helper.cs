using System;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventSourceDemo
{
public static class Helper
    {
        public static Event ConstructEvent(ResolvedEvent @event)
        {
            return (Event)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(@event.Event.Data),
                                        typeof(ShoppingCart).Assembly.GetType(@event.Event.EventType));
        }

        public static Type GetResolvedEventType(ResolvedEvent @event)
        {
            return typeof(ShoppingCart).Assembly.GetType(@event.Event.EventType);
        }

        public static Item[] Inventory = new Item[]{
                new Item{
                    ItemId = "cb03e407-d672-4b78-929b-d094f19680ef",
                    Description = "Skyrim VR",
                    Price = 520.0m
                },
                new Item{
                    ItemId = "9be268f1-984d-4075-a2b6-64768c90a091",
                    Description = "Oculus Go",
                    Price = 4000.0m
                },
                new Item{
                    ItemId = "09eebe75-f243-447b-bc94-3364825f9224",
                    Description = "Plush Zombie",
                    Price = 410.0m
                },
                new Item{
                    ItemId = "5453b48d-e8e0-4aef-832e-fe4e688d2c67",
                    Description = "Lego Star Wars Tie Fighter Set",
                    Price = 149.0m
                },
                new Item{
                    ItemId = "b95e0f8a-0fbc-4edc-a6f1-deeb9bc81ada",
                    Description = "XBox One",
                    Price = 3000.0m
                },
                new Item{
                    ItemId = "767082d7-e75a-4625-a6b7-adbbd011db5b",
                    Description = "Monster Hunter World (Xbox)",
                    Price = 600.0m
                },
                new Item{
                    ItemId = "7657e9da-c225-4a1a-9aee-198b6580a551",
                    Description = "God of War",
                    Price = 600.0m
                },
                new Item{
                    ItemId = "d0444945-e563-441c-96ce-8aeb03bae3ea",
                    Description = "Red Dragon Gaming Headset",
                    Price = 463.0m
                },
                new Item{
                    ItemId = "ac067bef-6824-4838-8f70-edf716734809",
                    Description = "Marvel vs Capcom Infinite",
                    Price = 352.0m
                },
                new Item{
                    ItemId = "e1eb510d-f4e7-4622-ab2e-b7059b3049b9",
                    Description = "Vermintide II",
                    Price = 175.0m
                },
                new Item{
                    ItemId = "057daa0e-acc2-4c40-af65-631c0e806fde",
                    Description = "Playstation 4",
                    Price = 5000.0m
                },
                new Item{
                    ItemId = "74238365-3ac6-4768-9b4a-5bc0a9723ae7",
                    Description = "The climb VR",
                    Price = 250.0m
                },
            };
    }
}