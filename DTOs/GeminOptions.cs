public sealed class GeminiOptions
{
    public string ApiKey { get; set; } = "";
    public string Model { get; set; } = "gemini-2.5-flash";
    public int MaxOutputTokens { get; set; } = 20000;
    public double Temperature { get; set; } = 0.7;
}