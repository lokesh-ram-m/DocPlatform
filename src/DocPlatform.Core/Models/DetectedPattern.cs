namespace DocPlatform.Core.Models;

// A heuristically-detected architecture pattern with the evidence for it.
// (Never asserted as certain — the docs present these as "detected".)
public class DetectedPattern
{
    public string Name { get; set; } = string.Empty;
    public string Evidence { get; set; } = string.Empty;
}
