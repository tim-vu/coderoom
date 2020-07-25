using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.RabbitMq.SubscriptionManager
{
    public class DefaultSubscriptionManager : ISubscriptionManager
    {
        private readonly object _lock = new object();
        private readonly List<Type> _types = new List<Type>();
        private readonly Dictionary<string, List<Type>> _subscriptions = new Dictionary<string, List<Type>>();
        
        public void AddSubscription<T, TH>()
        {
            var eventName = typeof(T).Name;
            
            lock (_lock)
            {
                _subscriptions.TryGetValue(eventName, out var handlers);

                handlers ??= new List<Type>();
                
                if (!handlers.Contains(typeof(TH)))
                {
                    handlers.Add(typeof(TH));
                }

                _subscriptions[eventName] = handlers;
                _types.Add(typeof(T));
            }
        }

        public List<Type> GetHandlersForEventName(string eventName)
        {
            lock (_lock)
            {
                return _subscriptions[eventName];
            }
        }

        public Type GetEventTypeByName(string eventName)
        {
            lock (_lock)
            {
                return _types.SingleOrDefault(t => t.Name == eventName);
            }
        }

        public bool HasSubscriptionForEvent(string eventName)
        {
            lock (_lock)
            {
                return _subscriptions.ContainsKey(eventName);
            }
        }
    }
}