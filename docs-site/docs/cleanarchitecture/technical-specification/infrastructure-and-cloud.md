---
sidebar_position: 3
---

# Infrastructure & Cloud

## Hosting & Runtime
The application is built on the ASP.NET Core framework and features a .NET library architecture that includes multiple projects such as `Domain`, `Infrastructure`, `Application`, and `Web`. The `Web` project includes APIs implemented as Minimal APIs, allowing streamlined endpoints for interfacing with its functionalities (e.g., managing todo items and retrieving weather forecasts). Additionally, there is an Angular single-page application (SPA) that serves as the client-side interface, which is containerizable.

## Cloud Services
Detected cloud capabilities include:
- Azure SDK via `Aspire.Hosting.Azure.AppContainers`, which suggests potential deployment capabilities to Azure cloud environments.
- Azure PostgreSQL as a managed database service, indicating support for PostgreSQL databases hosted in Azure.

However, explicit instances of cloud services are not detailed, and overall, the application remains host-agnostic and can be containerized.

## Configuration
Configuration sources are not explicitly detailed in the metadata. However, standard practices for .NET applications suggest that configuration settings can likely be sourced from `appsettings.json` files and environment variables based on the ASP.NET Core configuration paradigm. Additional configuration may be integrated through Azure services as part of the detected Azure capabilities.

## Deployment Considerations
Deployment strategies are likely flexible due to the containerizable architecture. There is a reliance on a standard web hosting approach in combination with Azure cloud capabilities for ease of deployment. The specific deployment details and environments, both for development and production, have not been explicitly detailed in the available metadata. Thus, specifics regarding CI/CD pipelines or container orchestration (e.g., Kubernetes, Docker Swarm) are not covered and remain unspecified.
