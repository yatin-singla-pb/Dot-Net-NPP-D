# PowerShell script to check MySQL database tables
# Make sure MySQL is installed and accessible via command line

Write-Host "=== NPP Contract Management Database Table Check ===" -ForegroundColor Green
Write-Host ""

# Database connection details
$server = "localhost"
$database = "NPPContractManagment"
$username = "root"
$password = ""

Write-Host "Connecting to MySQL database: $database" -ForegroundColor Yellow
Write-Host ""

# Create MySQL command to show all tables
$showTablesQuery = "USE $database; SHOW TABLES;"

# Create MySQL command to check specific tables
$checkTablesQuery = @"
USE $database;
SELECT 'Database Tables:' as Info;
SHOW TABLES;

SELECT 'Table Information:' as Info;
SELECT 
    TABLE_NAME,
    TABLE_ROWS,
    CREATE_TIME
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = '$database' 
ORDER BY TABLE_NAME;

SELECT 'OpCos Check:' as Info;
SELECT COUNT(*) as OpCos_Count FROM OpCos;

SELECT 'MemberAccounts Check:' as Info;
SELECT COUNT(*) as MemberAccounts_Count FROM MemberAccounts;

SELECT 'CustomerAccounts Check:' as Info;
SELECT COUNT(*) as CustomerAccounts_Count FROM CustomerAccounts;

SELECT 'Products Check:' as Info;
SELECT COUNT(*) as Products_Count FROM Products;

SELECT 'Manufacturers Check:' as Info;
SELECT COUNT(*) as Manufacturers_Count FROM Manufacturers;

SELECT 'Distributors Check:' as Info;
SELECT COUNT(*) as Distributors_Count FROM Distributors;
"@

# Save query to temp file
$tempFile = [System.IO.Path]::GetTempFileName() + ".sql"
$checkTablesQuery | Out-File -FilePath $tempFile -Encoding UTF8

Write-Host "Executing MySQL queries..." -ForegroundColor Yellow

try {
    # Execute MySQL command
    if ($password -eq "") {
        $result = mysql -u $username -e $checkTablesQuery 2>&1
    } else {
        $result = mysql -u $username -p$password -e $checkTablesQuery 2>&1
    }
    
    Write-Host "MySQL Query Results:" -ForegroundColor Green
    Write-Host "===================" -ForegroundColor Green
    $result | ForEach-Object { Write-Host $_ }
    
} catch {
    Write-Host "Error executing MySQL command: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternative: Run this SQL manually in your MySQL client:" -ForegroundColor Yellow
    Write-Host "========================================================" -ForegroundColor Yellow
    Write-Host $checkTablesQuery
}

# Clean up temp file
if (Test-Path $tempFile) {
    Remove-Item $tempFile
}

Write-Host ""
Write-Host "=== Check Complete ===" -ForegroundColor Green
