import { Component, TemplateRef, ViewChild } from '@angular/core';
import { CombinedSubcontactorEntityDto } from './combined-subcontactor-entity-dto';
import { ExportFileService } from '../../services/export-file.service';
import { LoginService } from '../../services/login.service';
import { ReportService } from '../../services/report.service';
import { TableComponent } from '../utils/table/table.component';
import { CommonModule } from '@angular/common';
import { AlertService } from '../../services/alert.service';

@Component({
  selector: 'app-combined-subcontractor-entity-report',
  imports: [TableComponent, CommonModule],
  templateUrl: './combined-subcontractor-entity-report.component.html',
  styleUrl: '../../../componentStyle.css'
})
export class CombinedSubcontractorEntityReportComponent {
  companyId: number | null = null;
  data: CombinedSubcontactorEntityDto[] = []
  displayedColumns = [
    "r_Date",
    "subContractorName",
    "quantity",
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
  @ViewChild('dateTemplateRef', { static: true }) dateTemplateRef!: TemplateRef<any>;
  @ViewChild('nameTemplateRef', { static: true }) nameTemplateRef!: TemplateRef<any>;
  @ViewChild('quatityTemplateRef', { static: true }) quatityTemplateRef!: TemplateRef<any>;
  @ViewChild('cashTemplateRef', { static: true }) cashTemplateRef!: TemplateRef<any>;
  @ViewChild('balanceTemplateRef', { static: true }) balanceTemplateRef!: TemplateRef<any>;
  @ViewChild('paidTemplateRef', { static: true }) paidTemplateRef!: TemplateRef<any>;
  constructor(
    private alert: AlertService,
    private exportService: ExportFileService,
    private loginService: LoginService,
    private reportService: ReportService
  ) { }
  ngOnInit(): void {
    this.companyId = this.loginService.companyId();
    if(this.companyId){
      this.reportService.getAllCombinedSubContractorEntityReportsGetDto().subscribe({
        next: (response: CombinedSubcontactorEntityDto[]) => {
          this.data = response;
        },
        error: (error) => {
          this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
        }
      })
      this.columnsInfo = {
        'r_Date': {
          'title': 'Date',
          'isSort': true,
          'templateRef': this.dateTemplateRef
        },
        'subContractorName': {
          'title': 'Sub-Contractor',
          'isSort': true,
          'templateRef': this.nameTemplateRef
        },
        'quantity': {
          'title': 'Quantity',
          'isSort': true,
          'templateRef': this.quatityTemplateRef
        },
        'cashAmount': {
          'title': 'Cash Amount',
          'isSort': true,
          'templateRef': this.cashTemplateRef
        },
        'balanceAmount': {
          'title': 'Balance Amount',
          'isSort': true,
          'templateRef': this.balanceTemplateRef
        },
        'paidAmount': {
          'title': 'Paid Amount',
          'isSort': true,
          'templateRef': this.paidTemplateRef
        },
      }
    }
    else {
      this.loginService.logout();
    }
  }

  exportToPdf() {
    this.exportService.printToPDF('table', 'Combined-Report.pdf', [
      'Date',
      'Sub-Contractor',
      'Quantity',
      'Cash Amount',
      'Balance Amount',
      'Paid Amount',
    ])
  }

  exportToExcel(){
    this.exportService.printToExcel('table', 'Combined-Report.xlsx', [
      'Date',
      'Sub-Contractor',
      'Quantity',
      'Cash Amount',
      'Balance Amount',
      'Paid Amount',
    ])
  }
}
