using EasyNetQ;
using EasyNetQ.Loggers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Configuration;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace Core.Messaging
{
    public class EasyNetQMessagingClient : IMessagingClient
    {
        private IBus messageBus;
        private string subscriptionId = null;


        public EasyNetQMessagingClient()
        {
            if (messageBus != null)
            {
                return;
            }

            //this.subscriptionId = ConfigurationManager.AppSettings["subscriptionId"];
            this.subscriptionId = System.Environment.MachineName;
            var hostName = ConfigurationManager.AppSettings["RabbitMQHostName"];
            var userName = ConfigurationManager.AppSettings["RabbitMQUserName"];
            var password = ConfigurationManager.AppSettings["RabbitMQPassword"];
            var requestedHeartbeat = ConfigurationManager.AppSettings["RabbitMQRequestedHeartbeat"];
            var prefetchCount = ConfigurationManager.AppSettings["RabbitMQPreFetchCount"];
            var connectionString = String.Format("host={0};requestedHeartbeat={3};prefetchcount={4}", hostName, requestedHeartbeat, prefetchCount);
            if (!String.IsNullOrEmpty(userName))
            {
                connectionString = String.Format("host={0};username={1};password={2};requestedHeartbeat={3};prefetchcount={4}", hostName, userName, password, requestedHeartbeat, prefetchCount);
            }
            messageBus = RabbitHutch.CreateBus(connectionString, x =>
            {
                RegisterServices(x);
            });
        }

        public void RegisterServices(global::EasyNetQ.IServiceRegister serviceRegister)
        {
            serviceRegister.Register<ISerializer>(y => { return new JsonSerializer(); });
            serviceRegister.Register<IEasyNetQLogger>(y => { return new ConsoleLogger(); });
        }

        public void Publish<T>(T request) where T : class, IRequest
        {
            this.messageBus.Publish(request);
        }

        public void Subscribe<T>(Action<T> handler) where T : class, IRequest
        {
            //Place holder for scaling the number of workers for each subscription
            int workerCount = Convert.ToInt32(ConfigurationManager.AppSettings["RabbitMQWorkers"]);
            for (int i = 0; i < workerCount; i++)
            {
                this.messageBus.Subscribe(subscriptionId, handler);
            }

        }

        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (messageBus != null)
                {
                    messageBus.Dispose();
                }
            }

            _disposed = true;
        }
    }

    public class JsonSerializer : ISerializer
    {
        public byte[] MessageToBytes<T>(T message) where T : class
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, JsonSerialisation.Settings));
        }

        public T BytesToMessage<T>(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes), JsonSerialisation.Settings);
        }

        public object BytesToMessage(string typeName, byte[] bytes)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), JsonSerialisation.Settings);
        }
    }

    public static class JsonSerialisation
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DefaultContractResolver
            {
                //DefaultMembersSearchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            },
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };
    }
}
