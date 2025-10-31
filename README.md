# Support Chat System

A backend microservice for managing support chat sessions and dynamically assigning agents using queueing logic, background monitoring, and clean architecture principles.

------------------------------------------------------------------------

## Solution Structure

ChatSupport
│
├── API → Web API layer exposing endpoints (Swagger)
├── Application → Core business logic, services, and interfaces
├── Domain → Entities, enums, and core domain rules
├── Infrastructure → EF Core, MongoDB, repositories, background services
│
├── API.Tests → Unit tests for API layer (controllers)
├── Application.Tests → Unit tests for application services
└── Infrastructure.Tests → Unit tests for repositories and infrastructure logic

---------------------------------------------------------------------------
### Implemented Requirements

- Developed a **Support Chat microservice** to manage chat sessions and agent assignment.
- Implemented **queueing logic** with **round-robin selection** based on agent seniority.
- Added a **polling mechanism** and background monitoring via `QueueMonitorService`.
- Integrated **SQL Server (EF Core)** for structured data (agents, teams).
- Integrated **MongoDB** for chat sessions, with an **in-memory fallback** for local runs.
- Applied **dependency injection**, **options pattern**, and configuration-based setup.
- Implemented **ChatController** with endpoints for:
  - Creating chat sessions  
  - Polling active sessions  
  - Retrieving all sessions

------------------------------------------------------------------------------
#### Additional Enhancements

- Added an **AdminController** *(extra feature)* to fetch all agents and teams.
- Enhanced internal **queueing system** for local development and testing — designed to be easily replaceable with **RabbitMQ** in production for scalability.
- Added **data seeding** for agents and teams during startup.
- Integrated **Swagger UI** for API testing and documentation.
- Implemented **comprehensive unit tests** for:
  - Controllers (API layer)
  - Services (Application layer)
  - Repositories (Infrastructure layer)
- Applied **Clean Architecture principles** (separation of concerns between layers).
-------------------------------------------------------------------------------------
##### Why EF Core and MongoDB?

- **EF Core (SQL Server)**  
  Used for structured, relational data such as agents, teams, and assignments — ensuring consistency and data integrity.  

- **MongoDB**  
  Used for flexible, unstructured data such as chat sessions and messages — providing scalability and performance benefits.  

- This hybrid design combines **consistency (SQL)** with **speed and scalability (Mongo)**, reflecting a real-world microservice environment.

------------------------------------------------------------------------------------------------
## Technologies Used

- **.NET 8 / C#**
- **Entity Framework Core**
- **MongoDB**
- **xUnit / Moq** for unit testing
- **Swagger / OpenAPI**
- **Dependency Injection**
- **Background Services**
- **In-Memory Database** for local tests
---------------------------------------------------------------------------------------------------------

## Running the Project

### Using Visual Studio

1. Set the **API** project as the startup project.  
2. Run the solution (press **F5**).  
3. Swagger UI will open automatically at:  
   https://localhost:5001/swagger  

### Without MongoDB
If MongoDB is not running locally, the project automatically uses an **in-memory fallback** by setting this in `appsettings.json`:
"UseInMemoryMongo": true

-------------------------------------------------------------------------------------------------------------

## Future Improvements

Replace in-memory queue with RabbitMQ for distributed scalability.

Add authentication and authorization (JWT).

Introduce caching (Redis) for active sessions.

