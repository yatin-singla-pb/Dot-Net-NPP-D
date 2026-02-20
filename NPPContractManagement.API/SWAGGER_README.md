# NPP Contract Management API - Swagger Documentation

## 🚀 Swagger UI Access

Once you run the API in Visual Studio, you can access the Swagger UI at:

**URL**: `http://localhost:5110/swagger`

## 📋 Available Endpoints

### Authentication Endpoints
- **POST** `/api/auth/login` - User login with JWT token generation
- **POST** `/api/auth/refresh-token` - Refresh JWT access token
- **POST** `/api/auth/logout` - User logout
- **POST** `/api/auth/forgot-password` - Request password reset
- **POST** `/api/auth/reset-password` - Reset password with token
- **POST** `/api/auth/set-password` - Set password for new users

### Test Endpoints
- **GET** `/api/test/health` - API health check (no auth required)
- **GET** `/api/test/auth-test` - Test authentication (requires JWT token)
- **GET** `/api/test/admin-test` - Test admin access (requires System Administrator role)

### User Management
- **GET** `/api/users` - Get all users
- **GET** `/api/users/{id}` - Get user by ID
- **POST** `/api/users` - Create new user
- **PUT** `/api/users/{id}` - Update user
- **DELETE** `/api/users/{id}` - Delete user


### User → Manufacturers
- GET `/api/users/{id}/manufacturers` — List assigned manufacturers for a user
- POST `/api/users/{id}/manufacturers` — Sync assigned manufacturers (body: array of manufacturer IDs)

### Role Management
- **GET** `/api/roles` - Get all roles
- **GET** `/api/roles/{id}` - Get role by ID
- **POST** `/api/roles` - Create new role
- **PUT** `/api/roles/{id}` - Update role

### Manufacturer Management
- **GET** `/api/manufacturers` - Get all manufacturers
- **GET** `/api/manufacturers/{id}` - Get manufacturer by ID
- **POST** `/api/manufacturers` - Create new manufacturer
- **PUT** `/api/manufacturers/{id}` - Update manufacturer

### Distributor Management
- **GET** `/api/distributors` - Get all distributors
- **GET** `/api/distributors/{id}` - Get distributor by ID
- **POST** `/api/distributors` - Create new distributor
- **PUT** `/api/distributors/{id}` - Update distributor

## 🔐 JWT Authentication in Swagger

1. **Login First**: Use the `/api/auth/login` endpoint with test credentials
2. **Copy Token**: From the login response, copy the `accessToken` value
3. **Authorize**: Click the "Authorize" button at the top of Swagger UI
4. **Enter Token**: In the authorization dialog, enter: `Bearer YOUR_TOKEN_HERE`
5. **Test Protected Endpoints**: Now you can test endpoints that require authentication

## 🧪 Test Credentials

Use these credentials to test the login functionality:
- **User ID**: `admin`
- **Password**: `Admin@123`

## 📊 Features

- **Interactive API Documentation**: Test all endpoints directly from the browser
- **JWT Authentication Support**: Built-in authorization testing
- **Request/Response Examples**: See sample data for all endpoints
- **Schema Documentation**: Detailed information about all DTOs and models
- **Error Response Documentation**: Clear information about error codes and messages

## 🛠️ Running the API

1. Open the solution in Visual Studio
2. Set `NPPContractManagement.API` as the startup project
3. Press F5 or click "Start"
4. Navigate to `http://localhost:5110/swagger` in your browser

## 📝 Notes

- The API runs on port 5110 by default
- Swagger UI is only available in Development environment
- All endpoints support JSON request/response format
- CORS is configured to allow requests from the Angular frontend (localhost:4200)
