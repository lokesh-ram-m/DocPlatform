---
sidebar_position: 5
---

# Security & Authentication

## Authentication
The application employs **JWT Bearer authentication**, leveraging ASP.NET Core Identity for managing user authentication and authorization capabilities.

## Authorization
Controllers have been explicitly defined in the application. However, the metadata does not provide specific information regarding which controllers are protected or the level of access control enforced. Therefore, it is unclear how various controllers are secured.

## Secret & Password Handling
No specific mechanisms for secret or password handling (e.g., BCrypt) were detected in the provided metadata.

## Security-relevant Libraries
The application utilizes the following security-relevant libraries:
- **Microsoft.AspNetCore.Authentication.JwtBearer** for JWT authentication.
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore** for managing identity and user accounts within the application.

Other than these, minimal other libraries specifically pertaining to security were detected.
