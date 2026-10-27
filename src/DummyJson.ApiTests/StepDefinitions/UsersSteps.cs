using System.Globalization;
using DummyJson.ApiTests.Clients;
using DummyJson.ApiTests.Models;
using DummyJson.ApiTests.Support;
using Reqnroll;
using RestSharp;

namespace DummyJson.ApiTests.StepDefinitions;

[Binding]
public sealed class UsersSteps(UsersClient usersClient)
{
    private const string UsersFilter = "users filter";
    private const string AddUser = "add user";

    private readonly UsersClient _usersClient = usersClient;

    private RestResponse<UsersListResponse>? _filterResponse;
    private RestResponse<User>? _addUserResponse;
    private CreateUserRequest? _submittedUser;

    [When("I filter users by hair colour {string}")]
    public async Task FilterUsersByHairColour(string hairColour)
    {
        _filterResponse = await _usersClient.FilterByHairColorAsync(hairColour);
    }

    [When("I add a new user with the following details:")]
    public async Task AddNewUserWithDetails(Table table)
    {
        var values = table.Rows.ToDictionary(
            row => row["Field"],
            row => row["Value"],
            StringComparer.OrdinalIgnoreCase);

        _submittedUser = new CreateUserRequest
        {
            FirstName = values["FirstName"],
            LastName = values["LastName"],
            Age = int.Parse(values["Age"], CultureInfo.InvariantCulture),
            Gender = values["Gender"],
            Hair = new Hair
            {
                Color = values["HairColour"],
                Type = values["HairType"]
            },
            Address = new Address
            {
                StreetAddress = values["Address"],
                City = values["City"],
                State = values["State"],
                StateCode = values["StateCode"],
                PostalCode = values["PostalCode"],
                Coordinates = new Coordinates
                {
                    Lat = double.Parse(values["Latitude"], CultureInfo.InvariantCulture),
                    Lng = double.Parse(values["Longitude"], CultureInfo.InvariantCulture)
                },
                Country = values["Country"]
            }
        };

        _addUserResponse = await _usersClient.AddUserAsync(_submittedUser);
    }

    [Then("the users filter response status code should be {int}")]
    public void UsersFilterStatusShouldBe(int statusCode)
    {
        ResponseAssertions.StatusCodeShouldBe(_filterResponse, statusCode, UsersFilter);
    }

    [Then("the users filter total should be {int}")]
    public void UsersFilterTotalShouldBe(int total)
    {
        Assert.That(ResponseAssertions.BodyOf(_filterResponse, UsersFilter).Total, Is.EqualTo(total));
    }

    [Then("the users filter result count should equal the reported total")]
    public void UsersFilterCountShouldEqualTotal()
    {
        var body = ResponseAssertions.BodyOf(_filterResponse, UsersFilter);

        Assert.That(body.Users, Has.Count.EqualTo(body.Total),
            "The number of returned users does not match the reported total.");
    }

    [Then("every returned user should have hair colour {string}")]
    public void EveryUserShouldHaveHairColour(string hairColour)
    {
        var users = ResponseAssertions.BodyOf(_filterResponse, UsersFilter).Users;

        Assert.That(users, Is.Not.Empty, "The users filter returned no users.");
        Assert.That(users.Select(user => user.Hair?.Color), Is.All.EqualTo(hairColour));
    }

    [Then("the add user response status code should be {int}")]
    public void AddUserStatusShouldBe(int statusCode)
    {
        ResponseAssertions.StatusCodeShouldBe(_addUserResponse, statusCode, AddUser);
    }

    [Then("the add user response should contain a generated id")]
    public void AddUserShouldContainId()
    {
        Assert.That(ResponseAssertions.BodyOf(_addUserResponse, AddUser).Id, Is.GreaterThan(0));
    }

    [Then("the add user response should echo the submitted user details")]
    public void AddUserShouldEchoSubmittedDetails()
    {
        Assert.That(_submittedUser, Is.Not.Null, "No user was submitted before checking the add user response.");

        var response = ResponseAssertions.BodyOf(_addUserResponse, AddUser);
        var request = _submittedUser!;

        Assert.That(response.Hair, Is.Not.Null, "The add user response did not include hair details.");
        Assert.That(response.Address, Is.Not.Null, "The add user response did not include an address.");
        Assert.That(response.Address!.Coordinates, Is.Not.Null, "The add user response address did not include coordinates.");

        Assert.Multiple(() =>
        {
            Assert.That(response.FirstName, Is.EqualTo(request.FirstName));
            Assert.That(response.LastName, Is.EqualTo(request.LastName));
            Assert.That(response.Age, Is.EqualTo(request.Age));
            Assert.That(response.Gender, Is.EqualTo(request.Gender));
            Assert.That(response.Hair!.Color, Is.EqualTo(request.Hair.Color));
            Assert.That(response.Hair.Type, Is.EqualTo(request.Hair.Type));
            Assert.That(response.Address.StreetAddress, Is.EqualTo(request.Address.StreetAddress));
            Assert.That(response.Address.City, Is.EqualTo(request.Address.City));
            Assert.That(response.Address.State, Is.EqualTo(request.Address.State));
            Assert.That(response.Address.StateCode, Is.EqualTo(request.Address.StateCode));
            Assert.That(response.Address.PostalCode, Is.EqualTo(request.Address.PostalCode));
            Assert.That(response.Address.Country, Is.EqualTo(request.Address.Country));
            Assert.That(response.Address.Coordinates!.Lat, Is.EqualTo(request.Address.Coordinates!.Lat).Within(0.000001));
            Assert.That(response.Address.Coordinates.Lng, Is.EqualTo(request.Address.Coordinates.Lng).Within(0.000001));
        });
    }
}
