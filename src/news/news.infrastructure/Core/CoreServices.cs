using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using news.application.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace news.infrastructure.Core
{
    public class CoreServices : ICoreService
    {
        private readonly HttpClient _httpClient;
        private readonly CoreSettings _coreSettings;
        private readonly ILogger<CoreServices> _logger;
        public CoreServices(HttpClient httpClient, IOptions<CoreSettings> options, ILogger<CoreServices> logger)
        {
            _httpClient = httpClient;
            _coreSettings = options.Value;
            _logger = logger;
        }

        public async Task<List<string>> GetPhoneNumbers(int userId, int complexId, CancellationToken cancellationToken = default)
        {
            string url = $"{_coreSettings.BaseURL}?userId={userId}&complexId={complexId}";
            HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
            var responseBody = JsonSerializer.Deserialize<List<string>>(await response.Content.ReadAsStringAsync()) ?? throw new Exception("response could not be deserialized");
            return responseBody;
        }
    }
}
