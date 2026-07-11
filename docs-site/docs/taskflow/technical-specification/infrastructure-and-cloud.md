---
sidebar_position: 3
---

# Infrastructure & Cloud

## Hosting & Runtime
The application, **TaskFlow**, is structured into two main components that utilize contemporary technologies:

1. **Backend (TaskManagementBackend)**: The backend is implemented as a .NET API targeting the .NET 9.0 framework. It runs as an ASP.NET Core service which allows it to be hosted in various environments, including on-premises or in the cloud. The backend is composed of various controllers that facilitate communication via RESTful endpoints.

2. **Frontend (TaskManagementFrontend)**: The frontend is built as an Angular Single Page Application (SPA). It is designed to be containerizable, allowing for flexible deployment options.

## Cloud Services
No specific cloud services were detected in the metadata. The application is host-agnostic and compatible with containerized deployments.

## Configuration
Configuration sources indicated include:
- **JWT Authentication**: The backend uses JWT Bearer tokens for authentication, suggesting environment configuration for secret keys and token settings may be required.
- Configurations can typically be managed through appsettings files or environment variables, though no specific sources were detailed in the metadata.

## Deployment Considerations
Deployment considerations for **TaskFlow** include:
- The backend and frontend can be deployed independently, with the frontend calling the backend API.
- Given that the application is built using .NET and Angular, standard practices for deploying .NET applications and SPAs apply.
- It may be beneficial to use a web server or reverse proxy for production environments, especially for routing to the API and serving the frontend application.
- The absence of details on message queues or specific cloud storage solutions indicates a straightforward deployment without complex dependencies.
