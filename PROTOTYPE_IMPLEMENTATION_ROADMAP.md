# Prototype Implementation Roadmap

**Date:** December 2024  
**Project:** SaaS Printing Management System  
**Phase:** Prototype Development  
**Status:** Ready to Start

---

## 🎯 PROTOTYPE SCOPE

### **What to Build (MVP):**
1. ✅ SuperAdmin: Manage Organizations & Printers
2. ✅ Organization Admin: Create Templates with Editable Fields
3. ✅ Employee: Edit Templates, Upload CSV, Send to Print
4. ✅ Sendout Tracking: Status updates from Printing Company
5. ✅ Multi-tenant: Data isolation per organization

### **What to Skip (Future):**
- Advanced analytics
- Complex workflows
- Email notifications
- Advanced reporting
- Mobile app

---

## 📋 IMPLEMENTATION PHASES

### **PHASE 1: SuperAdmin - Organization & Printer Management** (Week 1)

**⚠️ IMPORTANT: SuperAdmin CANNOT create templates. Only Tenants (Organization Admins) can create templates.**

#### **1.1 Organizations CRUD**
- [ ] `CreateOrganizationCommand` + Handler + Validator
- [ ] `UpdateOrganizationCommand` + Handler + Validator
- [ ] `DeleteOrganizationCommand` + Handler
- [ ] `GetOrganizationsQuery` + Handler (with pagination)
- [ ] `GetOrganizationByIdQuery` + Handler
- [ ] `ApproveOrganizationCommand` + Handler
- [ ] `DeactivateOrganizationCommand` + Handler
- [ ] `SuperAdminOrganizationsController`

#### **1.2 Printers CRUD**
- [ ] `CreatePrinterCommand` + Handler + Validator
- [ ] `UpdatePrinterCommand` + Handler + Validator
- [ ] `DeletePrinterCommand` + Handler
- [ ] `GetPrintersQuery` + Handler (with pagination)
- [ ] `GetPrinterByIdQuery` + Handler
- [ ] `SuperAdminPrintersController`

#### **1.3 Distribution (Printer-Organization Linking)**
- [ ] `CreateDistributionCommand` + Handler + Validator
- [ ] `UpdateDistributionCommand` + Handler + Validator
- [ ] `DeleteDistributionCommand` + Handler
- [ ] `GetDistributionsQuery` + Handler
- [ ] `GetOrganizationsByPrinterQuery` + Handler
- [ ] `GetPrintersByOrganizationQuery` + Handler
- [ ] `SuperAdminDistributionsController`

#### **1.4 Repositories**
- [ ] `IOrganizationRepository` + `OrganizationRepository`
- [ ] `IPrinterRepository` + `PrinterRepository`
- [ ] `IDistributionRepository` + `DistributionRepository`
- [ ] Update `IUnitOfWork` + `UnitOfWork`
- [ ] Register in DI

**Estimated Time:** 3-4 days

---

### **PHASE 2: Organization Admin (Tenant) - Template Management** (Week 2)

**⚠️ IMPORTANT: Only Tenants (Organization Admins) can create templates. SuperAdmin has NO access to template endpoints.**

#### **2.1 Templates CRUD**
- [ ] `CreateTemplateCommand` + Handler + Validator
- [ ] `UpdateTemplateCommand` + Handler + Validator
- [ ] `DeleteTemplateCommand` + Handler
- [ ] `GetTemplatesQuery` + Handler (filtered by OrganizationId)
- [ ] `GetTemplateByIdQuery` + Handler
- [ ] `OrganizationTemplatesController`

#### **2.2 Template Versions**
- [ ] `CreateTemplateVersionCommand` + Handler + Validator
- [ ] `UpdateTemplateVersionCommand` + Handler + Validator
- [ ] `DeleteTemplateVersionCommand` + Handler
- [ ] `GetTemplateVersionsQuery` + Handler
- [ ] `SetActiveVersionCommand` + Handler
- [ ] File upload service for PDF

#### **2.3 Template Pages**
- [ ] `CreateTemplatePageCommand` + Handler + Validator
- [ ] `UpdateTemplatePageCommand` + Handler + Validator
- [ ] `DeleteTemplatePageCommand` + Handler
- [ ] `GetTemplatePagesQuery` + Handler

#### **2.4 Template Fields (Editable Placeholders)**
- [ ] `CreateTemplateFieldCommand` + Handler + Validator
- [ ] `UpdateTemplateFieldCommand` + Handler + Validator
- [ ] `DeleteTemplateFieldCommand` + Handler
- [ ] `GetTemplateFieldsQuery` + Handler
- [ ] **Key:** Set `IsLocked` flag correctly

#### **2.5 Repositories**
- [ ] `ITemplateRepository` + `TemplateRepository`
- [ ] `ITemplateVersionRepository` + `TemplateVersionRepository`
- [ ] `ITemplatePageRepository` + `TemplatePageRepository`
- [ ] `ITemplateFieldRepository` + `TemplateFieldRepository`
- [ ] Update `IUnitOfWork` + `UnitOfWork`
- [ ] Register in DI

**Estimated Time:** 4-5 days

---

### **PHASE 3: CSV Upload & Content Generation** (Week 3)

#### **3.1 CSV Upload**
- [ ] `UploadCsvCommand` + Handler + Validator
- [ ] `GetCsvUploadsQuery` + Handler (filtered by OrganizationId)
- [ ] `GetCsvUploadByIdQuery` + Handler
- [ ] `DeleteCsvUploadCommand` + Handler
- [ ] `ICsvParserService` interface
- [ ] `CsvParserService` implementation (use CsvHelper NuGet)
- [ ] File upload to storage (`IFileStorageService`)

#### **3.2 Field Mapping**
- [ ] `MapCsvFieldsCommand` + Handler + Validator
- [ ] Auto-detect CSV columns
- [ ] Map CSV columns to TemplateField names
- [ ] Validate TemplateField exists
- [ ] Warn if mapping to locked field

#### **3.3 Content Generation**
- [ ] `GenerateContentFromCsvCommand` + Handler
- [ ] Parse CSV rows
- [ ] Create `Content` entity for each row
- [ ] Create `ContentFieldValue` for each mapped field
- [ ] Skip locked fields (or use default values)
- [ ] Bulk insert optimization

#### **3.4 Repositories**
- [ ] `ICsvUploadRepository` + `CsvUploadRepository`
- [ ] `IContentRepository` + `ContentRepository`
- [ ] `IContentFieldValueRepository` + `ContentFieldValueRepository`
- [ ] Update `IUnitOfWork` + `UnitOfWork`
- [ ] Register in DI

**Estimated Time:** 3-4 days

---

### **PHASE 4: Employee Content Management** (Week 4)

#### **4.1 Content CRUD**
- [ ] `CreateContentCommand` + Handler + Validator
- [ ] `UpdateContentCommand` + Handler + Validator
- [ ] `DeleteContentCommand` + Handler
- [ ] `GetContentsQuery` + Handler (filtered by OrganizationId)
- [ ] `GetContentByIdQuery` + Handler
- [ ] `ContentsController`

#### **4.2 Field Value Editing (With IsLocked Validation)**
- [ ] `UpdateContentFieldValueCommand` + Handler + Validator
- [ ] **Key Logic:** Check `TemplateField.IsLocked`
- [ ] If locked → Reject with error
- [ ] If unlocked → Allow update
- [ ] Validate field type (Text, Date, Number, etc.)

#### **4.3 PDF Generation**
- [ ] `GeneratePdfCommand` + Handler
- [ ] `IPdfGenerationService` interface
- [ ] `PdfGenerationService` implementation (iTextSharp/PdfSharp)
- [ ] Load TemplateVersion PDF
- [ ] Overlay field values at coordinates (X, Y)
- [ ] Apply field type formatting
- [ ] Save to storage, update `Content.GeneratedPdfUrl`

#### **4.4 Repositories**
- [ ] Already created in Phase 3

**Estimated Time:** 3-4 days

---

### **PHASE 5: Sendout & Tracking** (Week 5)

#### **5.1 Sendout Creation**
- [ ] `CreateSendoutCommand` + Handler + Validator
- [ ] Validate Content exists
- [ ] Validate Printer exists and is linked to Organization
- [ ] Generate unique `JobReference`
- [ ] Set initial status: `Submitted`
- [ ] Create initial `SendoutStatusHistory` entry

#### **5.2 Sendout API Integration**
- [ ] `SendToPrinterCommand` + Handler
- [ ] HTTP client to call Printer API endpoint
- [ ] Use `Printer.ApiKey` for authentication
- [ ] Send: `{ jobReference, pdfUrl, endUserDetails }`
- [ ] Handle API errors/retries

#### **5.3 Status Tracking**
- [ ] `UpdateSendoutStatusCommand` + Handler + Validator
- [ ] Validate status transition (Submitted → Received → Printing → Printed → Sent)
- [ ] Create `SendoutStatusHistory` entry
- [ ] Update `Sendout.CurrentStatus`
- [ ] Store notes, tracking number

#### **5.4 External API Endpoint (For Printing Company)**
- [ ] `ExternalSendoutsController`
- [ ] `ReceiveSendoutCommand` + Handler
- [ ] API key authentication middleware
- [ ] `UpdateSendoutStatusCommand` + Handler
- [ ] Webhook support (optional)

#### **5.5 Sendout Queries**
- [ ] `GetSendoutsQuery` + Handler (filtered by OrganizationId, Status)
- [ ] `GetSendoutByIdQuery` + Handler
- [ ] `GetSendoutStatusHistoryQuery` + Handler
- [ ] `SendoutsController`

#### **5.6 Repositories**
- [ ] `ISendoutRepository` + `SendoutRepository`
- [ ] `ISendoutStatusHistoryRepository` + `SendoutStatusHistoryRepository`
- [ ] Update `IUnitOfWork` + `UnitOfWork`
- [ ] Register in DI

**Estimated Time:** 4-5 days

---

## 🔧 INFRASTRUCTURE SETUP

### **Required NuGet Packages:**
- [ ] `CsvHelper` (for CSV parsing)
- [ ] `iTextSharp` or `PdfSharp` (for PDF generation)
- [ ] `Microsoft.EntityFrameworkCore.BulkExtensions` (for bulk inserts)
- [ ] `Polly` (for API retry logic)

### **Services to Create:**
- [ ] `ICsvParserService` + Implementation
- [ ] `IPdfGenerationService` + Implementation
- [ ] `IPrinterApiClient` + Implementation (for external API calls)

### **Middleware:**
- [ ] API key authentication middleware (for external endpoints)

---

## 🧪 TESTING CHECKLIST

### **Unit Tests:**
- [ ] Command handlers
- [ ] Query handlers
- [ ] Validators
- [ ] Services (CSV parser, PDF generator)

### **Integration Tests:**
- [ ] API endpoints
- [ ] Database operations
- [ ] Multi-tenant isolation
- [ ] Role-based access

### **Manual Testing:**
- [ ] SuperAdmin workflows
- [ ] Organization Admin workflows
- [ ] Employee workflows
- [ ] Sendout tracking
- [ ] API integration

---

## 📊 PROGRESS TRACKING

### **Week 1:**
- [ ] Phase 1 Complete
- [ ] SuperAdmin can manage Organizations
- [ ] SuperAdmin can manage Printers
- [ ] SuperAdmin can link Printers to Organizations
- [ ] **SuperAdmin CANNOT create templates** (Only Tenants can)

### **Week 2:**
- [ ] Phase 2 Complete
- [ ] Organization Admin can create Templates
- [ ] Organization Admin can define editable fields
- [ ] Organization Admin can set IsLocked flags

### **Week 3:**
- [ ] Phase 3 Complete
- [ ] CSV upload works
- [ ] Field mapping works
- [ ] Content generation from CSV works

### **Week 4:**
- [ ] Phase 4 Complete
- [ ] Employee can edit unlocked fields
- [ ] Employee cannot edit locked fields
- [ ] PDF generation works

### **Week 5:**
- [ ] Phase 5 Complete
- [ ] Sendout creation works
- [ ] API integration works
- [ ] Status tracking works

---

## 🚀 DEPLOYMENT CHECKLIST

- [ ] Database migrations applied
- [ ] Environment variables configured
- [ ] API keys configured
- [ ] File storage configured (Azure Blob/AWS S3)
- [ ] External API endpoints configured
- [ ] Logging configured
- [ ] Error handling configured

---

## 📝 NOTES

1. **Follow Existing Patterns:** Use existing CQRS, Repository, UnitOfWork patterns
2. **No Changes to Existing Code:** Only add new code
3. **Multi-Tenant:** Always filter by OrganizationId
4. **Security:** Validate roles, check IsLocked flags
5. **Performance:** Use bulk operations, caching, async processing

---

**Status:** ✅ **ROADMAP READY - START IMPLEMENTATION**

**Next Step:** Begin Phase 1 - SuperAdmin Organization & Printer Management

