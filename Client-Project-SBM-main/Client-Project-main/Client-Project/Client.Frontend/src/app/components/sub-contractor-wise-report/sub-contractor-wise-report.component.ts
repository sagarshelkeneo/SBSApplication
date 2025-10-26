import { Component, TemplateRef } from '@angular/core';
import { TableComponent } from "../utils/table/table.component";
import { SubContractorWiseReportDto } from './sub-contractor-wise-report-dto';
import { ReportService } from '../../services/report.service';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DatePickerModule } from 'primeng/datepicker';
import { ExportFileService } from '../../services/export-file.service';
import { LoginService } from '../../services/login.service';
import { AlertService } from '../../services/alert.service';

@Component({
  selector: 'app-sub-contractor-wise-report',
  imports: [TableComponent, ReactiveFormsModule, CommonModule, DatePickerModule],
  templateUrl: './sub-contractor-wise-report.component.html',
  styleUrl: '../../../componentStyle.css'
})
export class SubContractorWiseReportComponent {
  companyId: number | null = null;
  data: SubContractorWiseReportDto[] = []
  displayedColumns = [
    "subContractorName",
    "monthAndYear",
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
    private exportService: ExportFileService,
    private loginService: LoginService,
    private reportService: ReportService
  ) { }
  ngOnInit(): void {
    this.companyId = this.loginService.companyId();
    if(this.companyId){
      this.reportService.getAllSubContractorWiseReportsGetDto('').subscribe({
        next: (response: SubContractorWiseReportDto[]) => {
          this.data = response;
        },
        error: (error) => {
          this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
        }
      })
      this.columnsInfo = {
        'subContractorName': {
          'title': 'Sub-Contractor',
          'isSort': true,
          'templateRef': null
        },
        'monthAndYear': {
          'title': 'Month-Year',
          'isSort': true,
          'templateRef': null
        },
        'cashAmount': {
          'title': 'Cash Amount',
          'isSort': true,
          'templateRef': null
        },
        'balanceAmount': {
          'title': 'Balance Amount',
          'isSort': true,
          'templateRef': null
        },
        'paidAmount': {
          'title': 'Paid Amount',
          'isSort': true,
          'templateRef': null
        },
      }
    }
    else {
      this.loginService.logout();
    }
  }

  filterForm: FormGroup = new FormGroup({
    FromDate: new FormControl(null),
    ToDate: new FormControl(null),
    subContractorName: new FormControl(null),
  });

  resetFilter() {
    this.filterForm.reset({
      FromDate: null,
      ToDate: null,
      subContractorName: null,
    })
  }

  exportToPdf() {
    this.exportService.printToPDF('table', 'Sub-Contractor-Wise-Report.pdf', [
      'Sub-Contractor',
      'Month-Year',
      'Cash Amount',
      'Balance Amount',
      'Paid Amount',
    ])
  }

  exportToExcel(){
    this.exportService.printToExcel('table', 'Sub-Contractor-Wise-Report.xlsx', [
      'Sub-Contractor',
      'Month-Year',
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
    let url = '';
    const fromDate = this.filterForm.get('FromDate')?.value;
    const toDate = this.filterForm.get('ToDate')?.value;
    const subContractorName = this.filterForm.get('subContractorName')?.value
    let isStartUrl = false;
    if (subContractorName !== null && subContractorName !== '') {
      url += '?subcontractorName=' + subContractorName;
      isStartUrl = true;
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
    this.reportService.getAllSubContractorWiseReportsGetDto(url).subscribe({
      next: (response: SubContractorWiseReportDto[]) => {
        this.data = response;
      },
      error: (error) => {
        this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
      }
    })
  }
}
