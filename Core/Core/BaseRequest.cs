using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
namespace Core
{
    public abstract class BaseRequest : IRequest
    {
        public BaseRequest()
        {
            this.RequestId = CombGuidGenerator.Generate();
        }

        public Guid RequestId { get; set; }
    }
}
