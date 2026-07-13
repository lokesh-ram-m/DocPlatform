---
sidebar_position: 3
---

# Infrastructure & Cloud

## Hosting & Runtime
The application is built using ASP.NET Core and is designed as a containerizable solution. It employs .NET technologies that support various platforms, including Android, iOS, and macOS, ensuring a wide-ranging deployment capability across different environments. It leverages Minimal APIs for its controllers, which allows for lightweight and efficient endpoint definitions.

## Cloud Services
The following cloud capabilities have been detected:
- **Azure AI**
- **OpenAI / GitHub Models**
- **Azure SDK**

However, no specific cloud infrastructure or hosting environment is explicitly defined in the metadata, indicating that the application is host-agnostic and can be deployed on various cloud platforms or on-premises environments.

## Configuration
Configuration sources are not explicitly detailed in the metadata. There is an implication that configurations may utilize standard practices for ASP.NET Core applications, like `appsettings.json` or environment variables, but specific sources have not been identified.

## Deployment Considerations
Given the application's architecture as a set of .NET APIs, deployment considerations may include:
- Utilizing CI/CD pipelines due to the containerizable nature, allowing for seamless updates and rollbacks.
- Managing service dependencies effectively, particularly around the various APIs and services identified (e.g., EventBusRabbitMQ, Identity API, etc.).
- Handling security aspects via built-in authentication methods such as JWT Bearer authentication and OpenID Connect, which need to be configured correctly for secure access.
- Monitoring application performance and logging using built-in ASP.NET Core tools or through Azure capabilities, which may require specific setup.

Overall, while numerous potential deployment patterns can be inferred, specific deployment strategies or configurations are not detailed in the provided metadata.
