import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ReactiveFormsModule } from '@angular/forms';
import { AuthRoutingModule } from './auth.routes';
import { MainAuthComponent } from './main-auth/main-auth.component';
import { RouterOutlet } from '@angular/router';



@NgModule({
  declarations: [LoginComponent, RegisterComponent, MainAuthComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AuthRoutingModule,
    RouterOutlet,
  ],
  exports:[AuthRoutingModule]
})
export class AuthModule { }
