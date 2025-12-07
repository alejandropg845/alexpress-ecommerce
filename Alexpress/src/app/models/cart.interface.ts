export interface Cart {
    id: number,
    summary: number,
    cartProducts: CartProduct[]
}

export interface CartProduct {
    customizedDiscount: number,
    image: string,
    couponName: string,
    newPrice: number,
    productId: number,
    quantity: number,
    shippingPrice: number,
    title: string
} 