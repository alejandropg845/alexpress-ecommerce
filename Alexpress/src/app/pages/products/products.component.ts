import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { finalize, Subject, takeUntil } from 'rxjs';
import { LocationStrategy } from '@angular/common';
import { Router } from '@angular/router';
import { Product } from '../../models/product.interface';
import { ProductService } from '../../services/product.service';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { ToastrService } from 'ngx-toastr';
import { AddToCartDto } from '../../dtos/cart/addToCart.dto';
import { CartService } from '../../services/cart.service';
import { WishlistService } from '../../services/wishlist.service';
import { addToCartF } from '../../utils/add-to-cart';
import { UserService } from '../../services/user.service';
import { ProductThumbnail } from '../../models/product-thumbnail.interface';
import { AddToWishlist } from '../../utils/add-to-wishlist';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styles:
    `
  .css-layout {
    max-width: 256px;
    width: 160px;
  }

  @media (min-width:410px){
    .css-layout {
      width: 180px;
    }
  }

  @media (min-width:500px){
    .css-layout {
      width:220px;
    }
  }

  @media (min-width:610px){
    .css-layout {
      width:260px;
    }
  }

  @media (min-width:768px){
    .css-layout {
      width: 235px;
    }
  }

  @media (min-width:1024px){
    .css-layout {
      width: 235px;
    }
  }

  `
})
export class ProductsComponent implements OnInit, OnDestroy{

  @Input() title: string = "All products";
  destroy$ = new Subject<void>();
  @Input() products: ProductThumbnail[] = [];
  username: string | null = null;

  addToCart(productId:number){
  
    const body:AddToCartDto = {
      customizedDiscount: 0,
      couponName: null,
      productId,
      quantity: 1
    };

    this.cartService.addToCart(body)
    .subscribe({
      next: r => addToCartF(this.cartService, r, productId),
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  }

  addToWishList(productId: number) {
    this.wishlistService.addToWishlist(productId)
    .subscribe({
      next: product => AddToWishlist(this.wishlistService, product) 
      ,
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  }

  sortingPrice: boolean | null = null;
  sortingRating: boolean | null = null;
  sortingOrders: boolean | null = null;

  calculateStarRating(product: ProductThumbnail) {
    return product.accumulated / product.votes;
  }

  orderBy(value: string) {
    switch (value) {

      case 'none':
        this.products.sort((a, b) => a.id - b.id);
        break;

      case 'price':
        this.sortingPrice = !this.sortingPrice;
        (this.sortingPrice) ? this.products.sort((a, b) => b.price - a.price)
          : this.products.sort((a, b) => a.price - b.price);
        break;

      case 'rating':
        this.sortingRating = !this.sortingRating;
        (this.sortingRating) ? this.products.sort((a, b) => b.accumulated - a.accumulated)
          : this.products.sort((a, b) => a.accumulated - b.accumulated);
        break;

      case 'orders':
        this.sortingOrders = !this.sortingOrders;
        (this.sortingOrders) ? this.products.sort((a, b) => b.sold - a.sold)
          : this.products.sort((a, b) => a.sold - b.sold);
        break;
    }
  }

  getUsername(){
    this.userService.getUsername$
    .subscribe(username => this.username = username);
  }

  getProducts() {

      this.productService.products$
      .pipe(takeUntil(this.destroy$))
      .subscribe(products => this.products = products);
  }

  constructor(
    private productService: ProductService,
    public router: Router,
    private toastr:ToastrService,
    private cartService:CartService,
    private wishlistService:WishlistService,
    private userService:UserService
  ) { }

  ngOnInit(): void {
    this.productService.getProducts(0, null, 0);
    this.getProducts();
    this.getUsername();
  };

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

}
