# PowerShell script to test API endpoints and verify tables exist
Write-Host "=== NPP Contract Management API Test ===" -ForegroundColor Green
Write-Host ""

$baseUrl = "https://localhost:7199/api"

# Function to test an endpoint
function Test-Endpoint {
    param(
        [string]$endpoint,
        [string]$name
    )

    Write-Host "Testing $name endpoint..." -ForegroundColor Yellow

    try {
        $response = Invoke-WebRequest -Uri "$baseUrl/$endpoint" -Method GET -UseBasicParsing
        $statusCode = $response.StatusCode
        $content = $response.Content

        if ($statusCode -eq 200) {
            Write-Host "[SUCCESS] $name (Status: $statusCode)" -ForegroundColor Green

            # Try to parse JSON and count records
            try {
                $jsonData = $content | ConvertFrom-Json
                if ($jsonData -is [array]) {
                    Write-Host "   Records found: $($jsonData.Count)" -ForegroundColor Cyan
                    if ($jsonData.Count -gt 0) {
                        Write-Host "   Sample record exists" -ForegroundColor Gray
                    }
                } else {
                    Write-Host "   Response received" -ForegroundColor Gray
                }
            } catch {
                Write-Host "   Raw response length: $($content.Length) characters" -ForegroundColor Gray
            }
        } else {
            Write-Host "[FAILED] $name (Status: $statusCode)" -ForegroundColor Red
        }
    } catch {
        $errorMessage = $_.Exception.Message
        if ($errorMessage -like "*401*" -or $errorMessage -like "*Unauthorized*") {
            Write-Host "[AUTH REQUIRED] $name (401)" -ForegroundColor Yellow
        } elseif ($errorMessage -like "*404*" -or $errorMessage -like "*Not Found*") {
            Write-Host "[NOT FOUND] $name (404) - Table may not exist" -ForegroundColor Red
        } else {
            Write-Host "[ERROR] $name - $errorMessage" -ForegroundColor Red
        }
    }
    Write-Host ""
}

# Test all endpoints
Write-Host "Testing API endpoints to verify table existence..." -ForegroundColor Cyan
Write-Host ""

Test-Endpoint "opcos" "OpCos"
Test-Endpoint "memberaccounts" "Member Accounts"
Test-Endpoint "customeraccounts" "Customer Accounts"
Test-Endpoint "products" "Products"
Test-Endpoint "manufacturers" "Manufacturers"
Test-Endpoint "distributors" "Distributors"
Test-Endpoint "industries" "Industries"
Test-Endpoint "contracts" "Contracts"
Test-Endpoint "users" "Users"

Write-Host "=== API Test Complete ===" -ForegroundColor Green
Write-Host ""
Write-Host "RESULTS INTERPRETATION:" -ForegroundColor Cyan
Write-Host "- [SUCCESS] with records = Tables exist and contain data" -ForegroundColor Green
Write-Host "- [AUTH REQUIRED] = Tables exist but need authentication" -ForegroundColor Yellow
Write-Host "- [NOT FOUND] = Tables may not exist in database" -ForegroundColor Red
Write-Host "- [ERROR] = Connection or other technical issue" -ForegroundColor Red
