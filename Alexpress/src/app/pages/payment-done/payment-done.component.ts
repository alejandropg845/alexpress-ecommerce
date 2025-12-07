import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-payment-done',
  templateUrl: './payment-done.component.html',
  styles: ``
})
export class PaymentDoneComponent implements OnInit{
  
  emptyCart() {
    this.cartService.setCartValue(null);
  }

  constructor(private cartService:CartService){}

  ngOnInit(): void {
    
  }

  

}
