using DummyJson.ApiTests.Clients;
using DummyJson.ApiTests.Models;
using DummyJson.ApiTests.Support;
using Reqnroll;
using RestSharp;

namespace DummyJson.ApiTests.StepDefinitions;

[Binding]
public sealed class AuthSteps(AuthClient authClient, ApiContext apiContext, TestSettings settings)
{
    private const string Login = "login";
    private const string CurrentUser = "current user";

    private readonly AuthClient _authClient = authClient;
    private readonly ApiContext _apiContext = apiContext;
    private readonly TestSettings _settings = settings;

    private RestResponse<LoginResponse>? _loginResponse;
    private RestResponse<AuthenticatedUser>? _currentUserResponse;

    [When("I authenticate with the configured credentials")]
    public async Task AuthenticateWithConfiguredCredentials()
    {
        _loginResponse = await _authClient.LoginAsync(new LoginRequest
        {
            Username = _settings.Auth.Username,
            Password = _settings.Auth.Password
        });

        if (_loginResponse.IsSuccessful && !string.IsNullOrWhiteSpace(_loginResponse.Data?.AccessToken))
        {
            _apiContext.AccessToken = _loginResponse.Data.AccessToken;
        }
    }

    [Given("I am authenticated with the configured credentials")]
    public async Task GivenIAmAuthenticatedWithConfiguredCredentials()
    {
        await AuthenticateWithConfiguredCredentials();

        Assert.That(_loginResponse!.IsSuccessful, Is.True,
            $"Login failed for configured user '{_settings.Auth.Username}'. {ResponseDiagnostics.Describe(_loginResponse)}");
        Assert.That(ResponseAssertions.BodyOf(_loginResponse, Login).AccessToken, Is.Not.Null.And.Not.Empty,
            "Login succeeded but the access token was missing from the response body.");
    }

    [When("I request the current authenticated user")]
    public async Task RequestCurrentAuthenticatedUser()
    {
        Assert.That(_apiContext.AccessToken, Is.Not.Null.And.Not.Empty,
            "An access token must be stored before calling /auth/me.");

        _currentUserResponse = await _authClient.GetCurrentUserAsync();
    }

    [Then("the authentication response status code should be {int}")]
    public void AuthenticationStatusShouldBe(int statusCode)
    {
        ResponseAssertions.StatusCodeShouldBe(_loginResponse, statusCode, Login);
    }

    [Then("the authentication response should contain a non-empty access token")]
    public void AuthenticationShouldContainAccessToken()
    {
        Assert.That(ResponseAssertions.BodyOf(_loginResponse, Login).AccessToken, Is.Not.Null.And.Not.Empty,
            "The login response did not contain an access token.");
    }

    [Then("the authentication response should contain a non-empty refresh token")]
    public void AuthenticationShouldContainRefreshToken()
    {
        Assert.That(ResponseAssertions.BodyOf(_loginResponse, Login).RefreshToken, Is.Not.Null.And.Not.Empty,
            "The login response did not contain a refresh token.");
    }

    [Then("the access token should be stored for subsequent requests")]
    public void AccessTokenShouldBeStored()
    {
        var returnedToken = ResponseAssertions.BodyOf(_loginResponse, Login).AccessToken;

        Assert.Multiple(() =>
        {
            Assert.That(_apiContext.AccessToken, Is.Not.Null.And.Not.Empty,
                "No access token was stored for subsequent requests.");
            Assert.That(string.Equals(_apiContext.AccessToken, returnedToken, StringComparison.Ordinal), Is.True,
                "The stored access token does not match the token returned by the login response.");
        });
    }

    [Then("the current user response status code should be {int}")]
    public void CurrentUserStatusShouldBe(int statusCode)
    {
        ResponseAssertions.StatusCodeShouldBe(_currentUserResponse, statusCode, CurrentUser);
    }

    [Then("the current user id should be {int}")]
    public void CurrentUserIdShouldBe(int id)
    {
        Assert.That(ResponseAssertions.BodyOf(_currentUserResponse, CurrentUser).Id, Is.EqualTo(id));
    }

    [Then("the current user first name should be {string}")]
    public void CurrentUserFirstNameShouldBe(string firstName)
    {
        Assert.That(ResponseAssertions.BodyOf(_currentUserResponse, CurrentUser).FirstName, Is.EqualTo(firstName));
    }

    [Then("the current user last name should be {string}")]
    public void CurrentUserLastNameShouldBe(string lastName)
    {
        Assert.That(ResponseAssertions.BodyOf(_currentUserResponse, CurrentUser).LastName, Is.EqualTo(lastName));
    }
}
