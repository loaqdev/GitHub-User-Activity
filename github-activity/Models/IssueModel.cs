using System.Text.Json.Serialization;

namespace github_activity.Models;

internal record IssueModel
{
    [JsonPropertyName("number")]
    public int? Number { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }
}