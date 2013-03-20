using System;

namespace NuBot
{
    public interface IMessageBus
    {
        void Send<T>(T message);
        IObservable<T> Observe<T>();
    }

    public static class MessageBusExtensions
    {
        public static void On<T>(this IMessageBus bus, Action<T> action)
        {
            bus.Observe<T>().Subscribe(action);
        }
    }
}
