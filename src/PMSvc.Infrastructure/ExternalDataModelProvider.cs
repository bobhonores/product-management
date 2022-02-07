using Newtonsoft.Json;
using PMSvc.Core;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PMSvc.Infrastructure
{
    public class ExternalDataModelProvider : IExternalDataModelProvider
    {
        public class Settings
        {
            public string Uri { get; set; }
            public double Timeout { get; set; }
        }

        public class Response
        {
            [JsonProperty("event_id")]
            public string EventId { get; set; }
            [JsonProperty("deployment_id")]
            public string DeploymentId { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public decimal? Value { get; set; }
        }

        private readonly HttpClient _client;

        public ExternalDataModelProvider(HttpClient client)
        {
            _client = client;
        }

        public async Task<DataModel> Get(Product product, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _client.GetAsync($"/products/{product.Id}", cancellationToken);

                response.EnsureSuccessStatusCode();

                var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);

                var content = JsonConvert.DeserializeObject<Response>(stringContent);

                return new DataModel
                {
                    EventId = content.EventId,
                    DeploymentId = content.DeploymentId,
                    Timestamp = content.Timestamp,
                    Value = content.Value
                };
            }
            catch
            {
                return new DataModel();
            }
        }
    }
}
