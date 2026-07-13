---
sidebar_position: 6
---

# API Reference

### WebHooksApi (Minimal API)
**Base Route:** `/api/webhooks`

| Method | Action                             | Endpoint                      |
|--------|------------------------------------|-------------------------------|
| GET    | Get all webhooks                  | `/api/webhooks`               |
| GET    | Get a webhook by ID               | `/api/webhooks/{id:int}`      |
| POST   | Create a new webhook               | `/api/webhooks`               |
| DELETE | Delete a webhook by ID            | `/api/webhooks/{id:int}`      |

### CatalogApi (Minimal API)
**Base Route:** `/api/catalog`

| Method | Action                              | Endpoint                                       |
|--------|-------------------------------------|------------------------------------------------|
| GET    | Get all catalog items               | `/api/catalog/items`                           |
| GET    | Get catalog items by a specific criteria | `/api/catalog/items/by`                    |
| GET    | Get a catalog item by ID            | `/api/catalog/items/{id:int}`                 |
| GET    | Get catalog items by name           | `/api/catalog/items/by/{name:minlength(1)}`  |
| GET    | Get an image for a catalog item     | `/api/catalog/items/{id:int}/pic`            |
| GET    | Get items with semantic relevance    | `/api/catalog/items/withsemanticrelevance/{text:minlength(1)}` |
| GET    | Get all items with semantic relevance | `/api/catalog/items/withsemanticrelevance`   |
| GET    | Get items by type and brand         | `/api/catalog/items/type/{typeId}/brand/{brandId?}` |
| GET    | Get all items of a specific type and brand | `/api/catalog/items/type/all/brand/{brandId:int?}` |
| GET    | Get all catalog types                | `/api/catalog/catalogtypes`                   |
| GET    | Get all catalog brands               | `/api/catalog/catalogbrands`                  |
| PUT    | Update catalog items                 | `/api/catalog/items`                           |
| PUT    | Update a specific catalog item       | `/api/catalog/items/{id:int}`                 |
| POST   | Create a new catalog item            | `/api/catalog/items`                           |
| DELETE | Delete a catalog item by ID          | `/api/catalog/items/{id:int}`                 |

### OpenApi.Extensions (Minimal API)
**Base Route:** `/`

| Method | Action                              | Endpoint     |
|--------|-------------------------------------|--------------|
| GET    | Get Open API info                   | `/`          |

### WebhookEndpoints (Minimal API)
**Base Route:** `/`

| Method | Action                              | Endpoint          |
|--------|-------------------------------------|-------------------|
| POST   | Receive webhook                     | `/webhook-received` |

### AuthenticationEndpoints (Minimal API)
**Base Route:** `/`

| Method | Action                              | Endpoint    |
|--------|-------------------------------------|-------------|
| POST   | Logout                              | `/logout`   |

### AccountController
**Base Route:** `/`

| Method | Action                              | Endpoint    |
|--------|-------------------------------------|-------------|
| GET    | Get account information             | `/`         |
| POST   | Create or update account            | `/`         |
| GET    | Get account information             | `/`         |
| POST   | Create or update account            | `/`         |
| GET    | Get account information             | `/`         |

### ExternalController
**Base Route:** `/`

| Method | Action                              | Endpoint    |
|--------|-------------------------------------|-------------|
| GET    | Get external resources              | `/`         |
| GET    | Get external resources              | `/`         |

### DeviceController
**Base Route:** `/`

| Method | Action                              | Endpoint    |
|--------|-------------------------------------|-------------|
| GET    | Get device information              | `/`         |
| POST   | Register a new device               | `/`         |
| POST   | Another device action               | `/`         |

### ConsentController
**Base Route:** `/`

| Method | Action                              | Endpoint    |
|--------|-------------------------------------|-------------|
| GET    | Get consent information             | `/`         |
| POST   | Provide consent                     | `/`         |

### GrantsController
**Base Route:** `/`

| Method | Action                              | Endpoint    |
|--------|-------------------------------------|-------------|
| GET    | Get grant information               | `/`         |
| POST   | Request a new grant                 | `/`         |

### OrdersApi (Minimal API)
**Base Route:** `/api/orders`

| Method | Action                              | Endpoint                    |
|--------|-------------------------------------|-----------------------------|
| PUT    | Cancel an order                     | `/api/orders/cancel`       |
| PUT    | Ship an order                       | `/api/orders/ship`         |
| GET    | Get order by ID                    | `/api/orders/{orderId:int}` |
| GET    | Get all orders                      | `/api/orders`              |
| GET    | Get available card types            | `/api/orders/cardtypes`    |
| POST   | Create a draft order                | `/api/orders/draft`        |
| POST   | Create a new order                  | `/api/orders`              |
