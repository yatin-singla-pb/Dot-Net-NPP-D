# PowerShell Script to Test Swagger Endpoints
# Usage: .\test-swagger.ps1 -ApiUrl "http://34.66.36.52:8081"

param(
    [string]$ApiUrl = "http://34.66.36.52:8081"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing Swagger Endpoints" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "API URL: $ApiUrl" -ForegroundColor White
Write-Host ""

# Test 1: Swagger JSON
Write-Host "[1/4] Testing Swagger JSON endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$ApiUrl/swagger/v1/swagger.json" -Method Get -UseBasicParsing -TimeoutSec 10
    if ($response.StatusCode -eq 200) {
        Write-Host "[OK] Swagger JSON: OK (Status: $($response.StatusCode))" -ForegroundColor Green
        $jsonContent = $response.Content | ConvertFrom-Json
        Write-Host "  API Title: $($jsonContent.info.title)" -ForegroundColor Gray
        Write-Host "  API Version: $($jsonContent.info.version)" -ForegroundColor Gray
    } else {
        Write-Host "[FAIL] Swagger JSON: Failed (Status: $($response.StatusCode))" -ForegroundColor Red
    }
} catch {
    Write-Host "[ERROR] Swagger JSON: Error - $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 2: Swagger UI
Write-Host "[2/4] Testing Swagger UI endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$ApiUrl/swagger" -Method Get -UseBasicParsing -TimeoutSec 10
    if ($response.StatusCode -eq 200) {
        Write-Host "[OK] Swagger UI: OK (Status: $($response.StatusCode))" -ForegroundColor Green
        Write-Host "  Content-Type: $($response.Headers.'Content-Type')" -ForegroundColor Gray
    } else {
        Write-Host "[FAIL] Swagger UI: Failed (Status: $($response.StatusCode))" -ForegroundColor Red
    }
} catch {
    Write-Host "[ERROR] Swagger UI: Error - $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 3: Swagger UI index.html
Write-Host "[3/4] Testing Swagger UI (index.html)..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$ApiUrl/swagger/index.html" -Method Get -UseBasicParsing -TimeoutSec 10
    if ($response.StatusCode -eq 200) {
        Write-Host "[OK] Swagger UI (index.html): OK (Status: $($response.StatusCode))" -ForegroundColor Green
    } else {
        Write-Host "[FAIL] Swagger UI (index.html): Failed (Status: $($response.StatusCode))" -ForegroundColor Red
    }
} catch {
    Write-Host "[ERROR] Swagger UI (index.html): Error - $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 4: API Base
Write-Host "[4/4] Testing API base endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$ApiUrl/api" -Method Get -UseBasicParsing -TimeoutSec 10 -ErrorAction SilentlyContinue
    Write-Host "[OK] API Base: Accessible (Status: $($response.StatusCode))" -ForegroundColor Green
} catch {
    if ($_.Exception.Response.StatusCode.value__ -eq 404) {
        Write-Host "[OK] API Base: Accessible (404 is expected - no route at /api)" -ForegroundColor Green
    } else {
        Write-Host "[WARN] API Base: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}
Write-Host ""

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Test Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "If all tests passed, open in browser:" -ForegroundColor White
Write-Host "$ApiUrl/swagger" -ForegroundColor Cyan
Write-Host ""

