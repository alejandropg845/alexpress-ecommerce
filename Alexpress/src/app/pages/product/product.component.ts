import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subject, switchMap, takeUntil } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { WishlistService } from '../../services/wishlist.service';
import { Product } from '../../models/product.interface';
import { AddToCartDto } from '../../dtos/cart/addToCart.dto';
import { addToCartF as AddToCart } from '../../utils/add-to-cart';
import { AddToWishlist } from '../../utils/add-to-wishlist';
import { DeleteFromCart } from '../../utils/delete-from-cart';
import { animate, style, transition, trigger } from '@angular/animations';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styles: ``,
  animations: [
    trigger('fadeInOut', [
      transition(':enter', [
        style({ opacity: 0 }),
        animate('300ms ease-out', 
          style({ opacity: 1 }))
      ]),
      transition(':leave', [
        animate('200ms ease-in', 
          style({ opacity: 0 }))
      ])
    ])
  ]
})
export class ProductComponent implements OnInit, OnDestroy{


  quantity: number = 1;
  couponName: string | null = null;
  description:boolean = true;
  comments:boolean = false;
  product:Product | null = null;
  destroy$ = new Subject<void>();


  is50DiscountSelected: boolean = false;
  is50OffOneProductSelected: boolean = false;
  isFreeShippingSelected: boolean = false;
  customizedDiscount: number = 0;


  addQuantity(num:number){
    this.quantity += num;
    if(this.quantity === 0){
      this.quantity = 1;
    }
  }

  descriptionClick(){
    this.description = true;
    this.comments = false
  }

  commentsClick(){
    this.description = false;
    this.comments = true;
  }

  addToWishlist(productId:number){
    this.wishlistService.addToWishlist(productId)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: res => AddToWishlist(this.wishlistService, res),
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  }

  addToCart(productId:number){

    if (this.quantity <= 0){
      return;
    }

    if (this.couponName === 'is50OffOneProduct' && this.quantity < 2) {
      this.toastr.info("The selected coupon requires 2 or more units");
      return;
    }

    const body:AddToCartDto = {
      customizedDiscount: this.customizedDiscount,
      couponName: this.couponName,
      productId: productId,
      quantity: this.quantity
    };

    this.cartService.addToCart(body)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: res => AddToCart(this.cartService, res, productId),
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  }

  calculateStarRating(product:Product){
    return product.accumulated / product.votes;
  }

  getProduct() {
    this.product = null;
    this.activatedRoute.params
    .pipe(
      switchMap(({id}) => this.productService.getProduct(id)))
    .subscribe({
      next: product => {
        this.product = product;
      },
      error: err => {
        handleBackendErrorResponse(err,this.toastr);
      }
    });

  }


  onSelectedCoupon(coupon: string) {
    
    /* Quitar coupon cuando se vuelve a seleccionar el mismo */
    if (this.couponName || this.customizedDiscount) {
      this.is50DiscountSelected = false;
      this.is50OffOneProductSelected = false;
      this.isFreeShippingSelected = false;
      this.customizedDiscount = 0;
      this.couponName = null;
      return;
    }

    /* Seleccionar customizedDiscount */
    if (coupon !== 'is50OffOneProduct'
      && coupon !== 'isFreeShipping'
      && coupon !== 'is50Discount'
    ) {

      this.couponName = null;
      this.customizedDiscount = Number(coupon);
      return;
    }

    this.couponName = coupon;

    /* Estilizar cupon seleccionado */
    if (this.couponName === 'is50Discount')
      this.is50DiscountSelected = true;
    else if (this.couponName === 'isFreeShipping')
      this.isFreeShippingSelected = true;
    else if (this.couponName === 'is50OffOneProduct')
      this.is50OffOneProductSelected = true;

  }

  

  removeFromCart(productId: number) {

    this.cartService.removeFromCart(productId)
    .subscribe({
      next: res => DeleteFromCart(this.cartService, productId, res),
      error: err => handleBackendErrorResponse(err, this.toastr)
    })

  }

  showImage: boolean = false;

  openImage(event: Event) {

    this.showImage = true;
    event.preventDefault();
  }

  closeImage(){
    this.showImage = false;
    this.imageIndex = 0;
  }

  imageIndex: number = 0;

  nextImage(value: -1 | 1){

    if (value === 1 && this.imageIndex === this.product!.images.length - 1) {
      this.imageIndex = 0;
      return;
    }
    
    this.imageIndex += value;


  }

  constructor(private activatedRoute:ActivatedRoute, 
              private productService:ProductService,
              private toastr:ToastrService,
              private cartService:CartService,
              private wishlistService:WishlistService){}

  ngOnInit(): void {
    this.getProduct();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
