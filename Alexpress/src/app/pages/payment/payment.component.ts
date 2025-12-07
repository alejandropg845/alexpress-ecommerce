import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnDestroy, OnInit, QueryList, ViewChildren } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';



import { ToastrService } from 'ngx-toastr';

import { Router } from '@angular/router';

import { Subject, takeUntil } from 'rxjs';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { environment } from '../../../environments/environment';
import { Cart, CartProduct } from '../../models/cart.interface';
import { Address } from '../../models/model.interface';
import { AddressService } from '../../services/address.service';
import { CartService } from '../../services/cart.service';
import { AddToCartDto } from '../../dtos/cart/addToCart.dto';
import { addToCartF as AddToCart } from '../../utils/add-to-cart';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styles: ``
})
export class PaymentComponent implements OnInit{

  addresses:Address[] = [];
  cart:Cart | null = null;
  destroy$ = new Subject<void>();


  changeQuantity(productId:number, quantity: -1 | 1){
    
    const body:AddToCartDto = { 
      customizedDiscount: 0,
      couponName: null,
      productId,
      quantity
    };
    
    this.cartService.addToCart(body)
    .subscribe({
      next: res => AddToCart(this.cartService, res, productId),
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
    
  }

  getCart() {
    this.cartService.getCart$
    .pipe(takeUntil(this.destroy$))
    .subscribe(cart => this.cart = cart);
  }
  
  getUserAddresses(){
    this.addressService.getAddresses()
    .subscribe({
      next: addresses => this.addresses = addresses,
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  }

  onSubmit() {

  }

  addressId: number = 0;

  @ViewChildren('addressButton') addressButtonsRef!:QueryList<ElementRef>;

  onSelectedAddress(addressId: number, reference:HTMLButtonElement) {

    reference.classList.add("bg-cyan-300")

    this.addressId = addressId;
  }

  clearSelection() {
    this.addressButtonsRef.forEach(buttonRef => {
      const addressButton = buttonRef.nativeElement as HTMLButtonElement;
      addressButton.classList.remove("bg-cyan-300");
    });
    this.addressId = 0;
  }

  isButtonPressed: boolean = false;

  redirectToStripe() {

    if (this.isButtonPressed) return;

    if (!this.addressId) {
      this.toastr.info("You must specify an address");
      return;
    }

    if (!this.cart || this.cart.cartProducts.length === 0) {
      this.toastr.info("You haven't added any products yet");
      return;
    }

    this.orderService.summarizeOrder(this.addressId)
    .subscribe({
      next: res => window.location.href = res.sessionUrl,
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  }

  constructor(
    private addressService: AddressService, 
    private toastr: ToastrService,
    private cartService: CartService,
    private orderService:OrderService
  ){}


  ngOnInit(): void {
    this.getUserAddresses();
    this.getCart();
  }


}
