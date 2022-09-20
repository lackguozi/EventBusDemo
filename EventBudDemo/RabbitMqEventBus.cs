using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventBudDemo
{
    public class RabbitMqEventBus : IEventBus
    {
        private IModel consumerChannel;
        private readonly RabbitMqConnection persistentConnection;
        private readonly SubcriptionManager subcriptionManager;
        private string queueName;
        private readonly string exchangeName;
        private readonly IServiceScope serviceScope;
        private readonly IServiceProvider serviceProvider;

        public RabbitMqEventBus(RabbitMqConnection persistentConnection,IServiceScopeFactory serviceScopeFactory ,string exchangeName, string queueName)
        {
            this.persistentConnection = persistentConnection;
            this.subcriptionManager = new SubcriptionManager();
            this.exchangeName = exchangeName;
           
            this.queueName = queueName;
            //this.serviceScopeFactory = serviceScopeFactory;
            this.serviceScope = serviceScopeFactory.CreateScope();
            this.serviceProvider = serviceScope.ServiceProvider;

            this.consumerChannel = CreateConsumerChannel();
        }

        public void Publish(string eventName, object? eventData)
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }
            using(var channel = persistentConnection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);
                byte[] body;
                if(eventData == null)
                {
                    body = new byte[0];
                }
                else
                {
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                    };
                    body = JsonSerializer.SerializeToUtf8Bytes(eventData, eventData.GetType(), options);

                }
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;
                channel.BasicPublish(
                    exchange: exchangeName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties:properties,
                    body:body);

            }
        }

        public void Subscribe(string eventName, Type handlerType)
        {
            //检查处理事件是否继承于集成事件 
            // 集成事件订阅  包括检查rabbit连接状态，然后绑定消费通道
            // 订阅事件管理
            //开启消费
            CheckHandlerType(handlerType);
            DoInternalSubcription(eventName);
            subcriptionManager.AddSubcription(eventName, handlerType);
            StartBasicConsume();
            
        }
        public void CheckHandlerType(Type handlerType)
        {
            if(!typeof(IIntegrationEventHandle).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException($"{handlerType} doesn't inherit from IIntegrationEventHandle ",nameof(handlerType)) ;
            }
        }
        public void DoInternalSubcription(string eventName)
        {
            var containsKey = subcriptionManager.HasScriptionForEvent(eventName);
            if (!containsKey)
            {
                if (!persistentConnection.IsConnected)
                {
                    persistentConnection.TryConnect();
                }
                consumerChannel.QueueBind(queue: queueName, exchangeName, routingKey: eventName);
            }
        }
        public void Unsubscribe(string eventName, Type handleType)
        {
            throw new NotImplementedException();
        }
        private IModel CreateConsumerChannel()
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var channel = persistentConnection.CreateModel();
            channel.ExchangeDeclare(exchange: exchangeName,
                                    type: ExchangeType.Direct);

            channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                /*
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();*/
                Debug.Fail(ea.ToString());
            };

            return channel;
        }
        public void StartBasicConsume()
        {
            if (consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(consumerChannel);
                consumer.Received += Consumer_Received;
                consumerChannel.BasicConsume(queueName, false, consumer);
            }
        }
        public async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);
            try
            {
                await ProcessEvent(eventName, message);
                consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch(Exception ex)
            {
                consumerChannel.BasicReject(eventArgs.DeliveryTag, true);
                Debug.Fail(ex.ToString());
            }
        }
        private async Task ProcessEvent(string eventName, string message)
        {
            if (subcriptionManager.HasScriptionForEvent(eventName))
            {
                var subscriptions = subcriptionManager.GetHandlersEvent(eventName);
                foreach(var subscription in subscriptions)
                {
                    //各自在不同的Scope中，避免DbContext等的共享造成如下问题：
                    //The instance of entity type cannot be tracked because another instance
                    using var scope = serviceProvider.CreateScope();
                    IIntegrationEventHandle? handler = scope.ServiceProvider.GetService(subscription) as IIntegrationEventHandle;
                    if(handler != null)
                    {
                        await handler.Handle(eventName, message);
                    }
                    else
                    {
                        throw new ApplicationException($"无法创建{subscription}类型的服务");
                    }
                }
            }
            else
            {
                string entryAsm = Assembly.GetEntryAssembly().GetName().Name;
                Debug.WriteLine($"找不到可以处理eventName={eventName}的处理程序，entryAsm:{entryAsm}");
            }
        }

    }
}
