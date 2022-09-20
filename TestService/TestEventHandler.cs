using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBudDemo;
using TestService.Event;

namespace TestService
{
    [EventName("TestDemo.Api")]
    public class TestEventHandler : JsonIntegrationEventHandler<TestEvent>
    {
        public override Task HandleJson(string eventName, TestEvent? eventData)
        {
            return Task.Run( ()=>Console.WriteLine(eventData.message));
        }
    }
}
