# 🌿 PlanGuru API

**PlanGuru API** is a .NET 8.0 backend system designed to support plant enthusiasts in sharing knowledge, building communities, and accessing plant-related products.

---

## 🧱 Architecture

The project follows **Clean Architecture** principles with four distinct layers:

![Architecture Diagram Placeholder](https://github.com/user-attachments/assets/80b81608-d532-4366-ba29-625196e9c42e)

- **Domain** – Core business entities, interfaces, and domain logic  
- **Application** – Commands, queries, service interfaces, and orchestration logic  
- **Infrastructure** – Repository implementations, database access, and external integrations  
- **API Layer (PlanGuruAPI)** – Entry point with controllers, DTOs, and configuration  

---

## 🛠️ Technology Stack

- **.NET 8.0**  
- **Entity Framework Core** (In-Memory DB)  
- **MediatR** (CQRS pattern)  
- **AutoMapper** (object mapping)  
- **GraphQL** (flexible querying)  
- **REST Controllers**  
- **SignalR** (real-time communication)  
- **Swagger / OpenAPI** (API documentation)  

---

## 🌟 Core Features

PlanGuru API includes several integrated modules:

- **User Management** – Authentication, profiles, membership  
- **Plant Posts** – Content creation, interaction  
- **Comments** – Discussions, replies, voting  
- **Groups** – Community building and moderation  
- **Wiki** – Plant knowledge base and user contributions  
- **E-commerce** – Products and order management  
- **Chat System** – Real-time communication with SignalR  
- **Quiz System** – Educational quizzes  
- **Voting System** – Extensible via Strategy Pattern  

---

## 🗃️ Data Management

### Repository Pattern

Implements the **Repository Pattern** to decouple business logic from data access logic.

![Repository Diagram Placeholder](https://github.com/user-attachments/assets/bbd3f090-72f2-4ac8-a94b-9df40fc2882e)

Each domain entity has:
- An interface in the **Application Layer**
- An implementation in the **Infrastructure Layer**

---

## 🔁 Request Processing Flow

Uses the **CQRS (Command Query Responsibility Segregation)** pattern via **MediatR**:

- **Commands** handle write operations  
- **Queries** handle read operations  

This structure separates responsibilities and improves scalability and testability.

---

## 🚀 Getting Started

1. Clone the repository  
2. Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download)  
3. Open the solution in Visual Studio or your preferred IDE  
4. Build and run the project  
5. Visit `/swagger` for API docs  
6. Visit `/ui/graphql` to use the GraphQL Playground  

---

## ⚙️ Development

- Uses **In-Memory Database** for rapid development  
- Seed data is automatically populated at startup  

---

## 📄 License
---

## 👥 Contributors
---

## 📚 Recommended Wiki Pages

- 🌐 [DeepWiki Documentation](https://deepwiki.com/TriKhaiLe/PlanGuruAPI)
- 📘 [Database Design](https://github.com/TriKhaiLe/PlanGuruAPI/wiki/Database-Design)  
- 🌱 [Data Seeding](https://github.com/TriKhaiLe/PlanGuruAPI/wiki/Data-Seeding)  
- ❓ [Quiz System](https://github.com/TriKhaiLe/PlanGuruAPI/wiki/Quiz-System)
