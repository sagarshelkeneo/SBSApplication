import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { apiUrl } from '../../constant';
import { AdditionalEntityCreateDto, AdditionalEntityGetDto, AdditionalEntityUpdateDto } from '../components/additional-entity/additional-entity-dto';

@Injectable({
  providedIn: 'root'
})
export class AdditionalEntityService {
  constructor(private http: HttpClient) { }

  getAllAdditionalEntityGetDto(companyID:number): Observable<AdditionalEntityGetDto[]> {
    return this.http.get<AdditionalEntityGetDto[]>(`${apiUrl}/AdditionalEntity?companyId=${companyID}`);
  }

  editAdditionalEntityUpdateDto(formData: AdditionalEntityUpdateDto): Observable<AdditionalEntityGetDto[]> {
    return this.http.put<AdditionalEntityGetDto[]>(`${apiUrl}/AdditionalEntity`,formData);
  }

  addAdditionalEntityGetDto(formData: AdditionalEntityCreateDto): Observable<AdditionalEntityGetDto[]> {
    return this.http.post<AdditionalEntityGetDto[]>(`${apiUrl}/AdditionalEntity`,formData);
  }

  deleteAdditionalEntityGetDto(id: number, userId: number, companyID: number): Observable<AdditionalEntityGetDto[]> {
    return this.http.delete<AdditionalEntityGetDto[]>(`${apiUrl}/AdditionalEntity/${id}?updatedBy=${userId}&companyId=${companyID}`);
  }
}
