using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using RestSharp;

namespace DummyJson.ApiTests.Support;

/// <summary>
/// Formats HTTP responses for failure messages and logs, with credentials and tokens redacted.
/// </summary>
public static partial class ResponseDiagnostics
{
    private const int MaxBodyLength = 1000;
    private const string Redacted = "[REDACTED]";

    private static readonly HashSet<string> SensitiveProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "accessToken",
        "refreshToken",
        "token",
        "password"
    };

    private static readonly JsonSerializerOptions LogJsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string Describe(RestResponse response)
    {
        var parts = new List<string>
        {
            response.StatusCode == 0
                ? "No HTTP response was received"
                : $"Status: {(int)response.StatusCode} {response.StatusCode}"
        };

        if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
        {
            parts.Add($"Error: {Redact(response.ErrorMessage)}");
        }

        parts.Add($"Body: {FormatBody(response.Content)}");

        return string.Join(" | ", parts);
    }

    public static string Redact(string text)
    {
        var redacted = TryRedactJson(text) ?? SensitiveJsonValue().Replace(text, $"$1\"{Redacted}\"");
        redacted = BearerToken().Replace(redacted, $"Bearer {Redacted}");
        return Jwt().Replace(redacted, Redacted);
    }

    private static string? TryRedactJson(string text)
    {
        JsonNode? root;
        try
        {
            root = JsonNode.Parse(text);
        }
        catch (JsonException)
        {
            return null;
        }

        if (root is null)
        {
            return null;
        }

        MaskSensitiveProperties(root);
        return root.ToJsonString(LogJsonOptions);
    }

    private static void MaskSensitiveProperties(JsonNode node)
    {
        switch (node)
        {
            case JsonObject jsonObject:
                foreach (var name in jsonObject.Select(property => property.Key).ToList())
                {
                    if (SensitiveProperties.Contains(name))
                    {
                        jsonObject[name] = Redacted;
                    }
                    else if (jsonObject[name] is { } child)
                    {
                        MaskSensitiveProperties(child);
                    }
                }
                break;

            case JsonArray jsonArray:
                foreach (var item in jsonArray)
                {
                    if (item is not null)
                    {
                        MaskSensitiveProperties(item);
                    }
                }
                break;
        }
    }

    private static string FormatBody(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return "(empty)";
        }

        var redacted = Redact(content);
        return redacted.Length <= MaxBodyLength
            ? redacted
            : $"{redacted[..MaxBodyLength]}... ({redacted.Length - MaxBodyLength} more characters)";
    }

    // Fallback for text that is not valid JSON. The value pattern consumes escaped characters such as \"
    // and also matches an unterminated value so a cut-off body cannot leak the rest of a secret.
    [GeneratedRegex("(\"(?:accessToken|refreshToken|token|password)\"\\s*:\\s*)\"(?:[^\"\\\\]|\\\\.)*(?:\"|\\\\?$)", RegexOptions.IgnoreCase)]
    private static partial Regex SensitiveJsonValue();

    [GeneratedRegex(@"Bearer\s+[A-Za-z0-9\-._~+/]+=*", RegexOptions.IgnoreCase)]
    private static partial Regex BearerToken();

    [GeneratedRegex(@"eyJ[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+\.[A-Za-z0-9_-]*")]
    private static partial Regex Jwt();
}
