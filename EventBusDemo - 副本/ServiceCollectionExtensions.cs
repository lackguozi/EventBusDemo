using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventBudDemo
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services,string queueName,params Assembly[] assemblies)
        {
            return AddEventBus(services, queueName, assemblies.ToList());
        }
        public static IServiceCollection AddEventBus(this IServiceCollection services,string queueName,IEnumerable<Assembly> assemblies)
        {
            List<Type> eventHandlers = new List<Type>();
            foreach(var assembly in assemblies)
            {
                var types = assembly.GetTypes().Where(a => a.IsAssignableTo(typeof(IIntegrationEventHandle)) && a.IsAbstract ==false);
                eventHandlers.AddRange(types);
            }
            return AddEventBus(services, queueName, eventHandlers);
        }
        public static IServiceCollection AddEventBus(this IServiceCollection services,string queueName,IEnumerable<Type> handlerTypes)
        {
            foreach(var type in handlerTypes)
            {
                services.AddScoped(type, type);
            }
            services.AddSingleton<IEventBus>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<IntegrationEventRabbitMQOptions>>().Value;
                var factory = new ConnectionFactory()
                {
                    HostName = options.HostName,
                    UserName = "root",
                    Password = "123456",
                    DispatchConsumersAsync = true
                };
                var myConnection = new RabbitMqConnection(factory);

                var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                var eventBus = new RabbitMqEventBus(myConnection, serviceScopeFactory, options.ExchangeName, queueName);
                foreach(var type in handlerTypes)
                {
                    var eventNameAttrs = type.GetCustomAttributes<EventNameAttribute>();
                    if(eventNameAttrs.Any() == false)
                    {
                        throw new ApplicationException($"There shoule be at least one EventNameAttribute on {type}");
                    }
                    foreach(var attrs in eventNameAttrs)
                    {
                        eventBus.Subscribe(attrs.name, type);
                    }
                }
                return eventBus;
            });
            return services;
        }
    }
}
