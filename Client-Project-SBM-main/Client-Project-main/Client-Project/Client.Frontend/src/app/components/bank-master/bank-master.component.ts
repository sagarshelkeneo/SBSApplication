declare var bootstrap: any;
import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TableComponent } from '../utils/table/table.component';
import { LoginService } from '../../services/login.service';
import { BankMasterService } from '../../services/bank-master.service';
import { BankGetDto } from './bank-dtos';
import { ExportFileService } from '../../services/export-file.service';
import { AlertService } from '../../services/alert.service';
import { ActivatedRoute } from '@angular/router';
import { RoleAccessService } from '../../services/role-access.service';

@Component({
  selector: 'app-bank-master',
  imports: [TableComponent, CommonModule, ReactiveFormsModule],
  templateUrl: './bank-master.component.html',
  styleUrl: '../../../componentStyle.css'
})
export class BankMasterComponent implements OnInit {
  modalMode: 'view' | 'add' | 'edit' = 'view';
  searchVaue: string = '';
  userId: number | null = null;
  companyId: number | null = null;
  fullData: BankGetDto[] = []
  data: BankGetDto[] = [];
  screenCode: string | null = null;
  createAccess: boolean = false;
  editAccess: boolean = false;
  deleteAccess: boolean = false;
  displayedColumns: string[] = ['r_bankName', 'r_branch', 'action'];
  columnsInfo: {
    [key: string]: {
      'title'?: string,
      'isSort'?: boolean,
      'templateRef': TemplateRef<any> | null,
    }
  } = {};
  @ViewChild('actionTemplateRef', { static: true }) actionTemplateRef!: TemplateRef<any>;
  constructor(
    private route: ActivatedRoute,
    private roleAccessService: RoleAccessService,
    private alert: AlertService,
    private loginService: LoginService,
    private bankService: BankMasterService,
    private exportService: ExportFileService,
  ) { }

  ngOnInit(): void {
    this.userId = this.loginService.userId();
    this.companyId = this.loginService.companyId();
    this.screenCode = this.route.snapshot.data['screenCode'];
    if (this.companyId && this.userId && this.screenCode) {
      this.bankService.getAllBankMasterGetDto().subscribe({
        next: (response: BankGetDto[]) => {
          this.data = response;
          this.fullData = response;
        },
        error: (error) => {
          this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
        }
      });
      this.columnsInfo = {
        'r_bankName': {
          'title': 'Bank Name',
          'isSort': true,
          'templateRef': null
        },
        'r_branch': {
          'title': 'Branch',
          'isSort': true,
          'templateRef': null
        },
        'action': {
          'title': 'Action',
          'templateRef': this.actionTemplateRef
        }
      }
      const roleAccessList = this.roleAccessService.getAccessList().find(item => item.a_screenCode === this.screenCode);
      
      if(roleAccessList){
        this.createAccess = roleAccessList.a_createAccess;
        this.editAccess = roleAccessList.a_editAccess;
        this.deleteAccess = roleAccessList.a_deleteAccess;
      }
    }
    else {
      this.loginService.logout();
    }
  }

  bankMasterForm: FormGroup = new FormGroup(
    {
      id: new FormControl(''),
      bankName: new FormControl('', [Validators.required, Validators.maxLength(50),]),
      branch: new FormControl('', [Validators.required, Validators.maxLength(50),]),
      createdBy: new FormControl(''),
      updatedBy: new FormControl(''),
    }
  );

  addBankMasterGetDto() {
    this.bankMasterForm.patchValue({
      createdBy: this.userId
    })
    this.bankMasterForm.enable();
    this.modalMode = 'add';
  }

  exportToPdf() {
    this.exportService.printToPDF('table', 'bankMaster.pdf', [
      'Name',
      'Branch',
    ])
  }

  exportToExcel() {
    this.exportService.printToExcel('table', 'bankMaster.xlsx', [
      'Name',
      'Branch',
    ])
  }

  setSearchValue(value: string) {
    this.searchVaue = value;
  }

  onSearch() {
    this.data = this.fullData.filter(d => d.r_bankName.includes(this.searchVaue));
  }

  closeModal() {
    this.bankMasterForm.reset({
      id: '',
      bankName: '',
      branch: '',
      createdBy: '',
      updatedBy: '',
    })
    this.modalMode = 'view';
  }

  viewBankMasterGetDto(obj: BankGetDto) {
    this.bankMasterForm.patchValue({
      bankName: obj.r_bankName,
      branch: obj.r_branch,
    })
    this.bankMasterForm.disable();
    this.modalMode = 'view';
  }

  editCompanyMasterGetDto(obj: BankGetDto) {
    this.bankMasterForm.patchValue({
      id: obj.r_id,
      bankName: obj.r_bankName,
      branch: obj.r_branch,
      updatedBy: this.userId
    })
    this.bankMasterForm.enable();
    this.modalMode = 'edit';
  }

  deleteRowData(id: number) {
    this.alert.Delete.fire().then((result) => {
      if (result.isConfirmed && this.userId && this.companyId) {
        this.bankService.deleteBankMasterGetDto(id, this.userId).subscribe({
          next: (response: BankGetDto[]) => {
            this.fullData = response;
            this.onSearch();
            this.alert.Toast.fire('Deleted Successfully', '', 'success');
          },
          error: (error) => {
            this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
          }
        });
      }
    });
  }

  saveBankMasterGetDto() {
    if (this.bankMasterForm.invalid) {
      this.bankMasterForm.markAllAsTouched();
      console.log('bank master form invalid', this.bankMasterForm.value);
    }
    else {
      if (this.modalMode === 'edit') {
        this.bankService.editBankMasterUpdateDto(this.bankMasterForm.value).subscribe({
          next: (response: BankGetDto[]) => {
            this.fullData = response;
            this.onSearch()
            this.alert.Toast.fire('Updated Successfully', '', 'success')
            this.closeModal();
            const modalElement = document.getElementById('bankMaster-modal');
            if (modalElement) {
              const modalInstance = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
              modalInstance.hide();
            }
          },
          error: (error) => {
            this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
          }
        });
      }
      else if (this.modalMode === 'add') {
        this.bankService.addBankMasterGetDto(this.bankMasterForm.value).subscribe(
          {
            next: (response: BankGetDto[]) => {
              this.fullData = response;
              this.onSearch();
              this.alert.Toast.fire('Added Successfully', '', 'success')
              this.closeModal();
              const modalElement = document.getElementById('bankMaster-modal');
              if (modalElement) {
                const modalInstance = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
                modalInstance.hide();
              }
            },
            error: (error) => {
              this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
            }
          }
        );
      }
    }
  }
}
