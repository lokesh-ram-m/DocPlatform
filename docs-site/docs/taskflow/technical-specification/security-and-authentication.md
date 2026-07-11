---
sidebar_position: 5
---

# Security & Authentication

## Authentication
The application utilizes JWT Bearer authentication for secure user authentication. This mechanism allows users to log in and receive a token that can be used to access protected resources.

## Authorization
The metadata does not explicitly indicate whether any controllers are protected by authorization mechanisms. Additional specifics on controller-level security may be necessary to clarify this aspect.

## Secret & Password Handling
The application employs BCrypt for password hashing, ensuring secure storage of user credentials.

## Security-relevant Libraries
The following security-related libraries are detected in the application:
- **BCrypt.Net-Next**: Used for password hashing.
- **Microsoft.AspNetCore.Authentication.JwtBearer**: Implemented for JWT Bearer authentication.

Overall, the security handling appears robust, with focus on secure authentication and password management. However, there are no details regarding specific protected routes or authorization enforcement at the controller level.
