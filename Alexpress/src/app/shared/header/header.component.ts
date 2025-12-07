import { Component, ElementRef, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Subject, takeUntil } from 'rxjs';
import { Product } from '../../models/product.interface';
import { Cart } from '../../models/cart.interface';
import { UserService } from '../../services/user.service';
import { CartService } from '../../services/cart.service';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { WishlistService } from '../../services/wishlist.service';
import { AddToCartDto } from '../../dtos/cart/addToCart.dto';
import { addToCartF } from '../../utils/add-to-cart';
import { Router } from '@angular/router';
import { DeleteFromCart } from '../../utils/delete-from-cart';
import { Wishlist } from '../../models/wishlist.interface';
import { ProductService } from '../../services/product.service';
import { ProductThumbnail } from '../../models/product-thumbnail.interface';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styles: ``
})
export class HeaderComponent implements OnInit, OnDestroy{

  username: string | null = null;
  products:ProductThumbnail[] = [];
  cart:Cart | null = null;
  showDropMenu: boolean = false;
  isTouchDevice: boolean = ('ontouchstart' in window) || (navigator.maxTouchPoints > 0);
  destroy$ = new Subject<void>();
  showCart: boolean = false;
  wishlist: Wishlist | null = null;
  showFilteredProducts: boolean = false;

  getUsername() {
    this.userService.getUsername();
    this.userService.getUsername$
    .subscribe(username => this.username = username);
  }

  scrollToBottom(): void{
    this.showDropMenu = false;
    const height = document.body.scrollHeight;
    window.scrollTo({top:height, behavior:'smooth'});
  }

  filterProducts(input: string){

    if (!input) {
      this.products = [];
      return;
    }

    setTimeout(() => {
      
      this.productService.getProductsWithFilter(0, input, 0)
      .subscribe({
        next: products => this.products = products,
        error: err => handleBackendErrorResponse(err, this.toastr)
      })

    }, 600);
  }

  getCart(){
    if (!localStorage.getItem('alexpressAccessToken')) return;
    this.cartService.getCart();
    this.cartService.getCart$
    .pipe(
      takeUntil(this.destroy$)
    ).subscribe(res => this.cart = res);
  }

  showList: boolean = false;

  showListt(){
    this.showList = !this.showList;
  }

  getCartLength(){
    return this.cart ? this.cart.cartProducts.length
    : 0;
  }

  goToCart(){
    if (this.isTouchDevice) {
      this.router.navigate(["alexpress/cart"]);
      this.showCart = false;
    }
  }

  addToCart(quantity: 1 | -1, productId: number, couponName: string, customizedDiscount: number) {


    /* Si este metodo llega a ejecutarse, quiere decir que siempre va a existir un carrito y el producto.
    De esta manera, ponemos los valores de los Coupons por default en false porque estos no serán tomados cuando
    lo unico que se quiere cambiar es la cantidad del producto ya agregado. Solo importarían los valores de Coupon
    sólo si el producto no existiera en el carrito y por ende se está agregando, pero en este caso y como se mencionaba,
    este método siempre se ejecuta cuando existe el producto. */
    const body:AddToCartDto = {
      customizedDiscount,
      couponName,
      productId,
      quantity
    }

    this.cartService.addToCart(body)
    .subscribe({
      next: res => addToCartF(this.cartService, res, productId),
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  }

  removeFromCart(productId: number){
    this.cartService.removeFromCart(productId)
    .subscribe({
      next: r => DeleteFromCart(this.cartService, productId, r),
      error: err => handleBackendErrorResponse(err, this.toastr)
    })
  }

  logOut(): void{
    localStorage.removeItem('alexpressAccessToken');
    localStorage.removeItem('alexpressRefreshToken');
    this.username = null;
    this.cartService.setCartValue(null);
    this.wishList.setWishlistValue(null);
  }

  goToPayment(){
    this.router.navigate(["/alexpress/payment"]);
    this.showCart = false;
  }

  getWishList(){
    this.wishlistService.getWishList();
    this.wishlistService.getWishlist$
    .subscribe({
      next: wishlist => this.wishlist = wishlist,
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  }
  
  constructor(
    private userService:UserService, 
    private cartService:CartService, 
    private toastr:ToastrService,
    private wishList:WishlistService,
    private router:Router,
    private wishlistService:WishlistService,
    private productService:ProductService
  ){}

  ngOnInit(): void {
    if (localStorage.getItem('alexpressAccessToken')){
      this.getUsername();
      this.getCart();
      this.getWishList();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

}
