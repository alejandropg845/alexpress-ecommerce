import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../../services/user.service';
import { handleBackendErrorResponse } from '../../utils/error-handler';

@Component({
  selector: 'app-email-confirmation',
  templateUrl: './email-confirmation.component.html',
  styles: ``
})
export class EmailConfirmationComponent implements OnInit {

  emailValidator = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
  isSucceeded: boolean = false;


  verifyEmail() {

    const token = this.activatedRoute.snapshot.params["token"];
    const email = this.activatedRoute.snapshot.queryParams["email"];

    if (!token) {
      this.toastr.error("Token cannot be null");
      return;
    }

    if (!new RegExp(this.emailValidator).test(email)){
      this.toastr.error("Invalid email");
      return;
    }

    const body = { token, email }

    this.userService.verifyEmail(body)
    .subscribe({
      next: _ => this.isSucceeded = true,
      error: err => handleBackendErrorResponse(err, this.toastr)
    });

  }
  
  constructor(private activatedRoute: ActivatedRoute, private toastr:ToastrService, private userService:UserService){}

  ngOnInit(): void {
    this.verifyEmail();
  }


}
