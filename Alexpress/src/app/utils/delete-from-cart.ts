import { CartService } from "../services/cart.service";

export function DeleteFromCart(cartService:CartService, productId: number, response: { summary: number }) {

    const cart = cartService.getCurrentCartValue;

    const index = cart!.cartProducts.findIndex(p => p.productId === productId);

    cart!.cartProducts.splice(index, 1);

    cart!.summary = response.summary;

    cartService.setCartValue(cart);

}