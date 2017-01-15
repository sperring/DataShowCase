using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
namespace Services.Twitter.Model
{
    public class TweetRequest : BaseRequest
    {
        public string TweetText { get; set; }
    }
}
