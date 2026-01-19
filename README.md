# Car Dealership Management API

## Overview
A production-ready .NET 9 ASP.NET Core Web API for a comprehensive car dealership management system featuring CRUD operations, role-based access control (Admin/Customer), JWT authentication, and OTP-protected sensitive actions. The system demonstrates security best practices including purpose-specific OTP validation, input sanitization with FluentValidation, structured logging with Serilog, and comprehensive error handling.

## Features
**Authentication & Authorization**
- JWT Bearer token authentication
- Role-based authorization (Admin/Customer)
- OTP verification for sensitive operations (Register, Login, Purchase Request, Update Vehicle)

**Admin Capabilities**
- Add and update vehicles in inventory
- View all registered customers
- Process vehicle sales and purchase requests

**Customer Capabilities**
- Browse vehicles with filtering options
- View detailed vehicle information
- Submit purchase requests
- Track purchase history

**Security & Best Practices**
- One-Time Password (OTP) protection with purpose-specific validation
- Password hashing with PBKDF2
- OTP expiration handling (5 minutes)
- Input validation using FluentValidation
- Global error handling middleware
- Structured logging with Serilog

**API Documentation**
- Swagger/OpenAPI documentation
- JWT authentication integration in Swagger UI

## Acknowledgements
- Database id strategy is bad, this is known by me but I chose to do int to save time.
- Controller logic should be abstracted more into services, but not enough time to do so, most processing is done in the controllers directly except for user management and otp.
- Messy and ugly JWT configuration. Im new to .NET so security configuration was annoying.

## Technology Stack
- **.NET 9** - ASP.NET Core Web API
- **Entity Framework Core** - ORM with SQLite database
- **JWT Bearer** - Authentication
- **FluentValidation** - Request validation
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation

## Prerequisites
- .NET 9 SDK ([Download here](https://dotnet.microsoft.com/download/dotnet/9.0))

## How to Run

### Option 1: Using Docker (Recommended)

#### Prerequisites
- Docker and Docker Compose installed

#### Steps
1. **Build and run:**
   ```bash
   docker-compose up --build
   ```

2. **Access the API:**
   - API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger

3. **View OTP codes in logs:**
   ```bash
   docker-compose logs -f
   ```

4. **Stop containers:**
   ```bash
   docker-compose down
   ```

### Option 2: Running Locally

1. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

2. **Apply database migrations:**
   ```bash
   dotnet ef database update --project CarDealer.Api
   ```

3. **Run the API:**
   ```bash
   dotnet run --project CarDealer.Api
   ```

4. **Access Swagger UI:**
   - Navigate to: `https://localhost:7279/swagger` or `http://localhost:5133/swagger`

## Pre-seeded Data

### Admin User
- **Email**: `admin@cardealer.com`
- **Password**: `Admin123!`
- **Role**: Admin

### Vehicles
The system is pre-populated with 10 vehicles:
1. 2023 Toyota Camry - $28,500
2. 2022 Honda Accord - $26,800
3. 2024 Ford F-150 - $45,000
4. 2023 Chevrolet Silverado - $42,000
5. 2023 BMW X5 - $65,000
6. 2022 Mercedes-Benz C-Class - $48,000
7. 2023 Audi A4 - $44,000
8. 2024 Tesla Model 3 - $52,000
9. 2022 Nissan Altima - $24,500
10. 2023 Hyundai Sonata - $27,800

## API Endpoints

### Authentication (`/auth`)

#### Register New User
```http
POST /auth/register
Content-Type: application/json

{
  "email": "customer@example.com",
  "password": "Password123!",
  "role": "Customer"
}
```
**Response**: Returns request ID and initiates OTP generation (check logs/console)

#### Verify OTP (Complete Registration/Login)
```http
POST /auth/otp/verify
Content-Type: application/json

{
  "requestId": "guid-from-previous-step",
  "otpCode": "123456"
}
```
**Response**: Returns JWT token and user information

#### Login
```http
POST /auth/login
Content-Type: application/json

{
  "email": "admin@cardealer.com",
  "password": "Admin123!"
}
```
**Response**: Returns request ID and initiates OTP generation

### Vehicles (`/vehicles`)

#### Get All Vehicles (with filtering)
```http
GET /vehicles?make=Toyota&minPrice=20000&maxPrice=50000&status=Available
Authorization: Bearer {your-jwt-token}
```
**Query Parameters** (all optional):
- `make` - Filter by manufacturer
- `model` - Filter by model
- `minYear` - Minimum year
- `maxYear` - Maximum year
- `minPrice` - Minimum price
- `maxPrice` - Maximum price
- `color` - Filter by color
- `status` - Filter by status (Available/Sold/Reserved)

#### Get Vehicle by ID
```http
GET /vehicles/{id}
Authorization: Bearer {your-jwt-token}
```

#### Add Vehicle (Admin Only)
```http
POST /vehicles
Authorization: Bearer {your-jwt-token}
Content-Type: application/json

{
  "make": "Tesla",
  "model": "Model Y",
  "year": 2024,
  "price": 55000,
  "mileage": 0,
  "color": "Red",
  "status": "Available"
}
```

#### Update Vehicle (Admin Only - Requires OTP)
```http
PUT /vehicles/{id}
Authorization: Bearer {your-jwt-token}
Content-Type: application/json

{
  "make": "Tesla",
  "model": "Model Y",
  "year": 2024,
  "price": 52000,
  "mileage": 100,
  "color": "Red",
  "status": "Available"
}
```
**Response**: Returns request ID for OTP verification. Complete with `/auth/otp/verify`.

### Customers (`/customers`) - Admin Only

#### Get All Customers
```http
GET /customers
Authorization: Bearer {your-jwt-token}
```

### Purchases (`/purchases`)

#### Request Purchase (Customer Only - Requires OTP)
```http
POST /purchases/request
Authorization: Bearer {your-jwt-token}
Content-Type: application/json

{
  "vehicleId": 1
}
```
**Response**: Returns request ID for OTP verification. Complete with `/auth/otp/verify`.

#### Get Purchase History (Customer Only)
```http
GET /purchases/history
Authorization: Bearer {your-jwt-token}
```

### Sales (`/sales`) - Admin Only

#### Process Sale
```http
POST /sales/{requestId}/process
Authorization: Bearer {your-jwt-token}
```
Approves a purchase request and marks the vehicle as sold.

## OTP Flow

The system implements a comprehensive OTP flow for sensitive operations:

### 1. **Initiate Action**
Call the endpoint for the action you want to perform (register, login, purchase, update vehicle).

### 2. **OTP Generation**
- System generates a 6-digit OTP code
- OTP is hashed (PBKDF2) before storage
- Metadata includes: purpose, user ID/email, expiration time (5 minutes)
- OTP is logged to console and log files (simulated delivery)

### 3. **Check Logs for OTP**
Look for output like:
```
[OTP] Generated OTP for admin@cardealer.com
Purpose: Login
Code: 123456
Expires: 2026-01-19 10:25:00 UTC
```

### 4. **Verify OTP**
Submit OTP via `/auth/otp/verify` endpoint with:
- `requestId`: The GUID returned from step 1
- `otpCode`: The 6-digit code from logs

### 5. **Validation**
System validates:
- OTP exists and matches the request ID
- OTP hasn't expired (5-minute window)
- OTP code is correct (constant-time comparison)
- Purpose matches the original action

### 6. **Completion**
- For auth operations: Returns JWT token
- For other operations: Completes the requested action

## Data Model

### User
```csharp
{
  "id": int,
  "email": string,
  "passwordHash": string,
  "role": string, // "Admin" or "Customer"
  "createdAt": DateTime
}
```

### Vehicle
```csharp
{
  "id": int,
  "make": string,
  "model": string,
  "year": int,
  "price": decimal,
  "mileage": int,
  "color": string,
  "status": string, // "Available", "Sold", "Reserved"
  "createdAt": DateTime,
  "updatedAt": DateTime?
}
```

### PurchaseRequest
```csharp
{
  "id": int,
  "userId": int,
  "vehicleId": int,
  "status": string, // "Pending", "Approved", "Rejected"
  "requestDate": DateTime,
  "processedDate": DateTime?
}
```

### Sale
```csharp
{
  "id": int,
  "vehicleId": int,
  "customerId": int,
  "purchaseRequestId": int,
  "saleDate": DateTime,
  "salePrice": decimal
}
```

### OtpEntry
```csharp
{
  "id": int,
  "requestId": Guid, // Unique identifier for the OTP request
  "userId": int?,
  "email": string?,
  "otpCodeHash": string,
  "purpose": string, // "Register", "Login", "PurchaseRequest", "UpdateVehicle"
  "expiresAt": DateTime,
  "isVerified": bool,
  "createdAt": DateTime
}
```

## Configuration

Configuration is managed through `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=cardealer.db"
  },
  "Jwt": {
    "Secret": "YourSuperSecretKeyWithAtLeast32Characters!",
    "Issuer": "CarDealerApi",
    "Audience": "CarDealerClient",
    "ExpirationMinutes": 60
  },
  "Otp": {
    "ExpirationMinutes": 5,
    "CodeLength": 6
  }
}
```

**Security Note**: Change the JWT secret before deploying to production!

## Error Handling

The API implements comprehensive error handling:
- **Global Exception Middleware**: Catches unhandled exceptions
- **Validation Errors**: Returns structured 400 Bad Request responses
- **Authorization Errors**: Returns 401/403 responses
- **Not Found**: Returns 404 with descriptive messages
- **Server Errors**: Returns 500 with error tracking

Error Response Format:
```json
{
  "error": "Descriptive error message",
  "details": "Additional details if available"
}
```

## Logging

Structured logging with Serilog:
- **Console Sink**: Real-time log output
- **File Sink**: Rolling daily log files in `logs/` directory
- **Log Levels**: Information, Warning, Error

Logs include:
- HTTP request/response details
- OTP generation and verification attempts
- Database operations
- Authentication events
- Error stack traces

## Testing the API

### Quick Start Workflow

1. **Start the API** and access Swagger at `https://localhost:7279/swagger`

2. **Login as Admin**:
   - Call `POST /auth/login` with admin credentials
   - Check console/logs for OTP code
   - Call `POST /auth/otp/verify` with request ID and OTP
   - Copy the JWT token from response

3. **Authorize in Swagger**:
   - Click "Authorize" button in Swagger UI
   - Enter: `Bearer {your-token}`
   - Click "Authorize"

4. **Test Admin Operations**:
   - View all customers: `GET /customers`
   - Add a vehicle: `POST /vehicles`
   - Update a vehicle: `PUT /vehicles/{id}` (requires OTP)

5. **Register a Customer**:
   - Call `POST /auth/register` with customer role
   - Verify OTP to complete registration

6. **Test Customer Operations**:
   - Browse vehicles: `GET /vehicles`
   - Request purchase: `POST /purchases/request` (requires OTP)
   - View history: `GET /purchases/history`

7. **Process Sale (as Admin)**:
   - Call `POST /sales/{requestId}/process`

## Design Decisions & Assumptions

### Security
- **Password Hashing**: PBKDF2 with 100,000 iterations for secure storage
- **OTP Hashing**: OTPs are never stored in plain text
- **JWT Tokens**: 60-minute expiration with role-based claims
- **Constant-Time Comparison**: Prevents timing attacks on OTP verification

### OTP Implementation
- **Purpose-Specific**: Each OTP is tied to a specific action (Register, Login, PurchaseRequest, UpdateVehicle)
- **Expiration**: 5-minute validity window to balance security and usability
- **Simulated Delivery**: Console/log output instead of actual SMS/email integration
- **One-Time Use**: OTPs are marked as verified after successful use

### Database
- **SQLite**: Lightweight, file-based database suitable for development and demonstration
- **EF Core Migrations**: Version-controlled schema changes
- **Seeding**: Automatic population of initial data on first run

### API Design
- **RESTful Principles**: Resource-based URLs, standard HTTP methods
- **Filtering**: Query parameters for flexible vehicle search
- **Role-Based Authorization**: Attributes on controller actions
- **Validation**: FluentValidation for complex validation rules

### Error Handling
- **Global Middleware**: Centralized exception handling
- **Structured Responses**: Consistent error format across all endpoints
- **Logging**: Comprehensive error tracking with Serilog

## Project Structure

```
CarDealer.Api/
├── Controllers/          # API endpoints
├── Data/                 # DbContext and seeding
├── Domain/Entities/      # Data models
├── DTOs/                 # Request/Response objects with validators
├── Services/             # Business logic (JWT, OTP, User)
├── Middleware/           # Error handling middleware
├── Migrations/           # EF Core migrations
├── logs/                 # Application logs
└── appsettings.json      # Configuration
```

## Additional Files

- **swagger.json**: OpenAPI specification export (made with swagger)
- **postman-collection.json**: Postman collection for testing
- **CarDealer.Api.http**: HTTP request samples for Visual Studio

## Docker Architecture

- **Multi-stage build**: Optimizes image size by separating build and runtime stages
- **Persistent volumes**: Database and logs survive container restarts
- **Environment variables**: Configuration can be overridden via docker-compose.yml
- **Health checks**: Automatic container health monitoring
- **Port mapping**: Container port 8080 mapped to host port 5000

## Requirements Coverage

This implementation fulfills all requirements requested:

✅ **Core Requirements**
- User Management (Register, Login)
- Admin Use Cases (Add/Update Vehicle, View Customers, Process Sale)
- Customer Use Cases (Browse/View Vehicles, Purchase Request, View History)

✅ **Technical Requirements**
- .NET 9 ASP.NET Core Web API
- SQLite relational database
- Role-based authorization (Admin/Customer)
- OTP protection for Register, Login, Purchase Request, Update Vehicle
- Complete OTP flow (generation, validation, expiration, storage)
- Simulated OTP delivery (console output)
- Pre-populated with 10+ vehicles and admin user
- Proper error handling
- Input validation
- API documentation

✅ **Bonus Points**
- Swagger/OpenAPI documentation
- Configuration management (appsettings.json)
- Logging implementation (Serilog)
- Input sanitization (FluentValidation)
- Docker containerization
