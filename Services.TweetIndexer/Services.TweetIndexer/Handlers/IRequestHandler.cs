using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
namespace Services.TweetIndexer.Handlers
{
    public interface IRequestHandler<T> : IStartableRequestHandler, IStoppableRequestHandler where T : class, IRequest
    {
        void HandleRequest(T request);
    }
}
