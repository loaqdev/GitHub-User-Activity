using System.Text.Json.Serialization;

namespace github_activity.Models;

internal record PullRequestModel
{
    [JsonPropertyName("number")]
    public int? Number { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("merged")]
    public bool? Merged { get; init; }
}
