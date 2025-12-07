import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-recover-password',
  templateUrl: './recover-password.component.html',
  styles: ``
})
export class RecoverPasswordComponent implements OnInit{

  emailValidator = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";

  isRequestSent: boolean = false;
  isBeingRequested: boolean = false;
  isToken: boolean = false;

  showPassword1: boolean = false;
  showPassword2: boolean = false;

  email!: string;
  token!: string;

  onSubmit(email: string) {

    if (!email) {
      this.toastr.info("You must provide an email");
      return;
    }

    if (!new RegExp(this.emailValidator).test(email)) {
      this.toastr.info("That doesn't look like an email");
      return;
    }

    if (this.isBeingRequested) return;

    this.isBeingRequested = true;
    
    this.userService.requestPasswordReset(email)
    .pipe(finalize(() => this.isBeingRequested = false))
    .subscribe({
      next: _ => this.isRequestSent = true,
      error: err => handleBackendErrorResponse(err, this.toastr)
    })

  }

  checkIftoken(){
    const token = this.activatedRoute.snapshot.params["token"];
    const email = this.activatedRoute.snapshot.queryParams["email"];

    if (token && email) {
      this.token = token;
      this.email = email;
      this.isToken = true;
    }
  }

  changePassword(password1: string, password2: string) {

    if (!new RegExp(this.emailValidator).test(this.email)){
      this.toastr.info("You must provide a valid email");
      return;
    }

    if (!password1 || !password2) {
      this.toastr.info("You must provide a password");
      return;
    }

    if (password1 !== password2) {
      this.toastr.info("Passwords don't match");
      return;
    }

    if (!this.token){
      this.toastr.info("Token must have a valid value");
      return;
    }

    const body = {
      email: this.email,
      token: this.token,
      password1,
      password2
    };

    this.userService.changePassword(body)
    .subscribe({
      next: _ => this.toastr.success("Changed successfully"),
      error: err => handleBackendErrorResponse(err, this.toastr)
    })

  }

  constructor(private userService:UserService, 
    private toastr:ToastrService, 
    private activatedRoute:ActivatedRoute
  ){}

  ngOnInit(): void {
    this.checkIftoken();
  }

}
