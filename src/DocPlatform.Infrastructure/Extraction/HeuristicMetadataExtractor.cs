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
            if (project.Kind == ProjectKind.Angular) ExtractionHelpers.ExtractAngular(project);
            else ExtractDotNet(project);
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

            foreach (Match m in Regex.Matches(code, @"class\s+(\w+)\s*:\s*[\w<>,\s]*\bDbContext\b"))
                project.DbContexts.Add(m.Groups[1].Value);

            foreach (Match m in Regex.Matches(code, @"\binterface\s+(I\w+)\b"))
                project.Interfaces.Add(m.Groups[1].Value);

            foreach (Match m in Regex.Matches(code, @"(?:class|interface)\s+(\w*Service)\b"))
                project.Services.Add(m.Groups[1].Value);

            if (Regex.IsMatch(relDir, @"[/\\](Models|Entities|Domain)([/\\]|$)", RegexOptions.IgnoreCase))
                foreach (Match m in Regex.Matches(code, @"class\s+(\w+)\b"))
                    project.Entities.Add(m.Groups[1].Value);
        }

        ExtractionHelpers.Dedupe(project.Services);
        ExtractionHelpers.Dedupe(project.Interfaces);
        ExtractionHelpers.Dedupe(project.Entities);
        ExtractionHelpers.Dedupe(project.DbContexts);
    }
}
