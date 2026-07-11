---
sidebar_position: 6
---

# API Reference

### ProjectController
Base Route: `api/projects`
| Method | Action           | Endpoint          |
|--------|------------------|--------------------|
| GET    | GetAll           | /projects          |
| GET    | GetById          | /projects/{id}     |
| GET    | GetByUserId      | /projects/user/{userId} |
| POST   | Add              | /projects          |
| PUT    | Update           | /projects/{id}     |
| DELETE | Delete           | /projects/{id}     |

### UserController
Base Route: `api/users`
| Method | Action         | Endpoint        |
|--------|----------------|------------------|
| GET    | GetAll         | /users          |
| GET    | GetById        | /users/{id}     |
| PUT    | Update         | /users/{id}     |
| DELETE | Deactivate     | /users/{id}     |

### AuthController
Base Route: `api/auth`
| Method | Action   | Endpoint     |
|--------|----------|---------------|
| POST   | Login    | /login        |
| POST   | Register | /register     |

### AgentController
Base Route: `api/agent`
| Method | Action | Endpoint     |
|--------|--------|---------------|
| POST   | Chat   | /chat        |

### TaskController
Base Route: `api/tasks`
| Method | Action           | Endpoint          |
|--------|------------------|--------------------|
| GET    | GetAll           | /tasks             |
| GET    | GetById          | /tasks/{id}        |
| POST   | Add              | /tasks             |
| PUT    | Update           | /tasks/{id}        |
| DELETE | Delete           | /tasks/{id}        |
