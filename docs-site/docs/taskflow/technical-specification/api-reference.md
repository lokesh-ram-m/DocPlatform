---
sidebar_position: 6
---

# API Reference

### ProjectController
**Base Route:** `/api/projects`

| Method | Action                             | Endpoint                       |
|--------|------------------------------------|-------------------------------|
| GET    | Retrieve all projects              | `/api/projects`               |
| GET    | Retrieve a project by ID           | `/api/projects/{id}`          |
| GET    | Retrieve projects by user ID       | `/api/projects/user/{userId}` |
| POST   | Create a new project               | `/api/projects`               |
| PUT    | Update a project by ID             | `/api/projects/{id}`          |
| DELETE | Delete a project by ID             | `/api/projects/{id}`          |

### UserController
**Base Route:** `/api/users`

| Method | Action                             | Endpoint                       |
|--------|------------------------------------|-------------------------------|
| GET    | Retrieve all users                 | `/api/users`                  |
| GET    | Retrieve a user by ID              | `/api/users/{id}`             |
| PUT    | Update a user by ID                | `/api/users/{id}`             |
| DELETE | Delete a user by ID                | `/api/users/{id}`             |

### AuthController
**Base Route:** `/api/auth`

| Method | Action                             | Endpoint                       |
|--------|------------------------------------|-------------------------------|
| POST   | Log in a user                      | `/api/auth/login`             |
| POST   | Register a new user                | `/api/auth/register`          |

### AgentController
**Base Route:** `/api/agent`

| Method | Action                             | Endpoint                       |
|--------|------------------------------------|-------------------------------|
| POST   | Send a chat message via agent      | `/api/agent/chat`             |

### TaskController
**Base Route:** `/api/tasks`

| Method | Action                             | Endpoint                       |
|--------|------------------------------------|-------------------------------|
| GET    | Retrieve all tasks                 | `/api/tasks`                  |
| GET    | Retrieve a task by ID              | `/api/tasks/{id}`             |
| POST   | Create a new task                  | `/api/tasks`                  |
| PUT    | Update a task by ID                | `/api/tasks/{id}`             |
| DELETE | Delete a task by ID                | `/api/tasks/{id}`             |
