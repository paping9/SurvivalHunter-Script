using System;
using System.Linq;
using System.Collections.Generic;

namespace Message
{

    public interface IMessageBus
    {
        void PublishEvent<TMsgType>(TMsgType evt);
        void Subscribe(object subscriber);
        void Unsubscribe(object subscriber);
    }

    public interface IMessageSubscriber<in TMsgType>
    {
        void ReceiveMessage(TMsgType Msg);
    }

    public class MessageBus : IMessageBus
    {
        private readonly Dictionary<Type, List<WeakReference>> _subscribers = new();

        public void PublishEvent<TMsgType>(TMsgType msg)
        {
            var subscriberType = typeof(IMessageSubscriber<>).MakeGenericType(typeof(TMsgType));
            var subscribers = GetSubscriberList(subscriberType);
            var subsToRemove = new List<WeakReference>
            {
                Capacity = 0
            };

            var clonedSubscribers = new List<WeakReference>(subscribers);
            foreach (var weakSubscriber in clonedSubscribers)
            {
                if (weakSubscriber.IsAlive)
                {
                    var subscriber = (IMessageSubscriber<TMsgType>)weakSubscriber.Target;
                    subscriber?.ReceiveMessage(msg);
                }
                else
                {
                    subsToRemove.Add(weakSubscriber);
                }
            }

            if (subsToRemove.Count <= 0) return;
            foreach (var remove in subsToRemove)
            {
                subscribers.Remove(remove);
            }
        }

        public void Subscribe(object subscriber)
        {
            var subscriberTypes = subscriber.GetType()
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageSubscriber<>));

            var weakRef = new WeakReference(subscriber);

            foreach (var subscriberType in subscriberTypes)
            {
                var subscribers = GetSubscriberList(subscriberType);
                subscribers.Add(weakRef);
            }
        }

        public void Unsubscribe(object subscriber)
        {
            var subscriberTypes = subscriber.GetType()
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageSubscriber<>));

            foreach (var subscriberType in subscriberTypes)
            {
                var subscribers = GetSubscriberList(subscriberType);
                var idxTarget = subscribers.FindIndex((w) => w.IsAlive && w.Target == subscriber);
                if (idxTarget != -1)
                {
                    subscribers.RemoveAt(idxTarget);
                }
            }
        }

        private List<WeakReference> GetSubscriberList(Type subscriberType)
        {
            var found = _subscribers.TryGetValue(subscriberType, out var subscribersList);

            if (found) return subscribersList;
            subscribersList = new List<WeakReference>();
            _subscribers.Add(subscriberType, subscribersList);

            return subscribersList;
        }
    }
}
