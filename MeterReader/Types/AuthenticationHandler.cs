using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace MeterReader.Types
{
    internal class AuthenticationHandler : DelegatingHandler
    {
        private readonly Options _options;
        private readonly SemaphoreSlim _semaphore;
        private readonly HttpClient _httpClient;
        private string? _token;
        private DateTimeOffset _expires;

        public AuthenticationHandler(
            IOptions<Options> options,
            HttpClient httpClient
            )
        {
            _options = options.Value;
            _httpClient = httpClient;

            _semaphore = new SemaphoreSlim(1);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await AddHeadersAsync(request, cancellationToken);
            return await base.SendAsync(request, cancellationToken);
        }

        private async Task AddHeadersAsync(HttpRequestMessage httpRequest, CancellationToken cancellationToken)
        {
            if (!httpRequest.Headers.Contains("token"))
                httpRequest.Headers.Add("token", await GetTokenAsync());
        }

        private async Task<string> GetTokenAsync()
        {
            if (ShouldRefresh())
            {
                try
                {
                    await _semaphore.WaitAsync();

                    if (ShouldRefresh())
                    {
                        using var msg = await _httpClient.PostAsJsonAsync("auth", new
                        {
                            username = _options.Username,
                            password = _options.Password,
                        });

                        if (!msg.IsSuccessStatusCode)
                            throw new ApplicationException("Unable to authenticate");

                        var result = await msg.Content.ReadFromJsonAsync<TokenResult>();

                        _token = result.token;
                        _expires = DateTimeOffset.FromUnixTimeSeconds(result.exp);
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return _token!;
        }

        private bool ShouldRefresh() => _token == null || _expires < DateTimeOffset.UtcNow.AddMinutes(1);

        private readonly record struct TokenResult
        {
            public string token { get; init; }

            public long exp { get; init; }
        }
    }
}