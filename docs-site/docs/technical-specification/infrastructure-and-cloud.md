---
sidebar_position: 3
---

# Infrastructure & Cloud

## Hosting & Runtime
The **TaskFlow** application consists of two main components: the **TaskManagementBackend**, which is built as an ASP.NET Core service targeting .NET 9.0, and the **TaskManagementFrontend**, implemented as an Angular Single Page Application (SPA). Both components are containerizable, and thus can run in a variety of environments, including local, cloud, or on-premise hosting.

## Cloud Services
No specific cloud services have been detected for **TaskFlow**. It is host-agnostic and can operate effectively in containerized environments, ensuring flexibility in deployment options.

## Configuration
The application makes use of configuration sources, likely through `appsettings` and environment variables, as is common in ASP.NET Core applications. However, specific sources for configuration were not explicitly detailed in the extracted metadata.

## Deployment Considerations
The deployment of **TaskFlow** is grounded on typical practices found in .NET Core and Angular applications, employing best practices such as containerization for the backend and frontend components. However, specific deployment strategies, continuous integration/continuous deployment (CI/CD) tools, or environments (e.g., Kubernetes, Docker Swarm) were not detected and should be defined based on organizational standards or preferences.
