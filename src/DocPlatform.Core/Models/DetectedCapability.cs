namespace DocPlatform.Core.Models;

// A deterministically-detected technical capability (from package references).
// e.g. Category="Messaging", Name="RabbitMQ", Source="RabbitMQ.Client".
// Gives the AI grounded facts for the Technical Specification so it never invents
// infrastructure that isn't there.
public class DetectedCapability
{
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
}
