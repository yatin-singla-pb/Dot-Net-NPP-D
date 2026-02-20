# PowerShell script to create a sample Excel file for Velocity data
# This creates an Excel file with the same data as the CSV sample

# Create Excel COM object
$excel = New-Object -ComObject Excel.Application
$excel.Visible = $false
$excel.DisplayAlerts = $false

# Create a new workbook
$workbook = $excel.Workbooks.Add()
$worksheet = $workbook.Worksheets.Item(1)
$worksheet.Name = "Velocity Data"

# Add headers
$worksheet.Cells.Item(1, 1) = "distributor_id"
$worksheet.Cells.Item(1, 2) = "shipment_id"
$worksheet.Cells.Item(1, 3) = "sku"
$worksheet.Cells.Item(1, 4) = "quantity"
$worksheet.Cells.Item(1, 5) = "shipped_at"
$worksheet.Cells.Item(1, 6) = "origin"
$worksheet.Cells.Item(1, 7) = "destination"

# Format header row
$headerRange = $worksheet.Range("A1:G1")
$headerRange.Font.Bold = $true
$headerRange.Interior.ColorIndex = 15
$headerRange.Font.ColorIndex = 1

# Add data rows
$worksheet.Cells.Item(2, 1) = 1
$worksheet.Cells.Item(2, 2) = "SH001"
$worksheet.Cells.Item(2, 3) = "SKU123"
$worksheet.Cells.Item(2, 4) = 50
$worksheet.Cells.Item(2, 5) = "2024-12-01T10:00:00Z"
$worksheet.Cells.Item(2, 6) = "Warehouse A"
$worksheet.Cells.Item(2, 7) = "Store B"

$worksheet.Cells.Item(3, 1) = 1
$worksheet.Cells.Item(3, 2) = "SH002"
$worksheet.Cells.Item(3, 3) = "SKU456"
$worksheet.Cells.Item(3, 4) = 25
$worksheet.Cells.Item(3, 5) = "2024-12-01T11:30:00Z"
$worksheet.Cells.Item(3, 6) = "Warehouse A"
$worksheet.Cells.Item(3, 7) = "Store C"

$worksheet.Cells.Item(4, 1) = 2
$worksheet.Cells.Item(4, 2) = "SH003"
$worksheet.Cells.Item(4, 3) = "SKU789"
$worksheet.Cells.Item(4, 4) = 100
$worksheet.Cells.Item(4, 5) = "2024-12-01T14:00:00Z"
$worksheet.Cells.Item(4, 6) = "Warehouse B"
$worksheet.Cells.Item(4, 7) = "Store D"

$worksheet.Cells.Item(5, 1) = 1
$worksheet.Cells.Item(5, 2) = "SH004"
$worksheet.Cells.Item(5, 3) = "SKU123"
$worksheet.Cells.Item(5, 4) = 75
$worksheet.Cells.Item(5, 5) = "2024-12-01T15:00:00Z"
$worksheet.Cells.Item(5, 6) = "Warehouse A"
$worksheet.Cells.Item(5, 7) = "Store E"

$worksheet.Cells.Item(6, 1) = 3
$worksheet.Cells.Item(6, 2) = "SH005"
$worksheet.Cells.Item(6, 3) = "SKU999"
$worksheet.Cells.Item(6, 4) = 30
$worksheet.Cells.Item(6, 5) = "2024-12-02T09:00:00Z"
$worksheet.Cells.Item(6, 6) = "Warehouse C"
$worksheet.Cells.Item(6, 7) = "Store F"

# Auto-fit columns
$usedRange = $worksheet.UsedRange
$usedRange.EntireColumn.AutoFit() | Out-Null

# Save the file
$filePath = Join-Path $PSScriptRoot "sample_velocity_data.xlsx"
$workbook.SaveAs($filePath, 51) # 51 = xlOpenXMLWorkbook (.xlsx)

# Close and cleanup
$workbook.Close($false)
$excel.Quit()
[System.Runtime.Interopservices.Marshal]::ReleaseComObject($worksheet) | Out-Null
[System.Runtime.Interopservices.Marshal]::ReleaseComObject($workbook) | Out-Null
[System.Runtime.Interopservices.Marshal]::ReleaseComObject($excel) | Out-Null
[System.GC]::Collect()
[System.GC]::WaitForPendingFinalizers()

Write-Host "Excel file created successfully: $filePath" -ForegroundColor Green

