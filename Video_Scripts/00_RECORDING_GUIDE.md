# NPP Contract Management API - Video Recording Guide

## 📹 Complete Video Script Package

This package contains 8 comprehensive video scripts covering the entire NPP Contract Management API.

---

## 📋 Video Series Overview

| # | Video Title | Duration | Topics Covered |
|---|-------------|----------|----------------|
| 1 | Introduction & System Overview | 4 min | Architecture, tech stack, domain areas |
| 2 | Authentication & User Management APIs | 9 min | Login, users, roles, manufacturers |
| 3 | Master Data Management APIs | 10 min | Manufacturers, products, distributors, industries, OpCos |
| 4 | Proposal Management APIs | 12 min | CRUD, workflow, Excel import/export |
| 5 | Contract Management APIs | 19 min | CRUD, versioning, pricing, assignments, bulk renewal |
| 6 | Velocity Integration APIs | 11 min | CSV import, job tracking, shipment matching |
| 7 | Reporting & Lookup APIs | 10 min | Reports, dashboard, reference data |
| 8 | Core Domain Objects & Data Models | 12 min | All entities and relationships |
| 9 | Additional APIs | 9 min | Customer accounts, member accounts, distributor codes |

**Total Duration:** ~96 minutes (~1 hour 36 minutes)

---

## 🎬 Recording Setup

### Equipment Needed
- **Screen 1:** Display the script (this document)
- **Screen 2:** Record Swagger UI / API documentation
- **Microphone:** Clear audio recording
- **Screen Recording Software:** OBS Studio, Camtasia, or similar

### Screen Layout
```
┌─────────────────────┬─────────────────────┐
│                     │                     │
│   Screen 1          │   Screen 2          │
│   (Script)          │   (Swagger/Demo)    │
│                     │                     │
│   Read from here    │   Record this       │
│                     │                     │
└─────────────────────┴─────────────────────┘
```

### Recording Settings
- **Resolution:** 1920x1080 (Full HD)
- **Frame Rate:** 30 FPS
- **Audio:** 48kHz, 16-bit
- **Format:** MP4 (H.264 codec)

---

## 📝 Recording Tips

### Before Recording
1. ✅ Start the NPP API (`dotnet run` in API project)
2. ✅ Open Swagger UI: `http://localhost:5110/swagger`
3. ✅ Login and get JWT token for authenticated endpoints
4. ✅ Have sample data ready in database
5. ✅ Close unnecessary browser tabs
6. ✅ Disable notifications
7. ✅ Test microphone levels

### During Recording
1. 📖 **Read naturally** - Don't rush, speak clearly
2. 🖱️ **Show, don't just tell** - Expand endpoints in Swagger as you discuss them
3. ⏸️ **Pause between sections** - Makes editing easier
4. 🔍 **Zoom in** on important details (request/response bodies)
5. 🎯 **Follow the script** - But feel free to add natural transitions
6. 🔄 **Re-record if needed** - Don't worry about mistakes, just start the section again

### After Recording
1. ✂️ Edit out long pauses and mistakes
2. 🎵 Add intro/outro music (optional)
3. 📊 Add title cards for each section
4. 🔊 Normalize audio levels
5. 📤 Export in high quality

---

## 🎯 What to Show on Screen

### Video 1: Introduction
- NPP logo or dashboard
- Architecture diagram (create simple diagram)
- List of controllers in Swagger
- Domain model diagram

### Video 2: Authentication & User Management
- Swagger UI: AuthController
- Swagger UI: UsersController
- Swagger UI: RolesController
- Show login request/response
- Show user list with pagination
- Show user creation

### Video 3: Master Data Management
- Swagger UI: ManufacturersController
- Swagger UI: ProductsController
- Swagger UI: DistributorsController
- Swagger UI: IndustriesController
- Swagger UI: OpCosController
- Show GET requests with filters
- Show POST/PUT requests

### Video 4: Proposal Management
- Swagger UI: ProposalsController
- Show proposal list
- Show proposal details
- Show create proposal request
- Show workflow actions (submit, accept, reject)
- Show Excel template download
- Show Excel import response

### Video 5: Contract Management
- Swagger UI: ContractsController
- Show contract list
- Show contract details
- Show create contract from proposal
- Show versioning endpoints
- Show version comparison
- Show bulk renewal request

### Video 6: Velocity Integration
- Swagger UI: VelocityController
- Show CSV template
- Show ingest endpoint
- Show job list
- Show job details with errors
- Show shipment list (matched/unmatched)
- Show velocity reports

### Video 7: Reporting & Lookup
- Swagger UI: ReportsController
- Swagger UI: LookupController
- Show contract over-term report
- Show dashboard statistics
- Show lookup endpoints (proposal types, statuses, etc.)

### Video 8: Domain Objects
- Database diagram (create ERD)
- Show entity definitions
- Show relationships
- Highlight key fields
- Show complete data model

---

## 📊 Creating Visual Aids

### Architecture Diagram (Video 1)
Create a simple diagram showing:
```
┌─────────────────┐
│  Angular 18     │
│  Frontend       │
└────────┬────────┘
         │ HTTP/REST
         ▼
┌─────────────────┐
│  ASP.NET Core   │
│  Web API        │
└────────┬────────┘
         │ EF Core
         ▼
┌─────────────────┐
│  SQL Server     │
│  Database       │
└─────────────────┘
```

### Domain Model Diagram (Video 1 & 8)
Create ERD showing:
- Users → Roles (many-to-many)
- Users → Manufacturers (many-to-many)
- Manufacturers → Products (one-to-many)
- Proposals → ProposalProducts (one-to-many)
- Contracts → ContractPrices (one-to-many)
- Contracts → ContractVersions (one-to-many)

Use tools like:
- draw.io
- Lucidchart
- dbdiagram.io
- Visual Studio Database Diagram

---

## 🎤 Script Reading Tips

### Pacing
- **Normal speaking pace:** 150-160 words per minute
- **Technical content:** 120-140 words per minute (slower)
- **Pause after each slide:** 1-2 seconds

### Emphasis
- **Endpoint names:** Speak clearly, spell out if needed
- **HTTP methods:** Emphasize (GET, POST, PUT, DELETE)
- **Status codes:** Emphasize (200 OK, 201 Created, etc.)
- **Important fields:** Slow down for key properties

### Pronunciation
- **API:** "A-P-I" (spell it out)
- **JWT:** "J-W-T" or "jot" (your preference)
- **CRUD:** "crud" (rhymes with "mud")
- **DTO:** "D-T-O" (spell it out)
- **OpCo:** "op-co" (operating company)
- **GTIN:** "G-T-I-N" (spell it out)
- **UPC:** "U-P-C" (spell it out)

---

## ✅ Pre-Recording Checklist

### Environment Setup
- [ ] API is running on `http://localhost:5110`
- [ ] Swagger UI is accessible
- [ ] Database has sample data
- [ ] JWT token is ready for authenticated endpoints
- [ ] Browser is in full-screen mode
- [ ] Zoom level is 100% (or 125% for better visibility)

### Recording Software
- [ ] Screen recording software is open
- [ ] Recording area is set to Screen 2
- [ ] Microphone is selected and tested
- [ ] Audio levels are good (not too loud, not too quiet)
- [ ] Output folder is set

### Scripts
- [ ] All 8 scripts are open on Screen 1
- [ ] Scripts are easy to read (font size 14-16pt)
- [ ] Scripts are in order (01, 02, 03, etc.)

### Visual Aids
- [ ] Architecture diagram is ready
- [ ] ERD diagram is ready
- [ ] Any other diagrams are ready

---

## 🎬 Recording Workflow

### For Each Video:

1. **Prepare**
   - Open the script on Screen 1
   - Open Swagger UI on Screen 2
   - Navigate to the relevant controller
   - Take a deep breath

2. **Record**
   - Start screen recording
   - Count down: "3, 2, 1"
   - Begin reading the script
   - Show relevant endpoints as you discuss them
   - Expand request/response examples
   - Pause between slides

3. **Review**
   - Stop recording
   - Watch the recording
   - Check audio quality
   - Check if all endpoints were shown
   - Re-record if needed

4. **Save**
   - Save with clear filename: `NPP_API_Video_01_Introduction.mp4`
   - Move to output folder
   - Update checklist

---

## 📦 Deliverables

After recording all 8 videos, you should have:

1. ✅ 8 video files (MP4 format)
2. ✅ Total duration: ~81 minutes
3. ✅ Clear audio throughout
4. ✅ All endpoints demonstrated
5. ✅ Visual aids included

### File Naming Convention
```
NPP_API_Video_01_Introduction.mp4
NPP_API_Video_02_Authentication.mp4
NPP_API_Video_03_Master_Data.mp4
NPP_API_Video_04_Proposals.mp4
NPP_API_Video_05_Contracts.mp4
NPP_API_Video_06_Velocity.mp4
NPP_API_Video_07_Reports_Lookup.mp4
NPP_API_Video_08_Domain_Objects.mp4
```

---

## 🚀 Ready to Record?

1. Review this guide
2. Set up your recording environment
3. Start with Video 1 (Introduction)
4. Work through each video in order
5. Take breaks between videos
6. Review and edit as needed

**Good luck with your recording!** 🎥

---

## 📞 Questions?

If you have questions about the scripts or need clarification on any endpoint, refer to:
- Swagger UI documentation
- API source code
- Database schema
- This guide

**You've got this!** 💪

