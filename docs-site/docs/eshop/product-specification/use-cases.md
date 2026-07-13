---
sidebar_position: 3
---

# Use Cases for eShop

## Use Case 1: Product Manager - Feature Planning
As a Product Manager at eShop, I start my day by analyzing user feedback and market trends. I utilize the capabilities of Azure AI and OpenAI/GitHub Models to gain insights into potential new features that align with our users' needs. I gather data from our API documentation and detailed analytics to prioritize features that will enhance user experience and drive sales. Later, I present these findings to the Business Unit, ensuring that we are aligned on our product roadmap.

## Use Case 2: Business Unit - Performance Review
In a regular meeting, the Business Unit reviews the performance metrics of the eShop application. We look at user engagement, sales figures, and customer satisfaction scores, all enabled by the robust data access capabilities provided by Entity Framework Core and PostgreSQL. By analyzing these metrics, we discuss strategies to improve our offerings. Decisions are made based on quantitative data, ensuring our initiatives are targeted and effective.

## Use Case 3: End User - Placing an Order
As an end user of eShop, I log in using my OpenID Connect credentials. After browsing through the catalog via the Catalog API, I find a product I'd like to purchase. The smooth authentication process ensures that my account is secure. When I place my order, I experience a fast and responsive interface thanks to the caching capabilities of Redis. After confirming my purchase, I receive real-time notifications and updates through the messaging services supported by RabbitMQ, providing me with a seamless shopping experience.

## Use Case 4: Support Team - Managing User Queries
As a member of the support team, I regularly handle user queries related to account management. Using the AuthenticationEndpoints, I efficiently assist users in troubleshooting issues such as password recovery or account verification. The clarity of our API documentation helps me quickly find the information I need to guide users effectively, ensuring high satisfaction and swift resolutions for their concerns.

## Use Case 5: Developer - Implementing a New Feature
As a developer working on eShop, I start with the feature request from our Product Manager. I reference the OpenAPI documentation to understand existing endpoints and see where I can extend the functionality. Using ASP.NET Core and xUnit for testing, I implement the new feature while ensuring it aligns with best practices for authentication and data validation using FluentValidation. Once ready, I run tests before deploying my changes, ensuring a reliable update for our users.
