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
- [5. Event Storming](#5-event-storming)
- [6. Architecture](#6-architecture)
  - [6.1. Project Structure](#61-project-structure)
  - [6.2 Modules Structure](#62-module-structure)
  - [6.3 API Gateway Configuration](#63-api-gateway-configuration)
  - [6.4 Initializing Modules](#64-initializing-modules)
  - [6.5 Communications Between Bounded Contexts Or Modules](#65-communications-between-bounded-contexts-or-modules)
  - [6.6 CQRS](#6.6-cqrs)
    - [6.6.1 CQRS Command](#661-cqrs-command)
    - [6.6.2 CQRS Query](#662-cqrs-query)
    - [6.6.3 Results From Command Handlers](#663-results-from-command-handlers)
  - [6.7 Rich Domain Model](#67-rich-domain-model)
  - [6.8 Domain Events](#68-domain-events)
  - [6.9 Integration Events](#69-integration-events)
  - [6.10 Public Events - Domain Events Notifications](#610-public-events---domain-events-notifications)
- [7. How to Run](#7-how-to-run)
- [8. Contribution](#8-contribution)
- [9. License](#9-license)

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
| Identity Module | In-Progress 👷‍♂️ |
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

## 5. Event Storming
The first thing we started with was domain exploration with the help of Big Picture EventStorming. The description you found in the previous chapter, landed on our virtual wall.

The EventStorming session led us to numerous discoveries, modeled with the sticky notes.

Event storming consist of 3 main phases:
- Big Picture EventStorming
- Example Mapping
- Design Level EventStorming

for more information about event storming you can use [this link](https://spring.io/blog/2018/04/11/event-storming-and-spring-with-a-splash-of-ddd?utm_source=pocket_mylist).

I will put some part of event storming for this project at [Miro](https://miro.com/) web site for better understanding.

## 6. Architecture

### 6.1 Project Structure
At the very beginning, not to overcomplicated the project, we decided to assign each bounded context to a separate module, which means that the system is a modular monolith. As the name of `Modular Monolith architecture` suggests, the design of our architecture must be oriented towards high modularity so the system must have self-contained modules that provide the entire business functionality. This is why `domain-centric architecture` is best choice for this case.

In this project we use `Component-Based architecture` that is a hybrid approach between `layered architecture (hexagonal architecture)` and `feature-based architecture (vertical slice architecture)`. 

Instead of having a layered approach, horizontal slices, we instead split the application vertically into modular components, just like feature-based architecture.
A component in this context is a group of related functionality that resides behind a nice and clean interface.  A "component" in this sense is a combination of the business and data access logic related to a specific thing (e.g. domain concept, bounded context, etc). I give these components a public interface and package-protected implementation details, which includes the data access code. If that new feature set C needs to access data related to A and B, it is forced to go through the public interface of components A and B (In our case our components don't have direct communication and they use message broker for their communication, read more [here](https://www.kamilgrzybek.com/design/modular-monolith-integration-styles/)). No direct access to the data access layer is allowed and "hexagonal architecture" is a secondary organization mechanism in each module.

A couple of notes to notice here:
- The controller is part of the component. Though we can split controllers from component related code to separate the handling of HTTP requests from the component itself. By the way, not every component needs a controller as an Email service.
- Components can talk to each other through their interface unlike in feature-based architecture where service classes are hidden, and the only way to communicate is through the controller.

for more information you can use these links --> [Reference 1](https://medium.com/omarelgabrys-blog/component-based-architecture-3c3c23c7e348), [Reference 2](http://www.codingthearchitecture.com/2015/03/08/package_by_component_and_architecturally_aligned_testing.html)

Let’s see how such architecture can look like from high-level view:

![package by component](assets/images/package-by-component.png)

Looking at a high-level view, we can realize that our high level system architecture (component-based architecture) including `API` and `Modules` has some similarity to a hexagonal:
- Our controllers in API level are like a `primary adapter` and our modules `public api` are like a `primary port` 
- we have some secondary ports and adapters for communicate with message broker, database and other modules.

Let’s try to describe all the elements of our high level architecture view.

#### High Level Architecture API
API is entry point our system and could be implement different adapters like Rest/GraphQL/Soap. the main responsibility api is route the request to one of our modules. this API level act as a API Gateway in microservice but instead of network call it call our modules public api locally as a in-memory call. This API as a `Module API Adapter` will initialize our modules configuration with module `IModule` port. also will share some infrastructure between all modules as a root composition dependency container. all modules dependencies will register on this root dependency container.

#### High Level Component Module
Each module should be treated as a separate application. In other words, it’s a subsystem of our system. Thanks to this, it will have autonomy. It will be loosely or even not coupled to other modules (subsystems).

Since modules should be domain oriented we can use `domain centric` architecture on the level each module. Modules communicate each other through their public interface and asynchronously using In-Memory message broker, direct method calls are not allowed. for read more about the reason refer to [this article](https://www.kamilgrzybek.com/design/modular-monolith-integration-styles/).

#### Modules Init Port
`Module init port` or [IModule](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Modules/IModule.cs) is responsible for configuration each modules and with get required configuration from API as primary adapter and will initialize the module and all its dependencies to composition root dependency container which is in api gateway and all this modules setup and decencies will part of our root container. we could also create separate dependency container for each module but here I prefer only one composition root in API Gateway level and all modules dependencies will child of this composition root.

### 6.2 Modules Structure
Inner each modules we used hexagonal architecture but we can use also [vertical slice architecture](https://jimmybogard.com/vertical-slice-architecture/) also. for learn more hexagonal architecture please follow these links [Reference 1](https://herbertograca.com/2017/11/16/explicit-architecture-01-ddd-hexagonal-onion-clean-cqrs-how-i-put-it-all-together/), [Reference 2](https://github.com/Sairyss/domain-driven-hexagon).

Our hexagonal architecture in each module consists of 4 main parts:
- **Api** - this project that contain our module init port or [IModule](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Modules/IModule.cs), gets configurations from API and will register all module dependencies on composition root container. 
- **Application** - Here you should find the implementation of use cases related to the module. the application is responsible for requests processing. Application contains use cases, domain events, integration events and its contracts, internal commands.
- **Domain** - Domain Model in Domain-Driven Design terms implements the applicable Bounded Context
- **Infrastructure** - This is where the implementation of secondary adapters should be. Secondary adapters are responsible for communication with the external dependencies.
infrastructural code responsible for module initialization, background processing, data access, communication with Events Bus and other external components or systems

### 6.3 API Gateway Configuration
API Gateway project is bootstraper of our modular monolith application and it is start point of our application. This project is our composition root for all dependency injection. our modules don't have their own composition root and all modules register their dependencies in API composition root dependency container. all modules dependencies and configuration will pass from API as primary adapter to modules port `IModule` as primary port. 

All shared dependencies and building blocks between all modules should register in API Gateway because this dependencies will inject in our composition root dependency container and all modules can access to these dependencies easily. bellow code is a sample of registering some shared dependencies between all modules. 

``` csharp
//API - Startup.cs

public void ConfigureServices(IServiceCollection services)
{
    services.AddWebApi(Configuration);
    services.AddCore(Configuration, Assemblies);
    services.AddOTelIntegration(Configuration);
    services.AddHangfireScheduler(Configuration, "hangfire");
    services.AddSwagger(typeof(ApiRoot).Assembly);
    services.AddCustomValidators(Assemblies.ToArray());
    services.AddCors(Cors);
    services.AddHealthCheck(Configuration, AppOptions.ApiAddress);
    services.AddVersioning();
    services.AddFluentValidation(x => x.RegisterValidatorsFromAssemblies(Assemblies));
    services.AddCaching(Configuration);
    services.AddInMemoryMessaging(Configuration, "messaging");
    services.AddCqrs(Assemblies.ToArray());
    services.AddFeatureManagement();
    services.AddJwtAuthentication(Configuration, Modules);

    // configure modules dependencies
    foreach (var module in Modules)
    {
        module.ConfigureServices(services);
    }
}
```

### 6.4 Initializing Modules 
Our inner components or modules will initialize by public port of each module which is [IModule](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Modules/IModule.cs) our API Gateway startup will initialize each of our modules separately with calling 3 methods:
``` csharp
void Init(IConfiguration configuration);
void ConfigureServices(IServiceCollection services);
void Configure(IApplicationBuilder app, IWebHostEnvironment environment);
```

For init configuration for all modules we should call `Init` method of our `IModule` port of each component or module.

``` csharp
//API - Startup.cs

public Startup(IConfiguration configuration)
{
    Configuration = configuration;
    Cors = Configuration.GetSection("CORS").Get<Cors>();
    AppOptions = Configuration.GetSection("AppOptions").Get<AppOptions>();

    Assemblies = ModuleLoader.LoadAssemblies(configuration);
    Modules = ModuleLoader.LoadModules(Assemblies);
    // init modules configurations
    Modules.ToList().ForEach(x => x.Init(Configuration));
}
```
For setup dependency injection of our module on composition root container we should use `ConfigureServices` method on `IModule` port of our module.

``` csharp
//API - Startup.cs

public Startup(IConfiguration configuration)
{
    Configuration = configuration;
    Cors = Configuration.GetSection("CORS").Get<Cors>();
    AppOptions = Configuration.GetSection("AppOptions").Get<AppOptions>();

    Assemblies = ModuleLoader.LoadAssemblies(configuration);
    Modules = ModuleLoader.LoadModules(Assemblies);
    // init modules configurations
    Modules.ToList().ForEach(x => x.Init(Configuration));
}
```

### 6.5 Communications Between Bounded Contexts Or Modules

Communication between bounded contexts or modules are asynchronous via message broker. Bounded contexts don't share data, it's forbidden to create a transaction which spans more than one bounded context.[read more](https://www.kamilgrzybek.com/design/modular-monolith-integration-styles/)

Using message bus reduces coupling of bounded contexts through data replication across contexts which results to higher bounded contexts independence. Event publishing/subscribing is used in this project with some infrastructural code for handling this message passing and event driven architecture.

Our message should implement [IMessage](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Messaging/IMessage.cs) and our handlers should implement [IMessageHandler<TMessage>](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Messaging/IMessageHandler.cs) interface to participate in messaging mechanism.

To publish a message to message broker we can use our [ICommandProcessor](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/ICommandProcessor.cs) or [IPublisher](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Messaging/Transport/IPublisher.cs) and then using `PublishMessageAsync<TMessage>(TMessage messgae);` method to publish message on message broker. On the other side subscriber or listener module can subscribe on published message with a handler that implement [IMessageHandler<TMessage>](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Messaging/IMessageHandler.cs) interface .

A sample of publishing and subscribing on a message:

Message definition:

``` csharp
public class TestMessage : IMessage
{
    public string Data { get; init;}
}
```

The listener for `TestMessage` message in other modules:

``` csharp
public class TestMessageHandler : IMessageHandler<TestMessage>
{
    public Task HandleAsync(TestMessage message, IMessageContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
```

For publishing message:

``` csharp
CommandProcessor.PublishMessageAsync(new Test() { Data="test" });
```

### 6.6 CQRS

CQRS principle gives the flexibility in optimizing model for read and write operations. The simple version of CQRS is implemented in this application. On write operations, full logic is executed via aggregate (write model). On read operations, Dto objects (read model) are created via queries on repository level or directly on database context like ef core dbcontext. 


#### 6.6.1 CQRS Command

Our command should inherits from [ICommand<TResult>](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Cqrs/Commands/ICommand.cs) if our command will return a value and [ICommand](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Cqrs/Commands/ICommand.cs) if our command doesn't have a value. returning value from command is a trade-off in next section I will explain this case. for handling command our handler should implement [ICommandHandler<in TCommand, TResult>](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Cqrs/Commands/CommandHandler.cs) or [ICommandHandler<in TCommand>](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Cqrs/Commands/CommandHandler.cs).

For sending a command we can use `SendCommandAsync<T, TResult>(T command)` method of [ICommandProcessor](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/ICommandProcessor.cs) or we can use `Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)` method of our [IMediator](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Cqrs/IMediator.cs).

Example of a command handler:

command definition:

``` csharp
public class RegisterNewUserCommand : ICommand
{
    public RegisterNewUserCommand(string email, string firstName, string lastName,
        string name, string userName, string phoneNumber,string password,
        IReadOnlyList<string> permissions, UserType userType, bool isAdmin = false, bool isActive = true,
        IReadOnlyList<string>? roles = null, bool locked = false, bool emailConfirmed = false,
        string? photoUrl = null, string? status = null)
    {
        UserName = userName;
        IsActive = true;
        Email = email.ToLowerInvariant();
        FirstName = firstName;
        LastName = lastName;
        Name = name.Trim();
        UserType = userType;
        Password = password;
        PhoneNumber = phoneNumber;
        LockoutEnabled = locked;
        Roles = roles;
        Permissions = permissions;
        EmailConfirmed = emailConfirmed;
        PhotoUrl = photoUrl;
        Status = status;
        IsAdministrator = isAdmin;
        IsActive = isActive;
    }

    public string UserName { get; }
    public bool EmailConfirmed { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string PhoneNumber { get; }
    public string Name { get; }
    public bool IsAdministrator { get; }
    public string? PhotoUrl { get; }
    public UserType UserType { get; }
    public string? Status { get; }
    public string Password { get; }
    public bool LockoutEnabled { get; }
    public bool IsActive { get; }
    public IEnumerable<string>? Roles { get; }
    public IEnumerable<string>? Permissions { get; }
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CorrelationId { get; set; }
    public DateTime OccurredOn { get; set; } = DateTime.Now;
}
```

Our command handler definition:

``` csharp
public class RegisterNewUserCommandHandler : ICommandHandler<RegisterNewUserCommand>,
    IRetryableRequest<RegisterNewUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RegisterNewUserCommandHandler> _logger;
    private readonly ICommandProcessor _commandProcessor;
    private readonly ISqlDbContext _dbContext;

    public RegisterNewUserCommandHandler(IUserRepository userRepository,
        ILogger<RegisterNewUserCommandHandler> logger,
        ICommandProcessor commandProcessor,
        ISqlDbContext dbContext)
    {
        _userRepository = userRepository;
        _logger = logger;
        _commandProcessor = commandProcessor;
        _dbContext = dbContext;
    }

    public async Task<Unit> HandleAsync(RegisterNewUserCommand command,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command, nameof(RegisterNewUserCommand));
        User.CheckEmailValidity(command.Email);
        User.CheckEmailValidity(command.Email);
        User.CheckNameValidity(command.Name);

        var user = await _userRepository.FindByEmailAsync(command.Email);
        if (user is { })
        {
            _logger.LogError($"Email '{command.Email}' already in used");
            throw new EmailAlreadyInUsedException(command.Email);
        }

        user = await _userRepository.FindByNameAsync(command.Name);
        if (user is { })
        {
            _logger.LogError($"UserName '{command.Name}' already in used");
            throw new UserNameAlreadyInUseException(command.Name);
        }

        user = User.Of(command.Id,
            command.Email,
            command.FirstName,
            command.LastName,
            command.Name,
            command.UserName,
            command.PhoneNumber,
            command.Password,
            command.UserType,
            command.IsAdministrator,
            command.IsActive,
            command.LockoutEnabled,
            command.EmailConfirmed,
            command.PhotoUrl,
            command.Status);

        user.AssignPermission(command.Permissions?.Select(x => Permission.Of(x, "")).ToArray());
        user.AssignRole(command.Roles?.Select(x => Role.Of(x, x)).ToArray());

        await _commandProcessor.HandleTransactionAsync(_dbContext, user.Events?.ToList(), async () =>
        {
            await _userRepository.AddAsync(user);
            _logger.LogInformation($"Created an account for the user with ID: '{user.Id}'.");
        });

        var domainEvents = user.Events.ToArray();
        await _commandProcessor.PublishDomainEventAsync(domainEvents); 
        
        // Publish some integration event to message broker to consume by the subscriber modules
        await _commandProcessor.PublishMessageAsync(new NewUserRegisteredIntegrationEvent(user.Id.Id, user.UserName, user.Email,
         user.FirstName, user.LastName, user.Name));

        return Unit.Result;
    }
}
```

Now for sending our command we could use bellow code:

``` csharp
await commandProcessor.SendCommandAsync(registerNewUserCommand);

or 

await mediator.Send(registerNewUserCommand);
```

#### 6.6.2 CQRS Query

Our query should inherits from [IQuery<out TResult>](src/BuildingBlocks/BuildingBlocks/uildingBlocks.Cqrs/Queries/IQuery.cs) and `TResult` is our query output which is a `Dto` object actually. for handling Query our handler should implement [IQueryHandler<in TCommand, TResult>](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Cqrs/Commands/CommandHandler.cs) or [IQueryHandler<in TQuery, TResponse>](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Cqrs/Queries/QueryHandler.cs).

For sending a query we can use `Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)` method of our [IMediator](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Cqrs/IMediator.cs).

Example of query implementation with constructing Dto object inside repository:

Our query definition:

``` csharp
public class GetUserByIdQuery : IQuery<UserDto>
{
    public GetUserByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
```

Our query handler definition:

``` csharp
public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByIdAsync(query.Id.ToString());

        return _mapper.Map<UserDto>(user);
    }
}
```

Now for sending our query we could use bellow code:

``` csharp
await mediator.Send(getUserByIdQuery);
```

#### 6.6.3 Results From Command Handlers
The idea from CQRS, do not return anything from command processing. But in some cases, we need to get generated identifiers of new created resources. So as trade-off, command handlers can return generated identifiers after processing if it's needed.


### 6.7 Rich Domain Model
Rich domain model solution is used in this project. Domain model encapsulates internal structure and logic.

If our domain model is a Aggregate it should inherits from [AggregateRoot](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Domain/Types/AggregateRoot.cs) and If it is not a aggregate it should inherits from [EntityBase](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Domain/Types/EntityBase.cs) and our value objects should inherits from [ValueObject](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Domain/Types/ValueObject.cs).

An Implementation of a rich domain model:

``` csharp
public class User : AggregateRoot<Guid, UserId>
{
    // Using a private collection field, better for DDD Aggregate's encapsulation
    private readonly List<Role> _roles = new();
    private readonly List<RefreshToken> _refreshTokens = new();
    private readonly List<Permission> _permissions = new();
    public string UserName { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Name { get; private set; }
    public bool IsAdministrator { get; private set; }
    public string? PhotoUrl { get; private set; }
    public UserType UserType { get; private set; }
    public string? Status { get; private set; }
    public string Password { get; private set; }
    public string PhoneNumber { get; private set; }
    public bool LockoutEnabled { get; private set; }
    public bool IsActive { get; private set; }
    public bool PasswordExpired { get; private set; }
    public DateTime? LastPasswordChangedDate { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? ModifiedDate { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? ModifiedBy { get; private set; }
    public IReadOnlyList<Role> Roles => _roles.AsReadOnly();
    public IReadOnlyList<Permission> Permissions => _permissions.AsReadOnly();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    private User(UserId id,
        string email,
        string firstName,
        string lastName,
        string name,
        string userName,
        string phoneNumber,
        string password,
        UserType userType,
        bool isAdmin = false,
        bool isActive = true,
        bool locked = false,
        bool emailConfirmed = false,
        string? photoUrl = null,
        string? status = null,
        string? createdBy = null,
        DateTime? createdDate = null,
        string? modifiedBy = null,
        DateTime? modifiedDate = null)
    {
        CheckEmailValidity(email);
        CheckNameValidity(name);

        UserName = userName;
        PhoneNumber = phoneNumber;
        IsActive = true;
        Id = id;
        Email = email.ToLowerInvariant();
        FirstName = firstName;
        LastName = lastName;
        Name = name.Trim();
        UserType = userType;
        Password = password;
        LockoutEnabled = locked;
        EmailConfirmed = emailConfirmed;
        PhotoUrl = photoUrl;
        Status = status;
        IsAdministrator = isAdmin;
        IsActive = isActive;
        CreatedDate = createdDate ?? DateTime.Now;
        CreatedBy = createdBy;
        ModifiedBy = modifiedBy;
        ModifiedDate = modifiedDate;
    }
    
    public static User Of(UserId id,
        string email,
        string firstName,
        string lastName,
        string name,
        string userName,
        string phoneNumber,
        string password,
        UserType userType,
        bool isAdmin = false,
        bool isActive = true,
        bool locked = false,
        bool emailConfirmed = false,
        string? photoUrl = null,
        string? status = null,
        string? createdBy = null,
        DateTime? createdDate = null,
        string? modifiedBy = null,
        DateTime? modifiedDate = null)
    {
        var user = new User(id,
            email,
            firstName,
            lastName,
            name,
            userName,
            phoneNumber,
            password,
            userType,
            isAdmin,
            isActive,
            locked,
            emailConfirmed,
            photoUrl,
            status,
            createdBy,
            createdDate,
            modifiedBy,
            modifiedDate);

        user.AddDomainEvent(new NewUserRegisteredDomainEvent(user));
        
        return user;
    }

    #region Domain Operations
    public void AssignRole(params Role[]? roles)
    {
        if (roles is null)
            throw new Exception("Roles can't be null.");

        foreach (var role in roles)
        {
            var exists = _roles.Contains(role);
            if (!exists) _roles.Add(role);
        }
    }
    public void AssignRefreshToken(params RefreshToken[]? refreshTokens)
    {
        if (refreshTokens is null)
            throw new Exception("RefreshTokens can't be null.");

        foreach (var refreshToken in refreshTokens)
        {
            var exists = _refreshTokens.Contains(refreshToken);
            if (!exists) _refreshTokens.Add(refreshToken);
        }
    }

    public void AssignPermission(params Permission[]? permissions)
    {
        if (permissions is null)
            throw new Exception("Permissions can't be null.");

        foreach (var permission in permissions)
        {
            var exists = _permissions.Contains(permission);
            if (!exists) _permissions.Add(permission);
        }
    }

    public virtual void Patch(User target)
    {
    }

    public static void CheckEmailValidity(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidEmailException(email);

        if (!RegexConstants.Email.IsMatch(email))
            throw new InvalidEmailException(email);
    }
    public bool HasValidRefreshToken(string refreshToken)
    {
        return _refreshTokens.Any(rt => rt.Token == refreshToken && rt.IsActive);
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        _refreshTokens.Add(refreshToken);
    }

    public void RemoveRefreshToken(string refreshToken)
    {
        _refreshTokens.Remove(_refreshTokens.First(t => t.Token == refreshToken));
    }
    #endregion

    #region Domain Invariants

    public static void CheckNameValidity(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidNameException(name);
    }

    #endregion
}
```

## 6.8 Domain Events
A domain event is a side effect of executing a command or something that happened in particular domain,We create Domain Events to notify other parts of the same domain that something interesting happened and these other parts potentially can react to. Domain Event is usually immutable data-container class named in the past tense.

Our domain events should inherits from [IDomainEvent](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Domain/DomainEvents/IDomainEvent.cs) and our domain event handler should implement [IDomainEventHandler<in TDomainEvent>](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Domain/DomainEvents/DomainEventHandler.cs)

Example of a domain event:

``` csharp
// NewUserRegisteredDomainEvent.cs

public class NewUserRegisteredDomainEvent : IDomainEvent
{
    public NewUserRegisteredDomainEvent(User user)
    {
        User = user;
    }

    public User User { get; set;}
}


// NewUserRegisteredDomainEventHandler.cs
public class NewUserRegisteredDomainEventHandler: IDomainEventHandler<NewUserRegisteredDomainEvent>
{
    public Task HandleAsync(NewUserRegisteredDomainEvent domainEvent)
    {
        return Task.CompletedTask;
    }
}
```

## 6.9 Integration Events
In ddd when we want to communicate with external boundaries, we should use integration event. This message or integration event should contain only required data, and it should be as thin and small as possible. we should publish a message to our messaging system (message broker) as a publisher and all subscribers in other modules or services can subscribe on this integration event and perform their desire operations.

Our integration event should implement [IIntegrationEvent](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Domain/IntegrationEvents/IIntegrationEvent.cs) and all of our subscriber should implement [IIntegrationEventHandler<in TMessage>](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Domain/IntegrationEvents/IIntegrationEventHandler.cs).


``` csharp

// NewUserRegisteredIntegrationEvent.cs  ---> our integration event

public class NewUserRegisteredIntegrationEvent : IIntegrationEvent
{
    public Guid UserId { get; }
    public string Login { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Name { get; }

    public NewUserRegisteredIntegrationEvent(Guid userId, string login, string email, string firstName,
        string lastName, string name)
    {
        UserId = userId;
        Login = login;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Name = name;
    }
}

// RegisterNewUserCommandHandler.cs  ---> to publishing our integration event

public class RegisterNewUserCommandHandler : ICommandHandler<RegisterNewUserCommand>,
    IRetryableRequest<RegisterNewUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RegisterNewUserCommandHandler> _logger;
    private readonly ICommandProcessor _commandProcessor;
    private readonly ISqlDbContext _dbContext;

    public RegisterNewUserCommandHandler(IUserRepository userRepository,
        ILogger<RegisterNewUserCommandHandler> logger,
        ICommandProcessor commandProcessor,
        ISqlDbContext dbContext)
    {
        _userRepository = userRepository;
        _logger = logger;
        _commandProcessor = commandProcessor;
        _dbContext = dbContext;
    }

    public async Task<Unit> HandleAsync(RegisterNewUserCommand command,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command, nameof(RegisterNewUserCommand));
        User.CheckEmailValidity(command.Email);
        User.CheckEmailValidity(command.Email);
        User.CheckNameValidity(command.Name);

        var user = await _userRepository.FindByEmailAsync(command.Email);
        if (user is { })
        {
            _logger.LogError($"Email '{command.Email}' already in used");
            throw new EmailAlreadyInUsedException(command.Email);
        }

        user = await _userRepository.FindByNameAsync(command.Name);
        if (user is { })
        {
            _logger.LogError($"UserName '{command.Name}' already in used");
            throw new UserNameAlreadyInUseException(command.Name);
        }

        user = User.Of(command.Id,
            command.Email,
            command.FirstName,
            command.LastName,
            command.Name,
            command.UserName,
            command.PhoneNumber,
            command.Password,
            command.UserType,
            command.IsAdministrator,
            command.IsActive,
            command.LockoutEnabled,
            command.EmailConfirmed,
            command.PhotoUrl,
            command.Status);

        user.AssignPermission(command.Permissions?.Select(x => Permission.Of(x, "")).ToArray());
        user.AssignRole(command.Roles?.Select(x => Role.Of(x, x)).ToArray());

        await _commandProcessor.HandleTransactionAsync(_dbContext, user.Events?.ToList(), async () =>
        {
            await _userRepository.AddAsync(user);
            _logger.LogInformation($"Created an account for the user with ID: '{user.Id}'.");
        });

        var domainEvents = user.Events.ToArray();
        await _commandProcessor.PublishDomainEventAsync(domainEvents); 
        
        // Publish some integration event to message broker to consume by the subscriber modules
        await _commandProcessor.PublishMessageAsync(new NewUserRegisteredIntegrationEvent(user.Id.Id, user.UserName, user.Email,
         user.FirstName, user.LastName, user.Name));

        return Unit.Result;
    }
}
```

The listener for `NewUserRegisteredIntegrationEvent` integration event in other module:

``` csharp
// NewUserRegisteredIntegrationEventHandler.cs ---> our integration event subscriber

public class NewUserRegisteredIntegrationEventHandler : IIntegrationEventHandler<NewUserRegisteredIntegrationEvent>
{
    public Task HandleAsync(NewUserRegisteredIntegrationEvent @event, IMessageContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
```

## 6.10 Public Events - Domain Events Notifications

it is notification that domain event was published. If I want to inform my application that domain event was published I create notification class for it and as many handlers for this notification as I want. I always publish my notifications after transaction is committed. 

The complete process of executing a command handler looks like this:

1. Command Handler defines transaction boundary, Transaction is started when Command Handler is invoked and committed at the end
2. Get aggregate(s).
3. Invoke aggregate method.
4. Add domain events to Events collections in our domain (aggregate).
5. Publish domain events and handle them in end of our handler before committing transaction.
6. Save domain event notification (public event) in outbox
7. Save changes to DB and commit transaction.
8. Read public event from outbox and publish domain events notifications and handle them.
 
What happened when we want to execute something **outside this transaction**, like publishing a message to a message broker or calling a third-party service or sending email?

Because we have no control over external systems we can save our domain event notification on the same transaction as our command handler with an outbox pattern and after committing transaction we will pick our notification events from outbox in a background service and try to publish this notification event and execute their handler. If these notification events execute successfully, they are marked as proceed, else they retry again until they can execute successfully.

How do I know that particular domain event was published? 

Our public event should implement [IDomainEventNotification<TDomainEventType>](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Domain/DomainEventNotifications/IDomainEventNotification.cs) and their handlers should implement [IDomainEventNotificationHandler<in TDomainEventNotification>](src/BuildingBlocks/BuildingBlocks/BuildingBlocks.Core/Domain/DomainEventNotifications/IDomainEventNotificationHandler.cs).

For ensuring our `NewUserRegisteredDomainEvent` is published we create a `NewUserRegisteredNotification` event when the command handler publishes our domain events at the end of our command handler, all domain event notifications related to our domain events will save in the outbox, and the transaction will commit in end of handler. then our notification event publishes from outbox separately in a background service and all notification handlers will handle. Now in all our domain event notification handlers, we can do everything we need to execute out of our command handler transaction like publish a message to the broker or sending an email with a third party service. Here we can change of retry because of using the outbox pattern.

Here is a sample of this process:

``` csharp
// NewUserRegisteredNotification.cs

public class NewUserRegisteredNotification : DomainNotificationEventBase<NewUserRegisteredDomainEvent>
{
    public NewUserRegisteredNotification(NewUserRegisteredDomainEvent domainEvent, Guid id, Guid correlationId) : base(domainEvent, id,
        correlationId)
    {
    }
}


// NewUserRegisteredPublishEventHandler.cs

public class NewUserRegisteredPublishEventHandler : IDomainEventNotificationHandler<NewUserRegisteredNotification>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly IUserDomainEventsToIntegrationEventsMapper _userDomainToIntegrationEventMapper;
    
    public NewUserRegisteredPublishEventHandler(ICommandProcessor commandProcessor,
        IUserDomainEventsToIntegrationEventsMapper userDomainToIntegrationEventMapper)
    {
        _commandProcessor = commandProcessor;
        _userDomainToIntegrationEventMapper = userDomainToIntegrationEventMapper;
    }

    public async Task HandleAsync(NewUserRegisteredNotification notification)
    {
        var integrationEvents = _userDomainToIntegrationEventMapper.Map(notification.DomainEvent).ToArray();
        foreach (var integrationEvent in integrationEvents)
        {
            await _commandProcessor.PublishMessageAsync(integrationEvent);
        }
    }
}

// NewUserRegisteredSendEmailConfirmationHandler.cs

public class NewUserRegisteredSendEmailConfirmationHandler : IDomainEventNotificationHandler<NewUserRegisteredNotification>
{
    private readonly IMessagesScheduler _messagesScheduler;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public NewUserRegisteredSendEmailConfirmationHandler(IMessagesScheduler messagesScheduler,
        IHttpContextAccessor httpContextAccessor)
    {
        _messagesScheduler = messagesScheduler;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task HandleAsync(NewUserRegisteredNotification @event)
    {
        await _messagesScheduler.Enqueue(new SendVerificationEmailCommand(@event.DomainEvent.User.Id.Id.ToString(),
            _httpContextAccessor.HttpContext?.Request.Scheme,
            _httpContextAccessor.HttpContext?.Request.Host.Value));
    }
}
```
For more information see these articles [Reference 1](http://www.kamilgrzybek.com/design/how-to-publish-and-handle-domain-events/),[Reference 2](http://www.kamilgrzybek.com/design/handling-domain-events-missing-part/).

## 7. How To Run

## 8. Contribution
The application is in development status. you are feel free to submit pull request or create the issue.

## 9. Licence
The project is under [MIT license](https://github.com/mehdihadeli/store-modular-monolith/blob/main/LICENSE).