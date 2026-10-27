namespace DummyJson.ApiTests.Support;

/// <summary>
/// Centralised DummyJSON request paths used by the resource clients.
/// </summary>
public static class ApiEndpoints
{
    public static class Auth
    {
        public const string Login = "/auth/login";
        public const string Me = "/auth/me";
    }

    public static class Users
    {
        public const string Filter = "/users/filter";
        public const string Add = "/users/add";
    }

    public static class Products
    {
        public const string Root = "/products";
        public const string Search = "/products/search";

        public static string ById(int productId) => $"/products/{productId}";
    }

    public static class Carts
    {
        public const string Add = "/carts/add";

        public static string ById(int cartId) => $"/carts/{cartId}";
    }
}
