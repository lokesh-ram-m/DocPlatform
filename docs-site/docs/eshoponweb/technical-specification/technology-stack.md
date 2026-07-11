---
sidebar_position: 2
---

# Technology Stack

| Concern                  | Technology                             | Details                                                     |
|-------------------------|---------------------------------------|-------------------------------------------------------------|
| Runtime & Framework      | .NET                                   | Targeted for .NET applications with ASP.NET Core.           |
| Framework                | ASP.NET Core                          | Used for building web applications and APIs.                |
| Data Access              | Entity Framework Core                | Used for data access; includes EF Core with SQL Server and InMemory support. |
| Database                 | SQL Server, InMemory                  | Supports SQL Server for production and InMemory for testing. |
| Authentication           | ASP.NET Core Identity                 | Provides identity management, integrating with EF Core.     |
| Authentication           | JWT Authentication                    | Implements JWT bearer token authentication.                  |
| API Documentation        | OpenAPI / Swagger                     | Managed by Swashbuckle for generating API documentation.    |
| Frontend Framework       | Blazor                                | Used for building interactive web UIs.                      |
| Mapping                  | AutoMapper                            | Used for object-to-object mapping across the application.    |
| Validation               | FluentValidation                      | Used for model validation within the application.           |
| Testing Framework        | xUnit                                 | Framework for unit and functional testing.                  |
| Cloud                    | Azure SDK                            | Supports Azure services integration (detected via configuration for Azure SDK). |


### Libraries and Packages Used

- **Blazored.LocalStorage**: For local storage handling in Blazor applications.
- **BlazorInputFile**: For file uploads in Blazor.
- **MediatR**: For implementing the mediator pattern, likely used for processing requests in a decoupled manner.
- **Ardalis.GuardClauses**: For simplifying argument checks in services.

### Controllers and Endpoints

- The application employs **Minimal APIs** within the `PublicApi` repository with several GET, POST, PUT, and DELETE endpoints for managing catalog items and types.

### Services

- Services like `BasketService`, `OrderService`, and various view model services provide the application's business logic.

### Authentication Mechanisms

- The application includes authentication mechanisms utilizing ASP.NET Core Identity in conjunction with JWT Bearer tokens for securing API endpoints. 

### Cross-Cutting Concerns

- Logger services and error handling are likely facilitated through Microsoft.Extensions.Logging and Microsoft.AspNetCore.Diagnostics frameworks respectively. 

### Integration Testing

- The project structure includes integration tests, indicating a focus on verifying the behavior of the complete application stack through libraries like `Microsoft.AspNetCore.Mvc.Testing` for testing API endpoints. 

This table provides a comprehensive outline of the technologies and frameworks utilized within the eShopOnWeb application.
