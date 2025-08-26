using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameStore.Api.DTOs;

public class OpenAiApiClient
{
    private readonly HttpClient _http;
    private readonly OpenAiOptions _opts;

    public OpenAiApiClient(HttpClient http, Microsoft.Extensions.Options.IOptions<OpenAiOptions> opts)
    {
        _http = http;
        _opts = opts.Value;
    }

    public async Task<string> ChatAsync(string system, string user, int? maxTokens, double? temperature, CancellationToken ct)
    {
        var payload = new ChatCompletionRequest
        {
            Model = _opts.Model,
            Temperature = temperature ?? _opts.Temperature,
            MaxTokens = maxTokens ?? _opts.MaxTokens,
            Messages = new[]
            {
                new ChatMessage { Role = "system", Content = system },
                new ChatMessage { Role = "user",   Content = user }
            }
        };

        using var res = await _http.PostAsJsonAsync("v1/chat/completions", payload, Serializer.Options, ct);
        var json = await res.Content.ReadAsStringAsync(ct);

        if (!res.IsSuccessStatusCode)
            throw new InvalidOperationException($"OpenAI error {(int)res.StatusCode}: {json}");

        var parsed = JsonSerializer.Deserialize<ChatCompletionResponse>(json, Serializer.Options)
                     ?? throw new InvalidOperationException("Empty response from OpenAI.");

        var text = parsed.Choices?.FirstOrDefault()?.Message?.Content ?? "";
        return text;
    }

    // DTOs for request/response
    private static class Serializer
    {
        public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    private sealed class ChatCompletionRequest
    {
        [JsonPropertyName("model")] public string Model { get; set; } = "gpt-4o-mini";
        [JsonPropertyName("temperature")] public double Temperature { get; set; } = 0.7;
        [JsonPropertyName("max_tokens")] public int MaxTokens { get; set; } = 1200;
        [JsonPropertyName("messages")] public ChatMessage[] Messages { get; set; } = Array.Empty<ChatMessage>();
    }

    private sealed class ChatMessage
    {
        [JsonPropertyName("role")] public string Role { get; set; } = "user";
        [JsonPropertyName("content")] public string Content { get; set; } = "";
    }

    private sealed class ChatCompletionResponse
    {
        [JsonPropertyName("choices")] public List<Choice> Choices { get; set; } = new();
        public sealed class Choice
        {
            [JsonPropertyName("message")] public ChoiceMessage? Message { get; set; }
        }
        public sealed class ChoiceMessage
        {
            [JsonPropertyName("role")] public string? Role { get; set; }
            [JsonPropertyName("content")] public string? Content { get; set; }
        }
    }
}