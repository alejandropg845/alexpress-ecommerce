import { Component, OnDestroy, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { AddProductComponent } from "../add-product/add-product.component";
import { ActivatedRoute } from '@angular/router';
import { Subject, switchMap, takeUntil } from 'rxjs';
import { Product } from '../../models/product.interface';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-edit-product',
  templateUrl: './edit-product.component.html',
  styles: ``,

})
export class EditProductComponent implements OnInit{
  
  product: Product | null = null;

  getProduct(){
    this.activatedRoute.params
    .pipe(
      switchMap(({id}) => this.productService.getProductToUpdate(id)),
    ).subscribe({
      next: product => this.product = product,
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  
  }

  constructor(
    private productService:ProductService, 
    private activatedRoute:ActivatedRoute,
    private toastr:ToastrService
  ){}

  ngOnInit(): void {
    this.getProduct();
  }
  
}
