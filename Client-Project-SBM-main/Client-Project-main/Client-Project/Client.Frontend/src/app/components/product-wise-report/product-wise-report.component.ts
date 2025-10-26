import { Component, TemplateRef } from '@angular/core';
import { TableComponent } from "../utils/table/table.component";
import { ReportService } from '../../services/report.service';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { LoginService } from '../../services/login.service';
import { ExportFileService } from '../../services/export-file.service';
import { DatePickerModule } from 'primeng/datepicker';
import { AlertService } from '../../services/alert.service';

@Component({
  selector: 'app-product-wise-report',
  imports: [TableComponent, CommonModule, ReactiveFormsModule, DatePickerModule],
  templateUrl: './product-wise-report.component.html',
  styleUrl: '../../../componentStyle.css'
})
export class ProductWiseReportComponent {
  companyId: number | null = null;
  data: ProductWiseReportComponent[] = []
  displayedColumns = [
    "prodName",
    "subContractorName",
    "cashAmount",
    "balanceAmount",
    "paidAmount",
  ]
  columnsInfo: {
    [key: string]: {
      'title'?: string,
      'isSort'?: boolean,
      'templateRef': TemplateRef<any> | null,
    }
  } = {};
  constructor(
    private alert: AlertService,
    private loginService: LoginService,
    private exportService: ExportFileService,
    private reportService: ReportService
  ) { }
  ngOnInit(): void {
    this.companyId = this.loginService.companyId()
    if (this.companyId) {
      this.reportService.getAllProductWiseReportsGetDto('').subscribe({
        next: (response: ProductWiseReportComponent[]) => {
          this.data = response;
        },
        error: (error) => {
          this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
        }
      })
      this.columnsInfo = {
        'prodName': {
          'title': 'Product Name',
          'isSort': false,
          'templateRef': null
        },
        'subContractorName': {
          'title': 'Sub-Contractor',
          'isSort': true,
          'templateRef': null
        },
        'cashAmount': {
          'title': 'Cash Amount',
          'isSort': true,
          'templateRef': null
        },
        "balanceAmount": {
          'title': 'Balance Amount',
          'isSort': true,
          'templateRef': null
        },
        "paidAmount": {
          'title': 'Paid Amount',
          'isSort': true,
          'templateRef': null
        },
      }
    }
  }

  filterForm: FormGroup = new FormGroup({
    FromDate: new FormControl(null),
    ToDate: new FormControl(null),
    subContractorName: new FormControl(null),
    productName: new FormControl(null),
  });

  resetFilter() {
    this.filterForm.reset({
      FromDate: null,
      ToDate: null,
      subContractorName: null,
      productName: null,
    })
  }

  exportToPdf() {
    this.exportService.printToPDF('table', 'Product-Wise-Report.pdf', [
      'Product Name',
      'Sub-Contractor',
      'Cash Amount',
      'Balance Amount',
      'Paid Amount',
    ])
  }

  exportToExcel(){
    this.exportService.printToExcel('table', 'Product-Wise-Report.xlsx', [
      'Product Name',
      'Sub-Contractor',
      'Cash Amount',
      'Balance Amount',
      'Paid Amount',
    ])
  }

  onFilterSubmit() {
    function getDateFormat(inputDate: string): string {
      const date = new Date(inputDate);
      const dd = String(date.getDate()).padStart(2, '0');
      const mm = String(date.getMonth() + 1).padStart(2, '0');
      const yyyy = String(date.getFullYear());
      return `${yyyy}-${mm}-${dd}`;
    }
    let url = '' ;
    const fromDate = this.filterForm.get('FromDate')?.value;
    const toDate = this.filterForm.get('ToDate')?.value;
    const subContractorName = this.filterForm.get('subContractorName')?.value
    const productName = this.filterForm.get('productName')?.value
    let isStartUrl = false;
    if (subContractorName !== null && subContractorName !== '') {
      url += '?subcontractorName=' + subContractorName;
      isStartUrl = true;
    }
    if (productName !== null && productName !== '') {
      if(isStartUrl){
        url += '&productName=' + productName;
      }
      else {
        url += '?productName=' + productName;
        isStartUrl = true;
      }
    }
    if (fromDate !== null && fromDate !== '') {
      if(isStartUrl){
        url += '&fromDate=' + getDateFormat(fromDate);
      }
      else{
        url += '?fromDate=' + getDateFormat(fromDate);
        isStartUrl = true;
      }
    }
    if (toDate !== null && toDate !== '') {
      if(isStartUrl){
        url += '&toDate=' + getDateFormat(toDate);
      }
      else{
        url += '?toDate=' + getDateFormat(toDate);
      }
    }
    this.reportService.getAllProductWiseReportsGetDto(url).subscribe({
      next: (response: ProductWiseReportComponent[]) => {
        this.data = response;
      },
      error: (error) => {
        this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
      }
    })
  }
}
