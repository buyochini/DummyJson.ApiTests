using System.Text.Json.Serialization;

namespace DummyJson.ApiTests.Models;

public sealed class AddCartRequest
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("products")]
    public List<CartProductInput> Products { get; set; } = [];
}

public sealed class CartProductInput
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}

public sealed class CartResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("products")]
    public List<CartProduct> Products { get; set; } = [];

    [JsonPropertyName("total")]
    public decimal Total { get; set; }

    [JsonPropertyName("discountedTotal")]
    public decimal DiscountedTotal { get; set; }

    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("totalProducts")]
    public int TotalProducts { get; set; }

    [JsonPropertyName("totalQuantity")]
    public int TotalQuantity { get; set; }

    [JsonPropertyName("isDeleted")]
    public bool? IsDeleted { get; set; }

    [JsonPropertyName("deletedOn")]
    public DateTimeOffset? DeletedOn { get; set; }
}

public sealed class CartProduct
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("total")]
    public decimal Total { get; set; }

    [JsonPropertyName("discountPercentage")]
    public decimal DiscountPercentage { get; set; }

    [JsonPropertyName("discountedPrice")]
    public decimal? DiscountedPrice { get; set; }

    [JsonPropertyName("discountedTotal")]
    public decimal? DiscountedTotal { get; set; }
}
