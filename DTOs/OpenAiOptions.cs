using System;

namespace GameStore.Api.DTOs;

public class OpenAiOptions
{
    public string? ApiKey { get; set; }
    public string Model { get; set; } = "gpt-4o-mini";
    public int MaxTokens { get; set; } = 1200;
    public double Temperature { get; set; } = 0.7;
}