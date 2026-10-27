# DummyJSON API Test Automation Framework

C# API test automation suite for [DummyJSON](https://dummyjson.com) built for the STA Senior Test Analyst coding challenge.

The suite contains **nine Gherkin scenarios** covering Auth, Users, Products and Carts. Each scenario validates HTTP status codes, response payloads and relevant attributes.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Network access to `https://dummyjson.com`
- Visual Studio, Visual Studio Code, or JetBrains Rider (optional)

## How to run

From the repository root:

```bash
dotnet test
```

Run a single resource tag:

```bash
dotnet test --filter "Category=auth"
dotnet test --filter "Category=users"
dotnet test --filter "Category=products"
dotnet test --filter "Category=carts"
```

> Reqnroll maps feature tags such as `@auth` to NUnit categories, so the filters above select the matching feature scenarios.

Open `DummyJson.ApiTests.slnx` in Visual Studio / VS Code and run the tests from the Test Explorer if you prefer the IDE.

### HTML report

Each test run writes a Cucumber-style HTML report through Reqnroll's built-in formatter, configured in `src/DummyJson.ApiTests/reqnroll.json`. The report is a single file you can open offline. It lists every feature, scenario and step with its pass/fail status and supports filtering by tag (for example `@carts`).

The output path is relative to the test build output:

```text
src/DummyJson.ApiTests/bin/Debug/net10.0/reports/reqnroll_report.html     # dotnet test
src/DummyJson.ApiTests/bin/Release/net10.0/reports/reqnroll_report.html   # dotnet test -c Release
```

On macOS, open the latest report with:

```bash
open src/DummyJson.ApiTests/bin/Debug/net10.0/reports/reqnroll_report.html
```

Each run overwrites the report. It sits under `bin/`, so git ignores it.

## Project structure

```text
src/DummyJson.ApiTests/
  Features/           Gherkin scenarios (Auth, Users, Products, Carts)
  StepDefinitions/    Step bindings that orchestrate assertions
  Clients/            RestSharp resource clients (Auth, Users, Products, Carts)
  Models/             Request/response DTOs
  Support/            Hooks, ApiContext, TestSettings, ApiEndpoints
  appsettings.json    Base URL and test credentials
```


## Design decisions

### Reqnroll for Gherkin

The brief requires Gherkin. SpecFlow is end-of-life, so this project uses **Reqnroll** (the maintained SpecFlow successor) with the **NUnit** runner. Feature files stay readable for stakeholders while steps remain strongly typed C#.

### RestSharp as the HTTP client

RestSharp provides concise request building (`AddJsonBody`, query parameters, method helpers) without fighting `HttpClient` boilerplate. It is wrapped behind small resource clients so step definitions stay focused on behaviour and assertions. Request paths such as `/products/search` live in `ApiEndpoints` so clients do not scatter magic strings.

### Layered architecture

- **Features** describe intent.
- **Step definitions** translate intent into client calls and assertions.
- **Clients** isolate endpoint paths and HTTP details.
- **ApiContext** holds the access token and last response for the current scenario.
- **Hooks** register scenario dependencies and log request/response details on failure.

### Independent scenarios

No scenario depends on another scenario's side effects. Auth for `/auth/me` is performed in a `Given` step inside that scenario. Write operations are validated from the immediate response only.

### Stateless DummyJSON behaviour

DummyJSON simulates `POST` / `PUT` / `PATCH` / `DELETE`. Changes are **not** persisted. Tests therefore:

- assert status codes and echoed payload fields on create/update/delete responses
- never re-fetch a created/updated/deleted resource expecting the change to still be present

### Test data in feature tables

User creation and cart line items are expressed as Gherkin tables so the payload requirements from the brief remain visible in the feature files.

### Assertions

NUnit `Assert.That` constraints are used throughout. This avoids a separate assertion library and keeps the dependency set small.

`ResponseAssertions` guards every status and body check. If a response was never captured, or its body could not be deserialized, the step fails with a message that names the response and includes its status, any transport or deserialization error, and a truncated body. It does not fail with a null reference. A transport failure is reported as "no HTTP response" rather than as a made-up status code.

### Redacted diagnostics

Failure messages and the after-scenario failure log pass response details through `ResponseDiagnostics`. It masks `accessToken`, `refreshToken`, `token` and `password` values, `Bearer` headers, and any JWT-shaped string. The stored-token assertion compares the two tokens as a boolean, so a mismatch does not print either one.

## Assumptions

| Topic | Assumption |
| --- | --- |
| Auth user | DummyJSON demo user `emilys` / `emilyspass` (configurable in `appsettings.json`) |
| "Delete a Card" | Treated as a typo for **Delete a Cart** (`DELETE /carts/{id}`) |
| Red hair count | Validated via `GET /users/filter?key=hair.color&value=Red` against the live `total` (currently 25) |
| 15th product | Retrieved with `limit=1&skip=14` so the response contains only that product |
| Total discounted percentage | Interpreted as the **sum of each cart line item's `discountPercentage`** (see below). For products 1–5 this equals `62.11` |

### Total discounted percentage

The brief's "total discounted percentage" has two plausible readings, and they give very different numbers for products 1–5 (quantity 1 each):

| Interpretation | Calculation | Result |
| --- | --- | --- |
| Sum of line-item percentages (**used by this suite**) | `10.48 + 18.19 + 9.84 + 12.16 + 11.44` | `62.11` |
| Effective cart-wide discount rate | `(total − discountedTotal) / total × 100` = `(66.95 − 58) / 66.95 × 100` | ≈ `13.37` |

The suite asserts the sum because it is the literal total of the `discountPercentage` values. That sum is not a discount rate the customer receives: percentages applied to different prices cannot be added to give a cart-level rate. If the effective rate is what's intended, the step should calculate it from `total` and `discountedTotal` instead.

The scenario also checks that the response contains exactly the requested product IDs and quantities, and the requested `userId`. Without those checks, a cart with the wrong items could still pass if its product count and percentage sum happened to match.

## Configuration

`src/DummyJson.ApiTests/appsettings.json`:

```json
{
  "BaseUrl": "https://dummyjson.com",
  "Auth": {
    "Username": "emilys",
    "Password": "emilyspass"
  }
}
```

The expected identity of the authenticated user (id `1`, `Emily Johnson`) is stated only in `Auth.feature`, so there is one source of truth. If you change the configured credentials, update that scenario too.

## Scenario map

| # | Resource | Scenario | Endpoint |
| --- | --- | --- | --- |
| 1 | Auth | Authenticate and store access token | `POST /auth/login` |
| 2 | Auth | Validate current user id / names | `GET /auth/me` |
| 3 | Users | Count users with red hair | `GET /users/filter` |
| 4 | Users | Add a new user (full address) | `POST /users/add` |
| 5 | Products | iPhone 13 Pro price | `GET /products/search` |
| 6 | Products | 15th product via pagination | `GET /products?limit=1&skip=14` |
| 7 | Products | Update product 121 title | `PUT /products/121` |
| 8 | Carts | Add 5 products; total discount % | `POST /carts/add` |
| 9 | Carts | Delete a cart | `DELETE /carts/1` |
