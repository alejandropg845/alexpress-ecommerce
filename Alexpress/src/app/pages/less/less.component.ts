import { Component, OnInit } from '@angular/core';

import { ProductService } from '../../services/product.service';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { ToastrService } from 'ngx-toastr';
import { Product } from '../../models/product.interface';
import { ProductThumbnail } from '../../models/product-thumbnail.interface';

@Component({
  selector: 'app-less',
  templateUrl: './less.component.html',
  styles: ``
})
export class LessComponent implements OnInit{

  products: ProductThumbnail[] = [];

  getLessProducts() {

    this.productService.products$
    .subscribe(products => this.products = products);
  }

  constructor(private productService:ProductService, private toastr:ToastrService){}
  
  ngOnInit(): void {
    this.productService.getProductsWithFilter(0, null, 4)
    .subscribe({
      next: products => this.products = products,
      error: err => handleBackendErrorResponse(err, this.toastr)
    })
  }

}
