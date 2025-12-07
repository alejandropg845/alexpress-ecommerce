import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { CreateOrderDto } from '../dtos/order/createOrder.dto';
import { Order } from '../models/order.interface';
import { ReviewOrderDto } from '../dtos/order/reviewOrder.dto';

@Injectable({providedIn: 'root'})

export class OrderService {

    getOrders = () => this.http.get<Order[]>(environment.orderUrl);

    createOrder = (body:CreateOrderDto) => this.http.post(environment.orderUrl, body);

    summarizeOrder = (addressId: number) => this.http.post<{ sessionUrl: string }>(`${environment.orderUrl}/summarizeOrder/${addressId}`, null);

    reviewOrder = (review: ReviewOrderDto) => this.http.post(`${environment.orderUrl}/reviewOrder`, review);

    constructor(private readonly http:HttpClient) { }
    
}