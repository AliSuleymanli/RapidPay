Overview
RapidPay is a demonstration of a Clean Architecture, .NET Core solution for a payment provider’s authorization system. It includes:

Card Management (create, authorize, pay, get balance, update details)
Payment Fees (dynamic fee updates)
JWT Authentication
Idempotency (for create and pay endpoints)
Optimistic Concurrency (for updates that modify the card’s row)
Architecture
The solution follows Clean Architecture & Vertical Slice principles:

RapidPay.API

ASP.NET Core project.
Handles HTTP requests, configures services (DI), sets up authentication and swagger, etc.

RapidPay.Application

Contains vertical slices (commands/queries/handlers) and business logic.
Uses MediatR for request/response.
Contains FluentValidation validators.

RapidPay.Infrastructure

EF Core DbContext, entities, and repositories.
Services for Card logic, Payment Fees, etc.
Data seeding logic (users, initial fee record).

RapidPay.Tests

xUnit-based tests for handlers, validators, and other logic.
Setup Instructions
Clone the Repository

bash
Copy
git clone https://github.com/AliSuleymanli/RapidPay.git
cd RapidPay
Configure the Database

In appsettings.json (in RapidPay.API), update the ConnectionStrings:RapidPayConnection to point to your SQL Server or localdb instance.
Example:
json
Copy
"ConnectionStrings": {
  "RapidPayConnection": "Server=(localdb)\\mssqllocaldb;Database=RapidPayDb;Trusted_Connection=True;"
}
Apply EF Core Migrations

bash
Copy
dotnet ef database update --project RapidPay.Infrastructure --startup-project RapidPay.API
Run the Application

bash
Copy
dotnet run --project RapidPay.API
The API should be available at https://localhost:<port> (port varies).

Swagger Documentation

Navigate to https://localhost:<port>/swagger to view and test endpoints.
Seeding Data

On first run, the application seeds:
A test user (testuser / Test@123).
An initial PaymentFee record (CurrentFee = 1.0, etc.).
API Endpoints & Usage
Below are the main endpoints (all under api/Card/ except Auth):

1. Create Card
Endpoint: POST /api/Card/create
Body:
json
Copy
{
  "creditLimit": 1000
}
Idempotency: Requires X-Idempotency-Key header (string).
Response:
json
Copy
{
  "id": "GUID",
  "cardNumber": "123456789012345",
  "balance": 500,
  "creditLimit": 1000,
  "status": "Active"
}
2. Authorize Card
Endpoint: POST /api/Card/authorize
Body:
json
Copy
{
  "cardId": "GUID"
}
Response:
json
Copy
{
  "cardId": "GUID",
  "authorized": true,
  "message": "Authorization successful."
}
Checks if card is active, logs an authorization if not duplicated within 5s.
3. Pay with Card
Endpoint: POST /api/Card/pay
Body:
json
Copy
{
  "cardId": "GUID",
  "paymentAmount": 200
}
Idempotency: Requires X-Idempotency-Key header (string).
Response:
json
Copy
{
  "transactionId": "GUID",
  "cardId": "GUID",
  "paymentAmount": 200,
  "fee": 1.0,
  "newBalance": 799,
  "timestamp": "2023-01-01T12:00:00Z"
}
Deducts amount + fee, creates a transaction, updates the balance.
4. Get Card Balance
Endpoint: GET /api/Card/{cardId}/balance
Response:
json
Copy
{
  "cardId": "GUID",
  "balance": 799,
  "creditLimit": 1000,
  "availableBalance": 1799
}
5. Update Card Details
Endpoint: PUT /api/Card/update
Body:
json
Copy
{
  "cardId": "GUID",
  "newBalance": 800,
  "newCreditLimit": 1200,
  "newStatus": 2
}
Response:
json
Copy
{
  "id": "GUID",
  "cardNumber": "123456789012345",
  "balance": 800,
  "creditLimit": 1200,
  "status": 2
}
Uses an enum for status (e.g., Active=1, Inactive=2, etc.). Logs changes if any.
6. Authentication (Optional)
Endpoint: POST /api/Auth/login
Body:
json
Copy
{
  "userName": "testuser",
  "password": "Test@123"
}
Response:
json
Copy
{
  "token": "<JWT Token>",
  "user": {
    "id": "GUID",
    "userName": "testuser",
    "email": "testuser@example.com"
  }
}
For subsequent requests, include Authorization: Bearer <token> header.
Thread Safety & Concurrency
Optimistic Concurrency

Each card row has a [Timestamp] column (RowVersion) so EF Core throws DbUpdateConcurrencyException if another request updates the same row first.
In methods like PayWithCardAsync and UpdateCardDetailsAsync, we catch DbUpdateConcurrencyException and throw a domain-specific exception (e.g., CardConcurrencyException).
In-Memory & Distributed Locks

For a single instance, EF Core’s optimistic concurrency typically suffices.
If scaling to multiple instances, consider a distributed lock or advanced concurrency strategies.
Security
JWT Authentication (Optional)

You can protect endpoints with [Authorize] attributes.
A login endpoint issues tokens, which the client includes in the Authorization header.
FluentValidation

Each command has a validator ensuring valid inputs (e.g., non-empty GUID, non-negative amounts).
Custom Exceptions

e.g., CardNotFoundException, InsufficientFundsException to provide clear error handling.
Idempotency
CreateCard & PayWithCard require an X-Idempotency-Key header.
If a request is repeated with the same key, the system returns the previously stored result.
This prevents duplicate card creation or double-charging due to retries/timeouts.
Database Seeding
DataSeeder class seeds:
A test user (testuser) with a hashed password.
An initial PaymentFee record (CurrentFee = 1.0m) so the first transaction isn’t free.
Called at startup via:
csharp
Copy
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RapidPayDbContext>();
    await DataSeeder.SeedDataAsync(dbContext);
}
