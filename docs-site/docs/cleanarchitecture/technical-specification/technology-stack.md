---
sidebar_position: 2
---

# Technology Stack

| Concern                     | Technology                                | Details                                                 |
|----------------------------|-------------------------------------------|---------------------------------------------------------|
| Runtime & Frameworks       | .NET (net10.0)                           | The application targets .NET version 10.0.             |
|                            | ASP.NET Core                             | Framework for building web applications and APIs.       |
|                            | Angular                                   | Front-end framework used for building client-side applications. |
| Data Access                | Entity Framework Core                    | ORM used for database operations.                       |
|                            | Aspire.Npgsql.EntityFrameworkCore.PostgreSQL | Provides PostgreSQL support for EF Core.               |
|                            | Microsoft.EntityFrameworkCore.Sqlite      | Provides SQLite support for EF Core.                   |
| Authentication             | ASP.NET Core Identity                    | Framework for managing user identity and authentication.|
| API Documentation          | OpenAPI                                   | Documentation standard for APIs via Microsoft.AspNetCore.OpenApi. |
| AI/LLM                    | No AI/LLM technologies detected          |                                                        |
| Cross-Cutting Libraries     | AutoMapper                                | Used for object-object mapping.                         |
|                            | FluentValidation.DependencyInjectionExtensions | Used for validation within the application.           |
|                            | Moq                                       | Mocking library used in unit testing.                  |
|                            | NUnit                                     | Testing framework utilized for writing tests.          |
|                            | Microsoft.Extensions.Http.Resilience      | Provides resilience capabilities for HTTP requests.     |
|                            | OpenTelemetry                              | Used for application observability and performance tracking. |
|                            | Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore | Provides diagnostics support for EF Core.              |
| Cloud                      | Azure SDK                                 | Utilized for Azure services integration.                |
| Database                   | PostgreSQL                                | RDBMS chosen for data persistence with integration through EF Core. |

This table encompasses the detected technologies, their purposes, and implementations associated with the APPLICATION as derived from the provided metadata.
