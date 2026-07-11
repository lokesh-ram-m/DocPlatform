---
sidebar_position: 4
---

# Data, Storage & Messaging

## Databases
The application utilizes **SQL Server** as its database, as indicated by the inclusion of `Microsoft.Data.SqlClient` in the project's package references. There are no `DbContexts` defined in the metadata.

## Data Access Approach
The application employs **Dapper**, which is a micro-ORM for data access, as per the metadata. It is used to simplify the database interaction without the overhead of a full ORM framework. 

## Entities / Domain Model
The domain model consists of the following entities:
- **Project**
- **Task**
- **User**

These entities form the core data structures around which the application is designed.

## Caching
None detected.

## Messaging & Queuing
No message queue or event bus was detected in the current metadata.
