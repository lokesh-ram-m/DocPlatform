---
sidebar_position: 5
---

# Security & Authentication

## Authentication
The application utilizes JWT Bearer authentication for securing API endpoints, as indicated by the inclusion of the `Microsoft.AspNetCore.Authentication.JwtBearer` package. This mechanism allows for secure token-based authentication, ensuring that users can access the application only after proper authentication.

## Authorization
All controllers in the application that manage user actions and access, such as the `ManageController` and `UserController`, are likely protected. However, specific authorization policies and their implementation details are not provided in the metadata.

## Secret & Password Handling
The application references `Microsoft.AspNetCore.Identity.EntityFrameworkCore` in the `Infrastructure` project, suggesting that it utilizes ASP.NET Core Identity for managing user accounts and passwords. However, no specific password handling algorithm such as BCrypt was detected in the metadata. The handling of sensitive information (e.g., JWT secrets) is not detailed either.

## Security-relevant Libraries
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` for user management.
- `Microsoft.AspNetCore.Authentication.JwtBearer` for JWT authentication.
- `Swashbuckle.AspNetCore` for API documentation and security through OpenAPI.
  
No additional security-relevant libraries were detected beyond these.
