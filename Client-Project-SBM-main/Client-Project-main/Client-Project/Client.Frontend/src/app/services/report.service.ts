import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { apiUrl } from '../../constant';
import { PaidReportDto } from '../components/paid-reports/paid-report-dto';
import { UnpaidReportDto } from '../components/unpaid-reports/unpaid-report-dto';
import { ProductWiseReportComponent } from '../components/product-wise-report/product-wise-report.component';
import { SubContractorWiseReportDto } from '../components/sub-contractor-wise-report/sub-contractor-wise-report-dto';
import { CombinedSubcontactorEntityDto } from '../components/combined-subcontractor-entity-report/combined-subcontactor-entity-dto';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  constructor(private http: HttpClient) { }

  getAllPaidReportsGetDto(url:string): Observable<PaidReportDto[]> {
    return this.http.get<PaidReportDto[]>(`${apiUrl}/Report/paid-report${url}`);
  }
   getAllUnPaidReportsGetDto(url:string): Observable<UnpaidReportDto[]> {
    return this.http.get<UnpaidReportDto[]>(`${apiUrl}/Report/unpaid-report${url}`);
  }
  getAllProductWiseReportsGetDto(url:string): Observable<ProductWiseReportComponent[]> {
    return this.http.get<ProductWiseReportComponent[]>(`${apiUrl}/Report/product-wise-report${url}`);
  }
  getAllSubContractorWiseReportsGetDto(url:string): Observable<SubContractorWiseReportDto[]> {
    return this.http.get<SubContractorWiseReportDto[]>(`${apiUrl}/Report/subcontractor-wise-report${url}`);
  }
  getAllCombinedSubContractorEntityReportsGetDto(): Observable<CombinedSubcontactorEntityDto[]> {
    return this.http.get<CombinedSubcontactorEntityDto[]>(`${apiUrl}/Report/combined-subcontractor-entity`);
  }
}
