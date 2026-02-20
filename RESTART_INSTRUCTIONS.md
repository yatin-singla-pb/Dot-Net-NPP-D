# 🔄 Restart Instructions - Bulk Renewal Feature

## ⚠️ IMPORTANT: You Must Restart Both Backend and Frontend

The bulk renewal feature has been fully implemented, but you need to restart both the backend API and frontend application to see the changes.

---

## 🔧 Step 1: Restart the Backend (API)

### **Option A: Using Visual Studio**
1. Stop the current debugging session (Shift+F5)
2. Start debugging again (F5)
3. Wait for the API to start (you'll see "Now listening on: https://localhost:7199")

### **Option B: Using JetBrains Rider**
1. Stop the current run configuration
2. Click the Run button again
3. Wait for the API to start

### **Option C: Using Command Line**
```bash
# Navigate to the API project
cd E:\TestAIFixed\NPPContractManagement.API

# Run the API
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7199
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5199
```

---

## 🎨 Step 2: Restart the Frontend (Angular)

### **Option A: If Frontend is Running**
1. Stop the current Angular dev server (Ctrl+C in the terminal)
2. Start it again:
   ```bash
   cd E:\TestAIFixed\NPPContractManagement.Frontend
   npm start
   ```

### **Option B: If Frontend is Not Running**
```bash
cd E:\TestAIFixed\NPPContractManagement.Frontend
npm start
```

**Expected Output:**
```
** Angular Live Development Server is listening on localhost:4200 **
✔ Compiled successfully.
```

---

## 🔍 Step 3: Verify the Changes

### **1. Clear Browser Cache**
- Press **Ctrl+Shift+R** (Windows/Linux) or **Cmd+Shift+R** (Mac)
- Or open DevTools (F12) → Network tab → Check "Disable cache"

### **2. Login**
- Login as **System Administrator** or **Contract Manager**
- (Other roles won't see the bulk renewal feature)

### **3. Navigate to Contracts**
- Go to **Administration → Contracts**

### **4. Look for These New Elements:**

✅ **Checkbox in Table Header**
- You should see a checkbox in the first column header
- This is the "Select All" checkbox

✅ **Checkboxes in Each Row**
- Each contract row should have a checkbox in the first column

✅ **Bulk Actions Bar**
- When you select one or more contracts, a blue bar should appear above the table
- It shows: "X contract(s) selected"
- It has a button: "Create Renewal Proposals (X)"

---

## 🐛 Troubleshooting

### **Issue: I don't see checkboxes**

**Possible Causes:**

1. **Frontend not restarted**
   - Solution: Stop and restart the Angular dev server
   - Clear browser cache (Ctrl+Shift+R)

2. **Wrong user role**
   - Solution: Make sure you're logged in as System Administrator or Contract Manager
   - Other roles won't see the bulk renewal feature

3. **Browser cache**
   - Solution: Hard refresh (Ctrl+Shift+R)
   - Or clear browser cache completely

4. **Build errors**
   - Solution: Check the Angular terminal for compilation errors
   - Run `npm install` if needed

### **Issue: Backend errors**

**Check the API terminal for errors:**

1. **Service registration error**
   - Make sure `IBulkRenewalService` is registered in `Program.cs`
   - Line should be: `builder.Services.AddScoped<IBulkRenewalService, BulkRenewalService>();`

2. **Compilation errors**
   - Run `dotnet build` to check for errors
   - All files should compile successfully

### **Issue: Dialog doesn't open**

1. **Check browser console** (F12 → Console tab)
2. **Look for errors** related to MatDialog or BulkRenewalDialogComponent
3. **Verify Material modules** are imported

---

## ✅ Verification Checklist

After restarting, verify:

- [ ] Backend API is running on port 7199
- [ ] Frontend is running on port 4200
- [ ] Browser cache is cleared
- [ ] Logged in as System Administrator or Contract Manager
- [ ] Navigated to Contracts page
- [ ] Can see checkbox in table header
- [ ] Can see checkboxes in each row
- [ ] Selecting a contract shows bulk actions bar
- [ ] "Create Renewal Proposals" button is visible
- [ ] Clicking button opens dialog

---

## 🎯 Quick Test

1. **Restart backend** (stop and start the API)
2. **Restart frontend** (stop and start Angular dev server)
3. **Clear browser cache** (Ctrl+Shift+R)
4. **Login** as System Administrator
5. **Go to Contracts** page
6. **Look for checkboxes** in the first column

If you see checkboxes, the feature is working! ✅

---

## 📞 Still Not Working?

If you still don't see checkboxes after restarting:

1. **Check the browser console** (F12) for JavaScript errors
2. **Check the API terminal** for backend errors
3. **Verify your user role** - must be System Administrator or Contract Manager
4. **Try a different browser** to rule out cache issues
5. **Run `npm install`** in the Frontend directory to ensure all dependencies are installed

---

## 🎊 Summary

**To see the bulk renewal feature:**

1. ✅ **Stop and restart the backend API**
2. ✅ **Stop and restart the frontend Angular app**
3. ✅ **Clear browser cache** (Ctrl+Shift+R)
4. ✅ **Login as System Administrator or Contract Manager**
5. ✅ **Navigate to Contracts page**
6. ✅ **Look for checkboxes in the first column**

**The feature is fully implemented and ready - you just need to restart!** 🚀

