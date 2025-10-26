import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { apiUrl } from '../../constant';
import { ProductCreateDto, ProductGetDto, ProductUpdateDto } from '../components/product/product-dtos';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  constructor(private http: HttpClient) { }

  getAllProductGetDto(companyId: number): Observable<ProductGetDto[]> {
    return this.http.get<ProductGetDto[]>(`${apiUrl}/Product?companyId=${companyId}`);
  }

  editProductUpdateDto(formData: ProductUpdateDto): Observable<ProductGetDto[]> {
    return this.http.put<ProductGetDto[]>(`${apiUrl}/Product/update`, formData);
  }

  addProductGetDto(formData: ProductCreateDto): Observable<ProductGetDto[]> {
    return this.http.post<ProductGetDto[]>(`${apiUrl}/Product/create`, formData);
  }

  deleteProductGetDto(id: number, userId: number, companyId: number): Observable<ProductGetDto[]> {
    return this.http.delete<ProductGetDto[]>(`${apiUrl}/Product/${id}?updatedBy=${userId}&companyId=${companyId}`);
  }
}
