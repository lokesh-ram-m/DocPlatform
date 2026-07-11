---
sidebar_position: 4
---

# Data, Storage & Messaging

## Databases
The application uses **SQL Server** as its database. This is specified under the database capabilities available in the metadata, where the source is indicated as **Microsoft.Data.SqlClient**.

## Data Access Approach
The application employs **Dapper**, a micro-ORM, for data access. This is confirmed in the package references where Dapper is listed under the Data Access capabilities.

## Entities / Domain Model
The application includes the following domain entities:
- **Project**
- **Task**
- **User**

These entities are utilized across various controllers and services within the application.

## Caching
No caching mechanism was detected in the current metadata.

## Messaging & Queuing
No message queue or event bus was detected in the current metadata.
