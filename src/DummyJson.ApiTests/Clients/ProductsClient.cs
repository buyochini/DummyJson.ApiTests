using DummyJson.ApiTests.Models;
using DummyJson.ApiTests.Support;
using RestSharp;

namespace DummyJson.ApiTests.Clients;

public sealed class ProductsClient(ApiClient apiClient)
{
    private readonly ApiClient _apiClient = apiClient;

    public Task<RestResponse<ProductsListResponse>> SearchAsync(string query)
    {
        var request = new RestRequest(ApiEndpoints.Products.Search, Method.Get)
            .AddQueryParameter("q", query);

        return _apiClient.ExecuteAsync<ProductsListResponse>(request);
    }

    public Task<RestResponse<ProductsListResponse>> GetPaginatedAsync(int limit, int skip, string? select = null)
    {
        var request = new RestRequest(ApiEndpoints.Products.Root, Method.Get)
            .AddQueryParameter("limit", limit.ToString())
            .AddQueryParameter("skip", skip.ToString());

        if (!string.IsNullOrWhiteSpace(select))
        {
            request.AddQueryParameter("select", select);
        }

        return _apiClient.ExecuteAsync<ProductsListResponse>(request);
    }

    public Task<RestResponse<Product>> UpdateTitleAsync(int productId, string title)
    {
        var request = new RestRequest(ApiEndpoints.Products.ById(productId), Method.Put)
            .AddJsonBody(new UpdateProductRequest { Title = title });

        return _apiClient.ExecuteAsync<Product>(request);
    }
}
