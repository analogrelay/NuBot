using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuBot.Core
{
    public interface IMessageBus
    {
        void Send<T>(T message);
        IObservable<T> Observe<T>();
    }
}
