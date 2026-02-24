using System.Net.Http.Json;
using System.Text.Json;
using github_activity.Models;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        if (args == null || args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
        {
            Console.Error.WriteLine("Please enter username. \nUsage: get-activity \"username\"");
            return 4;
        }

        string user = args[0].Trim();
        var url = $"https://api.github.com/users/{user}/events";
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        try
        {
            using var _http = new HttpClient();
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("github-activity/1.0");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var activities = await _http.GetFromJsonAsync<List<UserActivityModel>>(url, options, cts.Token);

            if (activities == null || activities.Count == 0)
            {
                Console.WriteLine("Activities not found.");
                return 0;
            }

            var pushCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var a in activities)
            {
                if (a.EventType == "PushEvent")
                {
                    var repo = a.Repository?.Name ?? "Unknown Repository";
                    pushCounts[repo] = pushCounts.TryGetValue(repo, out var n) ? n + 1 : 1;
                }
            }

            var printedPushes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var act in activities)
            {
                var repoName = act.Repository?.Name ?? "Unknown Repository";
                string? message = act.EventType switch
                {
                    "PushEvent" =>
                        printedPushes.Contains(repoName)
                            ? null
                            : HandlePushAggregate(repoName, GetCommitsFromPayload(act.Payload, pushCounts.TryGetValue(repoName, out var cnt) ? cnt : 1)),
                    "IssuesEvent" => HandleIssuesEvent(act),
                    "IssueCommentEvent" => HandleIssueCommentEvent(act),
                    "WatchEvent" => HandleWatchEvent(act),
                    "ForkEvent" => $"Forked {repoName}",
                    "PullRequestEvent" => HandlePullRequestEvent(act),
                    "CreateEvent" => HandleCreateEvent(act),
                    "DeleteEvent" => HandleDeleteEvent(act),
                    "ReleaseEvent" => HandleReleaseEvent(act),
                    _ => null
                };

                if (act.EventType == "PushEvent" && message != null)
                    printedPushes.Add(repoName);

                if (!string.IsNullOrEmpty(message))
                    Console.WriteLine(message);
            }

            return 0;
        }
        catch (OperationCanceledException)
        {
            Console.Error.WriteLine("Request was canceled (Timeout).");
            return 2;
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"HTTP error: {ex.Message}");
            return 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 3;
        }
    }

    private static int GetCommitsFromPayload(PayloadModel? payload, int fallback)
    {
        if (payload != null)
        {
            if (payload.Commits != null)
                return payload.Commits.Count;
            if (payload.Size.HasValue)
                return payload.Size.Value;
        }
        return fallback <= 0 ? 1 : fallback;
    }

    private static string HandlePushAggregate(string repoName, int commits)
    {
        var plural = commits == 1 ? "commit" : "commits";
        return $"Pushed {commits} {plural} to {repoName}";
    }

    private static string? HandleIssuesEvent(UserActivityModel act)
    {
        var repo = act.Repository?.Name ?? "Unknown Repository";
        var action = act.Payload?.Action?.ToLowerInvariant() ?? "performed";
        return action switch
        {
            "opened" => $"Opened a new issue in {repo}",
            "closed" => $"Closed an issue in {repo}",
            "reopened" => $"Reopened an issue in {repo}",
            _ => $"{char.ToUpper(action[0]) + action.Substring(1)} an issue in {repo}"
        };
    }

    private static string? HandleIssueCommentEvent(UserActivityModel act)
    {
        var repo = act.Repository?.Name ?? "Unknown Repository";
        return $"Commented on an issue in {repo}";
    }

    private static string? HandleWatchEvent(UserActivityModel act)
    {
        var repo = act.Repository?.Name ?? "Unknown Repository";
        var action = act.Payload?.Action?.ToLowerInvariant() ?? "started";
        if (action == "started")
            return $"Starred {repo}";
        return $"Watched {repo}";
    }

    private static string? HandlePullRequestEvent(UserActivityModel act)
    {
        var repo = act.Repository?.Name ?? "Unknown Repository";
        var action = act.Payload?.Action?.ToLowerInvariant() ?? "performed";

        if (action == "opened")
            return $"Opened a pull request in {repo}";

        if (action == "closed")
        {
            if (act.Payload?.PullRequest?.Merged == true)
                return $"Merged a pull request in {repo}";
            return $"Closed a pull request in {repo}";
        }

        return $"{char.ToUpper(action[0]) + action.Substring(1)} a pull request in {repo}";
    }

    private static string? HandleCreateEvent(UserActivityModel act)
    {
        var repo = act.Repository?.Name ?? "Unknown Repository";
        var refType = act.Payload?.RefType?.ToLowerInvariant() ?? string.Empty;
        var @ref = act.Payload?.Ref ?? string.Empty;

        return refType switch
        {
            "branch" when !string.IsNullOrEmpty(@ref) => $"Created branch {@ref} in {repo}",
            "tag" when !string.IsNullOrEmpty(@ref) => $"Created tag {@ref} in {repo}",
            "repository" => $"Created repository {repo}",
            _ => $"Created {refType} in {repo}"
        };
    }

    private static string? HandleDeleteEvent(UserActivityModel act)
    {
        var repo = act.Repository?.Name ?? "Unknown Repository";
        var refType = act.Payload?.RefType ?? "ref";
        var @ref = act.Payload?.Ref ?? string.Empty;

        if (!string.IsNullOrEmpty(@ref))
            return $"Deleted {refType} {@ref} in {repo}";
        return $"Deleted {refType} in {repo}";
    }

    private static string? HandleReleaseEvent(UserActivityModel act)
    {
        var repo = act.Repository?.Name ?? "Unknown Repository";
        return $"Released {repo}";
    }
}