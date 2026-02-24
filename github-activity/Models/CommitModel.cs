using System.Text.Json.Serialization;

namespace github_activity.Models;

internal record CommitModel
{
    [JsonPropertyName("sha")]
    public string? Sha { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }
}
