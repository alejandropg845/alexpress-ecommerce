import { Component, OnDestroy } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { finalize, Subject, takeUntil, tap } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styles: ``
})
export class LoginComponent implements OnDestroy{

  loginForm:FormGroup;
  isPasswordVisible:boolean = false;
  isLoading:boolean = false;
  destroy$ = new Subject<void>();
  show2Auth: boolean = false;
  partialToken: string | null = null;

  validateField(field:string) {
    return (this.loginForm.get(field)?.touched && !this.loginForm.get(field)?.valid) 
    ? 'border-2 border-red-500' 
    : 'border-black';
  }

  logIn() {

    if (!this.loginForm.valid) {

      this.loginForm.markAllAsTouched();
      this.toastr.info("There are missing fields");
      return;
    }

    this.isLoading = true;

    this.userService.login(this.loginForm)
    .pipe(
      takeUntil(this.destroy$),
      tap(r => {

        if (r.ok) this.redirectSuccess(r.accessToken, r.refreshToken);

      }),
      finalize(() => this.isLoading = false)
    )
    .subscribe({
      next: res => {

        if (res.isTwoFactorEnabled) {
          this.show2Auth = true;
          this.partialToken = res.partialToken;
        }
      },
      error: err => handleBackendErrorResponse(err, this.toastr)
    })

  }

  redirectSuccess(accessToken: string, refreshToken: string) {
    localStorage.setItem('alexpressAccessToken', accessToken);
    localStorage.setItem('alexpressRefreshToken', refreshToken)
    this.router.navigate(["alexpress/home"]);
  }

  onSubmitTwoFactor(code: string) {

    if (code.length !== 6) {
      this.toastr.info("Code must have 6 digits");
      return;
    }

    this.userService.loginTwoFactor(code, this.partialToken!)
    .pipe(tap(res => {
      if (res.ok) this.redirectSuccess(res.accessToken, res.refreshToken);
    }))
    .subscribe({
      next: _ => {
        this.show2Auth = false;
        this.partialToken = null;
      },
      error: err => handleBackendErrorResponse(err, this.toastr)
    })

  }

  showPassword = () => this.isPasswordVisible = !this.isPasswordVisible;

  constructor(
    private fb:FormBuilder, 
    private toastr:ToastrService, 
    private readonly userService:UserService,
    private router:Router  
  ){

    this.loginForm = this.fb.group({

      username: [null, [Validators.required, Validators.maxLength(20)]],
      password: [null, [Validators.required, Validators.maxLength(120), Validators.minLength(5)]]

    });

  }
  
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }


}
