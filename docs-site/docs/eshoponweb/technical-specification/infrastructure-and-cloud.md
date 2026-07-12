---
sidebar_position: 3
---

# Infrastructure & Cloud

## Hosting & Runtime
The application **eShopOnWeb** is primarily built using **ASP.NET Core** technologies. It consists of multiple projects including **BlazorAdmin**, **Web**, **PublicApi**, and various libraries to support backend functionalities like **Infrastructure** and **ApplicationCore**. The architecture allows for **containerization**, making it suitable for deployment in various hosting environments. The components are designed to run as a centralized API service with a client-side Angular Single Page Application (SPA) through projects like **BlazorAdmin**.

## Cloud Services
The metadata indicates the presence of the **Azure SDK** associated with **Microsoft.VisualStudio.Azure.Containers.Tools.Targets**, signaling some capability to interact with Azure services. However, no specific cloud services (such as storage, databases, or computation) are explicitly detected from the provided metadata. Thus, the application architecture remains **host-agnostic/containerizable** and can be hosted on other cloud platforms or on-premises environments without dependency on a specific cloud infrastructure.

## Configuration
Configuration sources for the application may include standard ASP.NET Core options such as **appsettings.json** and environment variables. However, the metadata does not provide explicit details on these configuration sources; therefore, the specifics of configuration management are not documented.

## Deployment Considerations
Deployment strategies are not explicitly outlined in the metadata; however, the application can be expected to follow standard practices associated with deploying ASP.NET Core applications. This likely includes the use of containerization (e.g., Docker) given the referenced **Azure SDK**, and typical deployment pipelines could utilize CI/CD practices in cloud services or other container orchestration platforms. Furthermore, the authentication mechanism is set up via **ASP.NET Core Identity** and **JWT Bearer token authentication**, which should be considered during deployment for secure access management.
