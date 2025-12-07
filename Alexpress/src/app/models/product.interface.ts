export interface Product {
    id: number,
    appUserId: string,
    username: string,
    images: string[],
    title: string,
    categoryId: number,
    condition: string,
    conditionId: number,
    description: string,
    category: string,
    votes: number,
    accumulated: number,
    price: number,
    shippingPrice: number,
    stock: number,
    sold: number,
    isDeleted: boolean,
    coupon: Coupon,
    reviews: Review[],
}

interface Coupon {
    couponName: string,
    discount: number
}

interface Review {
    rating: number,
    comment: string | null,
    author: string,
    createdAt: Date
}