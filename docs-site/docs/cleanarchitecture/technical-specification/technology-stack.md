---
sidebar_position: 2
---

# Technology Stack

| Concern                     | Technology                                    | Details                                                                                |
|----------------------------|----------------------------------------------|----------------------------------------------------------------------------------------|
| Runtime & Framework         | .NET (net10.0)                              | The application is built on the .NET framework targeting version 10.0.                |
| Framework                   | ASP.NET Core                                 | Utilized for developing web applications, particularly for API endpoints.             |
| Frontend                    | Angular                                      | The application employs Angular for its client-side framework.                        |
| Data Access                 | Entity Framework Core                        | Implemented for data access and ORM capabilities.                                     |
| Database                    | PostgreSQL                                   | The database technology used for data persistence, supported through various libraries.|
| Authentication              | ASP.NET Core Identity                        | Provides identity management for user authentication and authorization.                |
| API Documentation           | OpenAPI / Swagger                           | Facilitates the documentation of the API using the OpenAPI specification.              |
| Mapping                     | AutoMapper                                   | An object-to-object mapping library for simplifying data transfer between layers.      |
| Validation                  | FluentValidation                             | Used for validating object properties in a fluent interface style.                     |
| Cloud                       | Azure SDK                                    | Incorporates various Azure services through Aspire hosting capabilities.              |
| Testing                     | Moq                                          | A mocking library for .NET, used to create test doubles.                             |
| Testing                     | NUnit                                        | A testing framework for executing unit and integration tests.                         |
| Services & Diagnostics      | Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore | Provides diagnostics for Entity Framework Core in ASP.NET applications.               |
| Resilience                  | Microsoft.Extensions.Http.Resilience        | Implements resilience strategies for HTTP calls.                                      |
| Service Discovery           | Microsoft.Extensions.ServiceDiscovery       | Facilitates service discovery patterns within the application.                         |
| Telemetry                   | OpenTelemetry                                 | Instrumentation for capturing telemetry data for performance monitoring.               |
| Cross-Cutting Libraries     | Aspire.Hosting.AppHost                       | Assists with hosting the application in various environments including Azure.          |
| Cross-Cutting Libraries     | CommunityToolkit.Aspire.Hosting.SQLite       | A library providing support for SQLite hosting.                                       |
| Cross-Cutting Libraries     | Aspire.Hosting.Azure.AppContainers           | Integration for hosting applications in Azure App Services using containers.          |
| Cross-Cutting Libraries     | Aspire.Hosting.Azure.PostgreSQL              | Support for using PostgreSQL within Azure-hosted applications.                        |
| Testing                     | Coverlet                                      | Used for measuring code coverage in .NET applications.                               |
| Testing                     | Shouldly                                     | An assertion library for easier test assertions in unit tests.                       |

This table presents a comprehensive overview of the technologies and capabilities utilized in the CleanArchitecture application, structured by their respective concerns.
