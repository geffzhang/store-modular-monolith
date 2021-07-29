[![Twitter URL](https://img.shields.io/badge/-@mehdi_hadeli-%231DA1F2?style=flat-square&logo=twitter&logoColor=ffffff)](https://twitter.com/mehdi_hadeli)
[![Linkedin Url URL](https://img.shields.io/badge/-mehdihadeli-blue?style=flat-square&logo=linkedin&logoColor=ffffff)](https://www.linkedin.com/in/mehdihadeli/)
[![blog](https://img.shields.io/badge/blog-dotnetuniversity.com-brightgreen?style=flat-square)](https://dotnetuniversity.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg?style=flat-square)](https://opensource.org/licenses/MIT)
[![Tweet](https://img.shields.io/twitter/url/http/shields.io.svg?style=social)][tweet] 


# Online Store Modular Monolith
> Implementing an online store Modular Monolith application with Domain-Driven Design and CQRS in .Net Core. In the plans, this application will be moved to microservices architecture in another repository which is avaiable in [online-store-microservice](https://github.com/mehdihadeli/online-store-microservice) repository.

🌀 Keep in mind this repository is work in progress and will be complete over time 🚀

# Support ⭐
If you like my work, feel free to:

- ⭐ this repository. And we will be happy together :)
- [![Tweet](https://img.shields.io/twitter/url/http/shields.io.svg?style=social)][tweet] about Online-Store-Modular-Monolith


Thanks a bunch for supporting me!

[tweet]: https://twitter.com/intent/tweet?url=https://github.com/mehdihadeli/Online-Store-Modular-Monolith&text=Implementing%20an%20online%20store%20Modular%20Monolith%20application%20with%20Domain-Driven%20Design%20and%20CQRS%20in%20.Net%20Core&hashtags=dotnetcore,dotnet,csharp,microservices,netcore,aspnetcore,ddd,cqrs

# Table of Contents

- [1. The goals of this project](#1-the-goals-of-this-project)
- [2. Plan](#2-plan)
- [3. Technologies & Libraries](#3-technologies)
- [4. Domain](#4-domain)
  - [4.1 Domain Description](#41-domain-description)
  - [4.2 Identity Module](#42-domain-description)
  - [4.3 Catalog Module](#43-catalog-module)
  - [4.4 Customers Module](#44-customers-module)
  - [4.5 Vendor Module](#45-vendor-module)
  - [4.6 Inventory Module](46-inventory-module)
  - [4.7 Order Module](47-order-module)
  - [4.8 Payment Module](48-payment-module)
  - [4.9 Shipping Module](49-shipping-module)
- [5. Architecture](#5-architecture)
  - [5.1. Module structure](#51-module-structure)
- [6. How to Run](#6-how-to-run)
- [7. Contribution](#7-contribution)
- [8. License](#8-license)

## 1. The Goals of This Project

- the **modular monolith** with **DDD** implementation.
- correct separation of bounded contexts.
- example of communications between bounded contexts through asynchronous **in-memory message broker** and extracting each module to microservices if we need to scale, we will change in-memory broker to a real broker like rabbitmq.
- example of simple **CQRS** implementation and **event driven architecture**.
- implementing various type of testing like **unit testing**,  **integration testing** and **end-to-end testing**.
- documentation of architecture decisions.
- using **inbox pattern** and **outbox pattern** for message passing between modules.
- using **best practice** and **design patterns**.
- using **c4 model** for architectural diagram.
- using **event storming** for extracting data model and bounded context (using Miro).
- presenting our **domain model** and our **use cases** in different diagrams (using PlantUML).

## 2. Plan
> This project is currently under development.

The issues are represented in [https://github.com/mehdihadeli/store-modular-monolith/issues](https://github.com/mehdihadeli/store-modular-monolith/issues)

High-level plan is represented in the table

| Feature | Status |
| ------- | ------ |
| Building Blocks | In-Progress 👷‍♂️|
| API Module | In-Progress 👷‍♂️ |
| Identity Module | Under Development 👷‍♂️ |
| Order Module | Not Started 🚩 |
| Customer Module | Not Started 🚩 |
| Catalog Module | Not Started 🚩 |
| Shipping Module | Not Started 🚩 |
| Vendor Module | Not Started 🚩 |
| Inventory Module | Not Started 🚩 |
| Payment Module | Not Started 🚩 |

## 3. Technologies & Libraries
- ✔️ **[`.NET Core 5`](https://dotnet.microsoft.com/download)** - .NET Framework and .NET Core, including ASP.NET and ASP.NET Core
- ✔️ **[`MVC Versioning API`](https://github.com/microsoft/aspnet-api-versioning)** - Set of libraries which add service API versioning to ASP.NET Web API, OData with ASP.NET Web API, and ASP.NET Core
- ✔️ **[`EF Core`](https://github.com/dotnet/efcore)** - Modern object-database mapper for .NET. It supports LINQ queries, change tracking, updates, and schema migrations
- ✔️ **[`FluentValidation`](https://github.com/FluentValidation/FluentValidation)** - Popular .NET validation library for building strongly-typed validation rules
- ✔️ **[`Swagger & Swagger UI`](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)** - Swagger tools for documenting API's built on ASP.NET Core
- ✔️ **[`Serilog`](https://github.com/serilog/serilog)** - Simple .NET logging with fully-structured events
- ✔️ **[`Reffit`](https://github.com/paulcbetts/refit)** - The automatic type-safe REST library for Xamarin and .NET.NET Standard 1.1 and .NET Framework 4.5 and higher, which is simple and customisable
- ✔️ **[`Polly`](https://github.com/App-vNext/Polly)** - Polly is a .NET resilience and transient-fault-handling library that allows developers to express policies such as Retry, Circuit Breaker, Timeout, Bulkhead Isolation, and Fallback in a fluent and thread-safe manner
- ✔️ **[`Scrutor`](https://github.com/khellang/Scrutor)** - Assembly scanning and decoration extensions for Microsoft.Extensions.DependencyInjection
- ✔️ **[`Opentelemetry-dotnet`](https://github.com/open-telemetry/opentelemetry-dotnet)** - The OpenTelemetry .NET Client
- ✔️ **[`BFF`](https://github.com/DuendeSoftware/BFF)** - Framework for ASP.NET Core to secure SPAs using the Backend-for-Frontend (BFF) pattern
- ✔️ **[`DbUp`](https://github.com/DbUp/DbUp)** - .NET library that helps you to deploy changes to SQL Server databases. It tracks which SQL scripts have been run already, and runs the change scripts that are needed to get your database up to date.
- ✔️ **[`Hangfire`](https://github.com/HangfireIO/Hangfire)** - Easy way to perform fire-and-forget, delayed and recurring tasks inside ASP.NET apps
- ✔️ **[`EasyCaching`](https://github.com/dotnetcore/EasyCaching)** - Open source caching library that contains basic usages and some advanced usages of caching which can help us to handle caching more easier.
- ✔️ **[`AutoMapper`](https://github.com/AutoMapper/AutoMapper)** - Convention-based object-object mapper in .NET.
- ✔️ **[`Hellang.Middleware.ProblemDetails`](https://github.com/khellang/Middleware/tree/master/src/ProblemDetails)** - A middleware for handling exception in .Net Core
- ✔️ **[`NetArchTest`](https://github.com/BenMorris/NetArchTest)** - A fluent API for .Net that can enforce architectural rules in unit tests.
- ✔️ **[`EntityFramework.Exceptions`](https://github.com/Giorgi/EntityFramework.Exceptions)** - Handle database errors easily when working with Entity Framework Core. Supports SQLServer, PostgreSQL, SQLite, Oracle and MySql
- ✔️ **[`EntityFrameworkCore.Triggered`](https://github.com/koenbeuk/EntityFrameworkCore.Triggered)** - Triggers for EFCore. Respond to changes in your DbContext before and after they are committed to the database.
## 4. Domain

### 4.1 Domain Description

**Online Store** is a simple store application that has the basic business scenario for online purchasing with some dedicated modules likes **Identity Module**, **Order Module**, **Customer Module**, **Catalog Module**, **Shipping Module**, **Vendor Module**, **Payment Module**. I will explain each module and its responsibility in separate sections

### 4.2 Identity Module
Identity module uses to authenticate and authorize users through a token. Also, this module is responsible for creating users and their corresponding roles and permission with using [.Net Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity) and [Identity Server](https://github.com/DuendeSoftware/BFF).

Each `Administrator`, `Customer` and `Vendor` is a **User**. To be a User, User Registration is required. Each User is assigned one or more User Role.

Each User Role has set of Permissions. A Permission defines whether User can invoke a particular action.

### 4.3 Catalog Module
The Catalog module presents the ability to add items to our store, It can be electronics, foods, books or anything else. Items can be grouped into categories and catalogs.

A catalog is defined as a list of items that a company showcases online. the catalog is a collection of items, which can be grouped into categories. An item can be assigned to only one category or be direct child of a catalog without any category.

A category is a container for other categories or items. Category in the catalog can have sub-categories. Categories allow building hierarchies and relationships between various items in the catalog. 

Buyer can browse the products list with supported filtering and sorting by product name and price. customer can see the detail of the product on the product list and in the detail page, can see a name, description, available product in the inventory,...

### 4.4 Customers Module
This modules is responsible for managing our customers information, track the activities and their types.

### 4.5 Vendor Module
Vendors is a special category of customers that should be considered separately.the vendor allows us to handle Vendors management in your store. 

Each product is assigned to a particular vendor whose details (including email address) are stored. When an order is placed an email is then sent to a vendor of each product in the order. The email includes the products, quantities, etc. The vendor ships the item to the customer on behalf of the merchant, who typically pays each of their vendors at the end of the month. 

Products from multiple independent vendors appear in the common product catalog and website visitors can shop at one store even if our products are supplied by different vendors.

Each vendor could be provided with an administrator panel access to manage their products, review sales reports, and order details regarding their products. Vendors can't see each other's activities.

### 4.6 Inventory Module
Inventory management is a system of stock level controlling and fulfillment centers management.

Inventory is often the largest item a business has in its current assets, meaning it must be accurately monitored. Inventory is counted and valued at the end of each accounting period to determine the company's profits or losses.
Some of features in this module are `Inventory tracking`, `Stock level controlling`, `Fulfillment center management`, `Preoder and backorder functions`

### 4.7 Order Module

The Orders Module main purpose is to store order details and manage orders created by users on client side. This module is not designed to be a full order processing system like ERP but serves as storage for customer orders details and can be synchronized with different external processing systems.
Some of this module responsibilities are `Saving orders`, `Saving order drafts`, `Ability to view and manage fulfillment, packages`, `Change discounts`

### 4.8 Payment Module
The payment module is responsible for payment process of our customer with different payment process and managing and tracking our payment history

### 4.9 Shipping Module
The Shipping Module provides the ability to extend shipping provider list with custom providers and also provides an interface and API for managing these shipping providers.

Some of shipping module capabilities are `Register Shipping methods`, `Edit Shipping method`, `Shipment details`, `Shipping settings`  

## 5. Architecture

## 6. How To Run

## 7. Contribution
The application is in development status. you are feel free to submit pull request or create the issue.

## 8. Licence
The project is under [MIT license](https://github.com/mehdihadeli/store-modular-monolith/blob/main/LICENSE).