---
sidebar_position: 2
---

# Architecture

## Detected Patterns
The architecture of the application likely follows several design patterns, including:
- **Layered Pattern**: The presence of services and interfaces suggests a separation between business logic and data access.
- **Dependency Injection (DI)**: The use of interfaces for services indicates that dependency injection may be employed to manage dependencies.
- **Repository Pattern**: The existence of repository interfaces such as `IProjectRepository`, `ITaskRepository`, and `IUserRepository` suggests a possible implementation of the repository pattern for data access abstraction.

## Backend Components
The backend of the application is structured around a .NET project titled **TaskManagementApi**, indicating it is built using ASP.NET Core targeting .NET 9.0. Its primary responsibilities include:
- **Authentication**: Managed through `AuthController`, allowing user login and registration.
- **Task Management**: Handled via `TaskController`, providing endpoints for CRUD operations on tasks.
- **User Management**: Facilitated by `UserController`, managing user data and deactivations.
- **Project Management**: Managed by `ProjectController`, allowing for project-related operations.
- **AI Interaction**: The `AgentController` suggests capabilities for chat-based interactions, potentially using the Semantic Kernel for AI.

Key services within the application include:
- `AuthService`
- `UserService`
- `ProjectService`
- `TaskService`
- `TaskAgentService`

## Frontend
The frontend component is developed using Angular and is organized into various components and routes:
- **Components**:
  - `assistant`
  - `login`
  - `navbar`
  - `project-list`
  - `register`
  - `task-list`

- **Routes**:
  - Root (`(root)`)
  - Wildcard route (`**`)
  - Specific routes for `assistant`, `login`, `projects`, `register`, and `tasks`

These components and routes suggest a user interface designed for managing tasks and projects, as well as user interactions.

## Data & Persistence
From the metadata, there is no explicit indication of `DbContext` classes; however, the presence of data packages such as **Dapper** and **Microsoft.Data.SqlClient** implies a potential use of direct SQL access methods or ORM functionalities for data manipulation. The application likely interacts with a SQL Server database, considering this metadata. The repository interfaces hint at managing entity instances such as `Project`, `Task`, and `User`, which reflects typical CRUD operations in a persistent storage context.
