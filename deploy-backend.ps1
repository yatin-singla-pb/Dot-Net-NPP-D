# Backend Deployment Script for NPP Contract Management
# This script builds and prepares the backend for production deployment

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "NPP Contract Management - Backend Deploy" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Set variables
$projectPath = "NPPContractManagement.API"
$outputPath = "publish"
$configuration = "Release"

# Step 1: Clean previous build
Write-Host "[1/5] Cleaning previous build..." -ForegroundColor Yellow
if (Test-Path $outputPath) {
    Remove-Item -Path $outputPath -Recurse -Force
    Write-Host "✓ Cleaned previous build" -ForegroundColor Green
}

# Step 2: Restore dependencies
Write-Host ""
Write-Host "[2/5] Restoring dependencies..." -ForegroundColor Yellow
dotnet restore $projectPath
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Failed to restore dependencies" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Dependencies restored" -ForegroundColor Green

# Step 3: Build the project
Write-Host ""
Write-Host "[3/5] Building project..." -ForegroundColor Yellow
dotnet build $projectPath --configuration $configuration --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Build failed" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Build successful" -ForegroundColor Green

# Step 4: Publish the project
Write-Host ""
Write-Host "[4/5] Publishing project..." -ForegroundColor Yellow
dotnet publish $projectPath --configuration $configuration --output $outputPath --no-build
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Publish failed" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Publish successful" -ForegroundColor Green

# Step 5: Verify output
Write-Host ""
Write-Host "[5/5] Verifying output..." -ForegroundColor Yellow
$dllPath = Join-Path $outputPath "NPPContractManagement.API.dll"
if (Test-Path $dllPath) {
    Write-Host "✓ Output verified" -ForegroundColor Green
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Deployment package ready!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Location: $outputPath" -ForegroundColor White
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. Copy the '$outputPath' folder to your production server" -ForegroundColor White
    Write-Host "2. Set environment variable: ASPNETCORE_ENVIRONMENT=Production" -ForegroundColor White
    Write-Host "3. Run: dotnet NPPContractManagement.API.dll --urls 'http://0.0.0.0:8081'" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host "✗ Output verification failed" -ForegroundColor Red
    exit 1
}

