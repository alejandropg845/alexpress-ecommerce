import { Component, OnDestroy, OnInit } from '@angular/core';
import { WishlistService } from '../../services/wishlist.service';
import { CartService } from '../../services/cart.service';
import { AddToCartDto } from '../../dtos/cart/addToCart.dto';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { ToastrService } from 'ngx-toastr';
import { Wishlist } from '../../models/wishlist.interface';
import { addToCartF } from '../../utils/add-to-cart';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-wish-list',
  templateUrl: './wish-list.component.html',
  styles: ``
})
export class WishListComponent implements OnInit{

  wishList:Wishlist | null = null;
  destroy$ = new Subject<void>();

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

  removeFromWishlist(id:number){
    this.wishlistService.removeFromWishList(id)
    .subscribe({
      next: _ => {

        const index = this.wishList!.wishListProducts.findIndex(p => p.productId === id)!;

        this.wishList!.wishListProducts.splice(index, 1);

      }
    });
  }

  getWishlist(){
    this.wishlistService.getWishlist$
    .pipe(takeUntil(this.destroy$))
    .subscribe(wishlist => this.wishList = wishlist);
  }
  
  constructor(private wishlistService:WishlistService, private cartService:CartService, private toastr:ToastrService){}

  ngOnInit(): void {
    this.getWishlist();
  }

}
