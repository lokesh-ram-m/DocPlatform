---
sidebar_position: 2
---

# Technology Stack

| Concern                    | Technology                                             | Details                                                  |
|---------------------------|-------------------------------------------------------|----------------------------------------------------------|
| **Runtime & Frameworks**  | .NET (net9.0)                                        | Target framework for backend.                            |
|                           | ASP.NET Core                                          | Web framework for building the API.                     |
|                           | Angular                                               | Framework for building the frontend.                     |
| **Data Access**           | Dapper                                                | Micro ORM used for data access.                          |
| **Database**              | SQL Server                                            | Database technology utilized for data storage.           |
|                           | Microsoft.Data.SqlClient                             | ADO.NET provider for SQL Server.                         |
| **Authentication**        | JWT Authentication                                   | Utilizes JSON Web Tokens for secure authentication.      |
|                           | Microsoft.AspNetCore.Authentication.JwtBearer        | Middleware for JWT Bearer authentication in ASP.NET.    |
| **API Documentation**     | OpenAPI                                              | API documentation standard enabled via ASP.NET.         |
|                           | Microsoft.AspNetCore.OpenApi                         | ASP.NET library for OpenAPI support.                     |
|                           | Swagger / Swashbuckle                                | Library for generating Swagger UI documentation.         |
| **AI/LLM**                | Semantic Kernel                                       | Framework for AI capabilities integrated from Microsoft.  |
|                           | Microsoft.SemanticKernel                              | Library facilitating semantic processing and AI features. |
|                           | Microsoft.SemanticKernel.Connectors.AzureAIInference | Connector for Azure AI inferences.                       |
| **Cross-Cutting Libraries**| BCrypt.Net-Next                                     | Library for password hashing and security.               |
