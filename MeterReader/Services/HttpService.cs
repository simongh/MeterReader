using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace MeterReader.Services
{
    internal class HttpService : IHttpService
    {
        private readonly HttpClient _client;
        private readonly ILogger<HttpService> _log;
        private readonly Types.Options _options;

        public HttpService(
            HttpClient client,
            IOptions<Types.Options> options,
            ILogger<HttpService> log)
        {
            _client = client;
            _log = log;
            _options = options.Value;
        }

        public async Task<string> GetVirtualEntitiesAsync()
        {
            var jdoc = JsonDocument.Parse(await _client.GetStreamAsync("virtualentity"));

            _log.LogDebug("Response: {response}", jdoc.RootElement);

            return jdoc.RootElement
                .EnumerateArray()
                .First()
                .GetProperty("veId")
                .GetString()!;
        }

        public async Task<IEnumerable<string>> GetResourcesAsync(string virtualEntityId)
        {
            if (string.IsNullOrEmpty(virtualEntityId))
                throw new ArgumentNullException("You must set the veID in the config first");

            var jdoc = JsonDocument.Parse(await _client.GetStreamAsync($"virtualentity/{virtualEntityId}/resources"));

            _log.LogDebug("Response: {response}", jdoc.RootElement);

            var resourceTypes = new[]
            {
                "gas.consumption",
                "electricity.consumption",
            };

            return jdoc.RootElement
                .GetProperty("resources")
                .EnumerateArray()
                .Where(j => resourceTypes.Contains(j.GetProperty("classifier").GetString()))
                .Select(j => j.GetProperty("resourceId").GetString())
                .ToArray()!;
        }

        public async Task<IEnumerable<Entities.Reading>> GetReadingsAsync(Types.ResourceFilter filter)
        {
            var jdoc = JsonDocument.Parse(await _client.GetStreamAsync($"resource/{filter.Id}/readings?{filter}"));

            _log.LogInformation("response: {response}", jdoc.RootElement);

            var classifier = jdoc.RootElement
                .GetProperty("classifier")
                .GetString() switch
            {
                "gas.consumption" => TariffType.GasRate,
                "electricity.consumption" => TariffType.ElectricityRate,
                _ => throw new ApplicationException($"Unexpected classifier")
            };

            return jdoc.RootElement
                .GetProperty("data")
                .EnumerateArray()
                .Skip(1)
                .Where(j => j.EnumerateArray().All(e => e.ValueKind != JsonValueKind.Null))
                .Select(j => new Entities.Reading
                {
                    Created = DateTimeOffset.FromUnixTimeSeconds(j.EnumerateArray().First().GetInt64()),
                    Value = j.EnumerateArray().Last().GetDecimal(),
                    Type = classifier,
                })
                .ToArray();
        }
    }
}