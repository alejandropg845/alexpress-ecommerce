import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { CloudinaryResponse } from '../responses/cloudinary.response';
import { FormGroup } from '@angular/forms';
import { Product } from '../models/product.interface';
import { BehaviorSubject } from 'rxjs';
import { handleBackendErrorResponse } from '../utils/error-handler';
import { ToastrService } from 'ngx-toastr';
import { ProductThumbnail } from '../models/product-thumbnail.interface';

@Injectable({providedIn: 'root'})

export class ProductService {

    private productsSource = new BehaviorSubject<ProductThumbnail[]>([]);

    get products$(){
        return this.productsSource.asObservable();
    }

    getUserProducts = () => this.http.get<ProductThumbnail[]>(`${environment.productUrl}/userProducts`);

    getProducts(categoryId: number, title: string | null, price: number) {
        this.http.get<ProductThumbnail[]>(`${environment.productUrl}/getProducts/${title}?categoryId=${categoryId}&price=${price}`)
        .subscribe({
            next: products => this.productsSource.next(products),
            error: err => handleBackendErrorResponse(err, this.toastr)
        });
    }

    getProductsWithFilter(categoryId: number, title: string | null, price: number) {
        return this.http.get<ProductThumbnail[]>(`${environment.productUrl}/getProducts/${title}?categoryId=${categoryId}&price=${price}`);
    }
    
    uploadImage = (data:FormData) => this.http.post<CloudinaryResponse>(environment.cloudinaryUrl, data);

    deleteImage = (publicId: string) => this.http.delete<any>(`${environment.productUrl}/deleteFromCloudinary/${publicId}`);
    
    addProduct = (data:FormGroup) => this.http.post(environment.productUrl, data.value);

    getProduct = (id:number) => this.http.get<Product>(`${environment.productUrl}/${id}`);

    getProductToUpdate = (id: number) => this.http.get<Product>(`${environment.productUrl}/productToUpdate/${id}`);

    deleteProduct = (productId: number) => this.http.delete(`${environment.productUrl}/${productId}`);

    updateProduct = (productId: number, body: FormGroup) => this.http.put(`${environment.productUrl}/${productId}`, body.value)
    
    constructor(private readonly http:HttpClient, private toastr:ToastrService) { }
    
}