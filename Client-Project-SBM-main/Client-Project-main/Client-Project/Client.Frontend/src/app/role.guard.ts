import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { firstValueFrom, Observable } from 'rxjs';
import { RoleAccessService } from './services/role-access.service';
import { LoginService } from './services/login.service';
import { RoleAccessDto } from './components/role-access/role-access-dto';
import { AlertService } from './services/alert.service';

@Injectable({
    providedIn: 'root'
})
export class RoleGuard implements CanActivate {

    constructor(
        private alert: AlertService,
        private loginService: LoginService, 
        private authService: RoleAccessService, 
        private router: Router) { }

    async canActivate(
        next: ActivatedRouteSnapshot,
        state: RouterStateSnapshot): Promise<boolean> {
        const screenCode = next.data['screenCode'];
        const accessList: RoleAccessDto[] = this.authService.getAccessList();
        if (accessList.length > 0) {
            const hasAccess = accessList.some((item) => item.a_screenCode === screenCode && item.a_viewAccess);
            if (!hasAccess) {
                this.router.navigate(['/home']);
                return false;
            }
            else {
                return true;
            }
        }
        else {
            const userName = this.loginService.user();
            if (userName) {
                let hasAccess = false
                await firstValueFrom(this.authService.getUserAccess(userName)).then((response: RoleAccessDto[]) => {
                    this.authService.roleGuardSetAccessList(response);
                    hasAccess = response.some((item) => item.a_screenCode === screenCode && item.a_viewAccess);
                }).catch((error) => {
                    this.alert.Toast.fire((error.error)?error.error:((error.message)?error.message:'Something went wrong'),'','error');
            console.error(error);
                })
                if (!hasAccess) {
                    this.router.navigate(['/home']);
                    return false;
                }
                else {
                    return true;
                }
            }
            else {
                this.loginService.logout();
                return false;
            }
        }
    }
}
