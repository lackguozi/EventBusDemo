using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBudDemo
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventNameAttribute:Attribute

    {
        public EventNameAttribute(string name)
        {
            this.name = name;
        }
        public string  name { get; set; }
    }
}
