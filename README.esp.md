<div align="center">
  <a href="./README.md">
    <img src="https://img.shields.io/badge/Read_in_English-EN-blue?style=for-the-badge" alt="Read in English">
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

# Demo del proyecto y características

## Identidad y 2FA
Implementación completa de Autenticación de Doble Factor (2FA) utilizando códigos QR y Apps Autenticadoras (Google/Microsoft), incluyendo códigos de recuperación.

| Activación de 2FA | Flujo de Login 2FA |
| :---: | :---: |
| ![Activar 2FA](alexpress-readme-assets/enable_2fa.gif) | ![Login con 2FA](alexpress-readme-assets/login_2fa.gif) |
| Usuario escanea QR y verifica código | Login requiere contraseña + OTP |

## Flujo de pago con Stripe
Flujo transaccional completo y pagos seguros vía Stripe.

#### 1. Carrito dinámico y cupones
Interacción en tiempo real con el carrito de compras y aplicación de descuentos.

![Añadir al Carrito](alexpress-readme-assets/add_to_cart.gif)

#### 2. Flujo de checkout seguro
Redirección a la pasarela segura de Stripe y procesamiento del pago.

![Checkout](alexpress-readme-assets/checkout.gif)

#### 3. Email de resumen de orden
El correo con el resumen de la orden se envía una vez finalizado el checkout.

![Resumen Email](alexpress-readme-assets/email_summary.gif)


### Moderación de contenido con IA
Integración con Azure Content Safety para bloquear automáticamente imágenes NSFW y texto ofensivo (título, descripción) en el producto.

| Seguridad de contenido |
| :---: |
| ![Moderación IA](alexpress-readme-assets/safety_content.gif) |
| Azure AI bloqueando contenido ofensivo |


# Despliegue

> 💡 **Nota:** Este proyecto está desplegado en Azure para fácil acceso. Dado que este proyecto integra múltiples servicios (Azure AI, Stripe, Cloudinary, Gmail), ejecutarlo localmente requiere una configuración compleja. Si deseas probar un proyecto mío localmente, por favor, revisa mi repositorio contenerizado [Scrum Task Manager](https://github.com/alejandropg845/scrum-task-manager).
>

<div align="center">
  <br/>
  <a href="https://alexpress-client-evcvg7ebguh7d4hd.canadacentral-01.azurewebsites.net/alexpress/home" target="_blank">
    <img src="https://img.shields.io/badge/Ver_Demo_en_Vivo-Visitar%20App-0078D4?style=for-the-badge&logo=microsoftazure&logoColor=white" alt="Ver Demo en Vivo">
  </a>
  <br/>
</div>

## Tech Stack

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


Hecho por [Alejandro.NET](https://alejandropg845.github.io/resume)
