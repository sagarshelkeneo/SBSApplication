import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { apiUrl } from '../../constant';
import { PaymentCreateDto, PaymentGetDto, PaymentUpdateDto } from '../components/payment/payment-dtos';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  constructor(private http: HttpClient) { }

  getAllPaymentGetDto(companyId: number): Observable<PaymentGetDto[]> {
    return this.http.get<PaymentGetDto[]>(`${apiUrl}/Payment?companyId=${companyId}`);
  }

  editPaymentUpdateDto(formData: PaymentUpdateDto): Observable<PaymentGetDto[]> {
    return this.http.put<PaymentGetDto[]>(`${apiUrl}/Payment/update`, formData);
  }

  addPaymentGetDto(formData: PaymentCreateDto): Observable<PaymentGetDto[]> {
    return this.http.post<PaymentGetDto[]>(`${apiUrl}/Payment`, formData);
  }

  deletePaymentGetDto(id:number, updatedBy: number, companyId: number): Observable<PaymentGetDto[]> {
    return this.http.delete<PaymentGetDto[]>(`${apiUrl}/Payment/delete?id=${id}&updatedBy=${updatedBy}&companyId=${companyId}`);
  }
}
