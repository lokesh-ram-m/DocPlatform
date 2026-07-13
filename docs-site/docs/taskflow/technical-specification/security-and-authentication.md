---
sidebar_position: 5
---

# Security & Authentication

## Authentication
The application employs **JWT Bearer authentication**. This mechanism is designed to allow users to securely log in and maintain authenticated sessions throughout the application.

## Authorization
The controllers within the `TaskManagementApi` are protected, ensuring that only authenticated users can access sensitive endpoints. The specific controllers include:

- **AuthController**: Handles authentication operations such as login and registration.
- **UserController**: Manages user-related actions which require authentication.
- **ProjectController**: Controls access to project data, restricted to authenticated users.
- **TaskController**: Provides access to task management features, also requiring authentication.

## Secret & Password Handling
The application utilizes **BCrypt** for password hashing, a strong and secure way to handle user passwords. This ensures that passwords are hashed and stored securely, mitigating the risks associated with password breaches.

## Security-relevant Libraries
The application integrates several security-related libraries:

- **JWT Authentication**: Used for handling authentication tokens.
- **BCrypt**: Employed for secure password hashing.

Overall, the application's security features are robust, with a strong authentication mechanism and secure password handling.
