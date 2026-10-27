using DummyJson.ApiTests.Models;
using DummyJson.ApiTests.Support;
using RestSharp;

namespace DummyJson.ApiTests.Clients;

public sealed class AuthClient(ApiClient apiClient)
{
    private readonly ApiClient _apiClient = apiClient;

    public Task<RestResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var restRequest = new RestRequest(ApiEndpoints.Auth.Login, Method.Post)
            .AddJsonBody(request);

        return _apiClient.ExecuteAsync<LoginResponse>(restRequest);
    }

    public Task<RestResponse<AuthenticatedUser>> GetCurrentUserAsync()
    {
        var restRequest = new RestRequest(ApiEndpoints.Auth.Me, Method.Get);
        return _apiClient.ExecuteAsync<AuthenticatedUser>(restRequest);
    }
}
