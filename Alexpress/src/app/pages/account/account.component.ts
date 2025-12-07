import { Component, OnDestroy, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../../services/user.service';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styles: ``
})
export class AccountComponent implements OnInit, OnDestroy{

  destroy$ = new Subject<void>();
  showCurrentPassword: boolean = false;
  showPass1: boolean = false;
  showPass2: boolean = false;
  username: string | null = null;
  showQr: boolean = false;
  qrValue: string | null = null;
  recoveryCodes: string[] = [];
  closeRecoveryCodes: boolean = false;
  isTwoFactorEnabled: boolean = false;
  isGetKeyClicked: boolean = false;

  changePassword(currentPassword: HTMLInputElement, pass1: HTMLInputElement, pass2: HTMLInputElement) {

    if (!currentPassword.value || !pass1.value || !pass2.value) {
      this.toastr.show("There are password fields missing");
      return;
    }

    if (pass1.value !== pass2.value) {
      this.toastr.show("Passwords don't match");
      return;
    }

    this.userService.changePasswordInApp(currentPassword.value, pass1.value, pass2.value)
    .subscribe({
      next: res => {
        this.toastr.success(res.message);
        currentPassword.value = "";
        pass1.value = "";
        pass2.value = "";
      },
      error: err => handleBackendErrorResponse(err, this.toastr)
    });

  }

  get2FAKey() {

    if (this.isTwoFactorEnabled) return;

    if (this.isGetKeyClicked) return;

    this.isGetKeyClicked = true;

    this.userService.get2FAKey()
    .subscribe({
      next: res => {
        this.showQr = true;
        this.qrValue = res.key;
      },
      error: err => handleBackendErrorResponse(err, this.toastr)
    });

  }

  set2FAauth(code: string) {

    if (this.isTwoFactorEnabled) return;

    if (!code) {
      this.toastr.error("You must type the 6-digits code");
      return;
    }

    if (code.length !== 6) {
      this.toastr.error("Code must have 6 digits");
      return;
    }

    this.userService.set2FAauthentication(code)
    .subscribe({
      next: res => {
        this.recoveryCodes = res.recoveryCodes;
        this.isTwoFactorEnabled = true;
        this.showQr = false;
        this.qrValue = null;
      },
      error: err => handleBackendErrorResponse(err, this.toastr)
    })

  }

  getUsername() {
    this.userService.getUsername$
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: username => this.username = username,
      error: err => handleBackendErrorResponse(err, this.toastr)
    })
  }

  getTwoFactorEnabled() {
    this.userService.getTwoFactorEnabled()
    .subscribe({
      next: res => this.isTwoFactorEnabled = res.isTwoFactorEnabled,
      error: err => handleBackendErrorResponse(err, this.toastr)
    })
  }

  constructor(private toastr:ToastrService, private userService:UserService){}

  ngOnInit(): void {
    this.getUsername();
    this.getTwoFactorEnabled();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

}
