---
sidebar_position: 4
---

# Data, Storage & Messaging

## Databases
The application leverages the **PostgreSQL** database, as indicated by the `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL` package included in the infrastructure project. One `DbContext` named **ApplicationDbContext** is defined in the **Infrastructure** project, which will be used for interacting with the PostgreSQL database.

## Data Access Approach
The application employs **Entity Framework Core** for data access, as evidenced by the presence of the `Microsoft.EntityFrameworkCore` package in the **Infrastructure** and **Application** projects. This allows for an Object-Relational Mapping (ORM) approach for database operations.

## Entities / Domain Model
The domain model is represented by two entities:
- **TodoItem**
- **TodoList**

These entities are defined within the **Domain** project and encapsulate the core data structures used within the application.

## Caching
None detected.

## Messaging & Queuing
No message queue or event bus was detected in the current metadata.
