using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.TweetIndexer
{
    public static class ObjectFactory
    {
        private static readonly Lazy<Container> _containerBuilder = new Lazy<Container>(Init, LazyThreadSafetyMode.ExecutionAndPublication);
        public static IContainer Container
        {
            get { return _containerBuilder.Value; }
        }
        private static Container Init()
        {
            return new Container(ct =>
            {
                ct.Scan(x =>
                {
                    x.AssembliesFromApplicationBaseDirectory();
                    x.WithDefaultConventions();
                });

                //RegisterImplementationsOfSpecificType<IRequestHandler>(ct);
                //RegisterImplementationsOfSpecificType<IRecurring>(ct);
                //RegisterImplementationsOfSpecificType<IOnceOff>(ct);

                ct.For<IServiceManager>().Singleton().Use<TweetIndexServiceManager>();
                //
                //ct.For<IRequestHandler<CurrencyCountryMerchantBatchSettlementRequest>>().Singleton().Use<SettlementBatchRequestHandler>();
                //ct.For<IStartableRequestHandler>().Use((context) => context.GetInstance<IRequestHandler<CurrencyCountryMerchantBatchSettlementRequest>>());
            });
        }
        private static void RegisterImplementationsOfSpecificType<T>(ConfigurationExpression ct)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(T).IsAssignableFrom(p));
            var concreteImplementations = types.Where(t => t.BaseType != null && t.IsAbstract == false);

            foreach (var t in concreteImplementations)
            {
                var defaultConstructor = t.GetConstructors().Where(a => a.GetParameters().Count() == 0).FirstOrDefault();
                dynamic instance = defaultConstructor.Invoke(null);
                ct.For<T>().Singleton().Use(instance);
            }
        }
    }

}
