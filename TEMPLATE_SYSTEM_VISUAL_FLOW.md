# Template System - Visual Flow Diagrams

**Date:** December 2024  
**Purpose:** Visual representation of Template System flows

---

## 🔄 FLOW 1: TEMPLATE CREATION (SuperAdmin)

```
┌─────────────┐
│ SuperAdmin  │
└──────┬──────┘
       │
       │ 1. POST /api/superadmin/templates
       │    { organizationId, title, slug, ... }
       ▼
┌──────────────────┐
│   Template       │
│   - Id: 1        │
│   - OrgId: 1     │
│   - Status: Draft│
└──────┬───────────┘
       │
       │ 2. POST /api/superadmin/templates/1/versions
       │    { versionNumber: 1, pdfUrl, designJson }
       ▼
┌──────────────────────┐
│  TemplateVersion     │
│  - Id: 1             │
│  - TemplateId: 1    │
│  - VersionNumber: 1 │
│  - DesignJson: {     │
│      editableOptions │
│    }                 │
└──────┬───────────────┘
       │
       │ 3. POST /api/superadmin/templates/1/versions/1/pages
       │    { pageNumber: 1, width, height }
       ▼
┌──────────────────┐
│  TemplatePage    │
│  - Id: 1         │
│  - PageNumber: 1 │
└──────┬───────────┘
       │
       │ 4. POST /api/superadmin/templates/1/versions/1/pages/1/fields
       │    { fieldName, fieldType, isLocked, x, y }
       ▼
┌──────────────────┐
│  TemplateField   │
│  - Id: 1         │
│  - FieldName     │
│  - IsLocked: ❌  │ ← Employee CAN edit
│                  │
│  - Id: 2         │
│  - FieldName     │
│  - IsLocked: ✅  │ ← Employee CANNOT edit
└──────────────────┘
```

---

## 🔄 FLOW 2: TEMPLATE EDITING PERMISSIONS

### **Organization Admin Edits:**

```
┌─────────────────┐
│ Organization    │
│ Admin           │
└────────┬────────┘
         │
         │ PUT /api/organizations/templates/1
         │ { title, thumbnailUrl, metadataJson }
         ▼
    ┌────────────┐
    │ Validation │
    └─────┬──────┘
          │
          │ Check DesignJson.editableOptions
          │ allowOrgAdminEdit: ["title", "thumbnailUrl"]
          ▼
    ┌─────────────┐
    │ ✅ ALLOWED  │
    └─────────────┘
    
    ❌ NOT ALLOWED:
    - TemplateVersion structure
    - TemplatePage structure
    - TemplateField coordinates
    - TemplateField.IsLocked flag
```

### **Employee Edits:**

```
┌──────────┐
│ Employee │
└────┬─────┘
     │
     │ PUT /api/contents/1/fields/1
     │ { value: "John Doe" }
     ▼
┌──────────────────┐
│  TemplateField   │
│  - Id: 1         │
│  - IsLocked: ❌  │ ← Check this flag
└────┬─────────────┘
     │
     │ IsLocked = false?
     ▼
┌─────────────┐
│ ✅ ALLOWED  │
│ Update Value│
└─────────────┘

     │ IsLocked = true?
     ▼
┌─────────────┐
│ ❌ REJECTED │
│ Unauthorized│
└─────────────┘
```

---

## 🔄 FLOW 3: CSV UPLOAD & PROCESSING

```
┌─────────────────┐
│ Organization    │
│ Admin           │
└────────┬────────┘
         │
         │ 1. POST /api/organizations/templates/1/csv-upload
         │    Upload: invoice-data.csv
         ▼
┌──────────────────┐
│   CsvUpload      │
│   - FileName     │
│   - FileUrl      │
│   - TotalRows: 100│
│   - MappingJson: {}│
└────────┬─────────┘
         │
         │ 2. POST /api/organizations/csv-uploads/1/map-fields
         │    { mappings: [{csvColumn, templateFieldName}] }
         ▼
┌──────────────────┐
│  MappingJson     │
│  {               │
│    "CustomerName"│
│    → "CustomerName"│
│    "Amount"      │
│    → "Amount"    │
│  }               │
└────────┬─────────┘
         │
         │ 3. POST /api/organizations/csv-uploads/1/generate-content
         │    { generateAll: true }
         ▼
┌─────────────────────────────────────┐
│  For Each CSV Row:                  │
│                                     │
│  ┌──────────────┐                  │
│  │   Content    │                  │
│  │   - Name      │                  │
│  │   - Status    │                  │
│  └──────┬───────┘                  │
│         │                           │
│         │ For Each Mapped Field:    │
│         ▼                           │
│  ┌──────────────────┐               │
│  │ ContentFieldValue│               │
│  │ - TemplateFieldId│               │
│  │ - Value          │               │
│  └──────────────────┘               │
│                                     │
│  Check: IsLocked?                   │
│  - If locked → Skip                 │
│  - If unlocked → Set value          │
└─────────────────────────────────────┘
```

---

## 🔄 FLOW 4: CONTENT GENERATION & PDF

```
┌──────────┐
│ Employee │
└────┬─────┘
     │
     │ POST /api/contents
     │ { templateVersionId, name, fieldValues }
     ▼
┌──────────────────┐
│   Content        │
│   - Id: 1        │
│   - Name         │
│   - Status: Draft│
└────┬─────────────┘
     │
     │ POST /api/contents/1/generate-pdf
     ▼
┌─────────────────────────────────────┐
│  PDF Generation Process:           │
│                                     │
│  1. Load TemplateVersion           │
│     - Pages                        │
│     - Fields (with coordinates)    │
│                                     │
│  2. Load Content                   │
│     - FieldValues                  │
│                                     │
│  3. Merge:                         │
│     Template PDF + Field Values    │
│     at coordinates (X, Y)           │
│                                     │
│  4. Save PDF to Storage            │
│                                     │
│  5. Update Content.GeneratedPdfUrl │
└────┬────────────────────────────────┘
     │
     │ GET /api/contents/1/pdf
     ▼
┌─────────────┐
│  PDF File   │
│  (Download) │
└─────────────┘
```

---

## 🔄 FLOW 5: COMPLETE END-TO-END FLOW

```
┌─────────────────────────────────────────────────────────────┐
│                    SUPERADMIN CREATES                        │
│                                                             │
│  1. Create Template                                        │
│  2. Create TemplateVersion (with editableOptions)          │
│  3. Create TemplatePage                                    │
│  4. Create TemplateFields (set IsLocked flags)             │
│                                                             │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│              ORGANIZATION ADMIN EDITS                        │
│                                                             │
│  1. View Template                                          │
│  2. Edit Allowed Fields Only                               │
│     (title, thumbnailUrl, metadataJson)                    │
│  3. Upload CSV File                                        │
│  4. Map CSV Columns → TemplateFields                      │
│  5. Generate Content from CSV                             │
│                                                             │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                  EMPLOYEE WORKS WITH                        │
│                                                             │
│  1. View Template                                          │
│  2. See Only Unlocked Fields                               │
│  3. Edit Content Field Values                              │
│     (Only where IsLocked = false)                          │
│  4. Generate PDF                                           │
│  5. Share Content                                           │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔐 PERMISSION MATRIX

| Action | SuperAdmin | OrgAdmin | Employee |
|--------|-----------|----------|----------|
| Create Template | ✅ | ❌ | ❌ |
| Edit Template Structure | ✅ | ❌ | ❌ |
| Edit Template Metadata | ✅ | ✅ (Limited) | ❌ |
| Set Field IsLocked | ✅ | ❌ | ❌ |
| Upload CSV | ✅ | ✅ | ❌ |
| Map CSV Fields | ✅ | ✅ | ❌ |
| Generate Content from CSV | ✅ | ✅ | ❌ |
| Create Content Manually | ✅ | ✅ | ✅ |
| Edit Content Field Value | ✅ | ✅ | ✅ (Only if IsLocked = false) |
| Generate PDF | ✅ | ✅ | ✅ |
| Share Content | ✅ | ✅ | ✅ |

---

## 📊 DATA FLOW DIAGRAM

```
┌──────────────┐
│   CSV File   │
└──────┬───────┘
       │ Parse
       ▼
┌──────────────┐      ┌──────────────────┐
│  CSV Rows    │──────▶│  TemplateField   │
│  (100 rows)  │ Map   │  - FieldName     │
└──────────────┘      │  - IsLocked      │
                      └────────┬──────────┘
                               │
                               │ Filter: IsLocked = false
                               ▼
                      ┌──────────────────┐
                      │  Content         │
                      │  - Name         │
                      │  - Status       │
                      └────────┬─────────┘
                               │
                               │ For Each Field
                               ▼
                      ┌──────────────────┐
                      │ ContentFieldValue│
                      │ - Value         │
                      └──────────────────┘
```

---

## 🎯 KEY DECISION POINTS

### **1. IsLocked Flag Check:**
```
IF TemplateField.IsLocked == true:
    ❌ Employee CANNOT edit
    ✅ OrgAdmin CAN edit (if in editableOptions)
    ✅ SuperAdmin CAN edit

IF TemplateField.IsLocked == false:
    ✅ Employee CAN edit
    ✅ OrgAdmin CAN edit
    ✅ SuperAdmin CAN edit
```

### **2. Editable Options Check:**
```
IF fieldName IN DesignJson.editableOptions.allowOrgAdminEdit:
    ✅ OrgAdmin CAN edit Template metadata

IF fieldName NOT IN DesignJson.editableOptions.allowOrgAdminEdit:
    ❌ OrgAdmin CANNOT edit Template metadata
```

### **3. CSV Mapping Validation:**
```
FOR EACH mapping:
    IF TemplateField.IsLocked == true:
        ⚠️  WARN: Mapping to locked field
        ⚠️  Employee cannot edit this value
    
    IF TemplateField.IsLocked == false:
        ✅ OK: Mapping to unlocked field
        ✅ Employee can edit this value
```

---

**Status:** ✅ **VISUAL FLOWS COMPLETE**

