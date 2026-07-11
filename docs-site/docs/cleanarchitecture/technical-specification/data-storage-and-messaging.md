---
sidebar_position: 4
---

# Data, Storage & Messaging

## Databases
The application employs **PostgreSQL** as its database management system, indicated by the use of the package `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL` within the infrastructure project. Additionally, it incorporates varying database technologies, including **SQLite** as referenced by `Microsoft.EntityFrameworkCore.Sqlite`, and **SQL Server** as provided by `Microsoft.EntityFrameworkCore.SqlServer`. The presence of the `ApplicationDbContext` suggests the use of a DbContext for handling data operations.

## Data Access Approach
The application utilizes **Entity Framework Core (EF Core)** for data access, as evidenced by the package reference to `Microsoft.EntityFrameworkCore`. This framework facilitates working with databases using .NET objects, allowing for a more streamlined and efficient interaction with data.

## Entities / Domain Model
The application defines the following domain entities:
- **TodoItem**
- **TodoList**

These entities reside within the **Domain** project but do not include any additional properties or relationships in the current metadata context.

## Caching
No caching strategies or technologies were detected in the current metadata.

## Messaging & Queuing
No message queue or event bus was detected in the current metadata.
