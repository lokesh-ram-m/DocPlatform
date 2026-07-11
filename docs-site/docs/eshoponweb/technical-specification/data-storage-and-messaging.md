---
sidebar_position: 4
---

# Data, Storage & Messaging

## Databases
The application utilizes two database contexts:
- **AppIdentityDbContext**: This context is likely designed to handle identity-related data, leveraging ASP.NET Core Identity.
- **CatalogContext**: This context is likely used for managing catalog-related data.

Both contexts are part of the **Infrastructure** project and are configured to support EF Core.

## Data Access Approach
The application employs **Entity Framework Core** (EF Core) for data access. This technology provides a robust framework for working with relational data and is included in the `Infrastructure` project through the `Ardalis.Specification.EntityFrameworkCore` package.

## Entities / Domain Model
The application defines a variety of entities within the **ApplicationCore** and **BlazorShared** projects. These include:
- **ApplicationCore Entities**: 
  - Address
  - Basket
  - BasketItem
  - Buyer
  - CatalogBrand
  - CatalogItem
  - CatalogItemDetails
  - CatalogItemOrdered
  - CatalogType
  - Order
  - OrderItem
  - PaymentMethod
- **BlazorShared Entities**:
  - CatalogBrand
  - CatalogItem
  - CatalogType
  - ErrorDetails
  - LookupData
- **UnitTests Entities**:
  - BasketAddItem
  - BasketRemoveEmptyItems
  - BasketTotalItems
  - OrderTotal

These entities represent core components of the application's domain model.

## Caching
No caching mechanism was detected in the current metadata.

## Messaging & Queuing
No message queue or event bus was detected in the current metadata.
