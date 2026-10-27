using DummyJson.ApiTests.Models;
using DummyJson.ApiTests.Support;
using RestSharp;

namespace DummyJson.ApiTests.Clients;

public sealed class UsersClient(ApiClient apiClient)
{
    private readonly ApiClient _apiClient = apiClient;

    public Task<RestResponse<UsersListResponse>> FilterByHairColorAsync(string hairColor)
    {
        var request = new RestRequest(ApiEndpoints.Users.Filter, Method.Get)
            .AddQueryParameter("key", "hair.color")
            .AddQueryParameter("value", hairColor)
            .AddQueryParameter("limit", "0");

        return _apiClient.ExecuteAsync<UsersListResponse>(request);
    }

    public Task<RestResponse<User>> AddUserAsync(CreateUserRequest user)
    {
        var request = new RestRequest(ApiEndpoints.Users.Add, Method.Post)
            .AddJsonBody(user);

        return _apiClient.ExecuteAsync<User>(request);
    }
}
