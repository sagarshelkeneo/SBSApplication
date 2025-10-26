import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RoleCreateDto, RoleGetDto, RoleUpdateDto } from '../components/role/role-dtos';
import { apiUrl } from '../../constant';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  constructor(private http: HttpClient) { }

  getAllRoleGetDto(): Observable<RoleGetDto[]> {
    return this.http.get<RoleGetDto[]>(`${apiUrl}/Role`);
  }

  editRoleUpdateDto(formData: RoleUpdateDto): Observable<RoleGetDto[]> {
    return this.http.put<RoleGetDto[]>(`${apiUrl}/Role`, formData);
  }

  addRoleGetDto(formData: RoleCreateDto): Observable<RoleGetDto[]> {
    return this.http.post<RoleGetDto[]>(`${apiUrl}/Role`, formData);
  }

  deleteRoleGetDto(id:number, updatedBy: number): Observable<RoleGetDto[]> {
    return this.http.delete<RoleGetDto[]>(`${apiUrl}/Role/${id}?updatedBy=${updatedBy}`);
  }
}
