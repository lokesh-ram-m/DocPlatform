---
sidebar_position: 6
---

# API Reference

### EndpointRouteBuilderExtensions (Minimal API)
| Method | Action      | Endpoint    |
|--------|-------------|-------------|
| GET    | pattern     | /           |
| POST   | pattern     | /           |
| PUT    | pattern     | /           |
| PATCH  | pattern     | /           |
| DELETE | pattern     | /           |

### TodoItems (Minimal API)
| Method | Action             | Endpoint         |
|--------|--------------------|------------------|
| POST   | CreateTodoItem     | /todoitems       |
| PUT    | {id}               | /todoitems/{id}  |
| PATCH  | UpdateDetail/{id}  | /todoitems/{id}/details |
| DELETE | {id}               | /todoitems/{id}  |

### Users (Minimal API)
| Method | Action   | Endpoint     |
|--------|----------|--------------|
| POST   | logout   | /users/logout |

### WeatherForecasts (Minimal API)
| Method | Action               | Endpoint                 |
|--------|----------------------|--------------------------|
| GET    | GetWeatherForecasts   | /weatherforecast         |

### TodoLists (Minimal API)
| Method | Action           | Endpoint        |
|--------|------------------|-----------------|
| GET    | GetTodoLists     | /todolists      |
| POST   | CreateTodoList   | /todolists      |
| PUT    | {id}             | /todolists/{id} |
| DELETE | {id}             | /todolists/{id} |
