---
sidebar_position: 2
---

# Technology Stack

| Concern                     | Technology                                         | Details                                                                                            |
|-----------------------------|---------------------------------------------------|----------------------------------------------------------------------------------------------------|
| Runtime & Frameworks        | .NET (net10.0)                                   | The application targets .NET 10.0.                                                                |
|                             | ASP.NET Core                                      | Used for building the web API and minimal APIs.                                                    |
|                             | Angular                                           | The application includes an Angular frontend.                                                     |
| Data Access                 | Entity Framework Core                             | Core library for data access.                                                                       |
|                             | Microsoft.EntityFrameworkCore                     | Provides database interaction capabilities.                                                         |
|                             | Aspire.Microsoft.EntityFrameworkCore.SqlServer   | Integration for SQL Server database access.                                                         |
|                             | Aspire.Npgsql.EntityFrameworkCore.PostgreSQL    | Integration for PostgreSQL database access.                                                        |
|                             | Microsoft.EntityFrameworkCore.Sqlite              | Integration for SQLite database access.                                                             |
| Authentication              | ASP.NET Core Identity                             | Used for implementing authentication features in the application.                                 |
| API Documentation           | OpenAPI                                           | Implemented via Microsoft.AspNetCore.OpenApi for API documentation.                               |
|                             | Microsoft.Extensions.ApiDescription.Server       | Used for improving API description and accessibility.                                             |
| AI/LLM                     | No AI/LLM technologies detected.                  |                                                                                                    |
| Cross-Cutting Libraries      | AutoMapper                                        | Used for object-to-object mappings throughout the application.                                     |
|                             | FluentValidation.DependencyInjectionExtensions     | Provides capabilities for model validation.                                                        |
|                             | Moq                                               | Mock library for unit testing.                                                                      |
|                             | NUnit                                             | Testing framework used for writing and executing tests.                                            |
|                             | Microsoft.Extensions.Http.Resilience             | Adds resilience capabilities to HTTP clients.                                                       |
|                             | Microsoft.Extensions.ServiceDiscovery             | Service discovery capabilities for cloud services.                                                 |
|                             | OpenTelemetry                                     | Used for tracking and providing insights into application performance                                |

The technologies listed provide a comprehensive framework for developing the application, ensuring efficient data management, secure authentication, and thorough testing capabilities.
