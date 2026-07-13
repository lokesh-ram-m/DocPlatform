---
sidebar_position: 6
---

# API Reference

### WebHooksApi (Minimal API)
**Base Route:** `/api/webhooks`

| Method | Action          | Endpoint           |
|--------|----------------|---------------------|
| GET    | Retrieve all   | `/`                 |
| GET    | Retrieve by ID | `/{id:int}`        |
| POST   | Create         | `/`                 |
| DELETE | Delete by ID   | `/{id:int}`        |

### CatalogApi (Minimal API)
**Base Route:** `api/catalog`

| Method | Action                      | Endpoint                                        |
|--------|----------------------------|--------------------------------------------------|
| GET    | Retrieve all items         | `/items`                                        |
| GET    | Retrieve items (duplicate) | `/items`                                        |
| GET    | Retrieve items by filters   | `/items/by`                                    |
| GET    | Retrieve by ID              | `/items/{id:int}`                              |
| GET    | Retrieve by name            | `/items/by/{name:minlength(1)}`               |
| GET    | Retrieve item picture       | `/items/{id:int}/pic`                          |
| GET    | Retrieve with relevance     | `/items/withsemanticrelevance/{text:minlength(1)}` |
| GET    | Retrieve items w/o params   | `/items/withsemanticrelevance`                  |
| GET    | Retrieve by type and brand  | `/items/type/{typeId}/brand/{brandId?}`       |
| GET    | Retrieve all by brand       | `/items/type/all/brand/{brandId:int?}`        |
| GET    | Retrieve catalog types      | `/catalogtypes`                                 |
| GET    | Retrieve catalog brands     | `/catalogbrands`                                |
| PUT    | Update item (no ID)        | `/items`                                        |
| PUT    | Update item by ID          | `/items/{id:int}`                              |
| POST   | Create item                | `/items`                                        |
| DELETE | Delete item by ID          | `/items/{id:int}`                              |

### OpenApi.Extensions (Minimal API)
**Base Route:** `/`

| Method | Action        | Endpoint |
|--------|---------------|----------|
| GET    | Get API info | `/`      |

### WebhookEndpoints (Minimal API)
**Base Route:** `webhook-received`

| Method | Action              | Endpoint              |
|--------|---------------------|-----------------------|
| POST   | Receive Webhook     | `/webhook-received`   |

### AuthenticationEndpoints (Minimal API)
**Base Route:** `/`

| Method | Action              | Endpoint              |
|--------|---------------------|-----------------------|
| POST   | User logout         | `/logout`             |

### AccountController
**Base Route:** `/`

| Method | Action               | Endpoint          |
|--------|----------------------|-------------------|
| GET    | Login page           | `Login`           |
| POST   | User login           | `Login`           |
| GET    | Logout page          | `Logout`          |
| POST   | User logout          | `Logout`          |
| GET    | Access Denied        | `AccessDenied`    |

### ExternalController
**Base Route:** `/`

| Method | Action               | Endpoint          |
|--------|----------------------|-------------------|
| GET    | Start external auth   | `Challenge`       |
| GET    | Callback handler      | `Callback`        |

### DeviceController
**Base Route:** `/`

| Method | Action               | Endpoint          |
|--------|----------------------|-------------------|
| GET    | Index                | `Index`           |
| POST   | User code capture    | `UserCodeCapture` |
| POST   | Callback             | `Callback`        |

### ConsentController
**Base Route:** `/`

| Method | Action               | Endpoint          |
|--------|----------------------|-------------------|
| GET    | Consent Index        | `Index`           |
| POST   | Submit Consent       | `Index`           |

### GrantsController
**Base Route:** `/`

| Method | Action               | Endpoint          |
|--------|----------------------|-------------------|
| GET    | Index of grants      | `Index`           |
| POST   | Revoke grant         | `Revoke`          |

### OrdersApi (Minimal API)
**Base Route:** `api/orders`

| Method | Action                     | Endpoint           |
|--------|----------------------------|---------------------|
| PUT    | Cancel order               | `/cancel`           |
| PUT    | Ship order                 | `/ship`             |
| GET    | Retrieve order by ID       | `{orderId:int}`     |
| GET    | Retrieve all orders        | `/`                 |
| GET    | Retrieve card types        | `/cardtypes`        |
| POST   | Create draft order         | `/draft`            |
| POST   | Create order               | `/`                 |
