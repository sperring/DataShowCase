using System;
using StructureMap;

namespace Services.TweetIndexer
{
    public class TweetIndexServiceManager : IServiceManager
    {
        public TweetIndexServiceManager()
        {
            this.container = ObjectFactory.Container;
        }
        private IContainer container;

        public void Start()
        {
            Console.WriteLine("Starting.....{0}", this.GetType().Name);
            var startableRequestHandlers = this.container.GetAllInstances<IStartableRequestHandler>();
            foreach (var requestHandler in startableRequestHandlers)
            {
                Console.WriteLine("Initialising {0}", requestHandler.GetType().Name);
                requestHandler.Start();
            }
        }

        public void Stop()
        {
            Console.WriteLine("Stopping.....{0}", this.GetType().Name);
        }
    }
}
