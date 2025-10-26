import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ChangePasswordDto, UserCreateDto, UserGetDto, UserUpdateDto } from '../components/user-master/user-dtos';
import { apiUrl } from '../../constant';

@Injectable({
  providedIn: 'root'
})
export class UserMasterService {
  constructor(private http: HttpClient) { }

  getAllUserGetDto(companyId: number): Observable<UserGetDto[]> {
    return this.http.get<UserGetDto[]>(`${apiUrl}/User?companyId=${companyId}`);
  }

  editUserUpdateDto(formData: UserUpdateDto): Observable<UserGetDto[]> {
    return this.http.put<UserGetDto[]>(`${apiUrl}/User`, formData);
  }

  toggleUserUpdateDto(formData: { id: number, isActive: number }, companyID: number): Observable<UserGetDto[]> {
    return this.http.put<UserGetDto[]>(`${apiUrl}/User/toggle-active?companyId=${companyID}`, formData);
  }

  changePassword(formData: ChangePasswordDto): Observable<any> {
    return this.http.post<any>(`${apiUrl}/User/change-password`, formData);
  }

  addUserGetDto(formData: UserCreateDto): Observable<UserGetDto[]> {
    return this.http.post<UserGetDto[]>(`${apiUrl}/User`, formData);
  }

  deleteUserGetDto(id: number, userId: number, companyId: number): Observable<UserGetDto[]> {
    return this.http.delete<UserGetDto[]>(`${apiUrl}/User/${id}?updatedBy=${userId}&companyId=${companyId}`);
  }
}
