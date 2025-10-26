import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiUrl } from '../../constant';
import { Observable } from 'rxjs';
import { CompanyMasterCreateDto, CompanyMasterGetDto, CompanyMasterUpdateDto } from '../components/company-master/company-master-dtos';

@Injectable({
  providedIn: 'root'
})
export class CompanyMasterServiceService {
  constructor(private http: HttpClient) { }

  getAllCompanyMasterGetDto(): Observable<CompanyMasterGetDto[]> {
    return this.http.get<CompanyMasterGetDto[]>(`${apiUrl}/Company`);
  }

  editCompanyMasterUpdateDto(formData:CompanyMasterUpdateDto): Observable<CompanyMasterGetDto[]> {
    return this.http.put<CompanyMasterGetDto[]>(`${apiUrl}/Company/update`,formData);
  }

  addCompanyMasterGetDto(formData:CompanyMasterCreateDto): Observable<CompanyMasterGetDto[]> {
    return this.http.post<CompanyMasterGetDto[]>(`${apiUrl}/Company/create`,formData);
  }

  deleteCompanyMasterGetDto(companyId: number, userId: number): Observable<CompanyMasterGetDto[]> {
    return this.http.delete<CompanyMasterGetDto[]>(`${apiUrl}/Company/${companyId}?updatedBy=${userId}`);
  }

  sendEmail(form:any): Observable<CompanyMasterGetDto[]> {
    return this.http.post<CompanyMasterGetDto[]>(`${apiUrl}/Company/send-report-email`,form);
  }
}
