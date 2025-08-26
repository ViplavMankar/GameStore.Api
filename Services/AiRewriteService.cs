using System.Text;
using System.Text.RegularExpressions;
using GameStore.Api.DTOs;

namespace GameStore.Api.Services;

public class AiRewriteService : IAiRewriteService
{
    private readonly GeminiApiClient _gemini;
    private readonly GeminiOptions _opts;

    public AiRewriteService(GeminiApiClient gemini,
        Microsoft.Extensions.Options.IOptions<GeminiOptions> opts)
    {
        _gemini = gemini;
        _opts = opts.Value;
    }

    public async Task<RewriteResult> RewriteAsync(RewriteRequest req, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(req.Input))
            throw new ArgumentException("RewriteRequest.Input is required.");

        var prompt = BuildPrompt(req);

        // map TargetWordCount to a safe token budget (≈ 1.5 tokens/word heuristic)
        int desiredTokens = req.TargetWordCount.HasValue
            ? Math.Clamp((int)Math.Round(req.TargetWordCount.Value * 1.5), 256, _opts.MaxOutputTokens)
            : _opts.MaxOutputTokens;

        double temp = req.Temperature ?? _opts.Temperature;

        var text = await _gemini.GenerateAsync(prompt, desiredTokens, temp, ct);

        // Try to extract title & summary (from YAML or headings/sections)
        var (title, summary) = ExtractMetadata(text);

        return new RewriteResult
        {
            Markdown = text,
            Title = title,
            Summary = summary,
            EstimatedWordCount = CountWords(text)
        };
    }

    private static string BuildPrompt(RewriteRequest r)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are an expert blog editor and SEO-savvy technical writer.");
        sb.AppendLine("Rewrite the provided input into a polished blog post with the following requirements.");
        sb.AppendLine();
        sb.AppendLine("## Requirements");
        if (!string.IsNullOrWhiteSpace(r.Audience))
            sb.AppendLine($"- Target audience: {r.Audience}");
        if (!string.IsNullOrWhiteSpace(r.Tone))
            sb.AppendLine($"- Desired tone: {r.Tone}");
        if (!string.IsNullOrWhiteSpace(r.StyleGuide))
            sb.AppendLine($"- Style guide/preferences: {r.StyleGuide}");
        if (r.SeoKeywords is { Length: > 0 })
            sb.AppendLine($"- Include SEO keywords (naturally, no stuffing): {string.Join(", ", r.SeoKeywords)}");
        if (r.TargetWordCount is int wc)
            sb.AppendLine($"- Aim for about {wc} words (±10%).");

        sb.AppendLine("- Use clear headings (H2/H3), short paragraphs, and bullet lists where helpful.");
        sb.AppendLine("- Include examples or code snippets only when they add value.");
        sb.AppendLine("- Avoid hallucinations; do not invent facts.");

        if (r.AddOutline)
            sb.AppendLine("- Start with a brief outline (H2: Outline) listing the main sections.");

        if (r.AddSummary)
            sb.AppendLine("- End with an H2: Summary section that recaps key takeaways in 3–5 bullets.");

        if (r.UseFrontMatter)
        {
            sb.AppendLine("- Prepend valid YAML front matter:");
            sb.AppendLine("  ---");
            sb.AppendLine("  title: <concise, compelling title>");
            sb.AppendLine("  description: <1–2 sentence meta description>");
            sb.AppendLine("  tags: [<seo keywords>]");
            sb.AppendLine("  ---");
        }
        else
        {
            sb.AppendLine("- Do NOT include YAML front matter.");
        }

        if (!string.IsNullOrWhiteSpace(r.Prompt))
        {
            sb.AppendLine();
            sb.AppendLine("## Additional instruction from user");
            sb.AppendLine(r.Prompt.Trim());
        }

        sb.AppendLine();
        sb.AppendLine("## Input to rewrite");
        sb.AppendLine("```");
        sb.AppendLine(r.Input!.Trim());
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("Return only the final article (Markdown).");

        return sb.ToString();
    }

    private static (string? title, string? summary) ExtractMetadata(string md)
    {
        // 1) YAML front matter title/description
        var yamlMatch = Regex.Match(md, @"^---\s*(.*?)\s*---", RegexOptions.Singleline);
        string? title = null;
        string? summary = null;

        if (yamlMatch.Success)
        {
            var yaml = yamlMatch.Groups[1].Value;
            var t = Regex.Match(yaml, @"^\s*title:\s*(.+)$", RegexOptions.Multiline);
            if (t.Success) title = CleanYamlScalar(t.Groups[1].Value);

            var d = Regex.Match(yaml, @"^\s*description:\s*(.+)$", RegexOptions.Multiline);
            if (d.Success) summary = CleanYamlScalar(d.Groups[1].Value);
        }

        // 2) If no title yet, use first ATX heading
        if (title == null)
        {
            var h1 = Regex.Match(md, @"^\s*#\s+(.+)$", RegexOptions.Multiline);
            if (h1.Success) title = h1.Groups[1].Value.Trim();
        }

        // 3) If no summary yet, try an H2 Summary section body (first paragraph/bullets)
        if (summary == null)
        {
            var summ = Regex.Match(md, @"^\s*##\s*Summary\s*\n([\s\S]*?)(\n##\s|\z)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (summ.Success)
            {
                var body = summ.Groups[1].Value.Trim();
                // Take first 1–2 lines for a compact summary
                var lines = body.Split('\n').Select(s => s.Trim('-', ' ', '\t')).Where(s => !string.IsNullOrWhiteSpace(s)).Take(2);
                summary = string.Join(" ", lines);
            }
        }

        return (string.IsNullOrWhiteSpace(title) ? null : title,
                string.IsNullOrWhiteSpace(summary) ? null : summary);
    }

    private static string CleanYamlScalar(string s)
        => s.Trim().Trim('"').Trim('\'');

    private static int CountWords(string s) =>
        string.IsNullOrWhiteSpace(s) ? 0 :
        s.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
}

