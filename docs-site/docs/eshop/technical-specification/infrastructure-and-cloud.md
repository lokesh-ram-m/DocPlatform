---
sidebar_position: 3
---

# Infrastructure & Cloud

## Hosting & Runtime
The application `eShop` is composed of multiple components and libraries, primarily leveraging the ASP.NET Core framework for the API services, with projects such as `Webhooks.API`, `Catalog.API`, `Ordering.API`, `Identity.API`, `PaymentProcessor`, and `Basket.API`. Each of these components is designed to run on the .NET 10.0 framework. The application also includes Angular Single Page Applications (SPAs) within the `ClientApp` and `HybridApp` projects, which cater to different platforms including Android, iOS, and macOS Catalyst. The entire solution appears to be containerizable due to its modular structure and dependency on .NET technologies.

## Cloud Services
The application makes use of several detected cloud capabilities including:
- **Azure AI:** Integrating Azure's AI offerings for enhanced functionalities.
- **OpenAI / GitHub Models:** Utilizing advanced AI models for various processing tasks.
- **Azure SDK:** Enabling interactions with Azure cloud services.

Additionally, the application is capable of utilizing the following services:
- **Redis:** For caching purposes.
- **RabbitMQ:** Implemented for messaging between services.

No specific cloud services infrastructure was outlined, indicating it is host-agnostic and containerizable.

## Configuration
Configuration sources for the application include typical .NET conventions such as:
- **appsettings.json**
- **Environment Variables**

These configurations likely incorporate settings for database connections, service endpoints, and other operational parameters, although specifics were not detailed in the provided metadata.

## Deployment Considerations
The deployment of `eShop` is likely feasible across various environments due to its cross-platform capabilities and modular architecture. The components can be deployed independently as microservices, which is consistent with patterns often observed in modern cloud-native applications. However, specific deployment strategies and CI/CD pipelines were not specified within the metadata, and thus should be established based on operational requirements and infrastructure capabilities. The detected use of RabbitMQ suggests that service communication will require appropriate handling of message configurations.
