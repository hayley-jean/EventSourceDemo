using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventSourceDemo;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace SubscriptionsDemo
{
    // This registers event handlers and then reads the events from Event Store
    // Registered handlers will be called when an event that they are interested in appears
    public class EventBus
    {
        private IEventStoreConnection _connection;
        private UserCredentials _credentials;

        private Dictionary<Type, List<Action<Event>>> _typeHash = new Dictionary<Type, List<Action<Event>>>();
        public EventBus(IEventStoreConnection connection, UserCredentials credentials)
        {
            _connection = connection;
            _credentials = credentials;           
        }

        public void RegisterHandler<T>(IHandle<T> handler) where T : Event
        {
            List<Action<Event>> handlers;
            if(!_typeHash.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Action<Event>>();
                _typeHash.Add(typeof(T), handlers);
            }
            handlers.Add(m => handler.Handle((T)m));
        }

        public async Task ReplayAllEvents()
        {
            var result = await _connection.ReadAllEventsForwardAsync(Position.Start, 4096, false, _credentials);
            foreach(var evnt in result.Events)
            {
                BroadcastEvent(evnt);
            }
        }

        private void BroadcastEvent(ResolvedEvent evnt)
        {
            var type = Helper.GetResolvedEventType(evnt);
            if(type == null) return;

            List<Action<Event>> handlers;
            if(_typeHash.TryGetValue(type, out handlers))
            {
                var @event = Helper.ConstructEvent(evnt);
                foreach(var handler in handlers)
                {
                    handler(@event);
                }
            }
        }
    }

    public interface IHandle<T> where T: Event
    {
        void Handle(T @event);
    }
}