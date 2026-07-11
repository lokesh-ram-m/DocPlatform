---
sidebar_position: 3
---

# Infrastructure & Cloud

## Hosting & Runtime
The application **eShopOnWeb** consists of various projects using ASP.NET Core for backend services and a Blazor framework for the admin interface. The following components are identified:

- **BlazorAdmin**: This is a .NET API that interacts with the core application for administrative purposes.
- **PublicApi**: Another .NET API, likely serving as the main interface for client interactions.
- **Web**: A .NET API which facilitates additional web-related functionalities and manages user interactions.

The architecture is containerizable, allowing deployment in environments like Docker or other orchestration services.

## Cloud Services
No dedicated cloud services were detected within the application. Instead, the application is designed to be host-agnostic and can run independently in various cloud environments or on-premises. However, there are references to Azure SDK components, indicating possible compatibility for Azure deployments without specifics on cloud services used.

## Configuration
Configuration details appear to be sourced from several potential locations including `appsettings.json` and environment variables, although specific configuration files or environmental setup were not explicitly detailed in the metadata.

## Deployment Considerations
The project utilizes several technologies such as:
- **Entity Framework Core** for data access,
- **JWT Authentication** for secure API access,
- **ASP.NET Core Identity** for user management,
- **OpenAPI / Swagger** for API documentation and exploration.

Deployment processes are grounded on typical standards for ASP.NET applications but are not elaborated upon in detail. As container-ready and built with extensibility in mind, considerations might include:
- Environment-specific settings,
- Infrastructure scaling based on expected load,
- Continuous integration/continuous deployment (CI/CD) practices are advisable but not specified.

It is recommended to further establish the deployment pipeline based on best practices for modern cloud applications, considering potential use of Azure DevOps or GitHub Actions for streamlining deployment to various environments.
