import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { FormGroup } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { handleBackendErrorResponse } from '../utils/error-handler';
import { ToastrService } from 'ngx-toastr';
import { LoginResponse } from '../responses/login.response';

@Injectable({providedIn: 'root'})

export class UserService {

    username$ = new BehaviorSubject<string | null>(null);

    get getUsername$(){
        return this.username$.asObservable();
    }

    setNullUsername = () => this.username$.next(null);

    login = (form:FormGroup) => this.httpClient.post<LoginResponse>(`${environment.userUrl}/login`, form.value);

    register = (form:FormGroup) => this.httpClient.post<{ message: string }>(`${environment.userUrl}/register`, form.value);
    
    getUsername() {
        this.httpClient.get<{ username: string }>(`${environment.userUrl}/credentials`)
        .subscribe({
            next: res => this.username$.next(res.username),
            error: err => handleBackendErrorResponse(err, this.toastr)
        });
    }

    requestPasswordReset(email: string) {
        return this.httpClient.post(`${environment.userUrl}/requestChangePassword`, {email});
    }

    changePassword(body: any) {
        return this.httpClient.post(`${environment.userUrl}/changePassword`, body);
    }

    verifyEmail(body: any) {
        return this.httpClient.post(`${environment.userUrl}/confirmEmail`, body);
    }

    changePasswordInApp(currentPassword: string, pass1: string, pass2: string) {

        const body = { currentPassword, pass1, pass2 };

        return this.httpClient.put<{ message: string }>(`${environment.userUrl}/changePasswordInApp`, body);
    }

    get2FAKey = () => this.httpClient.get<{ key:string }>(`${environment.userUrl}/get2FAKey`);

    set2FAauthentication = (code: string) => this.httpClient.post<{ recoveryCodes: string[] }>(`${environment.userUrl}/set2FA/${code}`, null);

    getTwoFactorEnabled = () => this.httpClient.get<{ isTwoFactorEnabled: boolean }>(`${environment.userUrl}/isTwoFactorEnabled`); 

    loginTwoFactor(code: string, partialToken: string) {

        const headers = new HttpHeaders().set("Authorization", "bearer "+partialToken);

        return this.httpClient.post<{ ok:boolean, accessToken: string, refreshToken: string }>(`${environment.userUrl}/loginTwoFactor/${code}`, null, { headers });
    }

    constructor(private readonly httpClient:HttpClient, private toastr:ToastrService) { }
    
}