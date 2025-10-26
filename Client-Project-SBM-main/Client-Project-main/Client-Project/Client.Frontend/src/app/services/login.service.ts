import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { AuthResponse, JwtClaims, Login } from '../components/login/login-dtos';
import { apiUrl } from '../../constant';
import { Router } from '@angular/router';
import { AlertService } from './alert.service';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  constructor(
    private route: Router,
    private alert: AlertService,
    private http: HttpClient
  ) { }

  login(formData: Login): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${apiUrl}/User/login`, formData);
  }

  logout() {
    sessionStorage.clear();
    this.route.navigate(['/'])
    this.alert.Toast.fire('Logged Out Successfully', '', 'success');
  }

  isLoggedIn(): boolean {
    return !!sessionStorage.getItem('token');
  }

  role() {
    const token = sessionStorage.getItem('token')
    if (token) {
      const decodedToken: JwtClaims = jwtDecode(token)
      if (decodedToken.role) {
        return decodedToken.role
      }
    }
    return null;
  }

  user() {
    const token = sessionStorage.getItem('token')
    if (token) {
      const decodedToken: JwtClaims = jwtDecode(token)
      if (decodedToken.user) {
        return decodedToken.user
      }
    }
    return null;
  }

  userId() {
    const token = sessionStorage.getItem('token')
    if (token) {
      const decodedToken: JwtClaims = jwtDecode(token)
      if (decodedToken.userId) {
        return decodedToken.userId
      }
    }
    return null;
  }

  companyId() {
    const token = sessionStorage.getItem('token')
    if (token) {
      const decodedToken: JwtClaims = jwtDecode(token)
      if (decodedToken.CompanyID) {
        return decodedToken.CompanyID
      }
    }
    return null;
  }

  email() {
    const token = sessionStorage.getItem('token')
    if (token) {
      const decodedToken: JwtClaims = jwtDecode(token)
      if (decodedToken.Email) {
        return decodedToken.Email
      }
    }
    return null;
  }
}
