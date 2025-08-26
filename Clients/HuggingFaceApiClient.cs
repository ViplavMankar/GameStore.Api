using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameStore.Api.Models;

public class HuggingFaceApiClient
{
    private readonly HttpClient _http;
    private readonly HuggingFaceOptions _opts;

    public HuggingFaceApiClient(HttpClient http, Microsoft.Extensions.Options.IOptions<HuggingFaceOptions> opts)
    {
        _http = http;
        _opts = opts.Value;
    }

    public async Task<string> GenerateTextAsync(string prompt, int maxTokens = 512, double temperature = 0.7, CancellationToken ct = default)
    {
        var payload = new
        {
            inputs = prompt,
            parameters = new
            {
                max_new_tokens = maxTokens,
                temperature = temperature
            }
        };

        var response = await _http.PostAsJsonAsync($"models/{_opts.Model}", payload, ct);

        var json = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"HuggingFace API error {(int)response.StatusCode}: {json}");

        // Response is an array of generations
        var parsed = JsonSerializer.Deserialize<HuggingFaceResponse[]>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                     ?? Array.Empty<HuggingFaceResponse>();

        return parsed.FirstOrDefault()?.GeneratedText ?? "";
    }

    private sealed class HuggingFaceResponse
    {
        [JsonPropertyName("generated_text")]
        public string GeneratedText { get; set; } = "";
    }
}