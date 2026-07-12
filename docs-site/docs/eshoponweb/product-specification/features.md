---
sidebar_position: 2
---

# eShopOnWeb Product Specification

## Task Management
- **Order Management**: Users can view their orders and details of specific orders, which helps them keep track of their purchases and order status. This enhances customer satisfaction by providing visibility and control over their orders.
  
- **Basket Management**: Users can manage their shopping basket, adding or removing items as needed. This function simplifies the online shopping experience, allowing users to curate their selections before purchase.

## User & Access Management
- **User Account Management**: Users can create and manage their accounts, including changing passwords and enabling two-factor authentication. This increases security and gives users control over their personal data and account settings.

- **Authentication**: The application uses JWT authentication for secure access to user accounts. This ensures that user data is protected and only accessible by authorized individuals.

## Catalog Management
- **Catalog Browsing**: Users can browse through a catalog of items, including brands and item types. This capability allows users to easily find products they are interested in, enhancing the shopping experience.

- **Catalog Item Details**: Users can view detailed information about specific catalog items, aiding informed purchasing decisions and ensuring a high level of product awareness.

- **Catalog Item Creation, Update, and Deletion**: Administrators can create, update, or delete catalog items. This feature provides flexibility and control over the inventory, ensuring that the product listings are current and relevant.

## API Documentation
- **OpenAPI/Swagger**: The application provides API documentation through Swagger. This feature allows developers to easily understand and integrate with the application's API, promoting better collaboration and understanding between technical and non-technical stakeholders.

## Security & Compliance
- **Identity Management**: The application utilizes ASP.NET Core Identity for user management. This capability secures user data and provides features such as user roles and claims, assisting in managing access and permissions effectively.

## Testing and Quality Assurance
- **Automated Testing**: Integration and unit tests are implemented to ensure code quality and behavior consistency. This capability helps maintain the application's reliability and minimizes the risk of defects in production. 

## Mapping and Data Access
- **AutoMapper**: The application employs AutoMapper for object-object mapping, improving efficiency when handling data transfer objects and enhancing code maintainability.

- **Entity Framework Core**: The application uses Entity Framework Core for data access, ensuring reliable and efficient database interactions. This capability simplifies CRUD operations and enhances database management.

These capabilities collectively create a robust platform for users to manage online shopping experiences efficiently, ensuring both ease of use and high levels of security and responsiveness.
