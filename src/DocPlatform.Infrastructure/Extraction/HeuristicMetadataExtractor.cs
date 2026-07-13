using System.Text.RegularExpressions;
using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;

namespace DocPlatform.Infrastructure.Extraction;

// FAST extractor: regex + file conventions (no compilation). Good for the POC.
// Swap for RoslynMetadataExtractor for higher accuracy — same IMetadataExtractor.
public class HeuristicMetadataExtractor : IMetadataExtractor
{
    public void Extract(RepositoryModel repository)
    {
        foreach (ProjectModel project in repository.Projects)
        {
            if (project.Kind == ProjectKind.Angular) continue;   // handled by AngularMetadataExtractor
            ExtractDotNet(project);
        }
    }

    private static void ExtractDotNet(ProjectModel project)
    {
        string projectDir = Path.GetDirectoryName(project.Path)!;

        if (project.PackageReferences.Any(p => p.Contains("Authentication", StringComparison.OrdinalIgnoreCase)))
            project.HasAuthentication = true;

        foreach (string file in ExtractionHelpers.EnumerateFiles(projectDir).Where(f => f.EndsWith(".cs")))
        {
            string code;
            try { code = File.ReadAllText(file); } catch { continue; }

            string relDir = Path.GetDirectoryName(file)!.Replace(projectDir, "");
            if (code.Contains("[Authorize]") || code.Contains("AddAuthentication"))
                project.HasAuthentication = true;

            // Controllers
            foreach (Match m in Regex.Matches(code, @"class\s+(\w+Controller)\b"))
            {
                var controller = new ControllerModel { Name = m.Groups[1].Value };
                Match route = Regex.Match(code, @"\[Route\(""([^""]+)""\)\]");
                if (route.Success) controller.Route = route.Groups[1].Value;

                foreach (Match action in Regex.Matches(code,
                    @"\[Http(Get|Post|Put|Delete|Patch)[^\]]*\][\s\S]{0,160}?public\s+[\w<>\[\]?\s]+?\s+(\w+)\s*\("))
                    controller.Actions.Add($"{action.Groups[1].Value.ToUpper()} {action.Groups[2].Value}");

                project.Controllers.Add(controller);
            }

            // Minimal APIs
            ExtractMinimalApis(code, file, project);

            // DbContexts (base type ends with DbContext, e.g. DbContext / IdentityDbContext)
            foreach (Match m in Regex.Matches(code, @"class\s+(\w+)\s*:\s*[^\{]*?\b\w*DbContext\b"))
                project.DbContexts.Add(m.Groups[1].Value);

            // Interfaces
            foreach (Match m in Regex.Matches(code, @"\binterface\s+(I\w+)\b"))
                project.Interfaces.Add(m.Groups[1].Value);

            // Services
            foreach (Match m in Regex.Matches(code, @"(?:class|interface)\s+(\w*Service)\b"))
                project.Services.Add(m.Groups[1].Value);

            // CQRS requests (commands/queries — class or record)
            foreach (Match m in Regex.Matches(code, @"(?:class|record)\s+(\w+(?:Command|Query))\b"))
                project.CqrsRequests.Add(m.Groups[1].Value);

            // Entities (classes under Models/Entities/Domain, filtered)
            if (Regex.IsMatch(relDir, @"[/\\](Models|Entities|Domain)([/\\]|$)", RegexOptions.IgnoreCase))
                foreach (Match m in Regex.Matches(code, @"(?:class|record)\s+(\w+)\b"))
                    if (ExtractionHelpers.IsLikelyEntity(m.Groups[1].Value))
                        project.Entities.Add(m.Groups[1].Value);
        }

        ExtractionHelpers.Dedupe(project.Services);
        ExtractionHelpers.Dedupe(project.Interfaces);
        ExtractionHelpers.Dedupe(project.Entities);
        ExtractionHelpers.Dedupe(project.DbContexts);
        ExtractionHelpers.Dedupe(project.CqrsRequests);
    }

    private static void ExtractMinimalApis(string code, string file, ProjectModel project)
    {
        var endpoints = new List<string>();
        foreach (Match m in Regex.Matches(code, @"\.Map(Get|Post|Put|Delete|Patch)\s*(?:<[^>]*>)?\s*\(\s*(?:""([^""]*)"")?"))
        {
            string verb = m.Groups[1].Value.ToUpper();
            string route = m.Groups[2].Success ? m.Groups[2].Value : "";
            endpoints.Add($"{verb} {route}".Trim());
        }

        if (endpoints.Count > 0)
        {
            var controller = new ControllerModel { Name = Path.GetFileNameWithoutExtension(file) + " (Minimal API)" };
            Match group = Regex.Match(code, @"\.MapGroup\s*\(\s*""([^""]*)""");
            if (group.Success) controller.Route = group.Groups[1].Value;
            controller.Actions.AddRange(endpoints);
            project.Controllers.Add(controller);
        }
    }
}
