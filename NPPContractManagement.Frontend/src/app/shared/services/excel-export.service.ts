import { Injectable } from '@angular/core';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';

@Injectable({
  providedIn: 'root'
})
export class ExcelExportService {

  constructor() { }

  /**
   * Export data to Excel file
   * @param data Array of objects to export
   * @param filename Name of the file (without extension)
   * @param sheetName Name of the worksheet
   */
  exportToExcel(data: any[], filename: string, sheetName: string = 'Sheet1'): void {
    if (!data || data.length === 0) {
      console.warn('No data to export');
      return;
    }

    // Create a new workbook
    const workbook = XLSX.utils.book_new();

    // Convert data to worksheet
    const worksheet = XLSX.utils.json_to_sheet(data);

    // Auto-size columns
    const columnWidths = this.calculateColumnWidths(data);
    worksheet['!cols'] = columnWidths;

    // Add worksheet to workbook
    XLSX.utils.book_append_sheet(workbook, worksheet, sheetName);

    // Generate Excel file
    const excelBuffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });

    // Save file
    this.saveExcelFile(excelBuffer, filename);
  }

  /**
   * Export multiple sheets to Excel file
   * @param sheets Array of sheet objects with data, name, and optional filename
   * @param filename Name of the file (without extension)
   */
  exportMultipleSheetsToExcel(sheets: { data: any[], name: string }[], filename: string): void {
    if (!sheets || sheets.length === 0) {
      console.warn('No sheets to export');
      return;
    }

    // Create a new workbook
    const workbook = XLSX.utils.book_new();

    sheets.forEach(sheet => {
      if (sheet.data && sheet.data.length > 0) {
        // Convert data to worksheet
        const worksheet = XLSX.utils.json_to_sheet(sheet.data);

        // Auto-size columns
        const columnWidths = this.calculateColumnWidths(sheet.data);
        worksheet['!cols'] = columnWidths;

        // Add worksheet to workbook
        XLSX.utils.book_append_sheet(workbook, worksheet, sheet.name);
      }
    });

    // Generate Excel file
    const excelBuffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });

    // Save file
    this.saveExcelFile(excelBuffer, filename);
  }

  /**
   * Calculate optimal column widths based on data
   */
  private calculateColumnWidths(data: any[]): any[] {
    if (!data || data.length === 0) return [];

    const keys = Object.keys(data[0]);
    const columnWidths: any[] = [];

    keys.forEach(key => {
      let maxLength = key.length; // Start with header length

      data.forEach(row => {
        const cellValue = row[key];
        if (cellValue !== null && cellValue !== undefined) {
          const cellLength = cellValue.toString().length;
          if (cellLength > maxLength) {
            maxLength = cellLength;
          }
        }
      });

      // Set minimum width of 10 and maximum width of 50
      const width = Math.min(Math.max(maxLength + 2, 10), 50);
      columnWidths.push({ width });
    });

    return columnWidths;
  }

  /**
   * Save Excel file to user's computer
   */
  private saveExcelFile(buffer: any, filename: string): void {
    const data = new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    const timestamp = new Date().toISOString().slice(0, 19).replace(/:/g, '-');
    saveAs(data, `${filename}_${timestamp}.xlsx`);
  }

  /**
   * Prepare data for export by flattening nested objects and formatting dates
   */
  prepareDataForExport(data: any[], excludeFields: string[] = []): any[] {
    return data.map(item => {
      const flatItem: any = {};

      Object.keys(item).forEach(key => {
        if (!excludeFields.includes(key)) {
          const value = item[key];

          // Handle dates
          if (value instanceof Date) {
            flatItem[key] = value.toLocaleDateString();
          }
          // Handle nested objects (flatten one level)
          else if (value && typeof value === 'object' && !Array.isArray(value)) {
            Object.keys(value).forEach(nestedKey => {
              flatItem[`${key}_${nestedKey}`] = value[nestedKey];
            });
          }
          // Handle arrays (convert to string)
          else if (Array.isArray(value)) {
            flatItem[key] = value.map(v => v.name || v.toString()).join(', ');
          }
          // Handle regular values
          else {
            flatItem[key] = value;
          }
        }
      });

      return flatItem;
    });
  }

  /**
   * Format field names for better Excel headers
   */
  formatHeaders(data: any[], headerOverrides: { [key: string]: string } = {}): any[] {
    if (!data || data.length === 0) return [];

    return data.map(item => {
      const formattedItem: any = {};

      Object.keys(item).forEach(key => {
        const formattedKey = headerOverrides[key] || key
          .replace(/([A-Z])/g, ' $1')
          .replace(/^./, str => str.toUpperCase())
          .trim();

        formattedItem[formattedKey] = item[key];
      });

      return formattedItem;
    });
  }
}
