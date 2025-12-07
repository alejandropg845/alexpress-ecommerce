import { AddToCartResponse } from "../responses/addToCart.response";
import { CartService } from "../services/cart.service";

export function addToCartF(cartService:CartService, r:AddToCartResponse, productId: number) {

    let cart = cartService.getCurrentCartValue;

    /* Entra en esta función solo si el producto fue eliminado (quantity era 1 y pasó a 0) */ 
    if (r.isProductRemoved) {

        const index = cart!.cartProducts.findIndex(p => p.productId === productId);

        cart!.cartProducts.splice(index, 1);

        cart!.summary = r.summary;

        cartService.setCartValue(cart);

        return;
    }

    /* Flujo normal de agregar producto */

    if (!cart) { // Verificar que el cart existe
        
        cart = { 
            cartProducts: [],
            id: r.cartId,
            summary: r.summary
        };

        cart.cartProducts.push(r.cartProduct);


    } else {

        const index = cart.cartProducts.findIndex(p => p.productId === productId);

        if (index !== -1) { // Ya existe en el carrito

            cart.cartProducts[index] = r.cartProduct;

        } else cart.cartProducts.push(r.cartProduct);

    }

    cart.summary = r.summary;

    cartService.setCartValue(cart);

}