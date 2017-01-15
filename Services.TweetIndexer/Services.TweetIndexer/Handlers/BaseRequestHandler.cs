using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Messaging;

namespace Services.TweetIndexer.Handlers
{
    public abstract class BaseRequestHandler<T> : IRequestHandler<T> where T : class, IRequest
    {
        protected IMessagingClient messagingClient;

        public BaseRequestHandler()
        {
            this.messagingClient = new EasyNetQMessagingClient();
            this.messagingClient.Subscribe<T>(HandleRequest);
        }

        public virtual void HandleRequest(T request)
        {

        }

        public void Start()
        {

        }

        public void Stop()
        {

        }
    }
}
