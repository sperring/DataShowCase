using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IMessagingClient : IDisposable
    {
        void Publish<T>(T request) where T : class, IRequest;

        void Subscribe<T>(Action<T> handler) where T : class, IRequest;
    }
}
