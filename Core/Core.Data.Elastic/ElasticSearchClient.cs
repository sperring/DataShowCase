using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Nest;
using Elasticsearch.Net;

namespace Core.Data.Elastic
{
    public interface ISearchResult { }
    public class ElasticSearchClient
    {
        private ElasticClient elasticClient;
        private ConnectionSettings connectionSettings;
        public ElasticSearchClient()
        {
            if (elasticClient != null)
            {
                return;
            }
            var elasticHost = new Uri(ConfigurationManager.AppSettings["ElasticSearchHostURI"]);
            var indexName = ConfigurationManager.AppSettings["DefaultElasticIndexName"];
            connectionSettings = new ConnectionSettings(elasticHost)
                .DefaultIndex(indexName);
            elasticClient = new ElasticClient();
        }
        public IIndexResponse Index<T>(T request) where T:class,IIndexRequest
        {
            return elasticClient.Index(request);
        }
        public ISearchResponse<ISearchResult> Search<Query>(Query q) where Query : class, ISearchRequest
        {
            return elasticClient.Search<ISearchResult>(q);
        }
    }
}
