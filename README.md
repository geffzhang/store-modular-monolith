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


# Project State

This project is currently under development, here I will list the state of each module

- Building Blocks - Under Development 🚧
- API - Under Development 🚧
- Identity Module - Under Development 🚧
- Catalog Module - Not Started 🚩
- Order Module - Not Started 🚩
- Customer Module - Not Started 🚩

# Table of Contents

- [1. The goals of this project](#1-the-goals-of-this-project)
- [2. Plan](#2-plan)
- [3. Architecture](#3-architecture)
    + [3.1. Module structure](#31-module-structure)
    + [3.2. Communications between bounded contexts](#32-communications-between-bounded-contexts)
    + [3.3. Validation](#33-validation)
    + [3.4. CQRS](#34-cqrs)
    + [3.5. The identifiers for communication between modules](#35-the-identifiers-for-communication-between-modules)
    + [3.6. API First](#36-api-first)
    + [3.7. Rich Domain Model](#37-rich-domain-model)
    + [3.8. Architecture Decisions](#38-architecture-decisions)
    + [3.9. Results from command handlers](#39-results-from-command-handlers)
    + [3.10. Architecture tests](#310-architecture-tests)
    + [3.11. Axon Framework](#311-axon-framework)
    + [3.12. Bounded context map](#312-bounded-context-map)
    + [3.13. Integration events inside application](#313-integration-events-inside-application)
    + [3.14. Technology stack](#314-technology-stack)
- [4. Contribution](#4-contribution)
- [5. Useful links](#5-useful-links)
- [6. License](#6-license)

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
- using **event storming** for extracting data model and bounded context (using miro for this purpose).
- presenting our **domain model** and our **use cases** in different diagrams.

## 2. Plan
> This project is currently under development, here I will list the state of each module.

The issues are represented in [https://github.com/mehdihadeli/store-modular-monolith/issues](https://github.com/mehdihadeli/store-modular-monolith/issues)

High-level plan is represented in the table

| Feature | Status |
| ------- | ------ |
| Modular monolith with base functionality | COMPLETED |
| Microservices |  |
| UI application |  |