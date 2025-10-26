import { Injectable } from '@angular/core';
import { jsPDF } from "jspdf";
import { autoTable } from "jspdf-autotable";
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';

@Injectable({
  providedIn: 'root'
})
export class ExportFileService {

  constructor() { }

  printToPDF(tableID: string, fileName: string, includeColumns: string[]) {
    const inputTable = document.getElementById(tableID) as HTMLTableElement;

    if (!inputTable) return;

    const headers: string[] = [];
    const rows: string[][] = [];

    const ths = inputTable.querySelectorAll('thead th');
    const allColumns = Array.from(ths).map(th => th.textContent?.trim() || '');

    // Get indexes of columns to include
    const columnIndexes = includeColumns.map(colName => allColumns.indexOf(colName)).filter(index => index !== -1);

    // Set headers
    columnIndexes.forEach(index => {
      headers.push(allColumns[index]);
    });

    // Process rows
    const trs = inputTable.querySelectorAll('tbody tr');
    trs.forEach(tr => {
      const tds = tr.querySelectorAll('td');
      const rowData: string[] = [];
      columnIndexes.forEach(index => {
        rowData.push(tds[index]?.textContent?.trim() || '');
      });
      rows.push(rowData);
    });

    const doc = new jsPDF();
    autoTable(doc, {
      head: [headers],
      body: rows
    });

    doc.save(fileName);
  }

  printToExcel(tableID: string, fileName: string, selectedHeaders: string[]): void {
    const table = document.getElementById(tableID) as HTMLTableElement;
    if (!table) {
      console.error('Table not found.');
      return;
    }

    const headerCells = Array.from(table.querySelectorAll('thead tr th'));
    const headerMap: number[] = [];

    // Map header text to column indices
    selectedHeaders.forEach(headerText => {
      const index = headerCells.findIndex(cell => cell.textContent?.trim() === headerText);
      if (index !== -1) {
        headerMap.push(index);
      }
    });

    // Extract filtered data
    const rows = Array.from(table.querySelectorAll('tbody tr'));
    const data: any[][] = [];

    // Add header row
    data.push(selectedHeaders);

    for (const row of rows) {
      const cells = Array.from(row.querySelectorAll('td'));
      const rowData = headerMap.map(index => cells[index]?.textContent?.trim() || '');
      data.push(rowData);
    }

    const worksheet = XLSX.utils.aoa_to_sheet(data);
    const workbook: XLSX.WorkBook = {
      Sheets: { 'Sheet1': worksheet },
      SheetNames: ['Sheet1']
    };

    const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    const blob = new Blob([excelBuffer], { type: 'application/octet-stream' });
    saveAs(blob, fileName);
  }
}
