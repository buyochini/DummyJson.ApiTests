using System.Text.Json.Serialization;

namespace DummyJson.ApiTests.Models;

public sealed class ProductsListResponse
{
    [JsonPropertyName("products")]
    public List<Product> Products { get; set; } = [];

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("skip")]
    public int Skip { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}

public sealed class Product
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}

public sealed class UpdateProductRequest
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}
