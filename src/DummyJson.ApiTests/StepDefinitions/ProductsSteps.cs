using DummyJson.ApiTests.Clients;
using DummyJson.ApiTests.Models;
using DummyJson.ApiTests.Support;
using Reqnroll;
using RestSharp;

namespace DummyJson.ApiTests.StepDefinitions;

[Binding]
public sealed class ProductsSteps(ProductsClient productsClient)
{
    private const string ProductsSearch = "products search";
    private const string ProductsPagination = "products pagination";
    private const string ProductUpdate = "product update";

    private readonly ProductsClient _productsClient = productsClient;

    private RestResponse<ProductsListResponse>? _searchResponse;
    private RestResponse<ProductsListResponse>? _paginationResponse;
    private RestResponse<Product>? _updateResponse;

    [When("I search products for {string}")]
    public async Task SearchProductsFor(string query)
    {
        _searchResponse = await _productsClient.SearchAsync(query);
    }

    [When("I retrieve products with limit {int} and skip {int} selecting {string}")]
    public async Task RetrieveProductsWithPagination(int limit, int skip, string select)
    {
        _paginationResponse = await _productsClient.GetPaginatedAsync(limit, skip, select);
    }

    [When("I update product {int} title to {string}")]
    public async Task UpdateProductTitle(int productId, string title)
    {
        _updateResponse = await _productsClient.UpdateTitleAsync(productId, title);
    }

    [Then("the products search response status code should be {int}")]
    public void ProductsSearchStatusShouldBe(int statusCode)
    {
        ResponseAssertions.StatusCodeShouldBe(_searchResponse, statusCode, ProductsSearch);
    }

    [Then("the product titled {string} should have price {decimal}")]
    public void ProductTitledShouldHavePrice(string title, decimal price)
    {
        var products = ResponseAssertions.BodyOf(_searchResponse, ProductsSearch).Products;
        var product = products.SingleOrDefault(item => string.Equals(item.Title, title, StringComparison.Ordinal));

        Assert.That(product, Is.Not.Null,
            $"Expected one product titled '{title}'. Returned titles: {string.Join(", ", products.Select(item => $"'{item.Title}'"))}.");
        Assert.That(product!.Price, Is.EqualTo(price));
    }

    [Then("the products pagination response status code should be {int}")]
    public void ProductsPaginationStatusShouldBe(int statusCode)
    {
        ResponseAssertions.StatusCodeShouldBe(_paginationResponse, statusCode, ProductsPagination);
    }

    [Then("the products pagination response should contain exactly {int} product")]
    public void ProductsPaginationShouldContainExactly(int count)
    {
        Assert.That(ResponseAssertions.BodyOf(_paginationResponse, ProductsPagination).Products, Has.Count.EqualTo(count));
    }

    [Then("the products pagination skip should be {int}")]
    public void ProductsPaginationSkipShouldBe(int skip)
    {
        Assert.That(ResponseAssertions.BodyOf(_paginationResponse, ProductsPagination).Skip, Is.EqualTo(skip));
    }

    [Then("the products pagination limit should be {int}")]
    public void ProductsPaginationLimitShouldBe(int limit)
    {
        Assert.That(ResponseAssertions.BodyOf(_paginationResponse, ProductsPagination).Limit, Is.EqualTo(limit));
    }

    [Then("the paginated product id should be {int}")]
    public void PaginatedProductIdShouldBe(int id)
    {
        Assert.That(GetSinglePaginatedProduct().Id, Is.EqualTo(id));
    }

    [Then("the paginated product title should be {string}")]
    public void PaginatedProductTitleShouldBe(string title)
    {
        Assert.That(GetSinglePaginatedProduct().Title, Is.EqualTo(title));
    }

    [Then("the paginated product price should be {decimal}")]
    public void PaginatedProductPriceShouldBe(decimal price)
    {
        Assert.That(GetSinglePaginatedProduct().Price, Is.EqualTo(price));
    }

    [Then("the product update response status code should be {int}")]
    public void ProductUpdateStatusShouldBe(int statusCode)
    {
        ResponseAssertions.StatusCodeShouldBe(_updateResponse, statusCode, ProductUpdate);
    }

    [Then("the updated product id should be {int}")]
    public void UpdatedProductIdShouldBe(int id)
    {
        Assert.That(ResponseAssertions.BodyOf(_updateResponse, ProductUpdate).Id, Is.EqualTo(id));
    }

    [Then("the updated product title should be {string}")]
    public void UpdatedProductTitleShouldBe(string title)
    {
        Assert.That(ResponseAssertions.BodyOf(_updateResponse, ProductUpdate).Title, Is.EqualTo(title));
    }

    private Product GetSinglePaginatedProduct()
    {
        var products = ResponseAssertions.BodyOf(_paginationResponse, ProductsPagination).Products;

        Assert.That(products, Has.Count.EqualTo(1), "Expected the paginated response to contain exactly one product.");
        return products[0];
    }
}
