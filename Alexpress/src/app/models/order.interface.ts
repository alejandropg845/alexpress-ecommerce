export interface Order {
    id: number,
    createdOn: Date,
    summary: number,
    rating: number,
    address: Address,
    orderedProducts: OrderedProduct[]
}

interface Address {
    city: string,
    country: string,
    fullName: string,
    phone: number,
    postalCode: string,
    residence: string
}

interface OrderedProduct {
    image: string,
    price: number,
    productId: number,
    quantity: number
}