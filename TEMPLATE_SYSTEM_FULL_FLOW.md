# Template System - Full Flow Document

**Date:** December 2024  
**Status:** Architecture & Flow Design  
**Purpose:** Complete roadmap for Template creation, editing permissions, CSV upload, and sharing

---

## 📋 TABLE OF CONTENTS

1. [System Overview](#system-overview)
2. [User Roles & Permissions](#user-roles--permissions)
3. [Template Creation Flow (SuperAdmin)](#template-creation-flow-superadmin)
4. [Template Editing Permissions](#template-editing-permissions)
5. [CSV Upload & Processing Flow](#csv-upload--processing-flow)
6. [Content Generation & Sharing](#content-generation--sharing)
7. [API Endpoints Required](#api-endpoints-required)
8. [Database Schema Considerations](#database-schema-considerations)
9. [Performance & Scalability](#performance--scalability)

---

## 🎯 SYSTEM OVERVIEW

### **Core Entities Flow:**
```
SuperAdmin Creates Template
    ↓
Template (OrganizationId, Title, Status)
    ↓
TemplateVersion (PdfUrl, DesignJson, IsActive)
    ↓
TemplatePage (PageNumber, Width, Height)
    ↓
TemplateField (FieldName, FieldType, IsLocked, Coordinates)
    ↓
Organization Admin Edits Editable Fields
    ↓
CSV Upload (Maps CSV Columns → TemplateFields)
    ↓
Content (Instance from Template + CSV Data)
    ↓
ContentFieldValue (Actual Values per Field)
    ↓
Generated PDF (Final Output)
```

---

## 👥 USER ROLES & PERMISSIONS

### **1. SuperAdmin (Platform Level)**
- ✅ **Create Templates** (Full Control)
- ✅ **Assign Templates to Organizations**
- ✅ **Set Editable Options** (Which fields OrgAdmin can edit)
- ✅ **Lock/Unlock Fields** (Which fields Employees can edit)
- ✅ **View All Templates** (Across all organizations)

### **2. Organization Admin (Org Level)**
- ✅ **View Templates** (Assigned to their organization)
- ✅ **Edit Editable Fields** (Only fields marked as editable by SuperAdmin)
- ✅ **Cannot Edit Locked Fields** (Structure, coordinates, locked fields)
- ✅ **Upload CSV Files** (For bulk content generation)
- ✅ **Share Templates** (Within organization)

### **3. Employee (Org Level)**
- ✅ **View Templates** (Assigned to their organization)
- ✅ **Edit Only Unlocked Fields** (Fields where `IsLocked = false`)
- ✅ **Cannot Edit Locked Fields** (Fields where `IsLocked = true`)
- ✅ **Create Content** (From templates + CSV data)
- ✅ **View Generated Content**

---

## 🏗️ TEMPLATE CREATION FLOW (SuperAdmin)

### **Step 1: SuperAdmin Creates Template**

**API:** `POST /api/superadmin/templates`

**Request Body:**
```json
{
  "organizationId": 1,  // Assign to specific organization
  "title": "Invoice Template",
  "slug": "invoice-template",
  "thumbnailUrl": "https://...",
  "status": "Draft",
  "metadataJson": {
    "category": "Invoice",
    "description": "Standard invoice template"
  }
}
```

**Process:**
1. SuperAdmin creates `Template` entity
2. Sets `OrganizationId` (target organization)
3. Sets `Status = Draft`
4. Returns `Template` with ID

---

### **Step 2: SuperAdmin Creates Template Version**

**API:** `POST /api/superadmin/templates/{templateId}/versions`

**Request Body:**
```json
{
  "versionNumber": 1,
  "pdfUrl": "https://storage.../template-v1.pdf",
  "designJson": {
    "globalSettings": {
      "fontFamily": "Arial",
      "fontSize": 12,
      "colorScheme": "default"
    },
    "editableOptions": {
      "allowOrgAdminEdit": ["title", "thumbnailUrl", "metadataJson"],
      "allowEmployeeEdit": ["fieldValues"]
    }
  },
  "isActive": true
}
```

**Process:**
1. Creates `TemplateVersion` linked to `Template`
2. Stores `DesignJson` with editable options
3. Sets `IsActive = true` (only one active version per template)

---

### **Step 3: SuperAdmin Creates Template Pages**

**API:** `POST /api/superadmin/templates/{templateId}/versions/{versionId}/pages`

**Request Body:**
```json
{
  "pageNumber": 1,
  "width": 210,
  "height": 297,
  "backgroundImageUrl": "https://storage.../page1-bg.png"
}
```

**Process:**
1. Creates `TemplatePage` linked to `TemplateVersion`
2. Stores page dimensions and background

---

### **Step 4: SuperAdmin Creates Template Fields**

**API:** `POST /api/superadmin/templates/{templateId}/versions/{versionId}/pages/{pageId}/fields`

**Request Body:**
```json
{
  "fieldName": "CustomerName",
  "fieldType": "Text",
  "x": 50,
  "y": 100,
  "width": 200,
  "height": 30,
  "isLocked": false,  // Employee CAN edit this
  "validationRulesJson": {
    "required": true,
    "maxLength": 100
  }
}
```

**OR for Locked Fields:**
```json
{
  "fieldName": "InvoiceNumber",
  "fieldType": "Text",
  "x": 50,
  "y": 50,
  "width": 150,
  "height": 30,
  "isLocked": true,  // Employee CANNOT edit this
  "validationRulesJson": {
    "required": true,
    "readOnly": true
  }
}
```

**Process:**
1. Creates `TemplateField` linked to `TemplatePage`
2. Sets `IsLocked` flag:
   - `IsLocked = false` → Employee CAN edit
   - `IsLocked = true` → Employee CANNOT edit
3. Stores coordinates and validation rules

---

## ✏️ TEMPLATE EDITING PERMISSIONS

### **Organization Admin Edits (Limited)**

**API:** `PUT /api/organizations/templates/{templateId}`

**Allowed Fields (from `DesignJson.editableOptions.allowOrgAdminEdit`):**
- `title` ✅
- `thumbnailUrl` ✅
- `metadataJson` ✅
- `status` ✅ (Can change Draft → Published)

**NOT Allowed:**
- `TemplateVersion` structure ❌
- `TemplatePage` structure ❌
- `TemplateField` coordinates ❌
- `TemplateField.IsLocked` flag ❌

**Validation:**
```csharp
// In UpdateTemplateCommandHandler
var editableOptions = JsonSerializer.Deserialize<EditableOptions>(
    templateVersion.DesignJson);

if (!editableOptions.AllowOrgAdminEdit.Contains(fieldName))
{
    throw new UnauthorizedAccessException(
        $"Field '{fieldName}' is not editable by Organization Admin.");
}
```

---

### **Employee Edits (Only Unlocked Fields)**

**API:** `PUT /api/templates/{templateId}/fields/{fieldId}/value`

**Request Body:**
```json
{
  "value": "John Doe"
}
```

**Validation:**
```csharp
// In UpdateTemplateFieldValueCommandHandler
var templateField = await _unitOfWork.TemplateFieldRepository
    .GetByIdAsync(fieldId);

if (templateField.IsLocked)
{
    throw new UnauthorizedAccessException(
        $"Field '{templateField.FieldName}' is locked and cannot be edited by employees.");
}

// Allow update
contentFieldValue.Value = request.Value;
```

---

## 📤 CSV UPLOAD & PROCESSING FLOW

### **Step 1: Organization Admin Uploads CSV**

**API:** `POST /api/organizations/templates/{templateId}/csv-upload`

**Request:** `multipart/form-data`
- File: `invoice-data.csv`
- Mapping: (Optional - auto-detect or manual)

**CSV Format Example:**
```csv
CustomerName,InvoiceNumber,Amount,DueDate
John Doe,INV-001,1000.00,2024-12-31
Jane Smith,INV-002,2500.00,2025-01-15
```

**Process:**
1. Upload CSV file to storage (`IFileStorageService`)
2. Parse CSV (using `CsvHelper` or similar)
3. Create `CsvUpload` entity:
   ```csharp
   var csvUpload = new CsvUpload
   {
       OrganizationId = currentUser.OrganizationId,
       TemplateId = templateId,
       FileName = file.FileName,
       FileUrl = storedFileUrl,
       TotalRows = rowCount,
       MappingJson = JsonSerializer.Serialize(mapping)
   };
   ```
4. Auto-detect or manual mapping:
   ```json
   {
     "CustomerName": "CustomerName",
     "InvoiceNumber": "InvoiceNumber",
     "Amount": "Amount",
     "DueDate": "DueDate"
   }
   ```

---

### **Step 2: Map CSV Columns to Template Fields**

**API:** `POST /api/organizations/csv-uploads/{csvUploadId}/map-fields`

**Request Body:**
```json
{
  "mappings": [
    {
      "csvColumn": "CustomerName",
      "templateFieldName": "CustomerName"
    },
    {
      "csvColumn": "InvoiceNumber",
      "templateFieldName": "InvoiceNumber"
    },
    {
      "csvColumn": "Amount",
      "templateFieldName": "Amount"
    },
    {
      "csvColumn": "DueDate",
      "templateFieldName": "DueDate"
    }
  ]
}
```

**Process:**
1. Validate CSV columns exist
2. Validate TemplateField names exist
3. Check if TemplateField is locked (warn if mapping to locked field)
4. Update `CsvUpload.MappingJson`

---

### **Step 3: Generate Content from CSV**

**API:** `POST /api/organizations/csv-uploads/{csvUploadId}/generate-content`

**Request Body:**
```json
{
  "generateAll": true,  // or specific row indices
  "rowIndices": [0, 1, 2]  // if generateAll = false
}
```

**Process:**
1. Read CSV file from storage
2. Parse rows based on `MappingJson`
3. For each row:
   ```csharp
   // Create Content instance
   var content = new Content
   {
       OrganizationId = csvUpload.OrganizationId,
       TemplateVersionId = activeTemplateVersion.Id,
       CsvUploadId = csvUpload.Id,
       Name = $"Invoice {row['InvoiceNumber']}",
       Status = "Draft"
   };
   
   // Create ContentFieldValue for each mapped field
   foreach (var mapping in mappings)
   {
       var templateField = await GetTemplateFieldByName(mapping.TemplateFieldName);
       
       // Check if field is locked
       if (templateField.IsLocked)
       {
           // Use default value or skip
           continue;
       }
       
       var fieldValue = new ContentFieldValue
       {
           ContentId = content.Id,
           TemplateFieldId = templateField.Id,
           Value = row[mapping.CsvColumn]
       };
       
       content.FieldValues.Add(fieldValue);
   }
   ```
4. Save all `Content` and `ContentFieldValue` entities
5. Return list of created `Content` IDs

---

### **Step 4: Generate PDF from Content**

**API:** `POST /api/contents/{contentId}/generate-pdf`

**Process:**
1. Load `Content` with `TemplateVersion` and `FieldValues`
2. Load `TemplateVersion` with `Pages` and `Fields`
3. Merge template PDF with field values:
   - Use PDF library (iTextSharp, PdfSharp, etc.)
   - Overlay field values at coordinates (X, Y)
   - Apply field type formatting (Date, Number, etc.)
4. Save generated PDF to storage
5. Update `Content.GeneratedPdfUrl`
6. Return PDF URL

---

## 🔗 CONTENT GENERATION & SHARING

### **Step 1: Employee Views Template**

**API:** `GET /api/templates/{templateId}`

**Response:**
```json
{
  "id": 1,
  "title": "Invoice Template",
  "organizationId": 1,
  "status": "Published",
  "versions": [
    {
      "id": 1,
      "versionNumber": 1,
      "isActive": true,
      "pages": [
        {
          "id": 1,
          "pageNumber": 1,
          "fields": [
            {
              "id": 1,
              "fieldName": "CustomerName",
              "fieldType": "Text",
              "isLocked": false,  // Employee CAN edit
              "x": 50,
              "y": 100
            },
            {
              "id": 2,
              "fieldName": "InvoiceNumber",
              "fieldType": "Text",
              "isLocked": true,  // Employee CANNOT edit
              "x": 50,
              "y": 50
            }
          ]
        }
      ]
    }
  ]
}
```

---

### **Step 2: Employee Creates Content Manually**

**API:** `POST /api/contents`

**Request Body:**
```json
{
  "templateVersionId": 1,
  "name": "Invoice #101",
  "fieldValues": [
    {
      "templateFieldId": 1,  // CustomerName (IsLocked = false)
      "value": "John Doe"
    }
    // Cannot include templateFieldId: 2 (InvoiceNumber) - it's locked
  ]
}
```

**Validation:**
- Check each `TemplateField.IsLocked`
- Reject if trying to set value for locked field
- Allow only unlocked fields

---

### **Step 3: Employee Edits Content**

**API:** `PUT /api/contents/{contentId}/fields/{fieldId}`

**Request Body:**
```json
{
  "value": "Jane Doe"
}
```

**Validation:**
- Check `TemplateField.IsLocked`
- If locked → Reject
- If unlocked → Allow update

---

### **Step 4: Share Content**

**API:** `POST /api/contents/{contentId}/share`

**Request Body:**
```json
{
  "shareType": "Link",  // or "Email"
  "recipients": ["user@example.com"],
  "expiryDays": 7
}
```

**Process:**
1. Generate shareable link (with token)
2. Store share record (if needed)
3. Send email with link (if shareType = Email)
4. Return shareable URL

---

## 📡 API ENDPOINTS REQUIRED

### **SuperAdmin Endpoints:**
```
POST   /api/superadmin/templates
GET    /api/superadmin/templates
GET    /api/superadmin/templates/{id}
PUT    /api/superadmin/templates/{id}
DELETE /api/superadmin/templates/{id}

POST   /api/superadmin/templates/{id}/versions
GET    /api/superadmin/templates/{id}/versions
PUT    /api/superadmin/templates/{id}/versions/{versionId}
DELETE /api/superadmin/templates/{id}/versions/{versionId}

POST   /api/superadmin/templates/{id}/versions/{versionId}/pages
GET    /api/superadmin/templates/{id}/versions/{versionId}/pages
PUT    /api/superadmin/templates/{id}/versions/{versionId}/pages/{pageId}
DELETE /api/superadmin/templates/{id}/versions/{versionId}/pages/{pageId}

POST   /api/superadmin/templates/{id}/versions/{versionId}/pages/{pageId}/fields
GET    /api/superadmin/templates/{id}/versions/{versionId}/pages/{pageId}/fields
PUT    /api/superadmin/templates/{id}/versions/{versionId}/pages/{pageId}/fields/{fieldId}
DELETE /api/superadmin/templates/{id}/versions/{versionId}/pages/{pageId}/fields/{fieldId}
```

### **Organization Admin Endpoints:**
```
GET    /api/organizations/templates
GET    /api/organizations/templates/{id}
PUT    /api/organizations/templates/{id}  // Limited fields only

POST   /api/organizations/templates/{id}/csv-upload
GET    /api/organizations/csv-uploads
GET    /api/organizations/csv-uploads/{id}
POST   /api/organizations/csv-uploads/{id}/map-fields
POST   /api/organizations/csv-uploads/{id}/generate-content
DELETE /api/organizations/csv-uploads/{id}
```

### **Employee Endpoints:**
```
GET    /api/templates
GET    /api/templates/{id}

POST   /api/contents
GET    /api/contents
GET    /api/contents/{id}
PUT    /api/contents/{id}/fields/{fieldId}  // Only unlocked fields
DELETE /api/contents/{id}

POST   /api/contents/{id}/generate-pdf
GET    /api/contents/{id}/pdf

POST   /api/contents/{id}/share
```

---

## 🗄️ DATABASE SCHEMA CONSIDERATIONS

### **Existing Schema (No Changes Needed):**
- ✅ `Template` - Already has `OrganizationId`, `Status`, `MetadataJson`
- ✅ `TemplateVersion` - Already has `DesignJson` (store editable options here)
- ✅ `TemplatePage` - Already has structure
- ✅ `TemplateField` - Already has `IsLocked` flag ✅
- ✅ `CsvUpload` - Already has `MappingJson`
- ✅ `Content` - Already has `CsvUploadId`
- ✅ `ContentFieldValue` - Already has structure

### **Optional Enhancements (Future):**
- Add `TemplateShare` table (for sharing links)
- Add `ContentVersion` (for content history)
- Add indexes on `Template.OrganizationId`, `Content.OrganizationId`

---

## ⚡ PERFORMANCE & SCALABILITY

### **1. Caching Strategy:**
- Cache `Template` with `Versions`, `Pages`, `Fields` (Redis)
- Cache `TemplateField` lookup by name (Dictionary)
- Cache CSV parsing results (temporary)

### **2. Bulk Operations:**
- Use `BulkInsert` for `ContentFieldValue` (EF Core Extensions)
- Process CSV in batches (100 rows at a time)
- Async PDF generation (background jobs)

### **3. File Storage:**
- Use Azure Blob Storage / AWS S3 (not local)
- CDN for PDF delivery
- Compress PDFs before storage

### **4. Database Optimization:**
- Index on `Template.OrganizationId`
- Index on `Content.OrganizationId`
- Index on `TemplateField.IsLocked` (for filtering)
- Index on `CsvUpload.TemplateId`

### **5. API Performance:**
- Pagination for all list endpoints
- Lazy loading disabled (use `.Include()` explicitly)
- DTOs (don't return full entities)

---

## 🔐 SECURITY CONSIDERATIONS

### **1. Authorization:**
- SuperAdmin: Check `Role = "SuperAdmin"`
- OrgAdmin: Check `Role = "OrgAdmin"` AND `OrganizationId` matches
- Employee: Check `Role = "Employee"` AND `OrganizationId` matches

### **2. Field-Level Security:**
- Validate `IsLocked` flag before allowing updates
- Validate editable options from `DesignJson`
- Prevent direct database manipulation

### **3. File Upload Security:**
- Validate CSV file type
- Limit file size (10MB max)
- Sanitize CSV content
- Validate CSV structure

---

## 📝 IMPLEMENTATION CHECKLIST

### **Phase 1: Template Creation (SuperAdmin)**
- [ ] Create Template CRUD endpoints
- [ ] Create TemplateVersion CRUD endpoints
- [ ] Create TemplatePage CRUD endpoints
- [ ] Create TemplateField CRUD endpoints
- [ ] Store editable options in `DesignJson`

### **Phase 2: Template Editing (OrgAdmin)**
- [ ] Implement limited update endpoint
- [ ] Validate editable options
- [ ] Prevent editing locked fields

### **Phase 3: CSV Upload**
- [ ] File upload endpoint
- [ ] CSV parsing service
- [ ] Field mapping endpoint
- [ ] Content generation from CSV

### **Phase 4: Content Management (Employee)**
- [ ] Content CRUD endpoints
- [ ] Field value update (with `IsLocked` check)
- [ ] PDF generation service
- [ ] Content sharing

### **Phase 5: Performance & Optimization**
- [ ] Add caching
- [ ] Optimize database queries
- [ ] Implement bulk operations
- [ ] Add monitoring

---

## 🎯 SUMMARY

**Key Points:**
1. ✅ SuperAdmin creates templates with full control
2. ✅ `DesignJson` stores editable options for OrgAdmin
3. ✅ `TemplateField.IsLocked` controls Employee editing
4. ✅ CSV upload → Mapping → Content generation
5. ✅ Content → PDF generation → Sharing

**Architecture:**
- Follow existing CQRS pattern
- Use existing `IUnitOfWork` and repositories
- No changes to existing entities
- Add new handlers/controllers as needed

**Next Steps:**
1. Implement SuperAdmin template creation endpoints
2. Implement OrgAdmin limited editing
3. Implement CSV upload flow
4. Implement Employee content management

---

**Status:** ✅ **ARCHITECTURE DESIGNED - READY FOR IMPLEMENTATION**

