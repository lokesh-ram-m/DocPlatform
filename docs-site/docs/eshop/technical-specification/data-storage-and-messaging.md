---
sidebar_position: 4
---

# Data, Storage & Messaging

## Databases
The application uses **PostgreSQL** as its database system. The following DbContexts manage the database interactions:
- **WebhooksContext**
- **CatalogContext**
- **ApplicationDbContext**
- **OrderingContext**

These contexts define how entities are mapped and how data is managed within the application.

## Data Access Approach
The application employs **Entity Framework Core** for data access. This ORM (Object-Relational Mapping) framework facilitates database operations and enables developers to interact with the database using .NET objects rather than raw SQL queries.

## Entities / Domain Model
The application consists of the following entities that represent its domain model:
- **WebhookSubscription**
- **CatalogBrand**
- **CatalogItem**
- **CatalogType**
- **Buyer**
- **Order**
- **OrderItem**
- **PaymentMethod**
- **ApplicationUser**
- **ConsentInputModel**
- **CardType**
- **BasketItem**
- **CustomerBasket**
- **Address**
- **Campaign**
- **CampaignItem**
- **CampaignRoot**
- **CatalogRoot**
- **GeolocationException**
- **Location**
- **LogoutParameter**
- **OrderCheckout**
- **PaymentInfo**
- **Position**
- **TabParameter**
- **UserInfo**
- **UserToken**
- **BuyerAggregateTest**
- **ComplexObject**
- **OrderAggregateTest**
- **ValueObjectA**
- **ValueObjectB**
- **ValueObjectTests**

These entities encapsulate the data and business logic associated with the application.

## Caching
**Redis** is utilized for caching within the application. This technology helps improve performance by temporarily storing data in memory, reducing the need for repeated database queries.

## Messaging & Queuing
The application employs **RabbitMQ** for messaging and queuing tasks. This message broker facilitates communication between different services within the application by allowing them to send and receive messages asynchronously.
