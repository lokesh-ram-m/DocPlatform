---
sidebar_position: 5
---

# Security & Authentication

## Authentication
The application utilizes **JWT (JSON Web Tokens)** for authentication, as indicated by the presence of `Microsoft.AspNetCore.Authentication.JwtBearer` in the metadata. This mechanism allows for secure communication between the client and server by ensuring that only validated users can access specific parts of the application.

## Authorization
The documentation does not specify which controllers are explicitly protected, but the presence of authentication capabilities indicates that certain endpoints may require authentication to access. However, specific authorization rules and protected endpoints are not detailed.

## Secret & Password Handling
There is no direct mention of specific secret or password handling mechanisms such as BCrypt in the detected capabilities or metadata. The application does integrate with **ASP.NET Core Identity**, which provides user management and authentication capabilities, but specific information on password handling methods is not provided.

## Security-relevant Libraries
The following security-relevant libraries are detected in the application:
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` - Provides identity functionalities including user authentication and management.
- `Microsoft.AspNetCore.Authentication.JwtBearer` - Facilitates JWT authentication.
- `Microsoft.AspNetCore.Components.Authorization` - Provides authorization support for Blazor components.

Overall, while the application demonstrates a robust authentication method and incorporates standard libraries for identity management, details on specific protections and password handling are minimal.
