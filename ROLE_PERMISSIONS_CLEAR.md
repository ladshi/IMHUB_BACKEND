# Role Permissions - Clear Definition

**Date:** December 2024  
**Status:** Final Clarification

---

## 🎯 ROLE PERMISSIONS MATRIX

### **1. SuperAdmin (Platform Level)**

**✅ CAN DO:**
- Manage Organizations (Create, Update, Approve, Deactivate)
- Manage Printers (Create, Update, Configure)
- Link Printers to Organizations (via Distribution)
- View all Organizations (across platform)
- View all Printers (global and organization-specific)
- Monitor system (dashboard, analytics)

**❌ CANNOT DO:**
- ❌ **Create Templates** (Only Tenants can create templates)
- ❌ **Manage Templates** (Only Tenants can manage templates)
- ❌ **Edit Templates** (Only Tenants can edit templates)
- ❌ **Create Content** (Only Tenants can create content)
- ❌ **Access Template endpoints** (Only Tenant endpoints)
- ❌ **Any Template-related operations**

**API Endpoints:**
```
✅ /api/superadmin/organizations/*
✅ /api/superadmin/printers/*
✅ /api/superadmin/distributions/*
❌ /api/superadmin/templates/* (DOES NOT EXIST)
❌ /api/organizations/templates/* (Tenant only)
```

---

### **2. Organization Admin (Tenant Level)**

**✅ CAN DO:**
- Create Templates (Upload PDF, Define editable placeholders)
- Manage Templates (Edit, Delete, Publish)
- Define Editable Areas (Set `TemplateField.IsLocked = false`)
- Lock Non-Editable Areas (Set `TemplateField.IsLocked = true`)
- Upload CSV Files (For bulk content generation)
- Map CSV Columns (To template fields)
- View All Content (Within their organization)
- Manage Employees (Within their organization)
- Create Content (From templates + CSV data)
- Send to Print (Via API to printing company)

**❌ CANNOT DO:**
- ❌ Manage Printers (Only SuperAdmin can)
- ❌ Link Printers to Organizations (Only SuperAdmin can)
- ❌ Access other organizations' data
- ❌ Access SuperAdmin endpoints

**API Endpoints:**
```
✅ /api/organizations/templates/*
✅ /api/organizations/templates/{id}/versions/*
✅ /api/organizations/templates/{id}/versions/{versionId}/pages/*
✅ /api/organizations/templates/{id}/versions/{versionId}/pages/{pageId}/fields/*
✅ /api/organizations/csv-uploads/*
✅ /api/organizations/contents/*
✅ /api/sendouts/*
❌ /api/superadmin/* (SuperAdmin only)
```

---

### **3. Employee (Tenant Level)**

**✅ CAN DO:**
- View Templates (Assigned to their organization)
- Edit Editable Areas Only (Fields where `IsLocked = false`)
- Create Content (From templates + CSV data)
- Upload CSV Files (With end user details)
- Send to Print (Via API to printing company)
- Track Sendout Status (Received, Printing, Sent, etc.)
- View Generated PDFs

**❌ CANNOT DO:**
- ❌ Create Templates (Only Organization Admin can)
- ❌ Edit Locked Fields (`IsLocked = true`)
- ❌ Manage Printers
- ❌ Link Printers to Organizations
- ❌ Access other organizations' data
- ❌ Access SuperAdmin endpoints
- ❌ Access Organization Admin management endpoints

**API Endpoints:**
```
✅ /api/templates (View only)
✅ /api/templates/{id} (View only)
✅ /api/contents/*
✅ /api/contents/{id}/fields/{fieldId} (Only if IsLocked = false)
✅ /api/contents/{id}/generate-pdf
✅ /api/csv-uploads/*
✅ /api/sendouts/*
❌ /api/organizations/templates/* (Create/Edit - OrgAdmin only)
❌ /api/superadmin/* (SuperAdmin only)
```

---

## 🔐 AUTHORIZATION RULES

### **Template Creation:**
```csharp
// ✅ ALLOWED: Organization Admin
if (currentUser.Role == "OrgAdmin" && currentUser.OrganizationId == template.OrganizationId)
{
    // Allow template creation
}

// ❌ NOT ALLOWED: SuperAdmin
if (currentUser.Role == "SuperAdmin")
{
    throw new UnauthorizedAccessException(
        "SuperAdmin cannot create templates. Only Organization Admins can.");
}

// ❌ NOT ALLOWED: Employee
if (currentUser.Role == "Employee")
{
    throw new UnauthorizedAccessException(
        "Employees cannot create templates. Only Organization Admins can.");
}
```

### **Template Access:**
```csharp
// ✅ Organization Admin: Can access templates in their organization
if (currentUser.Role == "OrgAdmin" && template.OrganizationId == currentUser.OrganizationId)
{
    // Allow access
}

// ✅ Employee: Can view templates in their organization
if (currentUser.Role == "Employee" && template.OrganizationId == currentUser.OrganizationId)
{
    // Allow view (read-only)
}

// ❌ SuperAdmin: Cannot access template endpoints
if (currentUser.Role == "SuperAdmin")
{
    throw new UnauthorizedAccessException(
        "SuperAdmin cannot access template endpoints.");
}
```

---

## 📋 IMPLEMENTATION CHECKLIST

### **SuperAdmin Controller:**
- [ ] `SuperAdminOrganizationsController` ✅
- [ ] `SuperAdminPrintersController` ✅
- [ ] `SuperAdminDistributionsController` ✅
- [ ] **NO Template Controller** ❌

### **Organization Admin Controller:**
- [ ] `OrganizationTemplatesController` ✅ (Tenant only)
- [ ] `OrganizationCsvUploadsController` ✅ (Tenant only)
- [ ] `OrganizationContentsController` ✅ (Tenant only)

### **Employee Controller:**
- [ ] `TemplatesController` ✅ (View only)
- [ ] `ContentsController` ✅ (Edit unlocked fields only)
- [ ] `SendoutsController` ✅

---

## 🎯 SUMMARY

**Key Points:**
1. ✅ **SuperAdmin:** Only manages Organizations, Printers, and Distribution linking
2. ✅ **Organization Admin (Tenant):** Creates and manages Templates
3. ✅ **Employee:** Views templates and edits only unlocked fields
4. ❌ **SuperAdmin CANNOT create templates** - This is tenant-only functionality

**Architecture:**
- SuperAdmin = Platform management
- Organization Admin = Tenant management (templates, content)
- Employee = End-user operations (editing, sending)

---

**Status:** ✅ **ROLE PERMISSIONS CLARIFIED**

