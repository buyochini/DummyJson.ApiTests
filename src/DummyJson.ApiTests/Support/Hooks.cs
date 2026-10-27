using DummyJson.ApiTests.Clients;
using Reqnroll;
using Reqnroll.BoDi;

namespace DummyJson.ApiTests.Support;

[Binding]
public sealed class Hooks(IObjectContainer container)
{
    private readonly IObjectContainer _container = container;

    [BeforeScenario]
    public void RegisterDependencies()
    {
        var settings = TestSettings.Load();
        var context = new ApiContext();
        var apiClient = new ApiClient(settings, context);

        _container.RegisterInstanceAs(settings);
        _container.RegisterInstanceAs(context);
        _container.RegisterInstanceAs(apiClient);
        _container.RegisterInstanceAs(new AuthClient(apiClient));
        _container.RegisterInstanceAs(new UsersClient(apiClient));
        _container.RegisterInstanceAs(new ProductsClient(apiClient));
        _container.RegisterInstanceAs(new CartsClient(apiClient));
    }

    [AfterScenario]
    public static void AfterScenario(ScenarioContext scenarioContext, ApiContext apiContext, ApiClient apiClient)
    {
        if (scenarioContext.TestError is not null)
        {
            TestContext.Out.WriteLine($"Scenario failed: {scenarioContext.ScenarioInfo.Title}");
            TestContext.Out.WriteLine($"Last request: {apiContext.LastRequestSummary ?? "(none)"}");

            if (apiContext.LastResponse is not null)
            {
                TestContext.Out.WriteLine($"Last response: {ResponseDiagnostics.Describe(apiContext.LastResponse)}");
            }
        }

        apiClient.Dispose();
    }
}
