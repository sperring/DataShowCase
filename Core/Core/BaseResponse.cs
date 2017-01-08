using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    [DataContract, Serializable]
    public abstract class BaseResponse
    {

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
