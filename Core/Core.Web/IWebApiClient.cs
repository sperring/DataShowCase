using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Web
{
    public interface IWebApiClient
    {
        T SendJsonRequest<T>(string endPointURL, System.Net.Http.HttpMethod httpMethod, dynamic requestObject);

        T SendRequest<T>(string endPointURL, System.Net.Http.HttpMethod httpMethod, dynamic requestObject);

        void SendRequestWithoutResponse(string endPointURL, System.Net.Http.HttpMethod httpMethod, dynamic requestObject);
    }
}
