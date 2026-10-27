using System.Text.Json.Serialization;

namespace DummyJson.ApiTests.Models;

public sealed class UsersListResponse
{
    [JsonPropertyName("users")]
    public List<User> Users { get; set; } = [];

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("skip")]
    public int Skip { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}

public sealed class User
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("age")]
    public int Age { get; set; }

    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;

    [JsonPropertyName("hair")]
    public Hair? Hair { get; set; }

    [JsonPropertyName("address")]
    public Address? Address { get; set; }
}

public sealed class Hair
{
    [JsonPropertyName("color")]
    public string Color { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

public sealed class Address
{
    [JsonPropertyName("address")]
    public string StreetAddress { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("stateCode")]
    public string StateCode { get; set; } = string.Empty;

    [JsonPropertyName("postalCode")]
    public string PostalCode { get; set; } = string.Empty;

    [JsonPropertyName("coordinates")]
    public Coordinates? Coordinates { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
}

public sealed class Coordinates
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lng")]
    public double Lng { get; set; }
}

public sealed class CreateUserRequest
{
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("age")]
    public int Age { get; set; }

    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;

    [JsonPropertyName("hair")]
    public Hair Hair { get; set; } = new();

    [JsonPropertyName("address")]
    public Address Address { get; set; } = new();
}
