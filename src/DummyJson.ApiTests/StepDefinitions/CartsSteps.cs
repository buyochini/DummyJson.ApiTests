using System.Globalization;
using DummyJson.ApiTests.Clients;
using DummyJson.ApiTests.Models;
using DummyJson.ApiTests.Support;
using Reqnroll;
using RestSharp;

namespace DummyJson.ApiTests.StepDefinitions;

[Binding]
public sealed class CartsSteps(CartsClient cartsClient)
{
    private const string AddCartDescription = "add cart";
    private const string DeleteCartDescription = "delete cart";

    private readonly CartsClient _cartsClient = cartsClient;

    private RestResponse<CartResponse>? _addCartResponse;
    private RestResponse<CartResponse>? _deleteCartResponse;
    private List<CartProductInput> _requestedProducts = [];

    [When("I add a cart for user {int} with the following products:")]
    public async Task AddCartForUserWithProducts(int userId, Table table)
    {
        _requestedProducts = table.Rows
            .Select(row => new CartProductInput
            {
                Id = int.Parse(row["ProductId"], CultureInfo.InvariantCulture),
                Quantity = int.Parse(row["Quantity"], CultureInfo.InvariantCulture)
            })
            .ToList();

        _addCartResponse = await _cartsClient.AddCartAsync(new AddCartRequest
        {
            UserId = userId,
            Products = _requestedProducts
        });
    }

    [When("I delete cart {int}")]
    public async Task DeleteCart(int cartId)
    {
        _deleteCartResponse = await _cartsClient.DeleteCartAsync(cartId);
    }

    [Then("the add cart response status code should be {int}")]
    public void AddCartStatusShouldBe(int statusCode)
    {
        ResponseAssertions.StatusCodeShouldBe(_addCartResponse, statusCode, AddCartDescription);
    }

    [Then("the cart should contain {int} products")]
    public void CartShouldContainProducts(int count)
    {
        var cart = ResponseAssertions.BodyOf(_addCartResponse, AddCartDescription);

        Assert.Multiple(() =>
        {
            Assert.That(cart.TotalProducts, Is.EqualTo(count), "Unexpected totalProducts value.");
            Assert.That(cart.Products, Has.Count.EqualTo(count), "Unexpected number of cart line items.");
        });
    }

    [Then("the cart should belong to user {int}")]
    public void CartShouldBelongToUser(int userId)
    {
        Assert.That(ResponseAssertions.BodyOf(_addCartResponse, AddCartDescription).UserId, Is.EqualTo(userId));
    }

    [Then("the cart should contain exactly the requested products and quantities")]
    public void CartShouldContainRequestedProductsAndQuantities()
    {
        var cart = ResponseAssertions.BodyOf(_addCartResponse, AddCartDescription);

        var expected = _requestedProducts.Select(product => (product.Id, product.Quantity));
        var actual = cart.Products.Select(product => (product.Id, product.Quantity));

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.EquivalentTo(expected),
                "The cart line items (product id, quantity) do not match the request.");
            Assert.That(cart.TotalQuantity, Is.EqualTo(_requestedProducts.Sum(product => product.Quantity)),
                "Unexpected totalQuantity value.");
        });
    }

    [Then("the sum of line-item discount percentages should be {decimal}")]
    public void SumOfLineItemDiscountPercentagesShouldBe(decimal expectedSum)
    {
        var products = ResponseAssertions.BodyOf(_addCartResponse, AddCartDescription).Products;

        Assert.That(products, Is.Not.Empty, "The cart response contained no line items.");

        var actualSum = Math.Round(
            products.Sum(product => product.DiscountPercentage),
            2,
            MidpointRounding.AwayFromZero);

        Assert.That(actualSum, Is.EqualTo(expectedSum));
    }

    [Then("the delete cart response status code should be {int}")]
    public void DeleteCartStatusShouldBe(int statusCode)
    {
        ResponseAssertions.StatusCodeShouldBe(_deleteCartResponse, statusCode, DeleteCartDescription);
    }

    [Then("the deleted cart id should be {int}")]
    public void DeletedCartIdShouldBe(int id)
    {
        Assert.That(ResponseAssertions.BodyOf(_deleteCartResponse, DeleteCartDescription).Id, Is.EqualTo(id));
    }

    [Then("the deleted cart isDeleted flag should be true")]
    public void DeletedCartIsDeletedShouldBeTrue()
    {
        Assert.That(ResponseAssertions.BodyOf(_deleteCartResponse, DeleteCartDescription).IsDeleted, Is.True);
    }

    [Then("the deleted cart should include a deletedOn timestamp")]
    public void DeletedCartShouldIncludeDeletedOn()
    {
        var deletedOn = ResponseAssertions.BodyOf(_deleteCartResponse, DeleteCartDescription).DeletedOn;

        Assert.That(deletedOn, Is.Not.Null, "The delete cart response did not include deletedOn.");
        Assert.That(deletedOn!.Value, Is.Not.EqualTo(default(DateTimeOffset)));
    }
}
