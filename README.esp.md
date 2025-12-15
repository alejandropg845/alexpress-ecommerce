<div align="center">
  <a href="./README.md">
    <img src="https://img.shields.io/badge/Leer_en_Espa%C3%B1ol-ES-red?style=for-the-badge" alt="Read in English">
  </a>
</div>

# Alexpress - Plataforma E-commerce

![Angular](https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![TailwindCSS](https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=for-the-badge&logo=tailwind-css&logoColor=white)
![.NET](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Azure](https://img.shields.io/badge/Azure-0078D4?style=for-the-badge&logo=microsoftazure&logoColor=white)
![Stripe](https://img.shields.io/badge/Stripe-008CDD?style=for-the-badge&logo=stripe&logoColor=white)

Plataforma de comercio electrónico inspirada en Aliexpress.

![Demo Alexpress](./MyVideo1.gif)

## Tech Stack

El proyecto utiliza un stack moderno enfocado en rendimiento y escalabilidad:

*   **Frontend:** Angular, TailwindCSS.
*   **Backend:** ASP.NET Core.
*   **Data:** Entity Framework Core, SQL Server.
*   **Cloud y servicios:** Microsoft Azure, Stripe, Cloudinary.

## Arquitectura y patrones

El sistema implementa una arquitectura en capas (Controller-Service-Repository).

*   **Bounded Contexts lógicos:** Se utilizan múltiples `DbContexts` para segregar responsabilidades de negocio (Inventario, Identidad, Ventas).
*   **Outbox Messages:** Implementación del Patrón Outbox utilizando un BackgroundService. Esto garantiza la ejecución confiable de tareas secundarias (como el envío de correos de confirmación y resúmenes de compra) al persistirlas en la base de datos (OutboxMessages) y procesarlas de forma asíncrona.
*   **Patrones Implementados:**
    *   **Repository Pattern:** Abstracción del acceso a datos.
    *   **Unit of Work:** Gestión de transacciones atómicas distribuidas entre múltiples DbContexts de base de datos.
    *   **Dependency Injection:** Desacoplamiento de dependencias para facilitar el testing y mantenimiento.

## Esquema de base de datos

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

    Product --> Category : tiene
    Product --> Condition : tiene
    Product "1" -- "0..1" Coupon : tiene
    Product "1" -- "*" ReviewItem : tiene
    
    Order "1" -- "*" OrderedProduct : contiene
    Order --> Address : se envía a
    OrderedProduct --> Product : references

    Cart "1" -- "*" CartProduct : contiene
    CartProduct --> Product : references

    WishList "1" -- "*" WishListProduct : contiene
    WishListProduct --> Product : references

    AppUser "1" .. "*" Product : vende
    AppUser "1" .. "*" Order : realiza
    AppUser "1" .. "1" Cart : gestiona
    AppUser "1" .. "1" WishList : gestiona
    AppUser "1" .. "*" Address : gestiona


```

## Autenticación y seguridad

*   **ASP.NET Core Identity:** Gestión robusta de usuarios y roles.
*   **Token-Based Authentication:** Implementación de JWT (JSON Web Tokens) con Refresh Tokens para manejo seguro de sesiones de larga duración.
*   **Two-Factor Authentication (2FA):** Soporte para autenticación de doble factor mediante QR compatibles con Google Authenticator.
*   **Account Recovery:** Flujos seguros de recuperación de contraseña vía email y in-app.

## Integraciones externas

El sistema se conecta con servicios de terceros para funcionalidades críticas:

*   **Stripe:** Procesamiento de pagos seguro mediante Webhooks, permitiendo la confirmación asíncrona y resiliente de las órdenes.
*   **Azure AI Content Safety:** Moderación automática de contenido visual y textual al momento de publicar productos, previniendo contenido inapropiado.
*   **Cloudinary:** Almacenamiento de imágenes en la nube.

## Funcionalidades principales

Descripción de las capacidades clave de la plataforma:

*   **Gestión de catálogo:**
    *   Publicación de productos con moderación automática (IA).
    *   Filtrado avanzado por rating, rango de precios, título y categorías.
*   **Sistema de compras:**
    *   Checkout seguro integrado con Stripe.
    *   Aplicación de cupones de descuento ofrecidos por el vendedor.
*   **Gestión de Ventas:**
    *   Creación y administración de cupones vinculados a productos específicos.
*   **Rating:**
    *   Sistema de reseñas y calificaciones por producto.
    *   Visualización de opiniones de otros compradores.

---
Hecho por [Alejandro.NET](https://alejandropg845.github.io/resume)
