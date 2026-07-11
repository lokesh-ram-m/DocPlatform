---
sidebar_position: 3
---

# Infrastructure & Cloud

## Hosting & Runtime
The application consists of multiple components that are designed to run in a .NET environment. The key components include:

- **ASP.NET Core Services**: The main web API is implemented using ASP.NET Core, specifically with the `Web` project which handles incoming HTTP requests routed through minimal APIs.
- **Angular SPA**: The client-side is built as a Single Page Application (SPA) using Angular. The Angular project (`cleanarchitecture.web`) hosts various components and routes for user interaction.
- **Containerizable**: The application is structured in a way that allows for containerization, facilitating deployment in cloud environments or on-premises.

## Cloud Services
Detected cloud capabilities include:
- **Azure SDK**: The application has integrated support for Azure services through `Aspire.Hosting.Azure.AppContainers`. 

No other specific cloud services were detected. The application's architecture appears to be host-agnostic and can run in a variety of environments due to its containerizable nature.

## Configuration
Configuration sources available in the application include:
- **Environment Variables**: These are likely utilized to manage different app settings across various environments.
- **appsettings.json**: Not explicitly mentioned in the metadata, but commonly used with .NET applications for configuration management.

There are no specific details on configuration sources beyond indicating the use of standard practices common in .NET applications.

## Deployment Considerations
The application likely supports various deployment scenarios considering its modular design and support for containerization. However, details on specific deployment methods (e.g., CI/CD pipelines, Azure App Services deployment specifics) were not found in the metadata.

It can be inferred that:
- The application can be hosted in Azure or any other compliant environment that supports ASP.NET Core and containers.
- The deployment may need to involve setting up database connections according to the detected PostgreSQL capabilities.

Additional operational concerns, such as scaling and monitoring, would depend on the specific hosting environment and configurations chosen during deployment.
