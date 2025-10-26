import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { apiUrl } from '../../constant';
import { InvoiceCreateDto, InvoiceGetDto, InvoiceUpdateDto } from '../components/invoice/invoice-dtos';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  constructor(private http: HttpClient) { }

  getAllInvoiceGetDto(companyId:number): Observable<InvoiceGetDto[]> {
    return this.http.get<InvoiceGetDto[]>(`${apiUrl}/Invoice?companyId=${companyId}`);
  }

  editInvoiceUpdateDto(formData: InvoiceUpdateDto): Observable<InvoiceGetDto[]> {
    return this.http.put<InvoiceGetDto[]>(`${apiUrl}/Invoice/update`, formData);
  }

  addInvoiceGetDto(formData: InvoiceCreateDto): Observable<InvoiceGetDto[]> {
    console.log(formData);
    return this.http.post<InvoiceGetDto[]>(`${apiUrl}/Invoice/create`, formData);
  }

  deleteInvoiceGetDto(id:number, companyId: number, userId: number): Observable<InvoiceGetDto[]> {
    return this.http.delete<InvoiceGetDto[]>(`${apiUrl}/Invoice/${id}?updatedBy=${userId}&companyId=${companyId}`);
  }
}
