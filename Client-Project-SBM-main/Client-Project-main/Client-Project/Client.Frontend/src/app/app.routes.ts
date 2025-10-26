import { Routes } from '@angular/router';
import { CompanyMasterComponent } from './components/company-master/company-master.component';
import { InvoiceComponent } from './components/invoice/invoice.component';
import { ProductComponent } from './components/product/product.component';
import { UserMasterComponent } from './components/user-master/user-master.component';
import { RoleComponent } from './components/role/role.component';
import { SubContractorComponent } from './components/sub-contractor/sub-contractor.component';
import { PaymentComponent } from './components/payment/payment.component';
import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { authGuard } from './auth.guard';
import { PaidReportsComponent } from './components/paid-reports/paid-reports.component';
import { UnpaidReportsComponent } from './components/unpaid-reports/unpaid-reports.component';
import { ProductWiseReportComponent } from './components/product-wise-report/product-wise-report.component';
import { SubContractorWiseReportComponent } from './components/sub-contractor-wise-report/sub-contractor-wise-report.component';
import { BankMasterComponent } from './components/bank-master/bank-master.component';
import { CombinedSubcontractorEntityReportComponent } from './components/combined-subcontractor-entity-report/combined-subcontractor-entity-report.component';
import { AdditionalEntityComponent } from './components/additional-entity/additional-entity.component';
import { RoleGuard } from './role.guard';
import { RoleAccessComponent } from './components/role-access/role-access.component';

export const routes: Routes = [
    { path: '', component: LoginComponent },
    { path: 'home', component: HomeComponent, canActivate: [authGuard,] },
    { path: 'invoice', component: InvoiceComponent, canActivate: [authGuard, RoleGuard], data: { screenCode: 'INVOICE' } },
    { path: 'product', component: ProductComponent, canActivate: [authGuard, RoleGuard], data: { screenCode: 'PRODUCT' }},
    { path: 'subContractor', component: SubContractorComponent, canActivate: [authGuard, RoleGuard], data: { screenCode: 'SUBCONTRACTOR' } },
    { path: 'payment', component: PaymentComponent, canActivate: [authGuard, RoleGuard], data: { screenCode: 'PAYMENT' }},
    { path: 'additionalEntity', component: AdditionalEntityComponent, canActivate: [authGuard, RoleGuard], data: { screenCode: 'ADDITIONALENTITY' } },

    { path: 'paidReport', component: PaidReportsComponent, canActivate: [authGuard, RoleGuard], data: { screenCode: 'PAIDREPORT' }},
    { path: 'unpaidReport', component: UnpaidReportsComponent, canActivate: [authGuard, RoleGuard], data: { screenCode: 'UNPAIDREPORT' }},
    { path: 'productWiseReport', component: ProductWiseReportComponent, canActivate: [authGuard, RoleGuard], data: { screenCode: 'PRODUCTWISEREPORT' }},
    { path: 'subContractorWiseReport', component: SubContractorWiseReportComponent, canActivate: [authGuard, RoleGuard], data: { screenCode: 'SUBCONTRACTORWISEREPORT' }},
    { path: 'combinedSubcontractorEntityReport', component: CombinedSubcontractorEntityReportComponent, canActivate: [authGuard, RoleGuard], data: { screenCode: 'COMBINEDREPORT' }},

    { path: 'companyMaster', component: CompanyMasterComponent, canActivate: [authGuard, RoleGuard], data: { screenCode: 'COMPANY' }},
    { path: 'userMaster', component: UserMasterComponent, canActivate: [authGuard, RoleGuard], data: { screenCode: 'USER' }},
    { path: 'roleMaster', component: RoleComponent, canActivate: [authGuard, RoleGuard], data: { screenCode: 'ROLE' }},
    { path: 'roleAccessMaster', component: RoleAccessComponent, canActivate: [authGuard], data: { screenCode: 'ROLEACCESS' }},
    { path: 'bankMaster', component: BankMasterComponent, canActivate: [authGuard, RoleGuard], data: { screenCode: 'BANK' }},
];
