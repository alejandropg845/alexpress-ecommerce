import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { Cart } from '../../models/cart.interface';
import { CartService } from '../../services/cart.service';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { WishlistService } from '../../services/wishlist.service';
import { AddToWishlist } from '../../utils/add-to-wishlist';
import { AddToCartDto } from '../../dtos/cart/addToCart.dto';
import { addToCartF } from '../../utils/add-to-cart';
import { DeleteFromCart } from '../../utils/delete-from-cart';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styles: ``
})
export class CartComponent implements OnInit, OnDestroy{

  cart:Cart | null = null;
  summary!:number;
  destroy$ = new Subject<void>();
  token = localStorage.getItem('alexpressAccessToken');

  getUserCart(){
    this.cartService.getCart();
    this.cartService.getCart$
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: cart => this.cart = cart,
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  }

  deleteCartProduct(productId:number){
    this.cartService.removeFromCart(productId)
    .subscribe({
      next: res => DeleteFromCart(this.cartService, productId, res),
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  }

  addToWishList(productId:number){
    this.wishlistService.addToWishlist(productId)
    .subscribe({
      next: res => AddToWishlist(this.wishlistService, res),
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  }

  addToCart(productId: number, quantity: -1 | 1) {

    const body:AddToCartDto = {
      customizedDiscount: 0,
      couponName: null,
      productId,
      quantity
    };

    this.cartService.addToCart(body)
    .subscribe({
      next: r => addToCartF(this.cartService, r, productId),
      error: err => handleBackendErrorResponse(err, this.toastr)
    });

  }

  constructor(private cartService:CartService,
              private toastr:ToastrService,
              private wishlistService:WishlistService
  ){}

  ngOnInit(): void {
    this.getUserCart();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

}
