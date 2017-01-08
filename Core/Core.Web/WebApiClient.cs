using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Core.Logging;

namespace Core.Web
{
    public class WebApiClient : IWebApiClient
    {
        private ILogger logger;
        private bool logDebugInfo;

        public WebApiClient()
        {
            this.logger = new Logger(this.GetType().ToString());
            if (ConfigurationManager.AppSettings["WebApiClientLogDebugInfo"] != null)
            {
                this.logDebugInfo = Convert.ToBoolean(ConfigurationManager.AppSettings["WebApiClientLogDebugInfo"]);
            }
        }

        public T SendRequest<T>(string endPointURL, HttpMethod httpMethod, dynamic requestObject)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("*******************************************************");
            sb.AppendFormat("Testing Endpoint: {0}", endPointURL);
            sb.AppendLine("*******************************************************");
            using (var apiClient = new HttpClient())
            {
                var request = new HttpRequestMessage(httpMethod, endPointURL);
                sb.AppendFormat("Request: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(request));
                sb.AppendLine("*******************************************************");
                var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                var serializer = new XmlSerializer(requestObject.GetType());
                var settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;

                using (var stream = new StringWriter())
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    serializer.Serialize(writer, requestObject, emptyNamepsaces);
                    request.Content = new StringContent(stream.ToString(), Encoding.UTF8, "application/xml");
                }

                var response = apiClient.SendAsync(request).Result;
                sb.AppendFormat("Response: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(response));
                sb.AppendLine("*******************************************************");
                if (logDebugInfo)
                {
                    this.logger.LogInfo(sb.ToString());
                }
                var message = response.Content.ReadAsStreamAsync().Result;

                var deserializer = new XmlSerializer(typeof(T));
                var result = (T)deserializer.Deserialize(message);
                return result;
            }
        }

        public async Task<T> SendAsyncRequest<T>(string endPointURL, HttpMethod httpMethod, dynamic requestObject)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("*******************************************************");
            sb.AppendFormat("Testing Endpoint: {0}", endPointURL);
            sb.AppendLine("*******************************************************");
            using (var apiClient = new HttpClient())
            {
                var request = new HttpRequestMessage(httpMethod, endPointURL);
                sb.AppendFormat("Request: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(request));
                sb.AppendLine("*******************************************************");
                var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                var serializer = new XmlSerializer(requestObject.GetType());
                var settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;

                using (var stream = new StringWriter())
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    serializer.Serialize(writer, requestObject, emptyNamepsaces);
                    request.Content = new StringContent(stream.ToString(), Encoding.UTF8, "application/xml");
                }

                var response = await apiClient.SendAsync(request);
                sb.AppendFormat("Response: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(response));
                sb.AppendLine("*******************************************************");
                if (logDebugInfo)
                {

                    this.logger.LogInfo(sb.ToString());
                }
                var message = response.Content.ReadAsStreamAsync().Result;

                var deserializer = new XmlSerializer(typeof(T));
                var result = (T)deserializer.Deserialize(message);
                return result;
            }
        }

        public void SendRequestWithoutResponse(string endPointURL, HttpMethod httpMethod, dynamic requestObject)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("*******************************************************");
            sb.AppendFormat("Testing Endpoint: {0}", endPointURL);
            sb.AppendLine("*******************************************************");
            using (var apiClient = new HttpClient())
            {
                var request = new HttpRequestMessage(httpMethod, endPointURL);
                sb.AppendFormat("Request: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(request));
                sb.AppendLine("*******************************************************");
                var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                var serializer = new XmlSerializer(requestObject.GetType());
                var settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;

                using (var stream = new StringWriter())
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    serializer.Serialize(writer, requestObject, emptyNamepsaces);
                    request.Content = new StringContent(stream.ToString(), Encoding.UTF8, "application/xml");
                }

                var response = apiClient.SendAsync(request).Result;
                sb.AppendFormat("Response: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(response));
                sb.AppendLine("*******************************************************");
                if (logDebugInfo)
                {
                    this.logger.LogInfo(sb.ToString());
                }
                var message = response.Content.ReadAsStreamAsync().Result;
            }
        }

        public T SendJsonRequest<T>(string endPointURL, HttpMethod httpMethod, dynamic requestObject)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("*******************************************************");
            sb.AppendFormat("Testing Endpoint: {0}", endPointURL);
            sb.AppendLine("*******************************************************");
            using (var apiClient = new HttpClient())
            {
                var request = new HttpRequestMessage(httpMethod, endPointURL);
                sb.AppendFormat("Request: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(request));
                sb.AppendLine("*******************************************************");

                if (requestObject != null)
                {
                    request.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestObject), Encoding.UTF8, "application/json");
                }

                var response = apiClient.SendAsync(request).Result;
                sb.AppendFormat("Response: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(response));
                sb.AppendLine("*******************************************************");
                if (logDebugInfo)
                {
                    this.logger.LogInfo(sb.ToString());
                }
                var message = response.Content.ReadAsStringAsync().Result;
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(message);
                return result;
            }
        }
    }
}
