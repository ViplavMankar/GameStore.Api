using System;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.DTOs;


public record RewriteRequest
{
    public string? Input { get; init; }
    public string? Prompt { get; init; }
    public string? Audience { get; init; }
    public string? Tone { get; init; }
    public string? StyleGuide { get; init; }
    public string[]? SeoKeywords { get; init; }

    [Range(50, 20000)]
    public int? TargetWordCount { get; init; }

    public double? Temperature { get; init; }

    public bool UseFrontMatter { get; init; } = true;
    public bool AddSummary { get; init; } = true;
    public bool AddOutline { get; init; } = false;
}