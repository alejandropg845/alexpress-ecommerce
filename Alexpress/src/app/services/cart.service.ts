import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { AddToCartDto } from '../dtos/cart/addToCart.dto';
import { Product } from '../models/product.interface';
import { BehaviorSubject } from 'rxjs';
import { Cart, CartProduct } from '../models/cart.interface';
import { handleBackendErrorResponse } from '../utils/error-handler';
import { ToastrService } from 'ngx-toastr';
import { AddToCartResponse } from '../responses/addToCart.response';

@Injectable({providedIn: 'root'})

export class CartService {

    cart = new BehaviorSubject<Cart | null>(null);

    get getCart$() {
        return this.cart.asObservable();
    }

    get getCurrentCartValue() {
        return this.cart.value;
    }

    setCartValue(cart: Cart | null){
        this.cart.next(cart);
    }

    

    getCart() {
        this.http.get<Cart | null>(environment.cartUrl)
        .subscribe({
            next: res => this.cart.next(res),
            error: err => handleBackendErrorResponse(err, this.toastr)
        });
    }

    addToCart = (body: AddToCartDto) => this.http.post<AddToCartResponse>(environment.cartUrl, body);

    removeFromCart = (productId: number) => this.http.delete<{summary: number}>(`${environment.cartUrl}/${productId}`);

    constructor(private readonly http:HttpClient, private toastr:ToastrService) { }
    
}