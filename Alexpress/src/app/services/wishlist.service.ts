import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Wishlist, WishlistItem } from '../models/wishlist.interface';
import { BehaviorSubject } from 'rxjs';
import { handleBackendErrorResponse } from '../utils/error-handler';
import { ToastrService } from 'ngx-toastr';

@Injectable({providedIn: 'root'})
export class WishlistService {

    wishlist = new BehaviorSubject<Wishlist | null>(null);
    
    get getWishlist$(){
        return this.wishlist.asObservable();
    }

    setWishlistValue(wishlist: Wishlist | null){
        this.wishlist.next(wishlist);
    }

    get getWishlistValue(){
        return this.wishlist.value;
    }

    getWishList() {
        this.http.get<Wishlist>(environment.wishlistUrl)
        .subscribe({
            next: res => this.wishlist.next(res),
            error: err => handleBackendErrorResponse(err, this.toastr)
        });
    }

    addToWishlist = (productId:number) => this.http.post<WishlistItem>(`${environment.wishlistUrl}/${productId}`, null); 
        
    removeFromWishList(productId: number) {
        return this.http.delete(`${environment.wishlistUrl}/deleteFromWishlist/${productId}`);
    }

    constructor(private readonly http:HttpClient, private toastr:ToastrService) { }
    
}