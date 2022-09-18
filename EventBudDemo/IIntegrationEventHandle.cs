using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBudDemo
{
    public interface IIntegrationEventHandle
    {
        Task Handle(string eventName, string eventData);
    }
}
