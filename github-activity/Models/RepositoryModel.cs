using System.Text.Json.Serialization;

namespace github_activity.Models;

internal record RepositoryModel
{
    [JsonPropertyName("id")]
    public long? Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
}