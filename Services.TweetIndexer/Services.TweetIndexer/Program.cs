using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Services.TweetIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = ObjectFactory.Container;

            HostFactory.Run(hostConfigurator =>
            {
                hostConfigurator.Service<IServiceManager>(serviceConfigurator =>
                {
                    serviceConfigurator.ConstructUsing(() => container.GetInstance<IServiceManager>());
                    serviceConfigurator.WhenStarted(service => service.Start());
                    serviceConfigurator.WhenStopped(service => service.Stop());
                });

                hostConfigurator.RunAsLocalSystem();

                hostConfigurator.SetDisplayName("Services.TweetIndexer.Windows");
                hostConfigurator.SetDescription("Windows Service for indexing tweets to Elastic from RabbitMQ");
                hostConfigurator.SetServiceName("Services.TweetIndexer.Windows");
            });
        }
    }
}
