using DocPlatform.Core.Models;

namespace DocPlatform.Application;

// Deterministically classifies package references into technical capability
// categories (auth, data, storage, messaging, cloud, caching, …). This gives the
// AI grounded facts so the Technical Specification never invents infrastructure.
public static class CapabilityClassifier
{
    // packageFragment (case-insensitive contains) -> (Category, Friendly name)
    private static readonly (string Fragment, string Category, string Name)[] Rules =
    {
        // Web / API
        ("Microsoft.AspNetCore",           "Web / API",        "ASP.NET Core"),
        ("Swashbuckle",                    "API Documentation","Swagger / Swashbuckle"),
        ("Microsoft.AspNetCore.OpenApi",   "API Documentation","OpenAPI"),

        // Data / ORM
        ("Dapper",                         "Data Access",      "Dapper (micro-ORM)"),
        ("EntityFrameworkCore",            "Data Access",      "Entity Framework Core"),
        ("MongoDB.Driver",                 "Data Access",      "MongoDB Driver"),

        // Databases / storage
        ("Microsoft.Data.SqlClient",       "Database",         "SQL Server"),
        ("System.Data.SqlClient",          "Database",         "SQL Server"),
        ("Npgsql",                         "Database",         "PostgreSQL"),
        ("MySql",                          "Database",         "MySQL"),
        ("Microsoft.Azure.Cosmos",         "Database",         "Azure Cosmos DB"),
        ("Azure.Storage.Blobs",            "Cloud Storage",    "Azure Blob Storage"),
        ("AWSSDK.S3",                      "Cloud Storage",    "AWS S3"),

        // Messaging / queuing
        ("RabbitMQ.Client",                "Messaging",        "RabbitMQ"),
        ("MassTransit",                    "Messaging",        "MassTransit"),
        ("Azure.Messaging.ServiceBus",     "Messaging",        "Azure Service Bus"),
        ("Confluent.Kafka",               "Messaging",        "Apache Kafka"),
        ("AWSSDK.SQS",                     "Messaging",        "AWS SQS"),

        // Caching
        ("StackExchange.Redis",            "Caching",          "Redis"),
        ("Microsoft.Extensions.Caching",   "Caching",          "Distributed / in-memory cache"),

        // Cloud SDKs
        ("Azure.",                         "Cloud",            "Azure SDK"),
        ("Microsoft.Azure",               "Cloud",            "Azure SDK"),
        ("AWSSDK.",                        "Cloud",            "AWS SDK"),

        // Authentication / security
        ("Authentication.JwtBearer",       "Authentication",   "JWT Bearer authentication"),
        ("Microsoft.AspNetCore.Identity",  "Authentication",   "ASP.NET Core Identity"),
        ("OpenIdConnect",                  "Authentication",   "OpenID Connect"),
        ("BCrypt",                         "Security",         "BCrypt password hashing"),

        // AI
        ("SemanticKernel",                 "AI / LLM",         "Semantic Kernel"),
        ("Azure.AI",                       "AI / LLM",         "Azure AI"),
        ("OpenAI",                         "AI / LLM",         "OpenAI / GitHub Models"),

        // Cross-cutting
        ("FluentValidation",               "Validation",       "FluentValidation"),
        ("AutoMapper",                     "Mapping",          "AutoMapper"),
        ("Serilog",                        "Logging",          "Serilog"),
        ("NLog",                           "Logging",          "NLog"),
        ("xunit",                          "Testing",          "xUnit"),
        ("nunit",                          "Testing",          "NUnit"),
        ("Moq",                            "Testing",          "Moq"),
    };

    public static List<DetectedCapability> Classify(ApplicationModel application)
    {
        var found = new List<DetectedCapability>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        IEnumerable<string> allPackages = application.Repositories
            .SelectMany(r => r.Projects)
            .SelectMany(p => p.PackageReferences);

        foreach (string package in allPackages)
        {
            foreach ((string fragment, string category, string name) in Rules)
            {
                if (!package.Contains(fragment, StringComparison.OrdinalIgnoreCase)) continue;
                string key = category + "|" + name;
                if (seen.Add(key))
                    found.Add(new DetectedCapability { Category = category, Name = name, Source = package });
            }
        }

        return found.OrderBy(c => c.Category).ThenBy(c => c.Name).ToList();
    }
}
