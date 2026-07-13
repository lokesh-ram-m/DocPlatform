---
sidebar_position: 4
---

# Data, Storage & Messaging

## Databases
The application utilizes **PostgreSQL** as its database management system. The following components persist data to PostgreSQL:
- Webhooks.API
- Catalog.API
- Identity.API
- Ordering.Infrastructure

## Data Access Approach
The application primarily employs **Entity Framework Core (EF Core)** for data access.

## Entities / Domain Model
The application defines the following domain model entities:
- **Identity.API**: 
  - `ApplicationUser`
  - `ConsentInputModel`
- **Ordering.API**:
  - `BasketItem`
  - `CustomerBasket`
  
No additional entities are defined in other components.

## Caching
**Redis** caching technology has been detected. 

## Messaging & Queuing
The application utilizes **RabbitMQ** as its messaging system. Several components depend on RabbitMQ:
- Webhooks.API
- Catalog.API
- Ordering.API
- PaymentProcessor
- Basket.API
- EventBus

Additionally, the **EventBus** library offers interfaces indicating its role in event handling within the architecture.
