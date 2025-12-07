import { WishlistItem } from "../models/wishlist.interface";
import { WishlistService } from "../services/wishlist.service";

export function AddToWishlist(wishlistService:WishlistService, product:WishlistItem) {

    let wishlist = wishlistService.getWishlistValue;

    if (!wishlist) wishlist = { wishListProducts: [] };

    wishlist.wishListProducts.push(product);

    wishlistService.setWishlistValue(wishlist);

}