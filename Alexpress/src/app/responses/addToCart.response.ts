import { CartProduct } from "../models/cart.interface";

export interface AddToCartResponse {
    cartId: number,
    cartProduct: CartProduct,
    isProductRemoved: boolean,
    summary: number
}