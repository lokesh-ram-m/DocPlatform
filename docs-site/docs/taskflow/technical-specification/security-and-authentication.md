---
sidebar_position: 5
---

# Security & Authentication

## Authentication
The application uses **JWT Bearer authentication** to manage authentication. This mechanism allows secure communication between the client and server by issuing a token upon successful login, which is then used for subsequent requests to verify user identity.

## Authorization
The exposed controllers include:
- `ProjectController`
- `UserController`
- `AuthController`
- `AgentController`
- `TaskController`

It is not explicitly stated which of these controllers are protected by authorization logic. Therefore, additional details on their security status regarding access and permissions are not provided.

## Secret & Password Handling
The application employs **BCrypt** for password hashing, which provides a secure method for managing user passwords. This technique helps protect user credentials from unauthorized access by hashing passwords before storage.

## Security-relevant Libraries
The notable libraries contributing to security in this application include:
- **BCrypt.Net-Next**: for hashing passwords securely.
- **Microsoft.AspNetCore.Authentication.JwtBearer**: for implementing JWT Bearer authentication.

No other security-relevant libraries were detected.
