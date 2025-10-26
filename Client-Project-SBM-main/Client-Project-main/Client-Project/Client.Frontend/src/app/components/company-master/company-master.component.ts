declare var bootstrap: any;
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { TableComponent } from '../utils/table/table.component';
import { CompanyMasterServiceService } from '../../services/company-master-service.service';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AlertService } from '../../services/alert.service';
import { CompanyMasterGetDto } from './company-master-dtos';
import { LoginService } from '../../services/login.service';
import { ExportFileService } from '../../services/export-file.service';
import { ActivatedRoute } from '@angular/router';
import { RoleAccessService } from '../../services/role-access.service';

@Component({
  selector: 'app-company-master',
  imports: [TableComponent, CommonModule, ReactiveFormsModule],
  templateUrl: './company-master.component.html',
  styleUrl: '../../../componentStyle.css'
})
export class CompanyMasterComponent implements OnInit {
  userId: number | null = null;
  companyId: number | null = null;
  searchVaue: string = '';
  screenCode: string | null = null;
  createAccess: boolean = false;
  editAccess: boolean = false;
  deleteAccess: boolean = false;
  constructor(
    private route: ActivatedRoute,
    private roleAccessService: RoleAccessService,
    private exportService: ExportFileService,
    private loginService: LoginService,
    private companyMasterService: CompanyMasterServiceService,
    private alert: AlertService
  ) { }
  modalMode: 'view' | 'edit' | 'add' = 'view';
  displayedColumns: string[] = ['name', 'phone', 'email', 'action'];
  data: CompanyMasterGetDto[] = [];
  fullData: CompanyMasterGetDto[] = [];
  columnsInfo: {
    [key: string]: {
      'title'?: string,
      'isSort'?: boolean,
      'templateRef': TemplateRef<any> | null,
    }
  } = {};
  @ViewChild('actionTemplateRef', { static: true }) actionTemplateRef!: TemplateRef<any>;

  companyMasterForm: FormGroup = new FormGroup(
    {
      id: new FormControl(''),
      name: new FormControl('', [Validators.required, Validators.maxLength(20),]),
      phone: new FormControl('', [Validators.required, Validators.pattern('^[0-9]{10}$'),]),
      email: new FormControl('', [Validators.required, Validators.email,Validators.maxLength(70),]),
      address: new FormControl('', [Validators.required, Validators.maxLength(250),]),
      createdBy: new FormControl(''),
      updatedBy: new FormControl(''),
    }
  );

  ngOnInit(): void {
    this.companyId = this.loginService.companyId();
    this.userId = this.loginService.userId();
    this.screenCode = this.route.snapshot.data['screenCode'];
    if(this.companyId && this.userId && this.screenCode) {
      this.companyMasterService.getAllCompanyMasterGetDto().subscribe({
        next: (response: CompanyMasterGetDto[]) => {
          this.data = response;
          this.fullData = response;
        },
        error: (error) => {
          this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
        }
      });
      this.columnsInfo = {
        'name': {
          'title': 'Name',
          'isSort': true,
          'templateRef': null
        },
        'phone': {
          'title': 'Phone No.',
          'isSort': true,
          'templateRef': null
        },
        'email': {
          'title': 'Email Address',
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

  exportToPdf(){
    this.exportService.printToPDF('table','companyMaster.pdf',[
      'Name',
      'Phone No.',
      'Email Address',
    ])
  }

  exportToExcel(){
    this.exportService.printToExcel('table', 'companyMaster.xlsx', [
      'Name',
      'Phone No.',
      'Email Address',
    ])
  }

  setSearchValue(value: string){
    this.searchVaue = value;
  }

  onSearch() {
    this.data = this.fullData.filter(d => d.name.includes(this.searchVaue));
  }

  closeModal() {
    this.companyMasterForm.reset({
      id: '',
      name: '',
      phone: '',
      email: '',
      address: '',
      createdBy: '',
      updatedBy: '',
    })
    this.modalMode = 'view';
  }

  viewCompanyMasterGetDto(obj: CompanyMasterGetDto) {
    this.companyMasterForm.patchValue({
      name: obj.name,
      phone: obj.phone,
      email: obj.email,
      address: obj.address
    })
    this.companyMasterForm.disable();
    this.modalMode = 'view';
  }

  editCompanyMasterGetDto(obj: CompanyMasterGetDto) {
    this.companyMasterForm.patchValue({
      id: obj.id,
      name: obj.name,
      phone: obj.phone,
      email: obj.email,
      address: obj.address,
      updatedBy: this.userId
    })
    this.companyMasterForm.enable();
    this.modalMode = 'edit';
  }

  addCompanyMasterGetDto() {
     this.companyMasterForm.patchValue({
      createdBy: this.userId
    })
    this.companyMasterForm.enable();
    this.modalMode = 'add';
  }

  deleteRowData(id: number) {
    this.alert.Delete.fire().then((result) => {
      if (result.isConfirmed && this.userId && this.companyId) {
        this.companyMasterService.deleteCompanyMasterGetDto(id, this.userId).subscribe({
          next: (response: CompanyMasterGetDto[]) => {
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

  saveCompanyMasterGetDto() {
    if (this.companyMasterForm.invalid) {
      this.companyMasterForm.markAllAsTouched();
      console.log('company master form invalid', this.companyMasterForm.value);
    }
    else {
      if (this.modalMode === 'edit') {
        this.companyMasterService.editCompanyMasterUpdateDto(this.companyMasterForm.value).subscribe({
          next: (response: CompanyMasterGetDto[]) => {
            this.fullData = response;
            this.onSearch()
            this.alert.Toast.fire('Updated Successfully', '', 'success')
            this.closeModal();
            const modalElement = document.getElementById('companyMaster-modal');
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
        this.companyMasterService.addCompanyMasterGetDto(this.companyMasterForm.value).subscribe(
          {
            next: (response: CompanyMasterGetDto[]) => {
              this.fullData = response;
              this.onSearch();
              this.alert.Toast.fire('Added Successfully', '', 'success')
              this.closeModal();
              const modalElement = document.getElementById('companyMaster-modal');
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
