declare var bootstrap: any;
import { Component, TemplateRef, ViewChild } from '@angular/core';
import { SubContractorService } from '../../services/sub-contractor.service';
import { AlertService } from '../../services/alert.service';
import { SubContractorGetDto } from './sub-contractor-dtos';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TableComponent } from '../utils/table/table.component';
import { LoginService } from '../../services/login.service';
import { ExportFileService } from '../../services/export-file.service';
import { RoleAccessService } from '../../services/role-access.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-sub-contractor',
  imports: [CommonModule, TableComponent, ReactiveFormsModule],
  templateUrl: './sub-contractor.component.html',
  styleUrl: '../../../componentStyle.css'
})
export class SubContractorComponent {
  screenCode: string | null = null;
  createAccess: boolean = false;
  editAccess: boolean = false;
  deleteAccess: boolean = false;
  companyId: number | null = null;
  userId: number | null = null;
  constructor(
    private route: ActivatedRoute,
    private roleAccessService: RoleAccessService,
    private exportService: ExportFileService,
    private loginService: LoginService,
    private subContractorService: SubContractorService,
    private alert: AlertService
  ) { }
  searchVaue: string = '';
  modalMode: 'view' | 'edit' | 'add' = 'edit';
  displayedColumns: string[] = ['name', 'action'];
  data: SubContractorGetDto[] = [];
  fullData: SubContractorGetDto[] = [];
  columnsInfo: {
    [key: string]: {
      'title'?: string,
      'isSort'?: boolean,
      'templateRef': TemplateRef<any> | null,
    }
  } = {};
  @ViewChild('actionTemplateRef', { static: true }) actionTemplateRef!: TemplateRef<any>;

  subContractorForm: FormGroup = new FormGroup(
    {
      id: new FormControl(''),
      companyId: new FormControl('', [Validators.required,]),
      name: new FormControl('', [Validators.required, Validators.maxLength(50),]),
      createdBy: new FormControl(''),
      updatedBy: new FormControl(''),
    }
  );

  ngOnInit(): void {
    this.companyId = this.loginService.companyId()
    this.userId = this.loginService.userId()
    this.screenCode = this.route.snapshot.data['screenCode'];
    if (this.companyId && this.userId && this.screenCode) {
      this.subContractorService.getAllSubContractorGetDto(this.companyId).subscribe({
        next: (response: SubContractorGetDto[]) => {
          this.fullData = response;
          this.data = response;
        },
        error: (error) => {
          this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
        }
      });
      this.columnsInfo = {
        'name': {
          'title': 'Sub Contractor',
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
      this.loginService.logout()
    }

  }

  exportToPdf() {
    this.exportService.printToPDF('table', 'subContractorMaster.pdf', [
      'Sub Contractor',
    ])
  }

  exportToExcel() {
    this.exportService.printToExcel('table', 'subContractorMaster.xlsx', [
      'Sub Contractor',
    ])
  }

  setSearchValue(value: string) {
    this.searchVaue = value;
  }

  onSearch() {
    this.data = this.fullData.filter(d => d.name.includes(this.searchVaue));
  }


  closeModal() {
    this.subContractorForm.reset({
      id: '',
      companyId: '',
      name: '',
      createdBy: '',
      updatedBy: '',
    })
    this.modalMode = 'view';
  }

  addSubContractorGetDto() {
    this.subContractorForm.patchValue({
      companyId: this.companyId,
      createdBy: this.userId,
    })
    this.subContractorForm.enable();
    this.modalMode = 'add';
  }

  viewAndEditSubContractorGetDto(obj: SubContractorGetDto, mode: 'view' | 'edit') {
    this.subContractorForm.patchValue({
      id: obj.id,
      companyId: obj.companyId,
      name: obj.name,
      updatedBy: this.userId,
    })
    if (mode === 'view') {
      this.subContractorForm.disable();
    }
    else {
      this.subContractorForm.enable();
    }
    this.modalMode = mode;
  }

  deleteRowData(id: number) {
    this.alert.Delete.fire().then((result) => {
      if (result.isConfirmed && this.userId && this.companyId) {
        this.subContractorService.deleteSubContractorGetDto(id, this.userId, this.companyId).subscribe({
          next: (response: SubContractorGetDto[]) => {
            this.fullData = response;
            this.onSearch();
            this.alert.Toast.fire('Deleted Successfully', '', 'success');
          },
          error: (error) => {
            this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
          }
        })
      }
    });
  }

  saveSubContractorGetDto() {
    if (this.subContractorForm.invalid) {
      this.subContractorForm.markAllAsTouched();
      console.log('SubContractor form invalid', this.subContractorForm.value);
    }
    else {
      if (this.modalMode === 'edit') {
        this.subContractorService.editSubContractorUpdateDto(this.subContractorForm.value).subscribe({
          next: (response: SubContractorGetDto[]) => {
            this.fullData = response;
            this.onSearch();
            this.alert.Toast.fire('Updated Successfully', '', 'success');
            this.closeModal();
            const modalElement = document.getElementById('subContractor-modal');
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
        this.subContractorService.addSubContractorGetDto(this.subContractorForm.value).subscribe(
          {
            next: (response: SubContractorGetDto[]) => {
              this.fullData = response;
              this.onSearch();
              this.alert.Toast.fire('Added Successfully', '', 'success');
              this.closeModal();
              const modalElement = document.getElementById('subContractor-modal');
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
