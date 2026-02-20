# Video Script 2: Authentication & User Management APIs

## SLIDE 1: Authentication Overview (30 seconds)

**[Show AuthController in Swagger]**

Welcome to Video 2, where we'll cover Authentication and User Management.

The NPP system uses JWT token-based authentication for secure API access.

All authentication endpoints are in the **AuthController** at the base path `/api/Auth`.

Let's walk through each endpoint.

---

## SLIDE 2: Login Endpoint (1 minute)

**[Show POST /api/Auth/login in Swagger]**

**Endpoint:** `POST /api/Auth/login`

**Purpose:** Authenticate a user and receive JWT tokens

**Request Body:**
```json
{
  "userId": "john.doe",
  "password": "SecurePassword123"
}
```

**Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "dGhpcyBpcyBhIHJlZnJl...",
  "userId": "john.doe",
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "roles": ["Contract Manager", "Manufacturer User"]
}
```

The access token is used in the Authorization header for all subsequent API calls.

The refresh token is used to get a new access token when it expires.

---

## SLIDE 3: Get Current User (45 seconds)

**[Show GET /api/Auth/me in Swagger]**

**Endpoint:** `GET /api/Auth/me`

**Purpose:** Get the currently authenticated user's information

**Headers Required:**
```
Authorization: Bearer {accessToken}
```

**Response (200 OK):**
```json
{
  "id": 5,
  "userId": "john.doe",
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "roles": ["Contract Manager"],
  "manufacturerIds": [1, 3, 5]
}
```

This endpoint is called when the app loads to verify the user is still authenticated.

---

## SLIDE 4: Password Management (1 minute 30 seconds)

**[Show Password Endpoints in Swagger]**

**1. Change Password**

**Endpoint:** `POST /api/Auth/change-password`

**Purpose:** Allow logged-in users to change their password

**Request Body:**
```json
{
  "currentPassword": "OldPassword123",
  "newPassword": "NewSecurePassword456"
}
```

**Response:** 200 OK with success message

---

**2. Forgot Password**

**Endpoint:** `POST /api/Auth/forgot-password`

**Purpose:** Request a password reset email

**Request Body:**
```json
{
  "email": "john.doe@example.com"
}
```

**Response:** 200 OK (always returns success for security)

The system sends an email with a reset token.

---

**3. Reset Password**

**Endpoint:** `POST /api/Auth/reset-password`

**Purpose:** Reset password using the token from email

**Request Body:**
```json
{
  "token": "abc123resettoken",
  "newPassword": "NewPassword789"
}
```

**Response:** 200 OK with success message

---

## SLIDE 5: Logout (30 seconds)

**[Show POST /api/Auth/logout in Swagger]**

**Endpoint:** `POST /api/Auth/logout`

**Purpose:** Invalidate the current session

**Headers Required:**
```
Authorization: Bearer {accessToken}
```

**Response:** 200 OK

This endpoint clears the refresh token from the database, preventing it from being used again.

---

## SLIDE 6: User Management - List Users (1 minute)

**[Show GET /api/Users in Swagger]**

**Endpoint:** `GET /api/Users`

**Purpose:** Get paginated list of all users

**Required Role:** System Administrator or Contract Manager

**Query Parameters:**
- `pageNumber` (default: 1)
- `pageSize` (default: 10, max: 100)
- `sortBy` (default: "Id")
- `sortDirection` ("asc" or "desc")
- `searchTerm` (optional - searches name, email, userId)
- `status` (optional - filter by active/inactive)

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": 1,
      "userId": "admin",
      "email": "admin@npp.com",
      "firstName": "System",
      "lastName": "Administrator",
      "roles": ["System Administrator"],
      "isActive": true
    }
  ],
  "totalCount": 25,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 3
}
```

---

## SLIDE 7: User Management - Get User by ID (45 seconds)

**[Show GET /api/Users/{id} in Swagger]**

**Endpoint:** `GET /api/Users/{id}`

**Purpose:** Get detailed information for a specific user

**Required Role:** System Administrator or Contract Manager

**Response (200 OK):**
```json
{
  "id": 5,
  "userId": "john.doe",
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "555-1234",
  "jobTitle": "Contract Manager",
  "roles": [
    { "id": 2, "name": "Contract Manager" }
  ],
  "manufacturerIds": [1, 3, 5],
  "isActive": true,
  "createdDate": "2024-01-15T10:30:00Z"
}
```

---

## SLIDE 8: User Management - Create User (1 minute)

**[Show POST /api/Users in Swagger]**

**Endpoint:** `POST /api/Users`

**Purpose:** Create a new user account

**Required Role:** System Administrator or Contract Manager

**Request Body:**
```json
{
  "userId": "jane.smith",
  "email": "jane.smith@example.com",
  "firstName": "Jane",
  "lastName": "Smith",
  "phoneNumber": "555-5678",
  "jobTitle": "Manufacturer Representative",
  "roleIds": [3],
  "manufacturerIds": [2, 4],
  "isActive": true
}
```

**Response (201 Created):**
Returns the created user object with ID.

**Note:** The system automatically sends a "Set Password" email to the new user.

---

## SLIDE 9: User Management - Update User (1 minute)

**[Show PUT /api/Users/{id} in Swagger]**

**Endpoint:** `PUT /api/Users/{id}`

**Purpose:** Update an existing user

**Required Role:** System Administrator or Contract Manager

**Request Body:**
```json
{
  "email": "jane.smith@newdomain.com",
  "firstName": "Jane",
  "lastName": "Smith-Johnson",
  "phoneNumber": "555-9999",
  "jobTitle": "Senior Contract Manager",
  "roleIds": [2, 3],
  "manufacturerIds": [2, 4, 6],
  "isActive": true
}
```

**Response (200 OK):**
Returns the updated user object.

**Note:** You cannot change the userId after creation.

---

## SLIDE 10: User-Manufacturer Assignment (1 minute)

**[Show User-Manufacturer Endpoints in Swagger]**

**1. Get User's Manufacturers**

**Endpoint:** `GET /api/Users/{id}/manufacturers`

**Purpose:** Get list of manufacturers assigned to a user

**Response:**
```json
[
  { "id": 1, "name": "Tyson Foods" },
  { "id": 3, "name": "Sysco Corporation" }
]
```

---

**2. Sync User's Manufacturers**

**Endpoint:** `POST /api/Users/{id}/manufacturers`

**Purpose:** Replace all manufacturer assignments for a user

**Request Body:**
```json
[1, 3, 5, 7]
```

**Response:** 200 OK

This completely replaces the user's manufacturer list with the provided IDs.

---

## SLIDE 11: Summary (30 seconds)

**[Show Summary Slide]**

We've covered:

✅ **Authentication:** Login, logout, password management  
✅ **User Management:** CRUD operations for users  
✅ **User-Manufacturer Assignment:** Link users to manufacturers  

**Key Points:**
- All endpoints use JWT authentication
- Role-based access control (System Administrator, Contract Manager)
- Pagination support for large datasets
- Manufacturer-specific access for users

In the next video, we'll cover Master Data Management APIs.

---

**[TOTAL TIME: ~9 minutes]**

