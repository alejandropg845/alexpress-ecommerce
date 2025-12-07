import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { ToastrService } from 'ngx-toastr';
import { Product } from '../../models/product.interface';
import { ProductThumbnail } from '../../models/product-thumbnail.interface';

@Component({
  selector: 'app-my-products',
  templateUrl: './my-products.component.html',
  styles: ``
})
export class MyProductsComponent implements OnInit{

  products:ProductThumbnail[] = [];

  getUserProducts() {
    this.productService.getUserProducts()
    .subscribe({
      next: products => this.products = products,
      error: err => handleBackendErrorResponse(err, this.toastr)
    })
  }

  removeProduct(productId: number){

    this.productService.deleteProduct(productId)
    .subscribe({
      next: _ => {
        
        const index = this.products.findIndex(p => p.id === productId);

        this.products.splice(index, 1);

      },
      error: err => handleBackendErrorResponse(err, this.toastr)
    });

  }

  constructor(
    private productService:ProductService,
    private toastr:ToastrService
  ){}

  ngOnInit(): void {
    this.getUserProducts();
  }

}
