---
sidebar_position: 1
---

# Architecture

## Detected Patterns
The application appears to follow a layered architecture pattern, likely influenced by principles from Clean Architecture, leveraging the use of interfaces and services. The use of Dependency Injection is also evident throughout the projects.

## Solution Structure
The solution consists of several repositories, each responsible for different aspects of the application:

- **eShopOnWeb**: Root repository
  - **BlazorAdmin**: A Blazor-based admin interface providing services for catalog management and HTTP operations.
  - **Infrastructure**: Contains data access layer services and DbContexts for identity and catalog data management.
  - **PublicApi**: Exposes a Minimal API for catalog features such as retrieving and manipulating catalog items and brands.
  - **ApplicationCore**: Contains core business logic, entities, and service interfaces for basket and order management.
  - **BlazorShared**: Houses shared libraries and services used by both the Blazor and Web projects.
  - **Web**: Provides user interactions and UI through ASP.NET Core, including controllers for user management and order handling.
  - **Tests**: Comprises integration tests, unit tests, and functional tests for ensuring application reliability.

## Component Responsibilities
- **BlazorAdmin**: Manages administrative tasks related to catalog items and user sessions.
  - Services: `CatalogItemService`, `CatalogLookupDataService`, `HttpService`, `ToastService`
  
- **Infrastructure**: Handles data persistence with Entity Framework Core and identity management.
  - Services: `BasketQueryService`, `IdentityTokenClaimService`
  - DbContexts: `AppIdentityDbContext`, `CatalogContext`

- **PublicApi**: Provides access to catalog endpoints for external consumption.
  - Controllers: Handles various API requests for catalog items, such as CRUD operations.

- **ApplicationCore**: Centralizes business logic and domain entities for orders and baskets.
  - Services: `BasketService`, `OrderService`, and associated interfaces.
  - Entities: Includes core objects like `CatalogItem`, `Order`, etc.

- **BlazorShared**: Provides shared DTOs and services facilitating communication between Blazor and other components.
  - Services: Shared interfaces for catalog queries.

- **Web**: Manages the frontend aspects of the application and user account functionalities.
  - Controllers: `OrderController`, `ManageController`, `UserController`
  
## How the Pieces Fit Together
The architecture of the application integrates several components as follows:

- The **frontend** is built in Blazor and ASP.NET Core, offering an interactive user experience while managing user sessions and catalog operations.
- **API** interactions are handled through the **PublicApi** project, which exposes endpoints for catalog management, allowing the frontend to fetch and manipulate data.
- **Data flow** occurs through the **Infrastructure** project, where services interact with the databases via Entity Framework Core, fetching the necessary data to support the API calls made from the frontend.
- Authentication is managed using **ASP.NET Core Identity** along with JWT authentication for securing API endpoints, ensuring that sensitive operations are accessible only to authorized users. 

Overall, the system is designed to facilitate a fluid and secure data exchange between users and the catalog management features, backed by a robust infrastructure layer.
