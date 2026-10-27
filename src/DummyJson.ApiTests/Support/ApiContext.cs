using RestSharp;

namespace DummyJson.ApiTests.Support;

/// <summary>
/// Scenario-scoped shared state for the access token and the most recent HTTP response.
/// </summary>
public sealed class ApiContext
{
    public string? AccessToken { get; set; }
    public RestResponse? LastResponse { get; set; }
    public string? LastRequestSummary { get; set; }
}
