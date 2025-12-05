# Backend API Testing Guide

## Prerequisites

1. **Database Setup:**
   - Ensure SQL Server is running
   - Database will be created automatically on first run
   - Default connection: `Server=localhost;Database=IMHubDB;Trusted_Connection=true;TrustServerCertificate=true;`

2. **Configuration:**
   - JWT Secret must be at least 32 characters (set in appsettings.Development.json or User Secrets)
   - Connection string configured in appsettings.Development.json

## Starting the Backend

```bash
cd IMHUB_BACKEND/IMHub.API
dotnet run
```

Backend will start on: `http://localhost:5299`

## Available Endpoints

### 1. OpenAPI/Swagger (Development Only)
- **GET** `http://localhost:5299/openapi/v1.json` - OpenAPI specification

### 2. Authentication Endpoints

#### Login
- **POST** `http://localhost:5299/api/auth/login`
- **Body:**
```json
{
  "email": "admin@imhub.com",
  "password": "Admin@123"
}
```
- **Response:**
```json
{
  "id": 1,
  "name": "System Administrator",
  "email": "admin@imhub.com",
  "role": "SuperAdmin",
  "organizationId": null,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### Register
- **POST** `http://localhost:5299/api/auth/register`
- **Body:**
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "organizationName": "Test Organization",
  "password": "Password123!"
}
```

#### Get Current User (Requires Auth)
- **GET** `http://localhost:5299/api/auth/me`
- **Headers:** `Authorization: Bearer {token}`

### 3. SuperAdmin Endpoints

#### Organizations
- **GET** `http://localhost:5299/api/superadmin/organizations` - List all organizations
- **POST** `http://localhost:5299/api/superadmin/organizations` - Create organization
- **GET** `http://localhost:5299/api/superadmin/organizations/{id}` - Get organization by ID
- **PUT** `http://localhost:5299/api/superadmin/organizations/{id}` - Update organization
- **DELETE** `http://localhost:5299/api/superadmin/organizations/{id}` - Delete organization

#### Printers
- **GET** `http://localhost:5299/api/superadmin/printers` - List all printers
- **POST** `http://localhost:5299/api/superadmin/printers` - Create printer
- **GET** `http://localhost:5299/api/superadmin/printers/{id}` - Get printer by ID
- **PUT** `http://localhost:5299/api/superadmin/printers/{id}` - Update printer
- **DELETE** `http://localhost:5299/api/superadmin/printers/{id}` - Delete printer

### 4. Organization Endpoints

#### Templates
- **GET** `http://localhost:5299/api/organizations/templates` - List templates
- **POST** `http://localhost:5299/api/organizations/templates` - Create template
- **GET** `http://localhost:5299/api/organizations/templates/{id}` - Get template by ID
- **PUT** `http://localhost:5299/api/organizations/templates/{id}` - Update template
- **DELETE** `http://localhost:5299/api/organizations/templates/{id}` - Delete template

#### Contents
- **GET** `http://localhost:5299/api/contents` - List contents
- **POST** `http://localhost:5299/api/contents` - Create content
- **GET** `http://localhost:5299/api/contents/{id}` - Get content by ID
- **PUT** `http://localhost:5299/api/contents/{id}` - Update content
- **DELETE** `http://localhost:5299/api/contents/{id}` - Delete content

#### Sendouts
- **GET** `http://localhost:5299/api/sendouts` - List sendouts
- **POST** `http://localhost:5299/api/sendouts` - Create sendout
- **GET** `http://localhost:5299/api/sendouts/{id}` - Get sendout by ID

## Testing with cURL

### Test Login Endpoint
```bash
curl -X POST http://localhost:5299/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@imhub.com","password":"Admin@123"}'
```

### Test Register Endpoint
```bash
curl -X POST http://localhost:5299/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"name":"Test User","email":"test@example.com","organizationName":"Test Org","password":"Test123!"}'
```

### Test Get Current User (with token)
```bash
curl -X GET http://localhost:5299/api/auth/me \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## Testing with Postman

1. Import the OpenAPI spec from `http://localhost:5299/openapi/v1.json`
2. Create a new request collection
3. Test endpoints as described above

## Testing with Browser

1. Open browser DevTools (F12)
2. Go to Network tab
3. Navigate to `http://localhost:5299/openapi/v1.json` to see API documentation
4. Use browser console to test API calls:

```javascript
// Test login
fetch('http://localhost:5299/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    email: 'admin@imhub.com',
    password: 'Admin@123'
  })
})
.then(r => r.json())
.then(data => {
  console.log('Login successful:', data);
  localStorage.setItem('token', data.token);
});
```

## Expected Default Data

After first run, the database will be seeded with:
- **SuperAdmin:** 
  - Email: `admin@imhub.com`
  - Password: `Admin@123`
- **Roles:** SuperAdmin, OrgAdmin, Manager, Employee, PrinterOperator

## Troubleshooting

### Database Connection Issues
- Check SQL Server is running
- Verify connection string in appsettings.Development.json
- Ensure database server allows trusted connections

### JWT Secret Issues
- Secret must be at least 32 characters
- Update in appsettings.Development.json or use User Secrets

### CORS Issues
- Frontend must run on `http://localhost:5173`
- CORS is configured for this origin only

### Port Already in Use
- Change port in `Properties/launchSettings.json`
- Or kill the process using port 5299

