---
sidebar_position: 2
---

# Architecture

## Detected Patterns
The architecture of the TaskFlow application likely follows several established patterns based on the metadata. There is evidence of the **Repository Pattern**, indicated by the presence of various interfaces such as `IProjectRepository`, `ITaskRepository`, and `IUserRepository`. Additionally, the presence of service interfaces (`IAuthService`, `IProjectService`, etc.) suggests a **Service Layer** pattern. The application appears to be structured in a **Layered Architecture**, based on the organization of controllers and services. Dependency Injection (DI) is also inferred through the design of interfaces and services, which is common in **Clean Architecture**.

## Backend Components
The backend is structured as a .NET API with the following main components:

- **TaskManagementApi**:
  - **Responsibilities**: This project handles the core API functionalities, including authentication, user management, project management, and task management.
  - **Controllers**:
    - `ProjectController`: Manages project CRUD operations.
    - `UserController`: Handles user-related actions.
    - `AuthController`: Manages user login and registration.
    - `AgentController`: Facilitates interaction with an agent (chat capabilities).
    - `TaskController`: Manages task-related operations.
  - **Services**: 
    - `AuthService`, `ProjectService`, `TaskService`, and `UserService`, which correspond to the controllers and are likely responsible for handling business logic and data interactions.

## Frontend
The frontend is developed as an Angular application, consisting of the following components and routing structure:

- **Components**:
  - `assistant`: Likely a component for assisting users in task or project management.
  - `login`: Used for user authentication.
  - `navbar`: A navigation bar for the application.
  - `project-list`: Displays a list of projects.
  - `register`: Handles user registration.
  - `task-list`: Displays a list of tasks.
  
- **Routes**:
  - Defined routes include home (`(root)`), a wildcard for unmatched routes (`**`), and specific paths for each major component such as `assistant`, `login`, `projects`, `register`, and `tasks`.

## Data & Persistence
Data persistence strategies suggest the use of **Dapper**, as indicated by the package references. However, there are no DbContexts explicitly defined in the provided metadata, which may imply the application leverages Dapper for direct SQL interactions instead of Entity Framework Core for ORM capabilities. The backend is designed to interact with SQL Server, as inferred from the package reference to `Microsoft.Data.SqlClient`.
