---
sidebar_position: 4
---

# Data, Storage & Messaging

## Databases
The application utilizes the following database technologies:
- **PostgreSQL**: Detected as the primary database solution through the metadata in the Infrastructure project, specifically referenced in the capabilities where it indicates dependency on `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL`. 

## Data Access Approach
The application's data access is conducted via:
- **Entity Framework Core (EF Core)**: This is identified as the primary ORM used, referenced in the capabilities section with the source `Microsoft.EntityFrameworkCore`.

## Entities / Domain Model
The domain model of the application includes the following entities:
- **TodoItem**
- **TodoList**

These entities are defined in the Domain project which is a part of the overall architecture.

## Caching
No caching mechanisms were detected based on the provided metadata.

## Messaging & Queuing
No message queue or event bus was detected in the current metadata.
