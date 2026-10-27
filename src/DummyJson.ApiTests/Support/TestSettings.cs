using Microsoft.Extensions.Configuration;

namespace DummyJson.ApiTests.Support;

public sealed class TestSettings
{
    public string BaseUrl { get; init; } = string.Empty;
    public AuthSettings Auth { get; init; } = new();

    public static TestSettings Load()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var settings = configuration.Get<TestSettings>()
            ?? throw new InvalidOperationException("Failed to load test settings from appsettings.json.");

        if (string.IsNullOrWhiteSpace(settings.BaseUrl))
        {
            throw new InvalidOperationException("BaseUrl must be configured in appsettings.json.");
        }

        return settings;
    }
}

public sealed class AuthSettings
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
