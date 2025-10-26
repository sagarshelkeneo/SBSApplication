import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SubContractorCreateDto, SubContractorGetDto, SubContractorUpdateDto } from '../components/sub-contractor/sub-contractor-dtos';
import { apiUrl } from '../../constant';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SubContractorService {
  constructor(private http: HttpClient) { }

  getAllSubContractorGetDto(companyId: number): Observable<SubContractorGetDto[]> {
    return this.http.get<SubContractorGetDto[]>(`${apiUrl}/SubContractor?companyId=${companyId}`);
  }

  editSubContractorUpdateDto(formData: SubContractorUpdateDto): Observable<SubContractorGetDto[]> {
    return this.http.put<SubContractorGetDto[]>(`${apiUrl}/SubContractor`, formData);
  }

  addSubContractorGetDto(formData: SubContractorCreateDto): Observable<SubContractorGetDto[]> {
    return this.http.post<SubContractorGetDto[]>(`${apiUrl}/SubContractor`, formData);
  }

  deleteSubContractorGetDto(id:number, updatedBy: number, companyId: number): Observable<SubContractorGetDto[]> {
    return this.http.delete<SubContractorGetDto[]>(`${apiUrl}/SubContractor/${id}?updatedBy=${updatedBy}&companyId=${companyId}`);
  }
}
