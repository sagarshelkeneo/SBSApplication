import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BankCreateDto, BankGetDto, BankUpdateDto } from '../components/bank-master/bank-dtos';
import { apiUrl } from '../../constant';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BankMasterService {
constructor(private http: HttpClient) { }

  getAllBankMasterGetDto(): Observable<BankGetDto[]> {
    return this.http.get<BankGetDto[]>(`${apiUrl}/BankMaster`);
  }

  editBankMasterUpdateDto(formData:BankUpdateDto): Observable<BankGetDto[]> {
    return this.http.put<BankGetDto[]>(`${apiUrl}/BankMaster`,formData);
  }

  addBankMasterGetDto(formData:BankCreateDto): Observable<BankGetDto[]> {
    return this.http.post<BankGetDto[]>(`${apiUrl}/BankMaster`,formData);
  }

  deleteBankMasterGetDto(id: number, userId: number): Observable<BankGetDto[]> {
    return this.http.delete<BankGetDto[]>(`${apiUrl}/BankMaster?id=${id}&updatedBy=${userId}`);
  }
}
