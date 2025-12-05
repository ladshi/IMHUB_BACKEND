# Complete API Testing Guide - IMHub Backend

## 📋 Table of Contents

1. [Setup & Authentication](#setup--authentication)
2. [Auth APIs](#auth-apis)
3. [User Management APIs](#user-management-apis)
4. [Organization Management APIs](#organization-management-apis)
5. [Template Management APIs](#template-management-apis)
6. [Content Management APIs](#content-management-apis)
7. [Sendout Management APIs](#sendout-management-apis)
8. [CSV Upload APIs](#csv-upload-apis)
9. [SuperAdmin APIs](#superadmin-apis)
10. [Testing Checklist](#testing-checklist)

---

## 🔧 Setup & Authentication

### Base URL
```
http://localhost:5299
```

### Authentication
Most APIs require JWT token in Authorization header:
```
Authorization: Bearer <token>
```

### Get Token
Use login endpoint to get token (see Auth APIs section).

---

## 1. Auth APIs

### 1.1 Login
**POST** `/api/auth/login`

**Request:**
```json
{
  "email": "admin@imhub.com",
  "password": "Admin@123"
}
```

**Response (200 OK):**
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

**Error (401 Unauthorized):**
```json
{
  "message": "Invalid Credentials"
}
```

---

### 1.2 Register
**POST** `/api/auth/register`

**Request:**
```json
{
  "name": "Lachchu",
  "email": "lachchu123@gmail.com",
  "organizationName": "IMHUB Organization",
  "password": "Lachchu123!"
}
```

**Response (200 OK):**
```json
{
  "id": 2,
  "name": "Lachchu",
  "email": "lachchu123@gmail.com",
  "organizationId": 1,
  "message": "Registration successful. Please wait for admin approval."
}
```

**Error (400 Bad Request):**
```json
{
  "message": "Employee role not found. Please contact administrator."
}
```
*Note: This error occurs if RoleSeeder didn't run. Restart API to ensure roles are created.*

**Error (400 Bad Request):**
```json
{
  "message": "Email already registered"
}
```

---

### 1.3 Get Current User
**GET** `/api/auth/me`

**Headers:**
```
Authorization: Bearer <token>
```

**Response (200 OK):**
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

**Error (401 Unauthorized):**
```json
{
  "message": "Invalid token"
}
```

---

### 1.4 Forgot Password
**POST** `/api/auth/forgot-password`

**Request:**
```json
{
  "email": "user@example.com"
}
```

**Response (200 OK):**
```json
{
  "message": "Password reset email sent successfully"
}
```

---

### 1.5 Reset Password
**POST** `/api/auth/reset-password`

**Request:**
```json
{
  "email": "user@example.com",
  "token": "reset-token-from-email",
  "newPassword": "NewPassword123!"
}
```

**Response (200 OK):**
```json
{
  "message": "Password reset successful"
}
```

---

## 2. User Management APIs

**Base Route:** `/api/users`  
**Required Role:** `OrgAdmin`  
**Required Header:** `Authorization: Bearer <token>`

### 2.1 Get All Users
**GET** `/api/users?pageNumber=1&pageSize=10&searchTerm=&isActive=`

**Query Parameters:**
- `pageNumber` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10)
- `searchTerm` (string, optional): Search by name/email
- `isActive` (bool, optional): Filter by active status

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "organizationId": 1,
        "organizationName": "IMHUB Organization",
        "name": "John Doe",
        "email": "john@example.com",
        "username": "johndoe",
        "isActive": true,
        "roles": ["Employee"],
        "createdAt": "2024-01-01T00:00:00Z",
        "updatedAt": "2024-01-01T00:00:00Z"
      }
    ],
    "totalCount": 1,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  },
  "message": "Users retrieved successfully."
}
```

---

### 2.2 Get User By ID
**GET** `/api/users/{id}`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "organizationId": 1,
    "organizationName": "IMHUB Organization",
    "name": "John Doe",
    "email": "john@example.com",
    "username": "johndoe",
    "isActive": true,
    "roles": ["Employee"],
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  },
  "message": "User retrieved successfully."
}
```

---

### 2.3 Create User
**POST** `/api/users`

**Request:**
```json
{
  "name": "Jane Doe",
  "email": "jane@example.com",
  "username": "janedoe",
  "password": "Password123!",
  "roleIds": [4]
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": 2,
    "organizationId": 1,
    "organizationName": "IMHUB Organization",
    "name": "Jane Doe",
    "email": "jane@example.com",
    "username": "janedoe",
    "isActive": true,
    "roles": ["Employee"],
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  },
  "message": "User created successfully."
}
```

---

### 2.4 Update User
**PUT** `/api/users/{id}`

**Request:**
```json
{
  "name": "Jane Smith",
  "email": "jane.smith@example.com",
  "username": "janesmith",
  "isActive": true,
  "roleIds": [4]
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 2,
    "name": "Jane Smith",
    "email": "jane.smith@example.com",
    "username": "janesmith",
    "isActive": true,
    "roles": ["Employee"]
  },
  "message": "User updated successfully."
}
```

---

### 2.5 Delete User
**DELETE** `/api/users/{id}`

**Response (200 OK):**
```json
{
  "message": "User deleted successfully"
}
```

---

## 3. Organization Management APIs (SuperAdmin)

**Base Route:** `/api/superadmin/organizations`  
**Required Role:** `SuperAdmin`  
**Required Header:** `Authorization: Bearer <token>`

### 3.1 Get All Organizations
**GET** `/api/superadmin/organizations?pageNumber=1&pageSize=10&searchTerm=`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "name": "IMHUB Organization",
        "domain": "imhub-organization",
        "tenantCode": "ABC12345",
        "planType": "Free",
        "isActive": false,
        "createdAt": "2024-01-01T00:00:00Z"
      }
    ],
    "totalCount": 1,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  },
  "message": "Organizations retrieved successfully."
}
```

---

### 3.2 Get Organization By ID
**GET** `/api/superadmin/organizations/{id}`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "IMHUB Organization",
    "domain": "imhub-organization",
    "tenantCode": "ABC12345",
    "planType": "Free",
    "isActive": false,
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "message": "Organization retrieved successfully."
}
```

---

### 3.3 Create Organization
**POST** `/api/superadmin/organizations`

**Request:**
```json
{
  "name": "New Organization",
  "domain": "new-org",
  "tenantCode": "NEW12345",
  "planType": "Free"
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": 2,
    "name": "New Organization",
    "domain": "new-org",
    "tenantCode": "NEW12345",
    "planType": "Free",
    "isActive": false
  },
  "message": "Organization created successfully."
}
```

---

### 3.4 Update Organization
**PUT** `/api/superadmin/organizations/{id}`

**Request:**
```json
{
  "name": "Updated Organization",
  "domain": "updated-org",
  "planType": "Premium"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Updated Organization",
    "domain": "updated-org",
    "planType": "Premium"
  },
  "message": "Organization updated successfully."
}
```

---

### 3.5 Approve Organization
**POST** `/api/superadmin/organizations/{id}/approve`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "IMHUB Organization",
    "isActive": true
  },
  "message": "Organization approved successfully."
}
```

---

### 3.6 Deactivate Organization
**POST** `/api/superadmin/organizations/{id}/deactivate`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "IMHUB Organization",
    "isActive": false
  },
  "message": "Organization deactivated successfully."
}
```

---

### 3.7 Delete Organization
**DELETE** `/api/superadmin/organizations/{id}`

**Response (200 OK):**
```json
{
  "message": "Organization deleted successfully"
}
```

---

## 4. Template Management APIs

**Base Route:** `/api/organizations/templates`  
**Required Role:** `OrgAdmin`  
**Required Header:** `Authorization: Bearer <token>`

### 4.1 Get All Templates
**GET** `/api/organizations/templates?pageNumber=1&pageSize=10&searchTerm=`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "name": "Invoice Template",
        "description": "Standard invoice template",
        "isActive": true,
        "currentVersionId": 1,
        "createdAt": "2024-01-01T00:00:00Z"
      }
    ],
    "totalCount": 1,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  },
  "message": "Templates retrieved successfully."
}
```

---

### 4.2 Get Template By ID
**GET** `/api/organizations/templates/{id}`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Invoice Template",
    "description": "Standard invoice template",
    "isActive": true,
    "currentVersionId": 1,
    "versions": [],
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "message": "Template retrieved successfully."
}
```

---

### 4.3 Create Template
**POST** `/api/organizations/templates`

**Request:**
```json
{
  "name": "New Template",
  "description": "Template description"
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": 2,
    "name": "New Template",
    "description": "Template description",
    "isActive": true,
    "currentVersionId": null
  },
  "message": "Template created successfully."
}
```

---

### 4.4 Update Template
**PUT** `/api/organizations/templates/{id}`

**Request:**
```json
{
  "name": "Updated Template",
  "description": "Updated description"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Updated Template",
    "description": "Updated description"
  },
  "message": "Template updated successfully."
}
```

---

### 4.5 Delete Template
**DELETE** `/api/organizations/templates/{id}`

**Response (200 OK):**
```json
{
  "message": "Template deleted successfully"
}
```

---

## 5. Content Management APIs

**Base Route:** `/api/contents`  
**Required Role:** `OrgAdmin, Employee`  
**Required Header:** `Authorization: Bearer <token>`

### 5.1 Get All Contents
**GET** `/api/contents?pageNumber=1&pageSize=10&templateId=&searchTerm=`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "name": "Invoice #001",
        "templateId": 1,
        "templateName": "Invoice Template",
        "generatedPdfUrl": "/pdfs/invoice-001.pdf",
        "createdAt": "2024-01-01T00:00:00Z"
      }
    ],
    "totalCount": 1,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  },
  "message": "Contents retrieved successfully."
}
```

---

### 5.2 Get Content By ID
**GET** `/api/contents/{id}`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Invoice #001",
    "templateId": 1,
    "templateName": "Invoice Template",
    "generatedPdfUrl": "/pdfs/invoice-001.pdf",
    "fieldValues": [
      {
        "templateFieldId": 1,
        "fieldName": "Customer Name",
        "value": "John Doe"
      }
    ],
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "message": "Content retrieved successfully."
}
```

---

### 5.3 Create Content
**POST** `/api/contents`

**Request:**
```json
{
  "name": "Invoice #002",
  "templateId": 1,
  "fieldValues": [
    {
      "templateFieldId": 1,
      "value": "Jane Doe"
    }
  ]
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": 2,
    "name": "Invoice #002",
    "templateId": 1,
    "templateName": "Invoice Template",
    "generatedPdfUrl": "/pdfs/invoice-002.pdf"
  },
  "message": "Content created successfully."
}
```

---

### 5.4 Update Content Field Value
**PUT** `/api/contents/{contentId}/fields/{templateFieldId}`

**Request:**
```json
{
  "value": "Updated Value"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "templateFieldId": 1,
    "fieldName": "Customer Name",
    "value": "Updated Value"
  },
  "message": "Field value updated successfully."
}
```

---

## 6. Sendout Management APIs

**Base Route:** `/api/sendouts`  
**Required Role:** `OrgAdmin, Employee`  
**Required Header:** `Authorization: Bearer <token>`

### 6.1 Get All Sendouts
**GET** `/api/sendouts?pageNumber=1&pageSize=10&status=&contentId=`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "contentId": 1,
        "contentName": "Invoice #001",
        "printerId": 1,
        "printerName": "Main Printer",
        "status": "Pending",
        "createdAt": "2024-01-01T00:00:00Z"
      }
    ],
    "totalCount": 1,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  },
  "message": "Sendouts retrieved successfully."
}
```

---

### 6.2 Get Sendout By ID
**GET** `/api/sendouts/{id}`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "contentId": 1,
    "contentName": "Invoice #001",
    "printerId": 1,
    "printerName": "Main Printer",
    "status": "Pending",
    "statusHistory": [
      {
        "status": "Pending",
        "changedAt": "2024-01-01T00:00:00Z",
        "changedBy": "Admin"
      }
    ],
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "message": "Sendout retrieved successfully."
}
```

---

### 6.3 Create Sendout
**POST** `/api/sendouts`

**Request:**
```json
{
  "contentId": 1,
  "printerId": 1,
  "quantity": 100
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": 2,
    "contentId": 1,
    "contentName": "Invoice #001",
    "printerId": 1,
    "printerName": "Main Printer",
    "status": "Pending",
    "quantity": 100
  },
  "message": "Sendout created successfully."
}
```

---

### 6.4 Send to Printer
**POST** `/api/sendouts/{sendoutId}/send-to-printer`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "status": "Sent",
    "externalJobId": "PRINT-12345"
  },
  "message": "Sendout sent to printer successfully."
}
```

---

### 6.5 Update Sendout Status
**PUT** `/api/sendouts/{sendoutId}/status`

**Request:**
```json
{
  "status": "Completed"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "status": "Completed"
  },
  "message": "Sendout status updated successfully."
}
```

---

### 6.6 Get Sendout Status History
**GET** `/api/sendouts/{sendoutId}/history`

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "status": "Pending",
      "changedAt": "2024-01-01T00:00:00Z",
      "changedBy": "Admin"
    },
    {
      "status": "Sent",
      "changedAt": "2024-01-01T01:00:00Z",
      "changedBy": "System"
    }
  ],
  "message": "Status history retrieved successfully."
}
```

---

## 7. CSV Upload APIs

**Base Route:** `/api/organizations/csv-uploads`  
**Required Role:** `OrgAdmin, Employee`  
**Required Header:** `Authorization: Bearer <token>`

### 7.1 Get All CSV Uploads
**GET** `/api/organizations/csv-uploads?pageNumber=1&pageSize=10&templateId=`

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "fileName": "data.csv",
        "templateId": 1,
        "templateName": "Invoice Template",
        "totalRows": 100,
        "processedRows": 50,
        "status": "Processing",
        "uploadedAt": "2024-01-01T00:00:00Z"
      }
    ],
    "totalCount": 1,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  },
  "message": "CSV uploads retrieved successfully."
}
```

---

### 7.2 Upload CSV
**POST** `/api/organizations/csv-uploads/templates/{templateId}`

**Request:** (multipart/form-data)
- `file`: CSV file (form-data)

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "fileName": "data.csv",
    "templateId": 1,
    "totalRows": 100,
    "processedRows": 0,
    "status": "Uploaded",
    "uploadedAt": "2024-01-01T00:00:00Z"
  },
  "message": "CSV file uploaded successfully."
}
```

---

### 7.3 Map CSV Fields
**POST** `/api/organizations/csv-uploads/{csvUploadId}/map-fields`

**Request:**
```json
{
  "fieldMappings": [
    {
      "csvColumn": "Customer Name",
      "templateFieldId": 1
    },
    {
      "csvColumn": "Amount",
      "templateFieldId": 2
    }
  ]
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "status": "Mapped",
    "fieldMappings": [...]
  },
  "message": "CSV fields mapped successfully."
}
```

---

### 7.4 Generate Content From CSV
**POST** `/api/organizations/csv-uploads/{csvUploadId}/generate-content`

**Request:**
```json
{
  "startRow": 0,
  "endRow": 100
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Invoice #001",
      "templateId": 1,
      "generatedPdfUrl": "/pdfs/invoice-001.pdf"
    }
  ],
  "message": "Generated 100 content items successfully."
}
```

---

## 8. SuperAdmin APIs

### 8.1 Printers Management
**Base Route:** `/api/superadmin/printers`  
**Required Role:** `SuperAdmin`

**Endpoints:**
- `GET /api/superadmin/printers` - Get all printers
- `GET /api/superadmin/printers/{id}` - Get printer by ID
- `POST /api/superadmin/printers` - Create printer
- `PUT /api/superadmin/printers/{id}` - Update printer
- `DELETE /api/superadmin/printers/{id}` - Delete printer

---

### 8.2 Distributions Management
**Base Route:** `/api/superadmin/distributions`  
**Required Role:** `SuperAdmin`

**Endpoints:**
- `GET /api/superadmin/distributions` - Get all distributions
- `GET /api/superadmin/distributions/{id}` - Get distribution by ID
- `POST /api/superadmin/distributions` - Create distribution
- `PUT /api/superadmin/distributions/{id}` - Update distribution
- `DELETE /api/superadmin/distributions/{id}` - Delete distribution

---

## 9. Testing Checklist

### ✅ Pre-Testing Setup

- [ ] SQL Server is running
- [ ] Database `IMHubDB` exists
- [ ] API is running on `http://localhost:5299`
- [ ] Roles are seeded (check API startup logs)
- [ ] SuperAdmin exists (email: `admin@imhub.com`, password: `Admin@123`)

### ✅ Auth APIs Testing

- [ ] Login with SuperAdmin credentials
- [ ] Register new user
- [ ] Get current user (with token)
- [ ] Forgot password
- [ ] Reset password

### ✅ User Management Testing

- [ ] Get all users (as OrgAdmin)
- [ ] Get user by ID
- [ ] Create user
- [ ] Update user
- [ ] Delete user

### ✅ Organization Management Testing

- [ ] Get all organizations (as SuperAdmin)
- [ ] Get organization by ID
- [ ] Create organization
- [ ] Update organization
- [ ] Approve organization
- [ ] Deactivate organization

### ✅ Template Management Testing

- [ ] Get all templates
- [ ] Get template by ID
- [ ] Create template
- [ ] Update template
- [ ] Delete template

### ✅ Content Management Testing

- [ ] Get all contents
- [ ] Get content by ID
- [ ] Create content
- [ ] Update content field value

### ✅ Sendout Management Testing

- [ ] Get all sendouts
- [ ] Get sendout by ID
- [ ] Create sendout
- [ ] Send to printer
- [ ] Update sendout status
- [ ] Get status history

---

## 🔍 Common Errors & Solutions

### Error: "Employee role not found"
**Solution:** Restart API to ensure RoleSeeder runs and creates roles.

### Error: "401 Unauthorized"
**Solution:** Check if token is valid and included in Authorization header.

### Error: "403 Forbidden"
**Solution:** Check if user has required role (OrgAdmin, SuperAdmin, etc.).

### Error: "400 Bad Request"
**Solution:** Check request body format and required fields.

---

## 📝 Notes

1. **Base URL:** All endpoints use `http://localhost:5299`
2. **Authentication:** Most endpoints require JWT token
3. **Roles:** 
   - `SuperAdmin` - Full access
   - `OrgAdmin` - Organization management
   - `Employee` - Limited access
4. **Pagination:** Most list endpoints support pagination
5. **Soft Delete:** Delete operations are soft deletes (IsDeleted flag)

---

**Last Updated:** 2024-12-04  
**API Version:** 1.0

