import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LoginService } from '../../services/login.service';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  userRole: string | null = '';
  constructor(
    private loginService: LoginService,
  ) { }
  ngOnInit(): void {
    this.userRole = this.loginService.role();
    if (!this.userRole) {
      this.loginService.logout();
    }
  }
  checkReportAccess = (): boolean => (this.userRole === 'Super Admin' || this.userRole === 'Admin' || this.userRole === 'Manager' || this.userRole === 'report_user');
}
