---
sidebar_position: 3
---

# HTTP API Documentation

## TaskManagementApi

### ProjectController
**Base Route:** `api/projects`

| Method | Action          | Endpoint               |
|--------|-----------------|------------------------|
| GET    | GetAll         | `/api/projects`        |
| GET    | GetById        | `/api/projects/{id}`   |
| GET    | GetByUserId    | `/api/projects/user/{userId}` |
| POST   | Add            | `/api/projects`        |
| PUT    | Update         | `/api/projects/{id}`   |
| DELETE | Delete         | `/api/projects/{id}`   |

### UserController
**Base Route:** `api/users`

| Method | Action          | Endpoint               |
|--------|-----------------|------------------------|
| GET    | GetAll         | `/api/users`           |
| GET    | GetById        | `/api/users/{id}`      |
| PUT    | Update         | `/api/users/{id}`      |
| DELETE | Deactivate      | `/api/users/{id}`      |

### AuthController
**Base Route:** `api/auth`

| Method | Action          | Endpoint               |
|--------|-----------------|------------------------|
| POST   | Login          | `/api/auth/login`      |
| POST   | Register       | `/api/auth/register`   |

### AgentController
**Base Route:** `api/agent`

| Method | Action          | Endpoint               |
|--------|-----------------|------------------------|
| POST   | Chat           | `/api/agent/chat`      |

### TaskController
**Base Route:** `api/tasks`

| Method | Action          | Endpoint               |
|--------|-----------------|------------------------|
| GET    | GetAll         | `/api/tasks`           |
| GET    | GetById        | `/api/tasks/{id}`      |
| POST   | Add            | `/api/tasks`           |
| PUT    | Update         | `/api/tasks/{id}`      |
| DELETE | Delete         | `/api/tasks/{id}`      |
