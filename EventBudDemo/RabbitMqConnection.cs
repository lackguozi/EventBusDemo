using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBudDemo
{
    public  class RabbitMqConnection
    {
        private readonly IConnectionFactory connectionFactory;
        private IConnection connection;
        private bool disposed;
        private readonly object sync_lock = new object();
        public RabbitMqConnection(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }
        public bool IsConnected 
        {
            get 
            {
                return connection != null && connection.IsOpen && !disposed;
            }

        }
        public bool TryConnect()
        {
            lock (sync_lock)
            {
                connection = connectionFactory.CreateConnection();
                if (IsConnected)
                {
                    connection.ConnectionShutdown += OnConnectionShutdown;
                    connection.CallbackException += OnCallbackException;
                    connection.ConnectionBlocked += OnConnectionBlocked;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            connection.Dispose();
        }
        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }
            return connection.CreateModel();

        }
        private void OnConnectionBlocked(object sender,ConnectionBlockedEventArgs e)
        {
            if (disposed)
            {
                return;
            }
            TryConnect(); ;
        }
        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (disposed) return;

            //_logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
        }
        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (disposed) return;

            //_logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }
    }
}
