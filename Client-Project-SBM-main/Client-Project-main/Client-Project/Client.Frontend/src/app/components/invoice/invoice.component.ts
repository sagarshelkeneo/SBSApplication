declare var bootstrap: any;
import { Component, TemplateRef, ViewChild } from '@angular/core';
import { TableComponent } from "../utils/table/table.component";
import { InvoiceService } from '../../services/invoice.service';
import { AlertService } from '../../services/alert.service';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { InvoiceGetDto } from './invoice-dtos';
import { ProductGetDto } from '../product/product-dtos';
import { ProductService } from '../../services/product.service';
import { SubContractorGetDto } from '../sub-contractor/sub-contractor-dtos';
import { SubContractorService } from '../../services/sub-contractor.service';
import { DatePickerModule } from 'primeng/datepicker';
import { LoginService } from '../../services/login.service';
import { ExportFileService } from '../../services/export-file.service';
import { ActivatedRoute } from '@angular/router';
import { RoleAccessService } from '../../services/role-access.service';

@Component({
  selector: 'app-invoice',
  imports: [TableComponent, CommonModule, ReactiveFormsModule, DatePickerModule],
  templateUrl: './invoice.component.html',
  styleUrl: '../../../componentStyle.css',
})
export class InvoiceComponent {
  companyId: number | null = null;
  userId: number | null = null;
  screenCode: string | null = null;
  createAccess: boolean = false;
  editAccess: boolean = false;
  deleteAccess: boolean = false;
  constructor(
    private roleAccessService: RoleAccessService,
    private route: ActivatedRoute,
    private exportService: ExportFileService,
    private loginService: LoginService,
    private invoiceService: InvoiceService,
    private productService: ProductService,
    private subContractorService: SubContractorService,
    private alert: AlertService
  ) { }
  modalMode: 'view' | 'edit' | 'add' = 'view';
  displayedColumns: string[] = ['srNo', 'r_invoiceDate','r_invoiceNo',  'r_subcontractorName','r_productName', 'r_commissionAmount', 'r_totalAmount', 'action'];
  fullData: InvoiceGetDto[] = [];
  data: InvoiceGetDto[] = [];
  products: ProductGetDto[] = [];
  subContractors: SubContractorGetDto[] = [];
  columnsInfo: {
    [key: string]: {
      'title'?: string,
      'isSort'?: boolean,
      'templateRef': TemplateRef<any> | null,
    }
  } = {};
  @ViewChild('actionTemplateRef', { static: true }) actionTemplateRef!: TemplateRef<any>;
  @ViewChild('invoiceDateTemplateRef', { static: true }) invoiceDateTemplateRef!: TemplateRef<any>;
  @ViewChild('srNoTemplateRef', { static: true }) srNoTemplateRef!: TemplateRef<any>;

  invoiceForm: FormGroup = new FormGroup(
    {
      id: new FormControl(''),
      invoiceNo: new FormControl('', [Validators.required,]),
      companyId: new FormControl(''),
      subcontractorId: new FormControl(''),
      productId: new FormControl('', [Validators.required,]),
      invoiceDate: new FormControl('', [Validators.required]),
      unitAmount: new FormControl('', [Validators.required, Validators.pattern('^[0-9]+(\.[0-9]{1,2})?$'),]),
      // status: new FormControl('', [Validators.required]),
      quantity: new FormControl('', [Validators.required, Validators.pattern('^[0-9]+'),]),
      totalAmount: new FormControl('', [Validators.required]),
      commissionPercentage: new FormControl('', [/*Validators.required,*/ Validators.pattern('^[0-9]+(\.[0-9]{1,2})?$'),]),
      commissionAmount: new FormControl('', [/*Validators.required,*/ Validators.pattern('^[0-9]+(\.[0-9]{1,2})?$'),]),
      paymentMode: new FormControl('', [Validators.required]),
      createdBy: new FormControl(''),
      updatedBy: new FormControl(''),
    }
  );

  filterForm: FormGroup = new FormGroup({
    FromDate: new FormControl(null),
    ToDate: new FormControl(null),
    subContractorName: new FormControl(null),
  });

  ngOnInit(): void {
    this.screenCode = this.route.snapshot.data['screenCode'];
    this.userId = this.loginService.userId();
    this.companyId = this.loginService.companyId();
    if (this.userId && this.companyId && this.screenCode) {
      this.invoiceForm.get('totalAmount')?.disable();
      this.productService.getAllProductGetDto(this.companyId).subscribe({
        next: (response: ProductGetDto[]) => {
          this.products = response
        },
        error: (error) => {
          this.alert.Toast.fire((error.error) ? error.error : ((error.message) ? error.message : 'Something went wrong'), '', 'error');
          console.error(error);
        }
      })
      this.subContractorService.getAllSubContractorGetDto(this.companyId).subscribe({
        next: (response: SubContractorGetDto[]) => {
          this.subContractors = response
        },
        error: (error) => {
          this.alert.Toast.fire((error.error) ? error.error : ((error.message) ? error.message : 'Something went wrong'), '', 'error');
          console.error(error);
        }
      })
      this.invoiceService.getAllInvoiceGetDto(this.companyId).subscribe({
        next: (response: InvoiceGetDto[]) => {

         this.fullData = response.map((item, index) => ({
      ...item,
      srNo: index + 1
    }));
   this.data = this.fullData.map((item, index) => ({ ...item, srNo: index + 1 }));

  },
        error: (error) => {
          this.alert.Toast.fire((error.error) ? error.error : ((error.message) ? error.message : 'Something went wrong'), '', 'error');
          console.error(error);
        }
      });

      const roleAccessList = this.roleAccessService.getAccessList().find(item => item.a_screenCode === this.screenCode);

      if (roleAccessList) {
        this.createAccess = roleAccessList.a_createAccess;
        this.editAccess = roleAccessList.a_editAccess;
        this.deleteAccess = roleAccessList.a_deleteAccess;
      }

      this.columnsInfo = {
        'srNo': {
  'title': 'Sr. No.',
  'isSort': false,
  'templateRef': this.srNoTemplateRef
},
        'r_invoiceNo': {
          'title': 'Invoice No.',
          'isSort': true,
          'templateRef': null
        },
        'r_invoiceType: string': {
  'title': 'Invoice Type',
  'isSort': true,
  'templateRef': null
},
        'r_subcontractorName': {
          'title': 'Sub-Contract Name',
          'isSort': true,
          'templateRef': null
        },
        'r_productName': {
          'title': 'Product Name',
          'isSort': true,
          'templateRef': null
        },
        'r_invoiceDate': {
          'title': 'Invoice Date',
          'isSort': true,
          'templateRef': this.invoiceDateTemplateRef
        },
        'r_unitAmount': {
          'title': 'Unit Amount',
          'isSort': true,
          'templateRef': null
        },
        'r_quantity': {
          'title': 'Quantity',
          'isSort': true,
          'templateRef': null
        },
        'r_commissionAmount': {
          'title': 'Commission Amount',
          'isSort': true,
          'templateRef': null
        },
        'r_totalAmount': {
          'title': 'Total Amount',
          'isSort': true,
          'templateRef': null
        },
        'action': {
          'title': 'Action',
          'templateRef': this.actionTemplateRef
        }
      }
    }
    else {
      this.loginService.logout();
    }
  }

  resetFilter() {
    this.filterForm.reset({
      FromDate: null,
      ToDate: null,
      subContractorName: null,
    })
  }

  exportToPdf() {
    this.exportService.printToPDF('table', 'invoice.pdf', [
      'Invoice No.',
      'Invoice Type',
      'Sub-Contract Name',
      'Invoice Date',
      'Unit Amount',
      'Quantity',
      'Total Amount',
    ])
  }

  exportToExcel() {
    this.exportService.printToExcel('table', 'invoice.xlsx', [
      'Invoice No.',
      'Invoice Type',
      'Sub-Contract Name',
      'Invoice Date',
      'Unit Amount',
      'Quantity',
      'Total Amount',
    ])
  }
onSearch() {
  const fromDate = this.filterForm.get('FromDate')?.value;
  const toDate = this.filterForm.get('ToDate')?.value;
  const subContractorName = this.filterForm.get('subContractorName')?.value?.toLowerCase();

  const filteredData = this.fullData.filter(d => {
    const invoiceDate = new Date(d.r_invoiceDate);
    let match = true;

    // ✅ From Date check
    if (fromDate) {
      match = match && invoiceDate >= new Date(fromDate);
    }

    // ✅ To Date check
    if (toDate) {
      match = match && invoiceDate <= new Date(toDate);
    }

    // ✅ SubContractorName or InvoiceNo check
    if (subContractorName) {
      match = match && (
        (d.r_invoiceNo?.toLowerCase().includes(subContractorName) || false) ||
        (d.r_subcontractorName?.toLowerCase().includes(subContractorName) || false)
      );
    }

    return match;
  });

  // Add srNo
  this.data = filteredData.map((item, index) => ({
    ...item,
    srNo: index + 1
  }));
}



  setUnitAmount(value: string) {
    this.invoiceForm.get('unitAmount')?.setValue((value !== '') ? value.split('_')[1] : '');
  }

  calculateTotalAmount() {
    const unitAmount = Number(this.invoiceForm.get('unitAmount')?.value)
    const quantity = Number(this.invoiceForm.get('quantity')?.value)
    this.invoiceForm.get('totalAmount')?.setValue((unitAmount && unitAmount > 0 && quantity && quantity > 0) ? unitAmount * quantity : '')
  }

  calculateCommissionAmount() {
    // const commissionPercentage = Number(this.invoiceForm.get('commissionPercentage')?.value)
    // const totalAmount = Number(this.invoiceForm.get('totalAmount')?.value)
    // this.invoiceForm.get('commissionAmount')?.setValue((commissionPercentage && totalAmount) ? (Math.floor((commissionPercentage * totalAmount) / 100)) : '')
  }

  closeModal() {
    this.invoiceForm.reset({
      id: '',
      invoicNo: '',
      companyId: '',
      subcontractorId: '',
      productId: '',
      unitAmount: '',
      invoiceDate: '',
      quantity: '',
      totalAmount: '',
      commissionPercentage: '',
      commissionAmount: '',
      paymentMode: '',
      createdBy: '',
      updatedBy: '',
    })
    this.modalMode = 'view';
  }

  viewAndEditInvoiceGetDto(obj: InvoiceGetDto, mode: 'view' | 'edit') {
    this.invoiceForm.patchValue({
      id: obj.r_id,
      invoiceNo: obj.r_invoiceNo,
      companyId: obj.r_companyId,
      subcontractorName: obj.r_subcontractorName,
      productId: obj.r_productId + '_' + obj.unitPrice,
      unitAmount: obj.r_unitAmount,
      invoiceDate: new Date(obj.r_invoiceDate),
      quantity: obj.r_quantity,
      totalAmount: obj.r_totalAmount,
      commissionPercentage: obj.r_commissionPercentage,
      commissionAmount: obj.r_commissionAmount,
      paymentMode: obj.r_invoiceType,
      updatedBy: (mode === 'edit') && this.loginService.userId(),
    })
    if (mode === 'view') {
      this.invoiceForm.disable();
    }
    else {
      this.invoiceForm.enable();
      this.invoiceForm.get('totalAmount')?.disable();
      // this.invoiceForm.get('commissionAmount')?.disable();
    }
    this.modalMode = mode;
  }

  addInvoiceGetDto() {
    this.invoiceForm.patchValue({
      companyId: this.loginService.companyId(),
      subcontractorId: '',
      createdBy: this.loginService.userId(),
    })
    this.invoiceForm.enable();
    this.invoiceForm.get('totalAmount')?.disable();
    // this.invoiceForm.get('commissionAmount')?.disable();
    this.modalMode = 'add';
  }

  deleteRowData(id: number) {
    this.alert.Delete.fire().then((result) => {
      if (result.isConfirmed && this.userId && this.companyId) {
        this.invoiceService.deleteInvoiceGetDto(id, this.companyId, this.userId).subscribe({
          next: (response: InvoiceGetDto[]) => {
            this.fullData = response;
            this.onSearch();
            this.alert.Toast.fire('Deleted Successfully', '', 'success');
          },
          error: (error) => {
            this.alert.Toast.fire((error.error) ? error.error : ((error.message) ? error.message : 'Something went wrong'), '', 'error');
            console.error(error);
          }
        });
      }
    });
  }

  saveInvoiceGetDto() {
    if (this.invoiceForm.invalid) {
      this.invoiceForm.markAllAsTouched();
      console.log('Invoice form invalid', this.invoiceForm.value);
    }
    else {
      function getDateFormat(inputDate: string): string {
        const date = new Date(inputDate);
        const dd = String(date.getDate()).padStart(2, '0');
        const mm = String(date.getMonth() + 1).padStart(2, '0');
        const yyyy = String(date.getFullYear());
        return `${yyyy}-${mm}-${dd}`;
      }
      const formData = this.invoiceForm.value;
      formData['invoiceDate'] = getDateFormat(this.invoiceForm.get('invoiceDate')?.value)
      formData['productId'] = (this.invoiceForm.get('productId')?.value).split('_')[0];
      formData['totalAmount'] = this.invoiceForm.get('totalAmount')?.value
      formData['commissionAmount'] = this.invoiceForm.get('commissionAmount')?.value
      if (this.modalMode === 'edit') {
        this.invoiceService.editInvoiceUpdateDto(formData).subscribe({
          next: (response: InvoiceGetDto[]) => {
            this.fullData = response;
            this.onSearch();
            this.alert.Toast.fire('Updated Successfully', '', 'success')
            this.closeModal()
            const modalElement = document.getElementById('invoice-modal');
            if (modalElement) {
              const modalInstance = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
              modalInstance.hide();
            }
          },
          error: (error) => {
            this.alert.Toast.fire((error.error) ? error.error : ((error.message) ? error.message : 'Something went wrong'), '', 'error');
            console.error(error);
          }
        });
      }
      else if (this.modalMode === 'add') {
        console.log(formData);
        this.invoiceService.addInvoiceGetDto(formData).subscribe(
          {
            next: (response: InvoiceGetDto[]) => {
              this.fullData = response;
              this.onSearch();
              this.alert.Toast.fire('Added Successfully', '', 'success')
              const modalElement = document.getElementById('invoice-modal');
              this.closeModal()
              if (modalElement) {
                const modalInstance = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
                modalInstance.hide();
              }
            },
            error: (error) => {
              this.alert.Toast.fire((error.error) ? error.error : ((error.message) ? error.message : 'Something went wrong'), '', 'error');
              console.error(error);
            }
          }
        );
      }
    }
  }
}
