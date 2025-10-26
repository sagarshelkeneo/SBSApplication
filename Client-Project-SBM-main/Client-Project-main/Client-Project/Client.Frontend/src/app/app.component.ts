import { Component, OnInit } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { AlertService } from './services/alert.service';
import { LoginService } from './services/login.service';
import { SideBarComponent } from "./components/side-bar/side-bar.component";
import { RoleAccessService } from './services/role-access.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterModule, SideBarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Client.Frontend';
  userName: string | null = '';

  constructor(private loginService: LoginService, private roleAccessService: RoleAccessService) { }

  checkLogin = (): boolean => {
    if (sessionStorage.getItem('token') != null) {
      this.userName = this.loginService.user();
      return true;
    }
    return false;
  };

  logoutUser() {
    this.loginService.logout();
  }
}
