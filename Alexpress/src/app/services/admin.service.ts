import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Product } from '../models/product.interface';
import { UserDto } from '../dtos/user/user.dto';

@Injectable({providedIn: 'root'})

export class AdminService {

    getProducts = () => this.http.get<Product[]>(`${environment.adminUrl}/products`);

    getUsers = () => this.http.get<UserDto[]>(`${environment.adminUrl}/users`);

    deleteProduct = (productId: number) => this.http.delete<any>(`${environment.adminUrl}/deleteProduct/${productId}`);

    disableUser = (userId: string) => this.http.put<any>(`${environment.adminUrl}/disableUser/${userId}`, null);

    constructor(private http: HttpClient) { }
    
}