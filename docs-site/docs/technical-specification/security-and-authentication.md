---
sidebar_position: 5
---

# Security & Authentication

## Authentication
The application employs **JWT Bearer authentication** for user authentication. This mechanism allows users to log in and receive a secure token, which is subsequently used for authenticating subsequent requests.

## Authorization
The specific authorization mechanisms used are not detailed in the metadata. However, it is noted that the application has controllers, which typically implies some level of access control. The following controllers manage user and project functionalities, but it is unclear if any of them are protected by authentication mechanisms:

- `ProjectController`
- `UserController`
- `AuthController`
- `AgentController`
- `TaskController`

## Secret & Password Handling
Password handling is facilitated by **BCrypt for password hashing**, ensuring that user passwords are stored securely.

## Security-relevant Libraries
The following libraries play a role in the security architecture of the application:

- **BCrypt.Net-Next**: For password hashing.
- **Microsoft.AspNetCore.Authentication.JwtBearer**: For handling JWT authentication.

No additional security-relevant libraries were detected beyond these.
