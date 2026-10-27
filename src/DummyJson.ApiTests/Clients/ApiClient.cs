using DummyJson.ApiTests.Support;
using RestSharp;

namespace DummyJson.ApiTests.Clients;

public sealed class ApiClient : IDisposable
{
    private readonly RestClient _client;
    private readonly ApiContext _context;

    public ApiClient(TestSettings settings, ApiContext context)
    {
        _context = context;
        _client = new RestClient(new RestClientOptions(settings.BaseUrl)
        {
            ThrowOnAnyError = false
        });
    }

    public async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request, CancellationToken cancellationToken = default)
    {
        ApplyBearerToken(request);

        var response = await _client.ExecuteAsync<T>(request, cancellationToken);
        Capture(request, response);
        return response;
    }

    public async Task<RestResponse> ExecuteAsync(RestRequest request, CancellationToken cancellationToken = default)
    {
        ApplyBearerToken(request);

        var response = await _client.ExecuteAsync(request, cancellationToken);
        Capture(request, response);
        return response;
    }

    public void Dispose() => _client.Dispose();

    private void ApplyBearerToken(RestRequest request)
    {
        if (!string.IsNullOrWhiteSpace(_context.AccessToken))
        {
            request.AddOrUpdateHeader("Authorization", $"Bearer {_context.AccessToken}");
        }
    }

    private void Capture(RestRequest request, RestResponse response)
    {
        _context.LastResponse = response;
        var outcome = response.StatusCode == 0
            ? "no HTTP response"
            : $"{(int)response.StatusCode} {response.StatusCode}";
        _context.LastRequestSummary = $"{request.Method} {request.Resource} -> {outcome}";
    }
}
