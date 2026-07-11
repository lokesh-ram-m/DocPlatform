---
sidebar_position: 1
---

# Architecture

## Detected Patterns
The architecture of **TaskFlow** likely follows a **Layered Architecture** pattern, focusing on separation of concerns with distinct layers for controllers, services, and data access. It appears to utilize **Dependency Injection** (DI) to manage dependencies, promoting loose coupling between components. The presence of interfaces and services suggests a design that aligns with **Clean Architecture principles**.

## Solution Structure
The **TaskFlow** application consists of two primary repositories:

1. **TaskManagementBackend**
   - **Project: TaskManagementApi**
     - **Responsibilities**: Provides a RESTful API to manage tasks, projects, and user authentication.
     - **Controllers**: Implements CRUD operations for projects and tasks, alongside user registration and authentication.
     - **Services**: Contains business logic for handling tasks, projects, and user authentication via various services (e.g., AuthService, ProjectService).
   
2. **TaskManagementFrontend**
   - **Project: task-management-frontend**
     - **Responsibilities**: Implements the user interface for the task management application using Angular. It handles user interactions and routing.

## Component Responsibilities
- **Controllers in TaskManagementApi**:
  - **ProjectController**: Manages project-related operations (create, read, update, delete).
  - **UserController**: Handles user management, including user information retrieval and deactivation.
  - **AuthController**: Manages user authentication (login and registration).
  - **AgentController**: Facilitates interactions with an AI agent via chat.
  - **TaskController**: Oversees task-related operations (create, read, update, delete).

- **Services in TaskManagementApi**:
  - **AuthService**: Implements authentication logic.
  - **ProjectService**: Handles business logic related to projects.
  - **TaskService**: Manages operations for task entities.
  - **UserService**: Responsible for user-related operations.
  - **TaskAgentService**: Interacts with the AI functionalities.

- **Angular Components in task-management-frontend**:
  - Components for user interface elements such as login, registration, project and task lists, and an assistant feature.

## How the Pieces Fit Together
The interaction between different components of the **TaskFlow** application follows the typical frontend-backend architecture:

- **Frontend (Angular)**:
  - Users interact with the application via the Angular components (e.g., login, project list).
  - Angular routes manage navigation between different views, connecting to the appropriate components.

- **API (TaskManagementApi)**:
  - The frontend communicates with the backend through RESTful API endpoints defined in the controllers (e.g., `api/projects`, `api/users`).
  - Authentication is managed by the AuthController, utilizing JWT authentication for secure user sessions.

- **Data Flow**:
  - Upon a user action in the frontend, an HTTP request is sent to the respective controller actions in the backend API.
  - The backend processes the request using appropriate services, accessing business logic and data handling as necessary.
  - Responses are sent back to the frontend for display or further interactions by the user.
