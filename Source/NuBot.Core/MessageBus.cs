// Based on http://blogs.clariusconsulting.net/kzu/event-centric-finding-key-business-value-by-leveraging-domain-events-and-reactive-extensions/

using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace NuBot
{
    [Export(typeof(IMessageBus))]
    public class MessageBus : IMessageBus
    {
        private readonly IScheduler _scheduler;
        private readonly ConcurrentDictionary<Type, object> _subjects = new ConcurrentDictionary<Type, object>();

        public MessageBus() : this(Scheduler.Default) {}
        public MessageBus(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public void Send<T>(T message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            object subject;
            if (_subjects.TryGetValue(typeof(T), out subject))
            {
                ((Subject<T>)subject).OnNext(message);
            }
        }

        public IObservable<T> Observe<T>()
        {
            return ((IObservable<T>)_subjects.GetOrAdd(typeof(T), type => new Subject<T>())).ObserveOn(_scheduler);
        }
    }
}
