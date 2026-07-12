---
sidebar_position: 4
---

# Data, Storage & Messaging

## Databases
The application utilizes the following databases through Entity Framework Core:

- **AppIdentityDbContext**: This context is responsible for managing identity-related entities.
- **CatalogContext**: This context manages catalog-related entities.

Both contexts are established in the `Infrastructure` project, which depends on the `ApplicationCore` project for entity definitions.

## Data Access Approach
The application employs **Entity Framework Core** as the data access approach, as indicated by the use of the library `Ardalis.Specification.EntityFrameworkCore`. This approach allows for the implementation of the Repository and Unit of Work patterns, simplifying the handling of data access logic.

## Entities / Domain Model
The application includes the following entities defined in the `ApplicationCore` project:

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

Additionally, the `BlazorShared` project includes:

- **CatalogBrand**
- **CatalogItem**
- **CatalogType**
- **ErrorDetails**
- **LookupData**

Entities like **BasketAddItem**, **BasketRemoveEmptyItems**, **BasketTotalItems**, and **OrderTotal** are defined in the `UnitTests` project, though they may serve specific testing purposes.

## Caching
None detected.

## Messaging & Queuing
No message queue or event bus was detected in the current metadata.
