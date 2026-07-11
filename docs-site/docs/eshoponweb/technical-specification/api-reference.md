---
sidebar_position: 6
---

# API Reference

### CatalogTypeListEndpoint (Minimal API)
- **Base Route**: Not explicitly defined
| Method | Action           | Endpoint             |
|--------|------------------|----------------------|
| GET    | Catalog Types    | api/catalog-types     |

### CatalogItemGetByIdEndpoint (Minimal API)
- **Base Route**: Not explicitly defined
| Method | Action                     | Endpoint                        |
|--------|----------------------------|---------------------------------|
| GET    | Get Catalog Item By Id     | api/catalog-items/{catalogItemId} |

### CatalogItemListPagedEndpoint (Minimal API)
- **Base Route**: Not explicitly defined
| Method | Action           | Endpoint             |
|--------|------------------|----------------------|
| GET    | List Catalog Items| api/catalog-items     |

### DeleteCatalogItemEndpoint (Minimal API)
- **Base Route**: Not explicitly defined
| Method | Action                  | Endpoint                        |
|--------|-------------------------|---------------------------------|
| DELETE | Delete Catalog Item     | api/catalog-items/{catalogItemId} |

### CreateCatalogItemEndpoint (Minimal API)
- **Base Route**: Not explicitly defined
| Method | Action                   | Endpoint             |
|--------|--------------------------|----------------------|
| POST   | Create Catalog Item      | api/catalog-items     |

### UpdateCatalogItemEndpoint (Minimal API)
- **Base Route**: Not explicitly defined
| Method | Action                   | Endpoint             |
|--------|--------------------------|----------------------|
| PUT    | Update Catalog Item      | api/catalog-items     |

### CatalogBrandListEndpoint (Minimal API)
- **Base Route**: Not explicitly defined
| Method | Action           | Endpoint             |
|--------|------------------|----------------------|
| GET    | Catalog Brands   | api/catalog-brands    |

### OrderController
- **Base Route**: [controller]/[action]
| Method | Action   | Endpoint                  |
|--------|----------|---------------------------|
| GET    | MyOrders | Order/MyOrders            |
| GET    | Detail   | Order/Detail              |

### ManageController
- **Base Route**: [controller]/[action]
| Method | Action                          | Endpoint                       |
|--------|---------------------------------|--------------------------------|
| GET    | MyAccount                       | Manage/MyAccount               |
| POST   | MyAccount                       | Manage/MyAccount               |
| POST   | SendVerificationEmail           | Manage/SendVerificationEmail   |
| GET    | ChangePassword                  | Manage/ChangePassword          |
| POST   | ChangePassword                  | Manage/ChangePassword          |
| GET    | SetPassword                     | Manage/SetPassword             |
| POST   | SetPassword                     | Manage/SetPassword             |
| GET    | ExternalLogins                  | Manage/ExternalLogins          |
| POST   | LinkLogin                       | Manage/LinkLogin               |
| GET    | LinkLoginCallback               | Manage/LinkLoginCallback       |
| POST   | RemoveLogin                     | Manage/RemoveLogin             |
| GET    | TwoFactorAuthentication         | Manage/TwoFactorAuthentication  |
| GET    | Disable2faWarning               | Manage/Disable2faWarning      |
| POST   | Disable2fa                     | Manage/Disable2fa             |
| GET    | EnableAuthenticator             | Manage/EnableAuthenticator     |
| GET    | ShowRecoveryCodes               | Manage/ShowRecoveryCodes       |
| POST   | EnableAuthenticator             | Manage/EnableAuthenticator     |
| GET    | ResetAuthenticatorWarning       | Manage/ResetAuthenticatorWarning|
| POST   | ResetAuthenticator              | Manage/ResetAuthenticator      |
| POST   | GenerateRecoveryCodes           | Manage/GenerateRecoveryCodes   |
| GET    | GenerateRecoveryCodesWarning    | Manage/GenerateRecoveryCodesWarning|

### UserController
- **Base Route**: [controller]
| Method | Action             | Endpoint        |
|--------|-------------------|------------------|
| GET    | Get Current User   | User              |
| POST   | Logout             | User              |
