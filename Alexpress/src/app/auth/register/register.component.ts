import { Component, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../../services/user.service';
import { finalize, Subject, takeUntil, tap } from 'rxjs';
import { handleBackendErrorResponse } from '../../utils/error-handler';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styles: ``
})
export class RegisterComponent implements OnDestroy{

  emailValidator = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
  registerForm:FormGroup;
  isPasswordVisible:boolean = false;
  isLoading:boolean = false;
  destroy$ = new Subject<void>();

  register(){

    if (!this.registerForm.valid){

      this.registerForm.markAllAsTouched();
      this.toastr.info("There are missing fields");
      return;

    }

    this.isLoading = true;

    this.userService.register(this.registerForm)
    .pipe(
      takeUntil(this.destroy$),
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: res => this.toastr.success(res.message),
      error: err => handleBackendErrorResponse(err, this.toastr)
    })
  }

  randomizedAccount() {

    let username = '';
    const characters = "ABCDEFGHIJKLMNOPQRSTVWXYZabcdefghiklmnopqrstvwxyz123456789";
    const charactersLength = characters.length;

    for (let i = 0; i < 5; i++) {
        username += characters.charAt(Math.floor(Math.random() * charactersLength));
    }

    const emailDomain = ["example.com", "test.com", "mail.com"];
    const randomDomain = emailDomain[Math.floor(Math.random() * emailDomain.length)];
    const email = `${username}${Math.floor(Math.random() * 100)}@${randomDomain}`;

    let password = '';
    const passwordLength = 8;
    for (let i = 0; i < passwordLength; i++) {
        password += characters.charAt(Math.floor(Math.random() * charactersLength));
    }

    this.registerForm.get('username')?.setValue(username);
    this.registerForm.get('email')?.setValue(email);
    this.registerForm.get('password')?.setValue(password);

  }

  validateField(field:string) {
    return (this.registerForm.get(field)?.touched && !this.registerForm.get(field)?.valid) 
    ? 'border-2 border-red-500' 
    : 'border-black';
  }

  showPassword = () => this.isPasswordVisible = !this.isPasswordVisible;

  constructor(
    private fb:FormBuilder,
    private toastr:ToastrService,
    private router:Router,
    private userService:UserService
  ){

    this.registerForm = this.fb.group({

      email: [null, [Validators.required, Validators.pattern(this.emailValidator), Validators.maxLength(60)]],
      username: [null, [Validators.required, Validators.maxLength(20)]],
      password: [null, Validators.required]

    });

  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
