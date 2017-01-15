using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Twitter.Model;
using Core;
using Core.Data.SQL;
using Newtonsoft.Json;

namespace Services.TweetIndexer.Handlers.Tweet
{
    public class TweetRequestHandler : BaseRequestHandler<TweetRequest>
    {
        private ILogger logger;
        public TweetRequestHandler()
        {
            this.logger = new DatabaseLogger(this.GetType().ToString());
        }
        public override void HandleRequest(TweetRequest request)
        {
            this.logger.CatchAndLog("Tweet process start", () => {
                base.HandleRequest(request);
                Console.WriteLine("{0} - {1}", this.GetType().Name, JsonConvert.SerializeObject(request));
                //ZBO.Services.Finance.Settlement.Settlement settlement = new Services.Finance.Settlement.Settlement();
                //settlement.Execute(request);
            });
        }
    }
}
