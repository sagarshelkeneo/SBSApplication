declare var bootstrap: any;
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { LoginService } from '../../services/login.service';
import { RouterModule } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserMasterService } from '../../services/user-master.service';
import { AlertService } from '../../services/alert.service';
import { CompanyMasterServiceService } from '../../services/company-master-service.service';

@Component({
  selector: 'app-side-bar',
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './side-bar.component.html',
  styleUrls: ['./side-bar.component.css', '../../../componentStyle.css']
})
export class SideBarComponent {
  userRole: string | null = '';
  userId: number | null = null;
  companyId: number | null = null;
  email: string | null = null;
  user: string | null = null;

  constructor(
    private alert: AlertService,
    private loginService: LoginService,
    private userService: UserMasterService,
    private companyService: CompanyMasterServiceService,
  ) { }
  ngOnInit(): void {
    this.userRole = this.loginService.role();
    this.companyId = this.loginService.companyId();
    this.userId = this.loginService.userId();
    this.user = this.loginService.user();
    this.email = this.loginService.email();
    if (!this.companyId || !this.userId || !this.email || !this.user) {
      this.loginService.logout();
    }
  }

  checkConfigAccess = (): boolean => (this.userRole === 'Super Admin' || this.userRole === 'config_user');
  checkMasterAccess = (): boolean => (this.userRole === 'Super Admin' || this.userRole === 'Admin' || this.userRole === 'Manager');
  checkNormalAccess = (): boolean => (this.userRole === 'Super Admin' || this.userRole === 'Admin' || this.userRole === 'Manager' || this.userRole === 'data_user');
  checkReportAccess = (): boolean => (this.userRole === 'Super Admin' || this.userRole === 'Admin' || this.userRole === 'Manager' || this.userRole === 'report_user');


  userChangeForm: FormGroup = new FormGroup(
    {
      username: new FormControl('', [Validators.required,]),
      email: new FormControl('', [Validators.required,]),
      currentPassword: new FormControl('', [Validators.required,]),
      newPassword: new FormControl('', [Validators.required, Validators.pattern('^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&-+=()])(?=\\S+$).{4,10}$'),]),
    }
  );

  emailForm: FormGroup = new FormGroup({
    companyId: new FormControl(''),
    toEmail: new FormControl('', [Validators.required,]),
    ccEmail: new FormControl(''),
  })

  closeModal() {
    this.userChangeForm.reset({
      username: '',
      email: '',
      currentPassword: '',
      newPassword: '',
    })
  }

  closeMailModal() {
    this.emailForm.reset({
      companyId: '',
      toEmail: '',
      ccEmail: '',
    })
  }

  saveUserGetDto() {
    this.userChangeForm.get('username')?.setValue(this.user);
    this.userChangeForm.get('email')?.setValue(this.email);
    if (this.userChangeForm.invalid) {
      this.userChangeForm.markAllAsTouched();
      console.log('Password Change Form Invalid: ', this.userChangeForm.value);
    }
    else {
      this.userService.changePassword(this.userChangeForm.value).subscribe({
        next: (response: any) => {
          this.alert.Toast.fire('Updated Successfully', '', 'success')
          this.closeModal();
          const modalElement = document.getElementById('change-modal');
          if (modalElement) {
            const modalInstance = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
            modalInstance.hide();
          }
          this.loginService.logout();
        },
        error: (error) => {
          this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
        }
      });
    }
  }
  saveMail() {
    if (this.emailForm.invalid) {
      this.emailForm.markAllAsTouched();
      console.log('Email Form Invalid: ', this.emailForm.value);
    }
    else {
      const form = {
        companyId: this.companyId,
        toEmail: this.emailForm.get('toEmail')?.value,
        ccEmail: null
      }
      this.companyService.sendEmail(form).subscribe({
        next: (response: any) => {
          this.alert.Toast.fire('Sent Successfully', '', 'success')
          this.closeModal();
          const modalElement = document.getElementById('mail-modal');
          if (modalElement) {
            const modalInstance = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
            modalInstance.hide();
          }
          this.loginService.logout();
        },
        error: (error) => {
          this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
        }
      });
    }
  }
}