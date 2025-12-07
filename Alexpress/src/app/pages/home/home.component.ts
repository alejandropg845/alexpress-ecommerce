import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product.interface';
import { ProductThumbnail } from '../../models/product-thumbnail.interface';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: ``
})
export class HomeComponent implements OnInit{

  allProducts       :ProductThumbnail[] = [];
  lessFourProducts  :ProductThumbnail[] = [];
  newProducts       :ProductThumbnail[] = [];
  fewUnits          :ProductThumbnail[] = [];

  getProducts(){
    this.productService.products$
    .subscribe(products => {

      const productsCopy = [...products];

      this.allProducts = products;
      this.fewUnits = productsCopy.filter(p => p.stock < 10);
      this.lessFourProducts = productsCopy.filter(p => p.price <= 3.99).splice(0,4);
      this.newProducts = productsCopy.sort((a,b) => b.id! - a.id!).splice(0,4);
    });
  };

  constructor(private productService:ProductService){}

  ngOnInit(): void {
    this.getProducts();
  }

}
