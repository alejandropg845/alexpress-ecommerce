import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MainComponent } from './main/main.component';
import { SharedModule } from '../shared/shared.module';
import { PagesRoutingModule } from './pages.routes';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { CategoriesComponent } from './categories/categories.component';
import { ProductsComponent } from './products/products.component';
import { HomeComponent } from './home/home.component';
import { ProductComponent } from './product/product.component';
import { OrdersComponent } from './orders/orders.component';
import { WishListComponent } from './wish-list/wish-list.component';
import { CartComponent } from './cart/cart.component';
import { AddProductComponent } from './add-product/add-product.component';
import { ReactiveFormsModule } from '@angular/forms';
import { MyProductsComponent } from './my-products/my-products.component';
import { EditProductComponent } from './edit-product/edit-product.component';
import { LessComponent } from './less/less.component';
import { PaymentComponent } from './payment/payment.component';
import { PaymentDoneComponent } from './payment-done/payment-done.component';
import { AddressesComponent } from './addresses/addresses.component';
import { CategoriesSliderComponent } from './categories-slider/categories-slider.component';
import { ManageProductsComponent } from '../auth/manage-products/manage-products.component';
import { ConfirmDialogComponent } from './confirm-dialog/confirm-dialog.component';
import { LoadingDialogComponent } from './loading-dialog/loading-dialog.component';
import { RecoverPasswordComponent } from './recover-password/recover-password.component';
import { EmailConfirmationComponent } from './email-confirmation/email-confirmation.component';
import { AccountComponent } from './account/account.component';
import { QRCodeModule } from 'angularx-qrcode';

@NgModule({
  declarations: [
    MainComponent,
    CategoriesComponent,
    HomeComponent,
    ProductsComponent,
    ProductComponent,
    OrdersComponent,
    EditProductComponent,
    WishListComponent,
    CartComponent,
    AddProductComponent,
    MyProductsComponent,
    LessComponent,
    PaymentComponent,
    PaymentDoneComponent,
    AddressesComponent,
    CategoriesSliderComponent,
    ManageProductsComponent,
    ConfirmDialogComponent,
    LoadingDialogComponent,
    RecoverPasswordComponent,
    EmailConfirmationComponent,
    AccountComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    PagesRoutingModule,
    RouterOutlet,
    RouterLink,
    RouterModule,
    ReactiveFormsModule,
    QRCodeModule
  ],
})
export class PagesModule { }
