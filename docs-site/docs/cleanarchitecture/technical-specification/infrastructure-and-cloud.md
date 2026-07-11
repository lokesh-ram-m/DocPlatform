---
sidebar_position: 3
---

# Infrastructure & Cloud

## Hosting & Runtime
The application **CleanArchitecture** is built using the ASP.NET Core framework and utilizes Angular for its frontend components. The backend is structured primarily as a set of .NET Class Libraries targeting `.NET 10.0`. The application components are designed to be containerizable, making it suitable for modern deployment practices, including microservices and cloud-based hosting environments.

## Cloud Services
The application has detected capabilities to integrate with Azure cloud services through the Azure SDK. This includes various Azure hosting options and database services such as Azure PostgreSQL. However, no specific cloud services have been noted beyond this general integration, and therefore, the application can be considered host-agnostic/containerizable. 

## Configuration
Configuration sources include references to environment variables and standard `appsettings` practices, although no specific configuration sources are explicitly detailed within the metadata. The choice of using ASP.NET Core Identity indicates there may be options for managing user authentication and related settings.

## Deployment Considerations
Deployment configurations are not directly specified in the metadata; however, the application leverages standard practices evident in its infrastructure. It uses container-friendly technologies like ASP.NET Core and has libraries for resilience and diagnostics, suggesting that careful attention should be paid to factors such as scaling, network configurations, and service discovery when deploying the application.
