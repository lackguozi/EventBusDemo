using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBudDemo
{
    public interface IEventBus
    {
        void Publish(string eventName,object? eventData);
        void Subscribe(string eventName,Type handleType);
        void Unsubscribe(string eventName, Type handleType);
    }
}
