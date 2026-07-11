---
sidebar_position: 6
---

# API Reference

### CatalogTypeListEndpoint (Minimal API)
| Method | Action | Endpoint |
|--------|--------|----------|
| GET    |        | api/catalog-types |

### CatalogItemGetByIdEndpoint (Minimal API)
| Method | Action | Endpoint |
|--------|--------|----------|
| GET    |        | api/catalog-items/{catalogItemId} |

### CatalogItemListPagedEndpoint (Minimal API)
| Method | Action | Endpoint |
|--------|--------|----------|
| GET    |        | api/catalog-items |

### DeleteCatalogItemEndpoint (Minimal API)
| Method | Action | Endpoint |
|--------|--------|----------|
| DELETE |        | api/catalog-items/{catalogItemId} |

### CreateCatalogItemEndpoint (Minimal API)
| Method | Action | Endpoint |
|--------|--------|----------|
| POST   |        | api/catalog-items |

### UpdateCatalogItemEndpoint (Minimal API)
| Method | Action | Endpoint |
|--------|--------|----------|
| PUT    |        | api/catalog-items |

### CatalogBrandListEndpoint (Minimal API)
| Method | Action | Endpoint |
|--------|--------|----------|
| GET    |        | api/catalog-brands |

### OrderController
| Method | Action      | Endpoint            |
|--------|-------------|---------------------|
| GET    | MyOrders    | [controller]/[action] |
| GET    | Detail      | [controller]/[action] |

### ManageController
| Method | Action                     | Endpoint            |
|--------|----------------------------|---------------------|
| GET    | MyAccount                  | [controller]/[action] |
| POST   | MyAccount                  | [controller]/[action] |
| POST   | SendVerificationEmail      | [controller]/[action] |
| GET    | ChangePassword             | [controller]/[action] |
| POST   | ChangePassword             | [controller]/[action] |
| GET    | SetPassword                | [controller]/[action] |
| POST   | SetPassword                | [controller]/[action] |
| GET    | ExternalLogins             | [controller]/[action] |
| POST   | LinkLogin                  | [controller]/[action] |
| GET    | LinkLoginCallback          | [controller]/[action] |
| POST   | RemoveLogin                | [controller]/[action] |
| GET    | TwoFactorAuthentication     | [controller]/[action] |
| GET    | Disable2faWarning         | [controller]/[action] |
| POST   | Disable2fa                | [controller]/[action] |
| GET    | EnableAuthenticator        | [controller]/[action] |
| GET    | ShowRecoveryCodes         | [controller]/[action] |
| POST   | EnableAuthenticator        | [controller]/[action] |
| GET    | ResetAuthenticatorWarning   | [controller]/[action] |
| POST   | ResetAuthenticator         | [controller]/[action] |
| POST   | GenerateRecoveryCodes      | [controller]/[action] |
| GET    | GenerateRecoveryCodesWarning| [controller]/[action] |

### UserController
| Method | Action         | Endpoint  |
|--------|----------------|-----------|
| GET    | GetCurrentUser | [controller] |
| POST   | Logout         | [controller] | 

### BaseApiController
| Method | Action | Endpoint          |
|--------|--------|-------------------|
|        |        | api/[controller]/[action] |
