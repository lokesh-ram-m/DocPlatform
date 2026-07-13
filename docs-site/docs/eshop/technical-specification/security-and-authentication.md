---
sidebar_position: 5
---

# Security & Authentication

## Authentication
The application utilizes several authentication schemes to manage user identity:

- **JWT Bearer**: A token-based authentication scheme that allows users to authenticate via JSON Web Tokens.
- **Cookie**: A traditional method of authentication using cookies to maintain user session states.
- **OpenID Connect**: An authentication layer on top of the OAuth 2.0 protocol, providing a way for users to authenticate via third-party providers.
- **ASP.NET Core Identity**: A membership system that adds login functionality to ASP.NET Core applications, managing users, passwords, and profile data.

## Authorization
The application does not define any specific authorization policies or roles. The following controllers have protection measures in place:

- **AllowAnonymous**:
  - `AccountController`
  - `ExternalController`
  - `HomeController`
  
- **Authorize**:
  - `DeviceController`
  - `DiagnosticsController`
  - `ConsentController`
  - `GrantsController`

This means that `AccountController`, `ExternalController`, and `HomeController` can be accessed by anyone without authentication. In contrast, `DeviceController`, `DiagnosticsController`, `ConsentController`, and `GrantsController` require users to be authenticated.

## Secret & Password Handling
No specific secret or password handling mechanisms such as encryptions like BCrypt were detected in the capabilities.

## Security-relevant Libraries
The application leverages ASP.NET Core for web security, but no additional security-libraries or patterns have been explicitly detected in the metadata.
