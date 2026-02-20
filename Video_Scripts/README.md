# NPP Contract Management API - Video Scripts Package

## 📦 Package Contents

This folder contains everything you need to create professional video tutorials for the NPP Contract Management API.

### 📄 Files Included

1. **00_RECORDING_GUIDE.md** ⭐ START HERE
   - Complete recording setup guide
   - Equipment checklist
   - Recording tips and best practices
   - Pre-recording checklist

2. **01_Introduction_Script.md**
   - System overview
   - Architecture
   - Domain areas
   - 4 minutes

3. **02_Authentication_APIs_Script.md**
   - Login/logout
   - User management
   - Role management
   - 9 minutes

4. **03_Master_Data_APIs_Script.md**
   - Manufacturers
   - Products
   - Distributors, Industries, OpCos
   - 10 minutes

5. **04_Proposal_APIs_Script.md**
   - Proposal CRUD
   - Workflow (submit, accept, reject)
   - Excel import/export
   - 12 minutes

6. **05_Contract_APIs_Script.md**
   - Contract CRUD
   - Versioning
   - Pricing management
   - Bulk renewal
   - 13 minutes

7. **06_Velocity_APIs_Script.md**
   - CSV import
   - Job tracking
   - Shipment matching
   - SFTP integration
   - 11 minutes

8. **07_Reports_Lookup_APIs_Script.md**
   - Contract reports
   - Dashboard statistics
   - Lookup endpoints
   - 10 minutes

9. **08_Domain_Objects_Script.md**
   - All entity models
   - Relationships
   - Data model overview
   - 12 minutes

10. **09_Additional_APIs_Script.md**
    - Customer Accounts
    - Member Accounts
    - Distributor Product Codes
    - Test Controller
    - 9 minutes

11. **API_Quick_Reference.md** 📋 KEEP HANDY
    - All endpoints at a glance
    - Sample requests/responses
    - Common parameters
    - Test data

---

## 🎯 How to Use This Package

### Step 1: Preparation (30 minutes)
1. Read **00_RECORDING_GUIDE.md** completely
2. Review **API_Quick_Reference.md**
3. Set up your recording environment
4. Test your equipment

### Step 2: Environment Setup (15 minutes)
1. Start the NPP API
2. Open Swagger UI
3. Login and get JWT token
4. Verify sample data exists
5. Open scripts on Screen 1
6. Open Swagger on Screen 2

### Step 3: Recording (2-3 hours)
1. Start with Video 1 (Introduction)
2. Record each video in order
3. Take 5-minute breaks between videos
4. Review each recording before moving on

### Step 4: Post-Production (1-2 hours)
1. Edit out mistakes and long pauses
2. Add intro/outro (optional)
3. Add title cards
4. Normalize audio
5. Export in high quality

---

## 📊 Total Time Estimate

| Phase | Time |
|-------|------|
| Preparation | 30 min |
| Environment Setup | 15 min |
| Recording (9 videos) | 2.5-3.5 hours |
| Post-Production | 1-2 hours |
| **TOTAL** | **4.5-6.5 hours** |

---

## 🎬 Recording Order

**Recommended order:**

1. ✅ Video 1: Introduction (warm-up, easiest)
2. ✅ Video 2: Authentication (fundamental)
3. ✅ Video 3: Master Data (builds on #2)
4. ✅ Video 4: Proposals (core functionality)
5. ✅ Video 5: Contracts (core functionality)
6. ⏸️ **BREAK** (15-30 minutes)
7. ✅ Video 6: Velocity (complex, need fresh mind)
8. ✅ Video 7: Reports & Lookup (easier, wind down)
9. ✅ Video 8: Domain Objects (technical)
10. ✅ Video 9: Additional APIs (finish strong)

---

## 💡 Tips for Success

### Before You Start
- ✅ Read all scripts once to familiarize yourself
- ✅ Practice reading a few paragraphs out loud
- ✅ Have water nearby (stay hydrated!)
- ✅ Eliminate distractions (phone on silent, close Slack/Teams)

### During Recording
- 🎤 Speak clearly and at a moderate pace
- 🖱️ Show what you're talking about in Swagger
- ⏸️ Pause between sections for easier editing
- 🔄 Don't worry about mistakes - just re-record that section
- 💧 Take sips of water during pauses

### After Recording
- 👀 Watch each video immediately after recording
- ✂️ Note timestamps of sections to edit
- 💾 Save raw recordings (don't delete until final export)
- 📝 Keep a log of what you've completed

---

## 📋 Script Format Explained

Each script uses consistent formatting:

### Slide Headers
```markdown
## SLIDE 1: Title (duration)
```
Indicates a new section/topic

### Screen Instructions
```markdown
**[Show XYZ in Swagger]**
```
What to display on Screen 2 (the recording screen)

### Endpoint Information
```markdown
**Endpoint:** `GET /api/Users`
**Purpose:** Get list of users
```
Key information to emphasize

### Code Blocks
```markdown
**Request Body:**
```json
{ ... }
```
```
Example requests/responses to show

### Speaking Text
Regular paragraphs are what you read aloud

---

## 🎨 Visual Aids Needed

You'll need to create these diagrams:

### 1. Architecture Diagram (Video 1)
Simple 3-tier diagram:
- Angular Frontend
- ASP.NET Core API
- SQL Server Database

**Tools:** draw.io, PowerPoint, or any diagramming tool

### 2. Entity Relationship Diagram (Videos 1 & 8)
Show main entities and relationships:
- Users, Roles, Manufacturers
- Products, Proposals, Contracts
- Velocity entities

**Tools:** dbdiagram.io, Lucidchart, or Visual Studio

### 3. Workflow Diagram (Video 4 - Optional)
Proposal workflow states:
Draft → Submitted → Under Review → Accepted → Awarded

**Tools:** draw.io, PowerPoint

---

## 🔧 Technical Requirements

### Software
- **API:** NPP Contract Management API (running)
- **Browser:** Chrome or Edge (for Swagger UI)
- **Recording:** OBS Studio, Camtasia, or similar
- **Editing:** Any video editor (optional)

### Hardware
- **Microphone:** USB microphone or headset (recommended)
- **Dual Monitors:** Or one large monitor split in half
- **Storage:** ~5GB free space for recordings

### Data
- Sample users, manufacturers, products
- At least 2-3 proposals in different states
- At least 1-2 contracts
- Sample velocity data (optional)

---

## 📤 Final Deliverables

After completing all recordings, you should have:

### Video Files
```
NPP_API_Video_01_Introduction.mp4          (4 min)
NPP_API_Video_02_Authentication.mp4        (9 min)
NPP_API_Video_03_Master_Data.mp4          (10 min)
NPP_API_Video_04_Proposals.mp4            (12 min)
NPP_API_Video_05_Contracts.mp4            (19 min)
NPP_API_Video_06_Velocity.mp4             (11 min)
NPP_API_Video_07_Reports_Lookup.mp4       (10 min)
NPP_API_Video_08_Domain_Objects.mp4       (12 min)
NPP_API_Video_09_Additional_APIs.mp4       (9 min)
```

### Total Duration
~96 minutes (1 hour 36 minutes)

### File Size
~500MB - 2GB (depending on quality settings)

---

## ✅ Quality Checklist

Before considering a video "done":

- [ ] Audio is clear (no background noise)
- [ ] Speaking pace is comfortable (not too fast)
- [ ] All endpoints mentioned are shown in Swagger
- [ ] Request/response examples are visible
- [ ] No long awkward pauses
- [ ] No major mistakes or confusion
- [ ] Video is properly named
- [ ] Duration matches expected time (±2 minutes)

---

## 🆘 Troubleshooting

### "I made a mistake while recording"
- Don't stop! Just pause, take a breath, and re-read that section
- You can edit out mistakes later

### "The script is too fast/slow"
- Adjust your reading pace
- Add natural pauses
- It's okay to deviate slightly from the script

### "I can't show something in Swagger"
- Skip it and add a note to edit in a screenshot later
- Or describe it verbally without showing

### "My voice sounds weird"
- Everyone feels this way! Your voice sounds fine to others
- Focus on clarity, not perfection

### "This is taking too long"
- Take breaks! Don't try to do all 8 videos in one session
- Split across 2-3 days if needed

---

## 🎓 Learning Resources

If you're new to video recording:

- **OBS Studio Tutorial:** https://obsproject.com/wiki/
- **Screen Recording Tips:** Search YouTube for "screen recording best practices"
- **Voice Recording Tips:** Search for "podcast recording tips"

---

## 📞 Support

If you have questions about:
- **Scripts:** Review the API_Quick_Reference.md
- **API:** Check Swagger UI documentation
- **Recording:** See 00_RECORDING_GUIDE.md
- **Technical Issues:** Check API logs or database

---

## 🎉 You're Ready!

Everything you need is in this folder. Take your time, follow the guide, and create great videos!

**Good luck!** 🚀

---

**Last Updated:** December 9, 2024  
**Version:** 1.0  
**Total Scripts:** 8 videos + 3 guides

