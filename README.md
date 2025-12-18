<div align="center">
  <a href="./README.esp.md">
    <img src="https://img.shields.io/badge/Leer_en_Espa%C3%B1ol-ES-red?style=for-the-badge" alt="Leer en Español">
  </a>
</div>

# Alexpress - E-commerce platform

![Angular](https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![TailwindCSS](https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=for-the-badge&logo=tailwind-css&logoColor=white)
![.NET](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Azure](https://img.shields.io/badge/Azure-0078D4?style=for-the-badge&logo=microsoftazure&logoColor=white)
![Stripe](https://img.shields.io/badge/Stripe-008CDD?style=for-the-badge&logo=stripe&logoColor=white)

E-commerce platform inspired by Aliexpress.

![Demo Alexpress](./ecommerce.gif)

## Tech stack

The project uses a modern stack focused on performance and scalability:

*   **Frontend:** Angular 17, TailwindCSS.
*   **Backend:** ASP.NET Core.
*   **Data:** Entity Framework Core, SQL Server.
*   **Cloud & services:** Microsoft Azure, Stripe, Cloudinary.

## Architecture and patterns

The system implements a layered architecture (Controller-Service-Repository).

*   **Logical bounded contexts:** Multiple `DbContexts` are used to segregate business responsibilities (Inventory, Identity, Sales).
*   **Outbox Messages:** Implementation of the Outbox Pattern using a BackgroundService worker. This ensures reliable execution of side effects (like sending email confirmations and purchase summaries) by persisting tasks to the database (OutboxMessages) and processing them asynchronously.
*   **Implemented patterns:**
    *   **Repository pattern:** Data access abstraction.
    *   **Unit of work:** Management of atomic distributed transactions across multiple database contexts.
    *   **Dependency injection:** Dependency decoupling to facilitate testing and maintenance.

## Database schema
```mermaid
classDiagram
    %% Core Identity
    class AppUser {
        +string Id
        +bool IsDisabled
    }
    
    class Address {
        +int Id
        +string AppUserId
        +string FullName
        +string Country
        +string City
    }

    
    class Product {
        +int Id
        +string AppUserId
        +string Title
        +decimal Price
        +int Stock
        +int CategoryId
        +int ConditionId
        +List~ReviewItem~ Reviews
    }

    class Category {
        +int Id
        +string Name
    }

    class Condition {
        +int Id
        +string Name
    }

    class Coupon {
        +int Id
        +string CouponName
        +decimal Discount
        +int ProductId
    }

    class ReviewItem {
        +int Id
        +int Rating
        +string Comment
        +int ProductId
    }

   
    class Order {
        +int Id
        +string AppUserId
        +DateTimeOffset CreatedOn
        +decimal Summary
        +int AddressId
        +List~OrderedProduct~ OrderedProducts
    }

    class OrderedProduct {
        +int Id
        +int OrderId
        +int ProductId
        +int Quantity
        +decimal Price
    }

    
    class Cart {
        +int Id
        +string AppUserId
        +decimal Summary
        +List~CartProduct~ CartProducts
    }

    class CartProduct {
        +int Id
        +int CartId
        +int ProductId
        +int Quantity
    }

    
    class WishList {
        +int Id
        +string AppUserId
        +List~WishListProduct~ WishListProducts
    }

    class WishListProduct {
        +int Id
        +int WishListId
        +int ProductId
    }

    Product --> Category : has
    Product --> Condition : has
    Product "1" -- "0..1" Coupon : has
    Product "1" -- "*" ReviewItem : has
    
    Order "1" -- "*" OrderedProduct : contains
    Order --> Address : ships to
    OrderedProduct --> Product : references

    Cart "1" -- "*" CartProduct : contains
    CartProduct --> Product : references

    WishList "1" -- "*" WishListProduct : contains
    WishListProduct --> Product : references

    %% Logical User Relations (Loose coupling via AppUserId)
    AppUser "1" .. "*" Product : sells
    AppUser "1" .. "*" Order : places
    AppUser "1" .. "1" Cart : owns
    AppUser "1" .. "1" WishList : owns
    AppUser "1" .. "*" Address : manages


```

## Authentication and security

*   **ASP.NET Core Identity:** Robust user and role management.
*   **Token-based authentication:** JWT (JSON Web Tokens) implementation with Refresh Tokens for secure long-lived session management.
*   **Two-factor authentication (2FA):** Support for two-factor authentication via QR codes compatible with Google Authenticator.
*   **Account recovery:** Secure password recovery flows via email and in-app.

## External integrations

The system connects with third-party services for critical functionalities:

*   **Stripe:** Secure payment processing via Webhooks, allowing asynchronous and resilient order confirmation.
*   **Azure AI Content Safety:** Automatic moderation of visual and textual content when publishing products, preventing inappropriate content.
*   **Cloudinary:** Cloud image storage.

## Main features

Description of the platform's key capabilities:

*   **Catalog management:**
    *   Product publishing with automatic moderation (AI).
    *   Advanced filtering by rating, price range, title, and categories.
*   **Shopping system:**
    *   Secure checkout integrated with Stripe.
    *   Application of discount coupons offered by the seller.
*   **Sales management:**
    *   Creation and administration of coupons linked to specific products.
*   **Rating:**
    *   Review and rating system per product.
    *   Visualization of opinions from other buyers.

---

<div align="center">
  <br/>
  <a href="https://alexpress-client-evcvg7ebguh7d4hd.canadacentral-01.azurewebsites.net/alexpress/home" target="_blank">
    <img src="https://img.shields.io/badge/View_Live_Demo-Visit%20App-0078D4?style=for-the-badge&logo=microsoftazure&logoColor=white" alt="View Live Demo">
  </a>
  <br/>
</div>

Made by [Alejandro.NET](https://alejandropg845.github.io/resume)
