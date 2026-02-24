using System.Text.Json.Serialization;

namespace github_activity.Models;

internal record PayloadModel
{
    [JsonPropertyName("size")]
    public int? Size { get; init; }

    [JsonPropertyName("commits")]
    public List<CommitModel>? Commits { get; init; }

    [JsonPropertyName("action")]
    public string? Action { get; init; }

    [JsonPropertyName("ref")]
    public string? Ref { get; init; }

    [JsonPropertyName("ref_type")]
    public string? RefType { get; init; }

    [JsonPropertyName("pull_request")]
    public PullRequestModel? PullRequest { get; init; }

    [JsonPropertyName("issue")]
    public IssueModel? Issue { get; init; }
}