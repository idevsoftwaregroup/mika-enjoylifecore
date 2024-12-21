using core.application.Contract.infrastructure.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace core.infrastructure.IdentityService
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _httpClient;
          private readonly ILogger<IdentityService> _logger;
        public IdentityService(HttpClient httpClient, ILogger<IdentityService> logger)
        {
            this._httpClient = httpClient;
            this._logger = logger;
        }
        public async Task<string> GetToken(int CoreId, CancellationToken cancellationToken = default)
        {
            try
            {
                var body = new
                {
                    coreId = CoreId
                };
                HttpContent httpContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("https://localhost:8002/api/Authentication/LoginLobbyMan", httpContent, cancellationToken);
                var responseData = await response.Content.ReadAsStringAsync(cancellationToken);
                if (responseData == null)
                {
                    return string.Empty;
                }
                return responseData;
            }
            catch (Exception)
            {
                return string.Empty;
            }
            
            
        }
    }
}
