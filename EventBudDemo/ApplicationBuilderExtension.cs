using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBudDemo
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseEventBus(this IApplicationBuilder builder)
        {
            object? eventBus = builder.ApplicationServices.GetService(typeof(IEventBus));
            if(eventBus == null)
            {
                throw new ApplicationException("找不到eventbus实例");
            }
            return builder;
        }
    }
}
