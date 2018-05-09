using System;

namespace EventSourceDemo
{
    public abstract class Event
    {
        public Guid Id;
        public DateTime CreatedDate;

        public Event()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
        }
    }

    public class ShoppingCartCreated : Event
    {
        public string CartId { get; private set; }

        public ShoppingCartCreated(string cartId)
        {
            CartId = cartId;
        }
    }

    public class ItemAdded : Event
    {
        public string CartId { get; private set; }
        public string ItemId { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }

        public ItemAdded(string cartId, string itemId, string description, decimal price)
        {
            CartId = cartId;
            ItemId = itemId;
            Description = description;
            Price = price;
        }
    }

    public class ItemRemoved : Event
    {
        public string CartId { get; private set; }
        public string ItemId { get; private set; }

        public ItemRemoved(string cartId, string itemId)
        {
            CartId = cartId;
            ItemId = itemId;
        }
    }

    public class OrderCompleted : Event
    {
        public string CartId { get; private set; }
        public string UserId { get; private set; }

        public OrderCompleted(string cartId, string userId)
        {
            CartId = cartId;
            UserId = userId;
        }
    }
}