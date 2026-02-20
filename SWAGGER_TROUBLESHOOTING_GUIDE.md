# SWAGGER NOT SHOWING - TROUBLESHOOTING GUIDE
**Date:** December 29, 2025  
**Issue:** Swagger UI not accessible at deployment URL

---

## 🔴 PROBLEM

Swagger UI is not showing up at: `http://34.66.36.52:8081/swagger`

---

## ✅ FIXES APPLIED

### 1. **Fixed Middleware Order**

**Issue:** Swagger middleware was in the wrong position in the pipeline.

**Fix:** Moved Swagger configuration to proper position (after HTTPS redirection, before exception handlers).

**File:** `NPPContractManagement.API/Program.cs`

**Updated Order:**
```csharp
// 1. HTTPS Redirection (Production only)
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// 2. Swagger (BEFORE exception handlers)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NPP Contract Management API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "NPP Contract Management API";
});

// 3. Exception Handlers
app.UseGlobalExceptionHandler();
app.UseRequestLoggingWithBody();

// 4. CORS
app.UseCors("AllowAngularApp");

// 5. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 6. Controllers
app.MapControllers();
```

---

## 🔍 DIAGNOSTIC STEPS

### Step 1: Check if API is Running

```bash
# Test if API is accessible
curl http://34.66.36.52:8081/api/health

# Or test any endpoint
curl http://34.66.36.52:8081/api/auth/login
```

**Expected:** Some response (even if it's an error, it means the API is running)

---

### Step 2: Check Swagger JSON Endpoint

```bash
# Test if Swagger JSON is generated
curl http://34.66.36.52:8081/swagger/v1/swagger.json
```

**Expected:** JSON response with OpenAPI specification

**If this fails:** Swagger is not configured correctly or the API is not running.

---

### Step 3: Check Swagger UI Endpoint

```bash
# Test if Swagger UI is accessible
curl -I http://34.66.36.52:8081/swagger

# Or with index.html
curl -I http://34.66.36.52:8081/swagger/index.html
```

**Expected:** HTTP 200 OK with `Content-Type: text/html`

---

### Step 4: Check for Redirects

```bash
# Check if there's a redirect happening
curl -L -v http://34.66.36.52:8081/swagger
```

**Look for:** Any 301/302 redirects that might be causing issues

---

## 🛠️ COMMON ISSUES & SOLUTIONS

### Issue 1: API Not Running

**Symptoms:**
- Connection refused
- Timeout errors
- No response at all

**Solutions:**
```bash
# Check if process is running
ps aux | grep dotnet

# Check if port is listening
netstat -an | grep 8081

# Restart the API
cd /path/to/publish
export ASPNETCORE_ENVIRONMENT=Production
dotnet NPPContractManagement.API.dll --urls "http://0.0.0.0:8081"
```

---

### Issue 2: Wrong Environment Variable

**Symptoms:**
- Swagger works locally but not on server
- Different behavior between environments

**Solution:**
```bash
# Make sure environment is set correctly
export ASPNETCORE_ENVIRONMENT=Production

# Or for Development
export ASPNETCORE_ENVIRONMENT=Development

# Verify it's set
echo $ASPNETCORE_ENVIRONMENT

# Restart API with environment variable
ASPNETCORE_ENVIRONMENT=Production dotnet NPPContractManagement.API.dll --urls "http://0.0.0.0:8081"
```

---

### Issue 3: Firewall Blocking

**Symptoms:**
- API works from server but not from external IP
- Timeout when accessing from browser

**Solution:**
```bash
# Check firewall rules (Linux)
sudo ufw status
sudo ufw allow 8081/tcp

# Or for iptables
sudo iptables -L -n | grep 8081
sudo iptables -A INPUT -p tcp --dport 8081 -j ACCEPT
```

---

### Issue 4: Reverse Proxy Issues

**Symptoms:**
- 404 errors
- Swagger UI loads but can't find swagger.json
- Base path issues

**Solution:**

If using **Nginx**, add this to your configuration:
```nginx
location /swagger {
    proxy_pass http://localhost:8081/swagger;
    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection keep-alive;
    proxy_set_header Host $host;
    proxy_cache_bypass $http_upgrade;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
}
```

If using **Apache**, add this:
```apache
ProxyPass /swagger http://localhost:8081/swagger
ProxyPassReverse /swagger http://localhost:8081/swagger
```

---

### Issue 5: HTTPS Redirect Breaking Swagger

**Symptoms:**
- Swagger redirects to HTTPS
- Mixed content warnings

**Solution:**

The code has been updated to only redirect HTTPS in Production. If still having issues:

```csharp
// Temporarily disable HTTPS redirect for testing
// Comment out this section in Program.cs
/*
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
*/
```

---

## 📋 DEPLOYMENT CHECKLIST

After fixing the code, follow these steps:

- [ ] **1. Rebuild the API**
  ```bash
  dotnet build --configuration Release
  ```

- [ ] **2. Publish the API**
  ```bash
  dotnet publish --configuration Release --output ./publish
  ```

- [ ] **3. Copy to Server**
  ```bash
  scp -r ./publish/* user@34.66.36.52:/path/to/deployment/
  ```

- [ ] **4. Stop Old Process**
  ```bash
  # Find the process
  ps aux | grep NPPContractManagement.API
  
  # Kill it
  kill -9 <PID>
  ```

- [ ] **5. Start New Process**
  ```bash
  cd /path/to/deployment
  export ASPNETCORE_ENVIRONMENT=Production
  nohup dotnet NPPContractManagement.API.dll --urls "http://0.0.0.0:8081" > api.log 2>&1 &
  ```

- [ ] **6. Verify Swagger**
  ```bash
  curl http://34.66.36.52:8081/swagger/v1/swagger.json
  ```

- [ ] **7. Test in Browser**
  - Open: `http://34.66.36.52:8081/swagger`
  - Should see Swagger UI

---

## 🧪 QUICK TEST SCRIPT

Save this as `test-swagger.sh`:

```bash
#!/bin/bash

API_URL="http://34.66.36.52:8081"

echo "Testing Swagger Endpoints..."
echo "=============================="
echo ""

echo "1. Testing Swagger JSON..."
curl -s -o /dev/null -w "Status: %{http_code}\n" $API_URL/swagger/v1/swagger.json
echo ""

echo "2. Testing Swagger UI..."
curl -s -o /dev/null -w "Status: %{http_code}\n" $API_URL/swagger
echo ""

echo "3. Testing Swagger UI (index.html)..."
curl -s -o /dev/null -w "Status: %{http_code}\n" $API_URL/swagger/index.html
echo ""

echo "4. Testing API Health..."
curl -s -o /dev/null -w "Status: %{http_code}\n" $API_URL/api/health
echo ""

echo "Done!"
```

Run it:
```bash
chmod +x test-swagger.sh
./test-swagger.sh
```

---

## ✅ EXPECTED RESULTS

After redeployment, you should see:

1. **Swagger JSON:** `http://34.66.36.52:8081/swagger/v1/swagger.json` → HTTP 200
2. **Swagger UI:** `http://34.66.36.52:8081/swagger` → HTTP 200 (HTML page)
3. **Browser:** Should display interactive Swagger documentation

---

## 📞 STILL NOT WORKING?

If Swagger still doesn't show up after following all steps:

1. **Check API logs:**
   ```bash
   tail -f /path/to/api.log
   ```

2. **Enable detailed logging:**
   Add to `appsettings.Production.json`:
   ```json
   "Logging": {
     "LogLevel": {
       "Default": "Debug",
       "Microsoft.AspNetCore": "Debug"
     }
   }
   ```

3. **Test locally first:**
   ```bash
   cd NPPContractManagement.API
   dotnet run
   # Then open: http://localhost:5143/swagger
   ```

4. **Check browser console** for JavaScript errors

---

**Status:** ✅ Code fixed - Ready for redeployment

