import { AfterViewInit, ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';

import { Subject, switchMap, takeUntil } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../models/product.interface';
import { ProductService } from '../../services/product.service';
import { WishlistService } from '../../services/wishlist.service';
import { CartService } from '../../services/cart.service';
import { AddToCartDto } from '../../dtos/cart/addToCart.dto';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { ToastrService } from 'ngx-toastr';
import { ProductThumbnail } from '../../models/product-thumbnail.interface';


@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styles: ``
})
export class CategoriesComponent implements OnDestroy, OnInit{

  productsByCategory:ProductThumbnail[] = [];
  destroy$ = new Subject<void>();
  title: string = "";

  addToCart(productId:number, quantity: 1 | -1){

    const body:AddToCartDto = {
      customizedDiscount: 0,
      couponName: null,
      productId,
      quantity
    };

    this.cartService.addToCart(body)
    .subscribe({
      next: res => {

        let cart = this.cartService.getCurrentCartValue;

        if (!cart) cart = { 
          id: res.cartId, 
          cartProducts: [], 
          summary: res.summary
        };

        cart.cartProducts.push(res.cartProduct);

        this.cartService.setCartValue(cart);
      },
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  }

  addToWishList(productId:number){
    

    this.wishlistService.addToWishlist(productId)
    .subscribe({
      next: addedProduct => {

        let wishlist = this.wishlistService.getWishlistValue;

        if (!wishlist) wishlist = { 
          wishListProducts: []
        };

        wishlist.wishListProducts.push(addedProduct);

        this.wishlistService.setWishlistValue(wishlist);
      },
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  }

  getProductsByCategory(){

    this.activatedRoute.params
    .pipe(
      switchMap(({id}) => {
        this.productService.getProducts(id, null, 0);
        return this.productService.products$;
      }),
      takeUntil(this.destroy$)
    )
    .subscribe(products => {
      this.productsByCategory = products;
      this.title = products[0]?.category;
      
    });
  }

  constructor(private wishlistService:WishlistService,
              private productService:ProductService,
              private cartService:CartService, 
              private activatedRoute:ActivatedRoute,
              private toastr:ToastrService){}
  
  ngOnInit(): void {
    
    this.getProductsByCategory();

    
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

}
