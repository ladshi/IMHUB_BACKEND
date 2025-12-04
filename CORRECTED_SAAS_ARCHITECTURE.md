# SaaS Printing Management System - Corrected Architecture

**Date:** December 2024  
**Project Type:** SaaS Multi-Tenant Printing Management Platform  
**Status:** Architecture Design - Prototype Phase

---

## 🎯 PROJECT OVERVIEW

### **Business Problem:**
- Printing company (Main Branch) has multiple Organizations below it
- Organizations have Employees (customers)
- Employees create PDFs/templates and need to manually:
  1. Go to printing company
  2. Print documents
  3. Send CSV files with end user details
  4. Post manually
- **Result:** Time waste, manual errors, inefficiency

### **Solution:**
- **SaaS Portal** for complete automation
- Organization Admins create templates with editable placeholders
- Employees edit only defined editable areas
- Upload CSV with end user details
- Send to printing company via API (no email/third party)
- Track sendout status (received, printing, sent, etc.)
- **Multi-tenant:** Data isolation per organization

---

## 🏗️ SYSTEM ARCHITECTURE

### **Hierarchy:**
```
Main Branch (Platform)
    ↓
SuperAdmin (Manages Organizations & Printers)
    ↓
Organizations (Tenants)
    ↓
Organization Admins (Create Templates)
    ↓
Employees (Edit Templates, Upload CSV, Send to Print)
    ↓
Printing Company (Receives via API, Prints, Sends)
```

---

## 👥 USER ROLES & RESPONSIBILITIES

### **1. SuperAdmin (Platform Level)**
**Responsibilities:**
- ✅ **Manage Organizations** (Create, Update, Approve, Deactivate)
- ✅ **Manage Printers** (Create, Update, Configure)
- ✅ **Link Printers to Organizations** (via Distribution table)
- ✅ **View All Organizations** (Across platform)
- ✅ **View All Printers** (Global and Organization-specific)
- ✅ **Monitor System** (Dashboard, Analytics)

**NOT Responsible For:**
- ❌ **Creating Templates** ❌ (Only Tenants can create templates)
- ❌ **Managing Content** ❌
- ❌ **Editing Templates** ❌
- ❌ **Any Template-related operations** ❌

---

### **2. Organization Admin (Tenant Level)**
**Responsibilities:**
- ✅ **Create Templates** (Upload PDF, Define editable placeholders)
- ✅ **Manage Templates** (Edit, Delete, Publish)
- ✅ **Define Editable Areas** (Set `TemplateField.IsLocked = false`)
- ✅ **Lock Non-Editable Areas** (Set `TemplateField.IsLocked = true`)
- ✅ **Upload CSV Files** (For bulk content generation)
- ✅ **Map CSV Columns** (To template fields)
- ✅ **View All Content** (Within organization)
- ✅ **Manage Employees** (Within organization)

**NOT Responsible For:**
- ❌ Managing Printers
- ❌ Linking Printers to Organizations
- ❌ Accessing other organizations' data

---

### **3. Employee (Tenant Level)**
**Responsibilities:**
- ✅ **View Templates** (Assigned to their organization)
- ✅ **Edit Editable Areas Only** (Fields where `IsLocked = false`)
- ✅ **Create Content** (From templates + CSV data)
- ✅ **Upload CSV Files** (With end user details)
- ✅ **Send to Print** (Via API to printing company)
- ✅ **Track Sendout Status** (Received, Printing, Sent, etc.)
- ✅ **View Generated PDFs**

**NOT Responsible For:**
- ❌ Creating Templates
- ❌ Editing Locked Fields (`IsLocked = true`)
- ❌ Managing Printers
- ❌ Accessing other organizations' data

---

## 🔄 COMPLETE WORKFLOW

### **Phase 1: SuperAdmin Setup**

```
┌─────────────┐
│ SuperAdmin  │
└──────┬──────┘
       │
       │ 1. Create Organization
       │    POST /api/superadmin/organizations
       │    { name, domain, tenantCode, planType }
       ▼
┌──────────────────┐
│   Organization    │
│   - Id: 1        │
│   - Name: "ABC"  │
│   - Status: Active│
└──────┬───────────┘
       │
       │ 2. Create Printer
       │    POST /api/superadmin/printers
       │    { name, location, apiKey, organizationId: null (Global) }
       ▼
┌──────────────────┐
│     Printer       │
│     - Id: 1      │
│     - Name: "HP" │
│     - ApiKey     │
└──────┬───────────┘
       │
       │ 3. Link Printer to Organization
       │    POST /api/superadmin/distributions
       │    { organizationId: 1, printerId: 1, isActive: true }
       ▼
┌──────────────────┐
│   Distribution    │
│   - OrgId: 1     │
│   - PrinterId: 1 │
│   - IsActive: ✅ │
└──────────────────┘
```

---

### **Phase 2: Organization Admin (Tenant) Creates Template**

**⚠️ IMPORTANT: Only Tenants (Organization Admins) can create templates. SuperAdmin CANNOT create templates.**

```
┌─────────────────┐
│ Organization    │
│ Admin (Tenant)  │
└────────┬────────┘
         │
         │ 1. Create Template
         │    POST /api/organizations/templates
         │    { title, slug, thumbnailUrl }
         ▼
┌──────────────────┐
│   Template       │
│   - OrgId: 1     │
│   - Title        │
│   - Status: Draft│
└──────┬───────────┘
       │
       │ 2. Upload PDF & Create Version
       │    POST /api/organizations/templates/1/versions
       │    { pdfUrl, versionNumber: 1 }
       ▼
┌──────────────────┐
│ TemplateVersion   │
│ - TemplateId: 1  │
│ - PdfUrl         │
│ - IsActive: ✅   │
└──────┬───────────┘
       │
       │ 3. Create Pages
       │    POST /api/organizations/templates/1/versions/1/pages
       │    { pageNumber, width, height }
       ▼
┌──────────────────┐
│  TemplatePage     │
│  - PageNumber: 1  │
└──────┬───────────┘
       │
       │ 4. Define Editable Fields
       │    POST /api/organizations/templates/1/versions/1/pages/1/fields
       │    { fieldName, fieldType, x, y, width, height, isLocked }
       ▼
┌──────────────────────────────────┐
│  TemplateField                    │
│  - FieldName: "CustomerName"      │
│  - IsLocked: ❌ (Employee CAN edit)│
│                                   │
│  - FieldName: "InvoiceNumber"     │
│  - IsLocked: ✅ (Employee CANNOT) │
└──────────────────────────────────┘
```

---

### **Phase 3: Employee Works with Template**

```
┌──────────┐
│ Employee │
└────┬─────┘
     │
     │ 1. View Template
     │    GET /api/templates/1
     │    (Shows only unlocked fields)
     ▼
┌──────────────────┐
│  Template View   │
│  - Editable: ✅  │
│  - Locked: ❌    │
└────┬─────────────┘
     │
     │ 2. Upload CSV with End User Details
     │    POST /api/contents/csv-upload
     │    File: end-users.csv
     │    { CustomerName, Address, Amount, ... }
     ▼
┌──────────────────┐
│   CsvUpload       │
│   - FileName      │
│   - TotalRows: 100│
│   - MappingJson   │
└──────┬────────────┘
       │
       │ 3. Map CSV Columns to Template Fields
       │    POST /api/csv-uploads/1/map-fields
       │    { mappings: [{csvColumn, templateFieldName}] }
       ▼
┌──────────────────┐
│  Field Mapping    │
│  CSV → Template   │
└──────┬────────────┘
       │
       │ 4. Generate Content from CSV
       │    POST /api/csv-uploads/1/generate-content
       │    (Creates Content + ContentFieldValue for each row)
       ▼
┌──────────────────┐
│   Content         │
│   - Name          │
│   - Status: Draft │
│   - FieldValues   │
└──────┬────────────┘
       │
       │ 5. Edit Editable Fields Only
       │    PUT /api/contents/1/fields/1
       │    { value: "John Doe" }
       │    (Validates IsLocked = false)
       ▼
┌──────────────────┐
│  Content Updated  │
│  (Only unlocked) │
└──────┬────────────┘
       │
       │ 6. Generate PDF
       │    POST /api/contents/1/generate-pdf
       │    (Merges template PDF + field values)
       ▼
┌──────────────────┐
│  Generated PDF    │
│  - PdfUrl         │
└──────┬────────────┘
       │
       │ 7. Send to Printing Company
       │    POST /api/sendouts
       │    { contentId: 1, printerId: 1, targetDate }
       ▼
┌──────────────────┐
│    Sendout        │
│    - JobReference │
│    - Status: Submitted│
│    - PrinterId    │
└──────┬────────────┘
       │
       │ 8. Send via API to Printing Company
       │    (HTTP POST to Printer.ApiKey endpoint)
       │    { pdfUrl, jobReference, endUserDetails }
       ▼
┌──────────────────┐
│ Printing Company │
│ Receives via API │
└──────────────────┘
```

---

### **Phase 4: Sendout Tracking**

```
┌──────────────────┐
│ Printing Company │
│ (External API)   │
└──────┬───────────┘
       │
       │ 1. Receives Sendout
       │    Status: "Received"
       │    POST /api/sendouts/{id}/status
       │    { status: "Received", notes: "..." }
       ▼
┌──────────────────┐
│ SendoutStatusHistory│
│ - Status: Received│
│ - UpdatedBy: API │
└──────┬───────────┘
       │
       │ 2. Printing Started
       │    Status: "Printing"
       │    PUT /api/sendouts/{id}/status
       │    { status: "Printing" }
       ▼
┌──────────────────┐
│ SendoutStatusHistory│
│ - Status: Printing│
└──────┬───────────┘
       │
       │ 3. Printing Completed
       │    Status: "Printed"
       │    PUT /api/sendouts/{id}/status
       │    { status: "Printed" }
       ▼
┌──────────────────┐
│ SendoutStatusHistory│
│ - Status: Printed │
└──────┬───────────┘
       │
       │ 4. Sent to End User
       │    Status: "Sent"
       │    PUT /api/sendouts/{id}/status
       │    { status: "Sent", trackingNumber: "..." }
       ▼
┌──────────────────┐
│ SendoutStatusHistory│
│ - Status: Sent   │
│ - TrackingNumber │
└──────────────────┘
```

---

## 📡 API ENDPOINTS STRUCTURE

### **SuperAdmin Endpoints:**

**⚠️ NOTE: SuperAdmin CANNOT access template endpoints. Only Organization Admins can.**

```
# Organizations Management
POST   /api/superadmin/organizations
GET    /api/superadmin/organizations
GET    /api/superadmin/organizations/{id}
PUT    /api/superadmin/organizations/{id}
DELETE /api/superadmin/organizations/{id}
POST   /api/superadmin/organizations/{id}/approve
POST   /api/superadmin/organizations/{id}/deactivate

# Printers Management
POST   /api/superadmin/printers
GET    /api/superadmin/printers
GET    /api/superadmin/printers/{id}
PUT    /api/superadmin/printers/{id}
DELETE /api/superadmin/printers/{id}

# Printer-Organization Linking (Distribution)
POST   /api/superadmin/distributions
GET    /api/superadmin/distributions
GET    /api/superadmin/distributions/{id}
PUT    /api/superadmin/distributions/{id}
DELETE /api/superadmin/distributions/{id}
GET    /api/superadmin/organizations/{orgId}/printers
GET    /api/superadmin/printers/{printerId}/organizations

# ❌ NO TEMPLATE ENDPOINTS FOR SUPERADMIN ❌
```

---

### **Organization Admin Endpoints:**

```
# Templates Management
POST   /api/organizations/templates
GET    /api/organizations/templates
GET    /api/organizations/templates/{id}
PUT    /api/organizations/templates/{id}
DELETE /api/organizations/templates/{id}

# Template Versions
POST   /api/organizations/templates/{id}/versions
GET    /api/organizations/templates/{id}/versions
PUT    /api/organizations/templates/{id}/versions/{versionId}
DELETE /api/organizations/templates/{id}/versions/{versionId}

# Template Pages
POST   /api/organizations/templates/{id}/versions/{versionId}/pages
GET    /api/organizations/templates/{id}/versions/{versionId}/pages
PUT    /api/organizations/templates/{id}/versions/{versionId}/pages/{pageId}
DELETE /api/organizations/templates/{id}/versions/{versionId}/pages/{pageId}

# Template Fields (Editable Placeholders)
POST   /api/organizations/templates/{id}/versions/{versionId}/pages/{pageId}/fields
GET    /api/organizations/templates/{id}/versions/{versionId}/pages/{pageId}/fields
PUT    /api/organizations/templates/{id}/versions/{versionId}/pages/{pageId}/fields/{fieldId}
DELETE /api/organizations/templates/{id}/versions/{versionId}/pages/{pageId}/fields/{fieldId}

# CSV Upload
POST   /api/organizations/templates/{id}/csv-upload
GET    /api/organizations/csv-uploads
GET    /api/organizations/csv-uploads/{id}
POST   /api/organizations/csv-uploads/{id}/map-fields
POST   /api/organizations/csv-uploads/{id}/generate-content
DELETE /api/organizations/csv-uploads/{id}

# Content Management
GET    /api/organizations/contents
GET    /api/organizations/contents/{id}
```

---

### **Employee Endpoints:**

```
# Templates (View Only)
GET    /api/templates
GET    /api/templates/{id}

# Content Management
POST   /api/contents
GET    /api/contents
GET    /api/contents/{id}
PUT    /api/contents/{id}/fields/{fieldId}  # Only if IsLocked = false
DELETE /api/contents/{id}

# PDF Generation
POST   /api/contents/{id}/generate-pdf
GET    /api/contents/{id}/pdf

# CSV Upload (Employee can also upload)
POST   /api/contents/csv-upload
GET    /api/csv-uploads
GET    /api/csv-uploads/{id}
POST   /api/csv-uploads/{id}/map-fields
POST   /api/csv-uploads/{id}/generate-content

# Sendout (Send to Print)
POST   /api/sendouts
GET    /api/sendouts
GET    /api/sendouts/{id}
GET    /api/sendouts/{id}/status-history

# Sendout Status Tracking
GET    /api/sendouts?status=Submitted
GET    /api/sendouts?status=Received
GET    /api/sendouts?status=Printing
GET    /api/sendouts?status=Printed
GET    /api/sendouts?status=Sent
```

---

### **Printing Company API (External):**

```
# Receive Sendout
POST   /api/external/sendouts/receive
Headers: { ApiKey: "..." }
Body: { jobReference, pdfUrl, endUserDetails }

# Update Status
PUT    /api/external/sendouts/{jobReference}/status
Headers: { ApiKey: "..." }
Body: { status: "Received" | "Printing" | "Printed" | "Sent", notes, trackingNumber }
```

---

## 🔐 MULTI-TENANT DATA ISOLATION

### **Organization-Level Isolation:**

```csharp
// In all queries, filter by OrganizationId
var templates = await _context.Templates
    .Where(t => t.OrganizationId == currentUser.OrganizationId)
    .ToListAsync();

// In all commands, validate OrganizationId matches
if (template.OrganizationId != currentUser.OrganizationId)
{
    throw new UnauthorizedAccessException(
        "Cannot access template from another organization.");
}
```

### **Role-Based Access:**

```csharp
// SuperAdmin: Can access all organizations
if (currentUser.Role == "SuperAdmin")
{
    // No filter
}
// OrgAdmin/Employee: Only their organization
else
{
    query = query.Where(x => x.OrganizationId == currentUser.OrganizationId);
}
```

---

## 🗄️ DATABASE SCHEMA (Already Exists)

### **Key Entities:**

1. **Organization** - Tenant
2. **User** - Users (Admins, Employees)
3. **Role** - User roles
4. **Printer** - Printing hardware
5. **Distribution** - Links Printer to Organization
6. **Template** - Template header
7. **TemplateVersion** - Template versions
8. **TemplatePage** - Template pages
9. **TemplateField** - Editable fields (`IsLocked` flag)
10. **CsvUpload** - CSV file uploads
11. **Content** - Content instances
12. **ContentFieldValue** - Field values
13. **Sendout** - Printing jobs
14. **SendoutStatusHistory** - Status tracking

---

## ⚡ KEY FEATURES

### **1. Editable Placeholders:**
- Admin defines editable areas (`IsLocked = false`)
- Employee can only edit unlocked fields
- Locked fields are protected

### **2. CSV Bulk Processing:**
- Upload CSV with end user details
- Map CSV columns to template fields
- Generate multiple Content instances
- Skip locked fields automatically

### **3. API Integration:**
- Send to printing company via API (no email)
- Secure API key authentication
- Real-time status updates

### **4. Sendout Tracking:**
- Track: Submitted → Received → Printing → Printed → Sent
- Full audit trail (`SendoutStatusHistory`)
- Tracking numbers
- Notes/comments

### **5. Multi-Tenant SaaS:**
- Complete data isolation per organization
- Role-based access control
- Scalable architecture

---

## 🚀 IMPLEMENTATION PRIORITY

### **Phase 1: Core Setup (SuperAdmin)**
1. Organization CRUD
2. Printer CRUD
3. Distribution (Printer-Organization linking)

**⚠️ SuperAdmin CANNOT create templates. Only Tenants can.**

### **Phase 2: Template Management (Organization Admin - Tenant Only)**
1. Template CRUD (Tenant only)
2. TemplateVersion CRUD (Tenant only)
3. TemplatePage CRUD (Tenant only)
4. TemplateField CRUD (Tenant only, with IsLocked)

### **Phase 3: Content & CSV (Employee)**
1. CSV upload
2. Field mapping
3. Content generation
4. Field value editing (with IsLocked validation)

### **Phase 4: Sendout & Tracking**
1. Sendout creation
2. API integration with printing company
3. Status tracking
4. Status history

### **Phase 5: PDF Generation**
1. PDF merge service
2. Field value overlay
3. PDF storage

---

## 📋 IMPLEMENTATION CHECKLIST

### **SuperAdmin Features:**
- [ ] Organization CRUD endpoints
- [ ] Printer CRUD endpoints
- [ ] Distribution CRUD endpoints
- [ ] Organization-Printer linking
- [ ] Organization approval workflow
- [ ] **NO Template endpoints** ❌ (Only Tenants can create templates)

### **Organization Admin Features:**
- [ ] Template CRUD endpoints
- [ ] TemplateVersion CRUD endpoints
- [ ] TemplatePage CRUD endpoints
- [ ] TemplateField CRUD endpoints
- [ ] Set IsLocked flags
- [ ] CSV upload endpoints
- [ ] Field mapping endpoints
- [ ] Content generation from CSV

### **Employee Features:**
- [ ] View templates (filtered by organization)
- [ ] Edit content field values (IsLocked validation)
- [ ] Upload CSV files
- [ ] Generate content from CSV
- [ ] Generate PDFs
- [ ] Create sendouts
- [ ] Track sendout status

### **Printing Company Integration:**
- [ ] External API endpoint (receive sendouts)
- [ ] API key authentication
- [ ] Status update endpoint
- [ ] Webhook support (optional)

### **Infrastructure:**
- [ ] Repositories for all entities
- [ ] UnitOfWork updates
- [ ] DI registration
- [ ] PDF generation service
- [ ] CSV parsing service
- [ ] API client for printing company

---

## 🔒 SECURITY CONSIDERATIONS

1. **Multi-Tenant Isolation:** Always filter by OrganizationId
2. **Role-Based Access:** Validate user role before operations
3. **Field-Level Security:** Check `IsLocked` before allowing edits
4. **API Security:** Secure API keys for printing company
5. **File Upload Security:** Validate CSV files, limit size
6. **Data Encryption:** Encrypt sensitive data at rest

---

## 📊 PERFORMANCE CONSIDERATIONS

1. **Bulk Operations:** Use BulkInsert for ContentFieldValue
2. **Caching:** Cache templates, printers, distributions
3. **Async Processing:** PDF generation, CSV processing
4. **Database Indexes:** OrganizationId, PrinterId, Status
5. **Pagination:** All list endpoints

---

**Status:** ✅ **ARCHITECTURE CORRECTED - READY FOR IMPLEMENTATION**

**Key Correction:** SuperAdmin only manages Organizations, Printers, and Distribution linking. Organization Admins manage Templates and Content.

