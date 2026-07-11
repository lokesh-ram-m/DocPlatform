---
sidebar_position: 6
---

# API Reference

### EndpointRouteBuilderExtensions (Minimal API)
Base Route: N/A  
| Method | Action | Endpoint |
|--------|--------|----------|
| GET    | pattern | /{pattern} |
| POST   | pattern | /{pattern} |
| PUT    | pattern | /{pattern} |
| PATCH  | pattern | /{pattern} |
| DELETE | pattern | /{pattern} |

### TodoItems (Minimal API)
Base Route: N/A  
| Method | Action               | Endpoint       |
|--------|---------------------|-----------------|
| POST   | CreateTodoItem      | /TodoItems      |
| PUT    | {id}                | /TodoItems/{id} |
| PATCH  | UpdateDetail/{id}   | /TodoItems/UpdateDetail/{id} |
| DELETE | {id}                | /TodoItems/{id} |

### Users (Minimal API)
Base Route: N/A  
| Method | Action | Endpoint   |
|--------|--------|------------|
| POST   | logout | /Users/logout |

### WeatherForecasts (Minimal API)
Base Route: N/A  
| Method | Action               | Endpoint                  |
|--------|---------------------|---------------------------|
| GET    | GetWeatherForecasts  | /WeatherForecasts         |

### TodoLists (Minimal API)
Base Route: N/A  
| Method | Action          | Endpoint       |
|--------|----------------|-----------------|
| GET    | GetTodoLists   | /TodoLists      |
| POST   | CreateTodoList | /TodoLists      |
| PUT    | {id}           | /TodoLists/{id} |
| DELETE | {id}           | /TodoLists/{id} |
