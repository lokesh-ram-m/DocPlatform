---
sidebar_position: 1
---

# Architecture

## Detected Patterns
The architecture of the TaskFlow application likely follows a **Repository Pattern** with a focus on separation of concerns across different layers. It exhibits characteristics of **Layered Architecture** and may embrace a **Clean Architecture** approach given the presence of interfaces and services.

## Solution Structure
The TaskFlow application consists of two main repositories:

1. **TaskManagementBackend**
   - **Project**: TaskManagementApi
     - **Responsibilities**: This is a .NET API backend that manages tasks, projects, and user authentication. It serves as the main interface for client applications to interact with the task management functionalities.
     - **Key Components**:
       - Controllers: Handle incoming API requests for projects, users, authentication, and tasks.
       - Services: Implement business logic for authentication, projects, tasks, and users.

2. **TaskManagementFrontend**
   - **Project**: task-management-frontend
     - **Responsibilities**: This is an Angular frontend application that provides a user interface for the task management system. It allows users to interact with projects and tasks through various components and routing.
     - **Key Components**:
       - Angular Components: Each component is responsible for rendering parts of the user interface, such as login, project lists, and task lists.

## Component Responsibilities

### TaskManagementBackend
- **Controllers**:
  - **ProjectController**: Manages project-related API endpoints for retrieval, addition, update, and deletion of projects.
  - **UserController**: Manages user-related API operations, such as retrieving and updating user details and deactivating users.
  - **AuthController**: Handles user authentication, providing login and registration endpoints.
  - **AgentController**: Facilitates interaction with an AI agent for chat-based operations.
  - **TaskController**: Manages tasks with endpoints for retrieval, addition, update, and deletion.

- **Services**:
  - **AuthService**: Handles authentication logic.
  - **ProjectService**: Manages project data and functionalities.
  - **TaskService**: Manages task data and functionalities.
  - **UserService**: Manages user data and functionalities.
  - **TaskAgentService**: (?! functionality not explicitly defined in the metadata)

### TaskManagementFrontend
- **Components**:
  - **assistant**: Provides an interface for interacting with the AI assistant.
  - **login**: Handles user authentication and login logic.
  - **navbar**: Displays navigation options throughout the app.
  - **project-list**: Renders a list of projects for user interaction.
  - **register**: Manages user registration.
  - **task-list**: Displays tasks associated with projects.

## How the Pieces Fit Together
- **Frontend** (Angular):
  - Users interact with the Angular application, which communicates with the backend API through defined routes.

- **API** (TaskManagementApi):
  - The Angular frontend sends requests to its designated API endpoints to perform operations such as user login, project additions, task management, etc.

- **Data Flow**:
  - Requests from the frontend (e.g., to add a task) are routed to the appropriate controller in the API (e.g., TaskController).
  - The controller invokes the corresponding service (e.g., TaskService) to handle business logic and interact with the database (using Dapper for data access).
  - The response is sent back through the API to the frontend, where it updates the user interface accordingly.
