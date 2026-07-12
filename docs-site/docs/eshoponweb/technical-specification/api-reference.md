---
sidebar_position: 6
---

# API Reference

### CatalogTypeListEndpoint (Minimal API)
- **Base Route:** Not specified  
- **Endpoint Table:**

| Method | Action | Endpoint             |
|--------|--------|----------------------|
| GET    | -      | api/catalog-types     |

### CatalogItemGetByIdEndpoint (Minimal API)
- **Base Route:** Not specified  
- **Endpoint Table:**

| Method | Action | Endpoint                  |
|--------|--------|---------------------------|
| GET    | -      | api/catalog-items/{catalogItemId} |

### CatalogItemListPagedEndpoint (Minimal API)
- **Base Route:** Not specified  
- **Endpoint Table:**

| Method | Action | Endpoint         |
|--------|--------|------------------|
| GET    | -      | api/catalog-items |

### DeleteCatalogItemEndpoint (Minimal API)
- **Base Route:** Not specified  
- **Endpoint Table:**

| Method | Action | Endpoint                  |
|--------|--------|---------------------------|
| DELETE | -      | api/catalog-items/{catalogItemId} |

### CreateCatalogItemEndpoint (Minimal API)
- **Base Route:** Not specified  
- **Endpoint Table:**

| Method | Action | Endpoint         |
|--------|--------|------------------|
| POST   | -      | api/catalog-items |

### UpdateCatalogItemEndpoint (Minimal API)
- **Base Route:** Not specified  
- **Endpoint Table:**

| Method | Action | Endpoint         |
|--------|--------|------------------|
| PUT    | -      | api/catalog-items |

### CatalogBrandListEndpoint (Minimal API)
- **Base Route:** Not specified  
- **Endpoint Table:**

| Method | Action | Endpoint          |
|--------|--------|-------------------|
| GET    | -      | api/catalog-brands |


### OrderController
- **Base Route:** [controller]/[action]  
- **Endpoint Table:**

| Method | Action     | Endpoint         |
|--------|------------|------------------|
| GET    | MyOrders   | Order/MyOrders    |
| GET    | Detail     | Order/Detail      |

### ManageController
- **Base Route:** [controller]/[action]  
- **Endpoint Table:**

| Method | Action                        | Endpoint                        |
|--------|-------------------------------|---------------------------------|
| GET    | MyAccount                     | Manage/MyAccount                |
| POST   | MyAccount                     | Manage/MyAccount                |
| POST   | SendVerificationEmail         | Manage/SendVerificationEmail    |
| GET    | ChangePassword                | Manage/ChangePassword           |
| POST   | ChangePassword                | Manage/ChangePassword           |
| GET    | SetPassword                   | Manage/SetPassword              |
| POST   | SetPassword                   | Manage/SetPassword              |
| GET    | ExternalLogins                | Manage/ExternalLogins           |
| POST   | LinkLogin                     | Manage/LinkLogin                |
| GET    | LinkLoginCallback             | Manage/LinkLoginCallback        |
| POST   | RemoveLogin                   | Manage/RemoveLogin              |
| GET    | TwoFactorAuthentication       | Manage/TwoFactorAuthentication  |
| GET    | Disable2faWarning             | Manage/Disable2faWarning        |
| POST   | Disable2fa                   | Manage/Disable2fa              |
| GET    | EnableAuthenticator            | Manage/EnableAuthenticator       |
| GET    | ShowRecoveryCodes             | Manage/ShowRecoveryCodes        |
| POST   | EnableAuthenticator            | Manage/EnableAuthenticator       |
| GET    | ResetAuthenticatorWarning      | Manage/ResetAuthenticatorWarning |
| POST   | ResetAuthenticator             | Manage/ResetAuthenticator       |
| POST   | GenerateRecoveryCodes         | Manage/GenerateRecoveryCodes    |
| GET    | GenerateRecoveryCodesWarning   | Manage/GenerateRecoveryCodesWarning |

### UserController
- **Base Route:** [controller]  
- **Endpoint Table:**

| Method | Action          | Endpoint       |
|--------|-----------------|----------------|
| GET    | GetCurrentUser  | User            |
| POST   | Logout          | User            |

### BaseApiController
- **Base Route:** api/[controller]/[action]  
- **Endpoint Table:**

| Method | Action | Endpoint        |
|--------|--------|-----------------|
| -      | -      | api/BaseApiController |
