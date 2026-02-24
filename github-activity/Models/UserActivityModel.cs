using System.Text.Json.Serialization;

namespace github_activity.Models;

internal record UserActivityModel
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("type")]
    public string EventType { get; init; } = string.Empty;

    [JsonPropertyName("repo")]
    public RepositoryModel? Repository { get; init; }

    [JsonPropertyName("payload")]
    public PayloadModel? Payload { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }
}