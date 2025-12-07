import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { OrderService } from '../../services/order.service';
import { Order } from '../../models/order.interface';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { ToastrService } from 'ngx-toastr';
import { AddReviewDto } from '../../dtos/review/addReview.dto';
import { ReviewOrderDto } from '../../dtos/order/reviewOrder.dto';


@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styles: `
  `
})
export class OrdersComponent implements OnDestroy, OnInit{

  destroy$ = new Subject<void>();
  orders: Order[] = [];
  hoverRating: number = 0;
  rating: number = 0;
  orderId: number = 0;
  showRatingWindow: boolean = false;

  onMouseOnRating = (value: number) => this.hoverRating = value;

  onMouseLeaveRating = () => this.hoverRating = 0;

  onSetRating = (value: number) => this.rating = value; 

  getOrders(){

    this.orderService.getOrders()
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: orders => this.orders = orders,
      error: err => handleBackendErrorResponse(err, this.toastr)
    })

  }

  reviewOrder(comment: string) {

    const rating = this.rating;
    const orderId = this.orderId;

    if (rating === 0) {
      this.toastr.error("Rating is missing");
      return;
    }

    if (!orderId) return;

    if (!comment) {
      this.toastr.error("Comment is missing");
      return;
    }

    if (comment.length > 150) {
      this.toastr.error("Comment max characters is 150");
      return;
    }

    const body = {
      comment,
      rating,
      orderId
    } as ReviewOrderDto;

    this.orderService.reviewOrder(body)
    .subscribe({
      next: _ => {

        this.orders.find(o => o.id === this.orderId)!.rating = rating;
        this.rating = 0;
        this.orderId = 0;
        this.showRatingWindow = false;
      },
      error: err => handleBackendErrorResponse(err, this.toastr)
    })
  }

  commentCounter: number = 0;

  onTypingComment(textarea: HTMLTextAreaElement) {
    const commentLength = textarea.value.length;
    this.commentCounter = commentLength;
    if (this.commentCounter === 150) return;
  }

  onCancelRating() {
    this.showRatingWindow = false;
    this.commentCounter = 0;
  }

  onShowRatingWindow(orderId: number) {
    this.showRatingWindow = true;
    this.orderId = orderId;
  }

  ngOnInit(): void {
    this.getOrders();
  }
  
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  constructor(private orderService:OrderService, private toastr:ToastrService){}

}
