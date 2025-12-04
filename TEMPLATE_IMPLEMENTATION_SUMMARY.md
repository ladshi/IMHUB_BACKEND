# Template System Implementation Summary

**Date:** December 2024  
**Status:** Ready for Implementation  
**Architecture:** Follow existing CQRS pattern, no changes to existing code

---

## ✅ WHAT'S ALREADY IN PLACE

### **Database Entities (No Changes Needed):**
- ✅ `Template` - Has `OrganizationId`, `Status`, `MetadataJson`
- ✅ `TemplateVersion` - Has `DesignJson` (store editable options here)
- ✅ `TemplatePage` - Complete structure
- ✅ `TemplateField` - Has `IsLocked` flag ✅ (This is the key!)
- ✅ `CsvUpload` - Has `MappingJson`
- ✅ `Content` - Has `CsvUploadId`
- ✅ `ContentFieldValue` - Complete structure

### **Existing Infrastructure:**
- ✅ `IUnitOfWork` pattern
- ✅ Repository pattern (`IGenericRepository`)
- ✅ CQRS with MediatR
- ✅ FluentValidation
- ✅ `ICurrentUserService` (for user context)
- ✅ `IFileStorageService` (for file uploads)

---

## 🎯 WHAT NEEDS TO BE IMPLEMENTED

### **Phase 1: SuperAdmin Template Creation**

**New Files Needed:**
```
ApplicationLayer/Features/Templates/
├── Commands/
│   ├── CreateTemplateCommand.cs
│   ├── CreateTemplateCommandHandler.cs
│   ├── CreateTemplateCommandValidator.cs
│   ├── CreateTemplateVersionCommand.cs
│   ├── CreateTemplateVersionCommandHandler.cs
│   ├── CreateTemplatePageCommand.cs
│   ├── CreateTemplatePageCommandHandler.cs
│   ├── CreateTemplateFieldCommand.cs
│   └── CreateTemplateFieldCommandHandler.cs
├── Queries/
│   ├── GetTemplateByIdQuery.cs
│   └── GetTemplateByIdQueryHandler.cs
└── DTOs/
    ├── TemplateDto.cs
    ├── TemplateVersionDto.cs
    ├── TemplatePageDto.cs
    └── TemplateFieldDto.cs

IMHub.API/Controllers/
└── SuperAdminTemplatesController.cs
```

**Key Logic:**
- SuperAdmin creates `Template` with `OrganizationId`
- Stores editable options in `TemplateVersion.DesignJson`:
  ```json
  {
    "editableOptions": {
      "allowOrgAdminEdit": ["title", "thumbnailUrl", "metadataJson"]
    }
  }
  ```
- Sets `TemplateField.IsLocked` flag:
  - `IsLocked = false` → Employee CAN edit
  - `IsLocked = true` → Employee CANNOT edit

---

### **Phase 2: Organization Admin Limited Editing**

**New Files Needed:**
```
ApplicationLayer/Features/Templates/
└── Commands/
    ├── UpdateTemplateCommand.cs (Limited)
    └── UpdateTemplateCommandHandler.cs

IMHub.API/Controllers/
└── OrganizationTemplatesController.cs
```

**Key Logic:**
```csharp
// In UpdateTemplateCommandHandler
var editableOptions = JsonSerializer.Deserialize<EditableOptions>(
    templateVersion.DesignJson);

// Only allow editing fields in editableOptions.allowOrgAdminEdit
if (!editableOptions.AllowOrgAdminEdit.Contains(fieldName))
{
    throw new UnauthorizedAccessException(
        $"Field '{fieldName}' is not editable by Organization Admin.");
}
```

---

### **Phase 3: CSV Upload & Processing**

**New Files Needed:**
```
ApplicationLayer/Features/CsvUploads/
├── Commands/
│   ├── UploadCsvCommand.cs
│   ├── UploadCsvCommandHandler.cs
│   ├── MapCsvFieldsCommand.cs
│   ├── MapCsvFieldsCommandHandler.cs
│   ├── GenerateContentFromCsvCommand.cs
│   └── GenerateContentFromCsvCommandHandler.cs
├── Queries/
│   ├── GetCsvUploadByIdQuery.cs
│   └── GetCsvUploadByIdQueryHandler.cs
└── Services/
    └── ICsvParserService.cs (New interface)

IMHub.API/Controllers/
└── CsvUploadsController.cs
```

**Key Logic:**
1. Upload CSV → Save to storage (`IFileStorageService`)
2. Parse CSV → Create `CsvUpload` entity
3. Map CSV columns → TemplateFields (validate `IsLocked`)
4. Generate `Content` + `ContentFieldValue` for each CSV row
5. Skip locked fields (or use default values)

---

### **Phase 4: Employee Content Management**

**New Files Needed:**
```
ApplicationLayer/Features/Contents/
├── Commands/
│   ├── CreateContentCommand.cs
│   ├── CreateContentCommandHandler.cs
│   ├── UpdateContentFieldValueCommand.cs
│   ├── UpdateContentFieldValueCommandHandler.cs
│   ├── GeneratePdfCommand.cs
│   └── GeneratePdfCommandHandler.cs
├── Queries/
│   ├── GetContentByIdQuery.cs
│   ├── GetContentByIdQueryHandler.cs
│   ├── GetContentsQuery.cs
│   └── GetContentsQueryHandler.cs
└── Services/
    └── IPdfGenerationService.cs (New interface)

IMHub.API/Controllers/
└── ContentsController.cs
```

**Key Logic:**
```csharp
// In UpdateContentFieldValueCommandHandler
var templateField = await _unitOfWork.TemplateFieldRepository
    .GetByIdAsync(fieldId);

if (templateField.IsLocked)
{
    throw new UnauthorizedAccessException(
        $"Field '{templateField.FieldName}' is locked and cannot be edited.");
}

// Allow update
contentFieldValue.Value = request.Value;
```

---

### **Phase 5: Repositories (If Needed)**

**New Repository Interfaces (if custom methods needed):**
```
ApplicationLayer/Common/Interfaces/IRepositories/
├── ITemplateRepository.cs
├── ITemplateVersionRepository.cs
├── ITemplatePageRepository.cs
├── ITemplateFieldRepository.cs
├── ICsvUploadRepository.cs
└── IContentRepository.cs

Infrastructure Layer/Repositories/
├── TemplateRepository.cs
├── TemplateVersionRepository.cs
├── TemplatePageRepository.cs
├── TemplateFieldRepository.cs
├── CsvUploadRepository.cs
└── ContentRepository.cs
```

**Update UnitOfWork:**
```csharp
public interface IUnitOfWork
{
    // ... existing repositories ...
    ITemplateRepository TemplateRepository { get; }
    ITemplateVersionRepository TemplateVersionRepository { get; }
    ITemplatePageRepository TemplatePageRepository { get; }
    ITemplateFieldRepository TemplateFieldRepository { get; }
    ICsvUploadRepository CsvUploadRepository { get; }
    IContentRepository ContentRepository { get; }
}
```

---

## 🔑 KEY IMPLEMENTATION POINTS

### **1. IsLocked Flag Validation:**
- ✅ Check `TemplateField.IsLocked` before allowing Employee to edit
- ✅ Use in `UpdateContentFieldValueCommandHandler`
- ✅ Use in CSV mapping validation

### **2. Editable Options:**
- ✅ Store in `TemplateVersion.DesignJson`
- ✅ Parse in `UpdateTemplateCommandHandler`
- ✅ Validate OrgAdmin can only edit allowed fields

### **3. CSV Processing:**
- ✅ Use `IFileStorageService` for file upload
- ✅ Parse CSV (use `CsvHelper` NuGet package)
- ✅ Map columns to `TemplateField` names
- ✅ Validate `IsLocked` before mapping
- ✅ Generate `Content` + `ContentFieldValue` in bulk

### **4. PDF Generation:**
- ✅ Create `IPdfGenerationService` interface
- ✅ Implement with iTextSharp or PdfSharp
- ✅ Merge template PDF with field values at coordinates
- ✅ Save to storage, update `Content.GeneratedPdfUrl`

---

## 📋 IMPLEMENTATION CHECKLIST

### **SuperAdmin Template Creation:**
- [ ] Create Template CRUD endpoints
- [ ] Create TemplateVersion CRUD endpoints
- [ ] Create TemplatePage CRUD endpoints
- [ ] Create TemplateField CRUD endpoints
- [ ] Store editable options in `DesignJson`
- [ ] Set `IsLocked` flags correctly

### **Organization Admin Editing:**
- [ ] Create limited update endpoint
- [ ] Parse `DesignJson` editable options
- [ ] Validate only allowed fields can be edited
- [ ] Prevent editing structure/coordinates

### **CSV Upload:**
- [ ] File upload endpoint
- [ ] CSV parsing service
- [ ] Field mapping endpoint
- [ ] Content generation from CSV
- [ ] Validate `IsLocked` during mapping

### **Employee Content Management:**
- [ ] Content CRUD endpoints
- [ ] Field value update (with `IsLocked` check)
- [ ] PDF generation service
- [ ] Content sharing (optional)

### **Repositories & UnitOfWork:**
- [ ] Create repository interfaces
- [ ] Create repository implementations
- [ ] Update `IUnitOfWork`
- [ ] Update `UnitOfWork`
- [ ] Register in DI

---

## 🚀 PERFORMANCE CONSIDERATIONS

### **1. Bulk Operations:**
- Use `BulkInsert` for `ContentFieldValue` (EF Core Extensions)
- Process CSV in batches (100 rows at a time)
- Async PDF generation (background jobs)

### **2. Caching:**
- Cache `Template` with `Versions`, `Pages`, `Fields`
- Cache `TemplateField` lookup by name
- Cache CSV parsing results (temporary)

### **3. Database:**
- Add indexes on `Template.OrganizationId`
- Add indexes on `Content.OrganizationId`
- Add indexes on `TemplateField.IsLocked`

---

## 🔐 SECURITY CHECKLIST

- [ ] Validate user role (SuperAdmin/OrgAdmin/Employee)
- [ ] Validate `OrganizationId` matches current user
- [ ] Check `IsLocked` flag before allowing edits
- [ ] Validate editable options from `DesignJson`
- [ ] Sanitize CSV file uploads
- [ ] Validate file size limits
- [ ] Prevent SQL injection (use parameterized queries)

---

## 📝 NOTES

1. **No Changes to Existing Code:** All new code follows existing patterns
2. **Use Existing Infrastructure:** `IUnitOfWork`, `IGenericRepository`, `ICurrentUserService`
3. **Follow CQRS Pattern:** Commands for writes, Queries for reads
4. **Use FluentValidation:** Validate all commands
5. **Performance First:** Bulk operations, caching, async processing

---

**Status:** ✅ **READY FOR IMPLEMENTATION**

**Next Step:** Start with Phase 1 (SuperAdmin Template Creation)

