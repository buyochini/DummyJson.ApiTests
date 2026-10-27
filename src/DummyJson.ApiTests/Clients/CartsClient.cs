using DummyJson.ApiTests.Models;
using DummyJson.ApiTests.Support;
using RestSharp;

namespace DummyJson.ApiTests.Clients;

public sealed class CartsClient(ApiClient apiClient)
{
    private readonly ApiClient _apiClient = apiClient;

    public Task<RestResponse<CartResponse>> AddCartAsync(AddCartRequest cart)
    {
        var request = new RestRequest(ApiEndpoints.Carts.Add, Method.Post)
            .AddJsonBody(cart);

        return _apiClient.ExecuteAsync<CartResponse>(request);
    }

    public Task<RestResponse<CartResponse>> DeleteCartAsync(int cartId)
    {
        var request = new RestRequest(ApiEndpoints.Carts.ById(cartId), Method.Delete);
        return _apiClient.ExecuteAsync<CartResponse>(request);
    }
}
