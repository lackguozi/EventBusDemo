using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestService.Event
{
   public record TestEvent(Guid guid ,string name,string message);
}
