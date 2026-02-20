# CORS ERROR FIX & DEPLOYMENT GUIDE
**Date:** December 22, 2025  
**Issue:** CORS error when accessing API from deployed frontend

---

## 🔴 ERROR DETAILS

### Error Message:
```
Access to XMLHttpRequest at 'http://34.9.77.60:8081/api/auth/login' from origin 'http://34.9.77.60:8080' 
has been blocked by CORS policy: Response to preflight request doesn't pass access control check: 
No 'Access-Control-Allow-Origin' header is present on the requested resource.
```

### Root Cause:
The API's CORS policy was not configured to allow requests from the production frontend origin (`http://34.9.77.60:8080`).

---

## ✅ FIXES APPLIED

### 1. Updated CORS Configuration in Backend

**File:** `NPPContractManagement.API/Program.cs`

**Changes:**
- Added production frontend origins to CORS policy
- Enabled credentials support
- Configured proper headers and methods

**Updated CORS Policy:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        // Allow both local development and production origins
        policy.WithOrigins(
                "http://localhost:4200", 
                "http://localhost:4201", 
                "http://localhost:4202",
                "http://34.9.77.60:8080",  // Production frontend
                "http://34.9.77.60:4200"   // Alternative production port
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
```

### 2. Created Production Configuration File

**File:** `NPPContractManagement.API/appsettings.Production.json` (NEW)

**Content:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=34.9.77.60;Database=NPPContractManagement;Uid=sa;Password=software@123;"
  },
  "AppSettings": {
    "FrontendUrl": "http://34.9.77.60:8080"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

### 3. Fixed Connection Strings

**Files Updated:**
- `NPPContractManagement.API/appsettings.json`
- `NPPContractManagement.API/appsettings.Development.json`

**Changes:**
- Uncommented `DefaultConnection` for local development
- Added `LocalDb` connection string
- Kept `DevDb` for production deployment

---

## 🚀 DEPLOYMENT STEPS

### Backend Deployment

1. **Build the API for Production**
   ```bash
   cd NPPContractManagement.API
   dotnet build --configuration Release
   ```

2. **Publish the API**
   ```bash
   dotnet publish --configuration Release --output ./publish
   ```

3. **Deploy to Server**
   - Copy the `publish` folder to the server
   - Ensure the server is running on port `8081`
   - Set environment variable: `ASPNETCORE_ENVIRONMENT=Production`

4. **Run the API**
   ```bash
   cd publish
   export ASPNETCORE_ENVIRONMENT=Production
   dotnet NPPContractManagement.API.dll --urls "http://0.0.0.0:8081"
   ```

### Frontend Deployment

1. **Verify API URL Configuration**
   
   **File:** `NPPContractManagement.Frontend/src/app/config/app.config.service.ts`
   
   Ensure it points to the production API:
   ```typescript
   apiUrl: 'http://34.9.77.60:8081/api'
   ```

2. **Build the Frontend for Production**
   ```bash
   cd NPPContractManagement.Frontend
   npm install
   npm run build
   ```

3. **Deploy Static Files**
   - Copy contents of `dist/NPPContractManagement.Frontend/browser/` to web server
   - Configure web server to serve on port `8080`
   - Set up URL rewriting for Angular routing

4. **Nginx Configuration Example**
   ```nginx
   server {
       listen 8080;
       server_name 34.9.77.60;
       root /path/to/dist/NPPContractManagement.Frontend/browser;
       index index.html;

       location / {
           try_files $uri $uri/ /index.html;
       }
   }
   ```

---

## 🔍 VERIFICATION STEPS

### 1. Check API is Running
```bash
curl http://34.9.77.60:8081/api/health
```

### 2. Check CORS Headers
```bash
curl -H "Origin: http://34.9.77.60:8080" \
     -H "Access-Control-Request-Method: POST" \
     -H "Access-Control-Request-Headers: Content-Type" \
     -X OPTIONS \
     --verbose \
     http://34.9.77.60:8081/api/auth/login
```

**Expected Response Headers:**
```
Access-Control-Allow-Origin: http://34.9.77.60:8080
Access-Control-Allow-Methods: POST, GET, PUT, DELETE, OPTIONS
Access-Control-Allow-Headers: Content-Type, Authorization
Access-Control-Allow-Credentials: true
```

### 3. Test Login from Frontend
- Navigate to `http://34.9.77.60:8080/login`
- Enter credentials
- Check browser console for errors
- Verify successful login

---

## 📋 CONFIGURATION SUMMARY

### Environment URLs

| Environment | Frontend URL | API URL | Database Server |
|-------------|-------------|---------|-----------------|
| **Local** | http://localhost:4201 | http://localhost:5143 | DESKTOP-0EM04K6 |
| **Production** | http://34.9.77.60:8080 | http://34.9.77.60:8081 | 34.9.77.60 |

### CORS Allowed Origins
- ✅ `http://localhost:4200`
- ✅ `http://localhost:4201`
- ✅ `http://localhost:4202`
- ✅ `http://34.9.77.60:8080` (Production)
- ✅ `http://34.9.77.60:4200` (Alternative)

---

## 🛠️ TROUBLESHOOTING

### Issue: Still Getting CORS Error

**Possible Causes:**
1. API not running or not accessible
2. API running on wrong port
3. Firewall blocking requests
4. Wrong environment configuration

**Solutions:**
```bash
# Check if API is running
netstat -an | grep 8081

# Check API logs
tail -f /path/to/api/logs/app.log

# Restart API with correct environment
export ASPNETCORE_ENVIRONMENT=Production
dotnet NPPContractManagement.API.dll --urls "http://0.0.0.0:8081"
```

### Issue: 401 Unauthorized

**Cause:** JWT token issues or authentication configuration

**Solution:**
- Verify JWT secret key matches in all environments
- Check token expiry settings
- Clear browser cache and cookies

### Issue: Database Connection Failed

**Cause:** Wrong connection string or database not accessible

**Solution:**
```bash
# Test database connection
mysql -h 34.9.77.60 -u sa -p -e "SHOW DATABASES;"

# Verify connection string in appsettings.Production.json
```

---

## 📝 FILES MODIFIED

1. ✅ `NPPContractManagement.API/Program.cs` - Updated CORS policy
2. ✅ `NPPContractManagement.API/appsettings.Production.json` - Created new file
3. ✅ `NPPContractManagement.API/appsettings.json` - Fixed connection strings
4. ✅ `NPPContractManagement.API/appsettings.Development.json` - Fixed connection strings

---

## ✅ NEXT STEPS

1. **Rebuild and redeploy the backend** with the updated CORS configuration
2. **Restart the API service** on the production server
3. **Test the login** from the frontend
4. **Monitor logs** for any errors
5. **Verify all API endpoints** are accessible

---

**Status:** ✅ CORS configuration fixed and ready for deployment

