export interface Wishlist {
    wishListProducts: WishlistItem[]
}

export interface WishlistItem {
    productId: number,
    image: string,
    title: string
}
