using Amazon.Runtime;
using Elasticsearch.Net;
using Elasticsearch.Net.Aws;
using Nest;
using SmartApartmentData.Infrastructure.Settings.Models;
using System;

namespace SmartApartmentData.Infrastructure.Settings
{    
    public class AWSSearchContext
    {
        private readonly BasicAWSCredentials _awsCredentials;
        private static ElasticClient _client;
        private readonly ConnectionSettings _connectionSettings;

        public AWSSearchContext(AWSSettings settings)
        {
            _awsCredentials = new BasicAWSCredentials(settings.AccessKey,
                        settings.SecretKey);

            var httpConnection = new AwsHttpConnection(_awsCredentials,
                Amazon.RegionEndpoint.GetBySystemName(settings.Region));

            var pool = new SingleNodeConnectionPool(new Uri(settings.ElasticUrl));

            _connectionSettings = new ConnectionSettings(pool, httpConnection);
            
            _client = new ElasticClient(_connectionSettings);
        }

        public ElasticClient Client
        {
            get
            {
                return _client;
            }
        }
    }
}
