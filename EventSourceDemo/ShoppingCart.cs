using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;

namespace EventSourceDemo
{
    public class ShoppingCart
    {
        public string Id { get; private set; }
        // Expected Version is the number of events to expect in the stream
        // This is used when writing to ensure that nothing has changed
        public int ExpectedVersion { get { return _expectedVersion; } }

        private List<Item> _items = new List<Item>();
        private bool _completed;

        // ExpectedVersion starts at -1, meaning the stream does not exist
        private int _expectedVersion = -1;
        private List<Event> _unsavedChanges = new List<Event>();

        // In real life, this constructor should be private
        // But for the purposes of this demo, we are simplifying a bit
        public ShoppingCart()
        {
        }

        // These methods would be called by commands
        // They create the events
        // Any validation should be done before events are created
        public static ShoppingCart Create(string id)
        {
            var shoppingCart = new ShoppingCart();
            shoppingCart.ApplyInternal(new ShoppingCartCreated(id));
            return shoppingCart;
        }

        public void AddItem(string itemId, string description, decimal price)
        {
            if(_completed) throw new OrderAlreadyCompleted();
            ApplyInternal(new ItemAdded(Id, itemId, description, price));
        }

        public void RemoveItem(string itemId)
        {
            if(_completed) throw new OrderAlreadyCompleted();
            if (!_items.Any(x => x.ItemId == itemId)) throw new ItemNotInCart();
            ApplyInternal(new ItemRemoved(Id, itemId));
        }

        public void CompleteOrder(string userId)
        {
            if(_completed) throw new OrderAlreadyCompleted();
            ApplyInternal(new OrderCompleted(Id, userId));
        }

        // These methods "apply" the events - they change state in our aggregate
        private void Apply(ShoppingCartCreated @event)
        {
            Id = @event.CartId;
        }

        private void Apply(ItemAdded @event)
        {
            _items.Add(new Item {
                ItemId = @event.ItemId,
                Description = @event.Description,
                Price = @event.Price
            });
        }

        private void Apply(ItemRemoved @event)
        {
            _items.Remove(_items.FirstOrDefault(x => x.ItemId == @event.ItemId));
        }

        private void Apply(OrderCompleted @event)
        {
            _completed = true;
        }

        // These methods are our aggregate's internals
        // Unsaved changes stores events that have not been written to ES yet
        private void ApplyInternal(Event @event)
        {
            _unsavedChanges.Add(@event);
            Apply((dynamic)@event);
        }

        public Event[] GetChanges()
        {
            return _unsavedChanges.ToArray();
        }

        public void MarkChangesAsCommitted()
        {
            _expectedVersion += _unsavedChanges.Count;
            _unsavedChanges.Clear();
        }

        // When loading from history, we deserialize the events and apply them
        // Note that we do not add events loaded from history to the unsaved changes, as they have been saved already
        // This is also why we have separate "ApplyInternal" and "Apply" methods, so we can decide when to update unsaved changes
        public void LoadFromHistory(ResolvedEvent[] events)
        {
            foreach (var @event in events)
            {
                var constructedEvent = Helper.ConstructEvent(@event);
                Apply((dynamic)constructedEvent);
                _expectedVersion++;
            }
        }
    }

    public struct Item
    {
        public string ItemId;
        public string Description;
        public decimal Price;
    }
}