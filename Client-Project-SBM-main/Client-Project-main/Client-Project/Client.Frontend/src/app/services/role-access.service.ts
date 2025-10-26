import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { apiUrl } from '../../constant';
import { RoleAccessByRoleIdDto, RoleAccessDto, RoleAccessUpdateDto } from '../components/role-access/role-access-dto';
import { AlertService } from './alert.service';

@Injectable({
  providedIn: 'root'
})
export class RoleAccessService {
  constructor(
    private alert: AlertService,
    private http: HttpClient
  ) { }

  private accessList: RoleAccessDto[] = []

  getUserAccess(username: string): Observable<RoleAccessDto[]> {
    return this.http.get<RoleAccessDto[]>(`${apiUrl}/RoleAccess?username=${username}`)
  }

  getRoleAccessByRoleId(roleId: number): Observable<RoleAccessByRoleIdDto[]> {
    return this.http.get<RoleAccessByRoleIdDto[]>(`${apiUrl}/RoleAccess/${roleId}`);
  }

  setAccessList(username: string) {
    this.http.get<RoleAccessDto[]>(`${apiUrl}/RoleAccess?username=${username}`).subscribe({
      next: (response: RoleAccessDto[]) => {
        this.accessList = response;
      },
      error: (error) => {
        this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
      }
    })
  }

  roleGuardSetAccessList(accessListThroughRoleGuard: RoleAccessDto[]) {
    this.accessList = accessListThroughRoleGuard;
  }

  getAccessList(): RoleAccessDto[] {
    return this.accessList
  }

  postRoleAccessForm(formData: RoleAccessUpdateDto) {
    return this.http.put<any>(`${apiUrl}/RoleAccess`, formData)
  }
}
