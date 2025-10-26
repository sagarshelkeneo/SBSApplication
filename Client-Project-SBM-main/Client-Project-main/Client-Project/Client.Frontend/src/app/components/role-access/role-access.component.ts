import { Component, OnInit } from '@angular/core';
import { RoleAccessService } from '../../services/role-access.service';
import { RoleAccessByRoleIdDto, RoleAccessDto } from './role-access-dto';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { RoleService } from '../../services/role.service';
import { RoleGetDto } from '../role/role-dtos';
import { LoginService } from '../../services/login.service';
import { AlertService } from '../../services/alert.service';

@Component({
  selector: 'app-role-access',
  imports: [CommonModule, FormsModule],
  templateUrl: './role-access.component.html',
  styleUrls: ['../../../componentStyle.css', './role-access.components.css']
})
export class RoleAccessComponent implements OnInit {
  userId: number | null = null;
  screenCode: string | null = null;
  createAccess: boolean = false;
  editAccess: boolean = true;
  deleteAccess: boolean = false;
  username: string | null = null;
  data: RoleAccessByRoleIdDto[] = [];
  roles: RoleGetDto[] = [];
  constructor(
    private alert: AlertService,
    private route: ActivatedRoute,
    private roleAccessService: RoleAccessService,
    private roleService: RoleService,
    private loginService: LoginService,
  ) { }
  ngOnInit(): void {
    this.userId = this.loginService.userId();
    this.screenCode = this.route.snapshot.data['screenCode'];
    if (this.screenCode && this.userId) {
      this.roleService.getAllRoleGetDto().subscribe({
        next: (response: RoleGetDto[]) => {
          this.roles = response;
        },
        error: (error) => {
          this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
        }
      })

      const roleAccessList = this.roleAccessService.getAccessList().find(item => item.a_screenCode === this.screenCode);

      if (roleAccessList) {
        this.createAccess = roleAccessList.a_createAccess;
        this.editAccess = roleAccessList.a_editAccess;
        this.deleteAccess = roleAccessList.a_deleteAccess;
      }
    }
    else {
      this.loginService.logout();
    }
  }

  onSearch(value: string) {
    if (value !== '') {
      this.roleAccessService.getRoleAccessByRoleId(Number(value)).subscribe({
        next: (response: RoleAccessByRoleIdDto[]) => {
          this.data = response;
        },
        error: (error) => {
          this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
        }
      })
    }
    else {
      this.data = [];
    }
  }

  saveRoleAccess(ngForm: NgForm, id: number) {
    const form = ngForm.value
    form['id'] = id;
    form['updatedBy'] = this.userId;
    this.roleAccessService.postRoleAccessForm(form).subscribe({
      next: (response: any) => {
        this.alert.Toast.fire('Updated Successfully', '', 'success')
      },
      error: (error) => {
        this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
      }
    })
  }
}
