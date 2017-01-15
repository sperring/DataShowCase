using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.TweetIndexer
{
    public interface IServiceManager
    {
        void Start();

        void Stop();
    }
}
