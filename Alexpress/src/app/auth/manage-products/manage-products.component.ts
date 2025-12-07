import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { firstValueFrom, Subject, takeUntil } from 'rxjs';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { ConfirmDialogService } from '../../services/confirm-dialog.service';
import { Product } from '../../models/product.interface';
import { ProductService } from '../../services/product.service';
import { AdminService } from '../../services/admin.service';
import { UserDto } from '../../dtos/user/user.dto';

@Component({
  selector: 'app-manage-products',
  templateUrl: './manage-products.component.html',
  styles: ``
})
export class ManageProductsComponent implements OnInit{

  products:Product[] = [];
  deletedProducts: Product[] = [];
  disabledUsers: UserDto[] = [];
  users: UserDto[] = [];

  getProducts(){
    this.adminService.getProducts()
    .subscribe({
      next: products => {
        this.products = products.filter(p => !p.isDeleted);
        this.deletedProducts = products.filter(p => p.isDeleted);
      },
      error: err => handleBackendErrorResponse(err, this.toastr)
    })
  }

  getUsers(){
    this.adminService.getUsers()
    .subscribe({
      next: users => {
        this.users = users.filter(u => !u.isDisabled);
        this.disabledUsers = users.filter(u => u.isDisabled);
      },
      error: err => handleBackendErrorResponse(err, this.toastr)
    })
  }

  disableUser(user: UserDto){ 
    this.adminService.disableUser(user.id)
    .subscribe({
      next: _ => { 
        
        const index = this.users.findIndex(u => u.id === user.id);

        this.users.splice(index, 1);

        this.disabledUsers.push(user);

      },
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  }

  removeProduct(product: Product){
    this.adminService.deleteProduct(product.id)
    .subscribe({
      next: res => {

        const index = this.products.findIndex(p => p.id === product.id);

        this.products.splice(index, 1);

        this.deletedProducts.push(product);

      },
      error: err => handleBackendErrorResponse(err, this.toastr)
    })
  }

  constructor(private adminService:AdminService, private toastr:ToastrService) {}

  ngOnInit(): void {
    this.getUsers();
    this.getProducts();
  }

  ngOnDestroy(){
    
  }

}
