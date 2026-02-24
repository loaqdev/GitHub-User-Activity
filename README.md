# GitHub User Activity

A small .NET console tool that fetches and summarizes a GitHub user's public activity (pushes, issues, PRs, stars, etc.) and prints human‑readable lines like:

- `Pushed 3 commits to kamranahmedse/developer-roadmap`
- `Opened a new issue in kamranahmedse/developer-roadmap`
- `Starred kamranahmedse/developer-roadmap`

Inspired by: https://roadmap.sh/projects/github-user-activity

## Features
- Retrieves the latest public events from the GitHub Events API for a given user.
- Type‑safe `Payload` model for convenient access to common fields (commits, action, pull_request, issue, ref_type).

## Prerequisites
- .NET 10 SDK (or later)
- Git (to clone the repository)

The project targets C# 14 and .NET 10.

## Install

Clone the repository:
```git clone https://github.com/loaqdev/GitHub-User-Activity.git cd GitHub-User-Activity```

Restore and build (optional; `dotnet run` will implicitly restore/build):
```
dotnet restore
dotnet build
```

## Run

Run the console application and pass a GitHub username as the first argument. From the repository root:
```dotnet run --project github-activity <github_username>```

Examples:
```dotnet run --project github-activity loaqdev```

The program will print summarized activity lines to the console.

## Notes
- The app uses the unauthenticated GitHub Events API — subject to rate limits. If you need higher limits, add GitHub authentication headers.
- The program aggregates `PushEvent` instances per repository by counting occurrences in the returned events. If the API payload contains commit counts, the model will use them when available.

## Contributing
Contributions welcome — open an issue or pull request on the repository.
