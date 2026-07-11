---
sidebar_position: 3
---

# Infrastructure & Cloud

## Hosting & Runtime
The **eShopOnWeb** application comprises several projects, each serving distinct roles:

- **BlazorAdmin**: This is a .NET API project that runs as an ASP.NET Core service.
- **Infrastructure**: A .NET Library that aggregates shared utilities and services needed across the application.
- **PublicApi**: Also a .NET API project, this project implements a Minimal API structure in ASP.NET Core, allowing for streamlined interaction with its endpoints.
- **ApplicationCore**: A .NET Library housing the core business logic and entities.
- **BlazorShared**: A .NET Library shared between the Blazor components and the main application logic.
- **Web**: This is another .NET API project implementing the server-side functionalities for the web application.

The entire architecture is containerizable, allowing for deployment in various hosting environments, though specific hosting services are not detailed in the metadata.

## Cloud Services
The application demonstrates capabilities associated with cloud services through the Azure SDK (`Microsoft.VisualStudio.Azure.Containers.Tools.Targets`). However, there are no specific cloud services or platforms explicitly mentioned within the metadata. This indicates that the application is host-agnostic and is designed to be containerizable.

## Configuration
Configuration sources indicate the presence of various application settings likely managed through `appsettings.json` files and environment variables, although specific configurations are not detailed in the provided metadata.

## Deployment Considerations
Deployment considerations are derived from architecture and dependencies detected in the metadata. The application employs multiple projects that depend on one another, highlighting a layered architecture. Each component is designed to be loosely coupled, which eases the deployment process.

The use of ASP.NET Core Identity for authentication implies that secure deployment practices will be necessary. Additionally, certain projects reference SQL Server and Entity Framework Core, suggesting that database migration and initialization might be required during deployment. Nevertheless, detailed deployment mechanisms (such as CI/CD setups) are not provided in the metadata.
