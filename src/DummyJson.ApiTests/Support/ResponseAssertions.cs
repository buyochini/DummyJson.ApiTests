using RestSharp;

namespace DummyJson.ApiTests.Support;

/// <summary>
/// Response guards that fail with a descriptive, redacted message instead of a null reference.
/// </summary>
public static class ResponseAssertions
{
    public static void StatusCodeShouldBe(RestResponse? response, int expectedStatusCode, string description)
    {
        var captured = Captured(response, description);

        Assert.That((int)captured.StatusCode, Is.EqualTo(expectedStatusCode),
            $"Unexpected status code for the {description} response. {ResponseDiagnostics.Describe(captured)}");
    }

    public static T BodyOf<T>(RestResponse<T>? response, string description)
        where T : class
    {
        var captured = Captured(response, description);

        if (captured.Data is null)
        {
            Assert.Fail(
                $"The {description} response body could not be read as {typeof(T).Name}. " +
                ResponseDiagnostics.Describe(captured));
        }

        return captured.Data!;
    }

    private static TResponse Captured<TResponse>(TResponse? response, string description)
        where TResponse : RestResponse
    {
        if (response is null)
        {
            Assert.Fail($"No {description} response was captured. Check that the request step ran before this assertion.");
        }

        return response!;
    }
}
