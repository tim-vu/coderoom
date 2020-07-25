using System;
using System.Collections.Generic;

namespace Infrastructure.RabbitMq.SubscriptionManager
{
    public interface ISubscriptionManager
    {
        void AddSubscription<T, TH>();

        List<Type> GetHandlersForEventName(string eventName);

        Type GetEventTypeByName(string eventName);

        bool HasSubscriptionForEvent(string eventName);
    }
}