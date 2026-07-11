---
sidebar_position: 4
---

# Data, Storage & Messaging

## Databases
The following databases are detected in the application metadata:

- **AppIdentityDbContext**: This context is likely used for managing application identity and authentication-related data.
- **CatalogContext**: This context appears to be focused on handling data related to the product catalog.

## Data Access Approach
The application employs **Entity Framework Core** as its primary data access technology. This is evidenced by the presence of the `Microsoft.EntityFrameworkCore` package and specific database contexts.

## Entities / Domain Model
The application contains the following entities that comprise its domain model:

- **Address**
- **Basket**
- **BasketItem**
- **Buyer**
- **CatalogBrand**
- **CatalogItem**
- **CatalogItemDetails**
- **CatalogItemOrdered**
- **CatalogType**
- **Order**
- **OrderItem**
- **PaymentMethod**

Additional entities relevant to testing include:

- **BasketAddItem**
- **BasketRemoveEmptyItems**
- **BasketTotalItems**
- **OrderTotal**

## Caching
No caching mechanism was detected.

## Messaging & Queuing
No message queue or event bus was detected in the current metadata.
