using System.Diagnostics;

namespace DocPlatform.Infrastructure.Sourcing;

// Resolves a repo entry to a local working copy:
//   - git URL  -> shallow-clones it into the work directory (once)
//   - local path -> used as-is
public static class GitSourceResolver
{
    public static string Resolve(string entry, string workDirectory, Action<string>? log = null)
    {
        log ??= _ => { };

        if (!IsGitUrl(entry))
            return entry; // local path

        Directory.CreateDirectory(workDirectory);
        string name = RepoName(entry);
        string destination = Path.Combine(workDirectory, name);

        if (Directory.Exists(destination))
        {
            log($"Using cached clone: {name}");
            return destination;
        }

        log($"Cloning {entry} ...");
        RunGit($"clone --depth 1 {entry} \"{destination}\"");
        return destination;
    }

    private static bool IsGitUrl(string entry) =>
        entry.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
        entry.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
        entry.StartsWith("git@", StringComparison.OrdinalIgnoreCase);

    private static string RepoName(string url)
    {
        string last = url.TrimEnd('/').Split('/').Last();
        return last.EndsWith(".git", StringComparison.OrdinalIgnoreCase) ? last[..^4] : last;
    }

    private static void RunGit(string arguments)
    {
        var psi = new ProcessStartInfo("git", arguments)
        {
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
        using Process process = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start git.");
        process.WaitForExit();
        if (process.ExitCode != 0)
            throw new InvalidOperationException($"git {arguments} failed: {process.StandardError.ReadToEnd()}");
    }
}
