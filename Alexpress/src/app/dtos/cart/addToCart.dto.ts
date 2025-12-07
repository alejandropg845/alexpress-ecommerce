export interface AddToCartDto {
    productId: number,
    quantity: number,
    customizedDiscount: number,
    couponName: string | null
}