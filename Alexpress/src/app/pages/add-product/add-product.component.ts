import { Component, ElementRef, Input, OnChanges, OnDestroy, OnInit, QueryList, SimpleChanges, ViewChild, ViewChildren} from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { catchError, forkJoin, map, of, Subject, switchMap, takeUntil, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { handleBackendErrorResponse, handleCloudinaryErrorResponse } from '../../utils/error-handler';
import { LoadingDialogService } from '../../services/loading-dialog.service';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product.interface'

@Component({
  selector: 'app-add-product',
  templateUrl: './add-product.component.html',
  styles: ``
})
export class AddProductComponent implements OnChanges{

  isValid:boolean = true;
  urls:string[] = [];
  files: File[] = [];
  titleLength:number = 0;
  descriptionLength:number = 0;
  isVisible:boolean = false;
  destroy$ = new Subject<void>();
  isEditing: boolean = false;
  form:FormGroup;

  @Input() product:Product | null = null;

  checkLength(controlName:string){
    const control = this.form.get(controlName)?.value.length;
    if (controlName === 'title') this.titleLength = control;
    if (controlName === 'description') this.descriptionLength = control;
  }

  onSelectedImages(event:any){

    let files = event.target.files;

    
    for (let i = 0; i < files.length; i++) {

      const file = files[i];

      if (file.size > 5242880){
        this.toastr.error('Image size must be less than 5MB', 'File size error');
        continue;
      }

      if(!file.type.includes('image')){
        this.toastr.error('That\'s not an image', 'File format error');
        continue;
      }

      this.files.push(file);
    }
    
    
  }

  setValueToSrc(img:HTMLImageElement, file:File){
    const url = URL.createObjectURL(file);
    img.setAttribute('src', url);
  }

  onDropImages(event:DragEvent){
    event.preventDefault();
    event.stopPropagation();
    const file = event.dataTransfer?.files;
    const dropZone = event.currentTarget as HTMLElement;
    dropZone.classList.remove('border-cyan-600','border-4');
    this.addFilesToDropZone(file!);
  }

  onDragOverImages(event:DragEvent){
    event.preventDefault();
    event.stopPropagation();
    const dropZone = event.currentTarget as HTMLElement;
    dropZone.classList.add('border-cyan-600', 'transition', 'ease-in-out', 'duration-300', 'border-4');
    dropZone.classList.remove('border-red-600');
  }

  onDragLeaveImages(event:DragEvent){
    event.preventDefault();
    event.stopPropagation();
    const dropZone = event.currentTarget as HTMLElement;
    dropZone.classList.add('border-black','border');
    dropZone.classList.remove('border-cyan-600', 'border-4');
  }


  addFilesToDropZone(files:FileList){

    for (let i = 0; i < files.length; i++) {
      
      let file = files[i];
      
      if(file.size > 5242880){
        this.toastr.error('Image size must be less than 5MB', 'File size error');
        continue;
      }

      if (!file.type.includes('image')){
        this.toastr.error('That\'s not an image', 'File format error');
        continue;
      }
      
      if (!this.files.includes(file)){
        this.files.push(file);
      }

    }
    
    
  }

  
  onSubmit(){

    if (!this.files[0]) { 
      this.toastr.error("You forgot to select an image", "No image selected"); 
      return;
    }

    if (this.files.length !== 4){
      this.toastr.error("You must upload 4 images, you have uploaded "+this.files.length);
      return;
    }

    if (this.form.hasError("conflictingDiscountSelection")) {
      this.toastr.error("You cannot have both discount and coupon selected");
      this.form.markAllAsTouched();
      return;
    }

    if (!this.form.valid) {
      this.toastr.error("Make sure you filled all fields correctly", "Fields error");
      this.form.markAllAsTouched();
      return;
    }

    let discount = this.form.get('coupon.discount')!.value;

    if (discount === null) this.form.get('coupon.discount')!.setValue(0);

    /* En dado caso de que falle al agregar el producto, ya tenemos las urls de las imágenes subidas
    en el arreglo urls por lo que no hace falta volver a subirlas a cloudinary*/
    const urls$ = this.urls.length !== 4 ? this.getImagesUrls() : of(this.urls);

    if (!this.isEditing) {

      urls$.pipe(
        switchMap(urls => {
          this.urls = urls;
          this.form.get('images')?.setValue(this.urls);

          return this.productService.addProduct(this.form);
          
        }),
        takeUntil(this.destroy$)
      ).subscribe({
        next: _ => {
          this.router.navigateByUrl("/alexpress/my_products");
        },
        error: err => handleBackendErrorResponse(err, this.toastr)
      });
    } else {

      urls$.pipe(
        switchMap(urls => {

          this.urls = urls;
          this.form.get('images')?.setValue(this.urls);

          return this.productService.updateProduct(this.product!.id, this.form);
          
        }),
        takeUntil(this.destroy$)
      ).subscribe({
        next: _ => {
          this.router.navigateByUrl("/alexpress/my_products");
        },
        error: err => handleBackendErrorResponse(err, this.toastr)
      });

    }

    
  }

  setCouponName(couponName: string | null) {

    this.form.get("coupon.couponName")?.setValue(couponName);

  }

  getImagesUrls() {

    const selectedImagesObservable = this.files.map(file => {
      let data = new FormData();
      data.append('file',file); 
      data.append('upload_preset', 'cloudinary_products');
      return this.productService.uploadImage(data).pipe(map(url => url.secure_url));
    });

    return forkJoin(selectedImagesObservable)
    .pipe(
      catchError((err) => {
        handleCloudinaryErrorResponse(err, this.toastr);
        return throwError(() => err);
      })
    );
    
  }

  validateField(field:string){
    return (this.form.get(field)?.touched && !this.form.get(field)?.valid) 
    ? 'border-red-600' : 'border-black';;
  }

  @ViewChildren('couponName') couponNames!:QueryList<ElementRef>;

  setProductToEdit() {

    if (!this.product) return;

    this.isEditing = true;

    this.convertToImage(this.product.images);

    this.form.get("images")?.setValue(this.product.images);
    this.form.get("title")?.setValue(this.product.title);
    this.form.get("description")?.setValue(this.product.description);
    this.form.get("price")?.setValue(this.product.price);
    this.form.get("categoryId")?.setValue(this.product.categoryId);
    this.form.get("conditionId")?.setValue(this.product.conditionId);
    this.form.get("shippingPrice")?.setValue(this.product.shippingPrice);
    this.form.get("stock")?.setValue(this.product.stock);
    
    if (this.product.coupon.discount) { // Agregar cupon de descuento si tiene
      this.form.get('coupon.discount')?.setValue(this.product.coupon.discount);
      return;
    }

    if (this.product.coupon.couponName) { // Agregar cupon definido si tiene

      this.couponNames.forEach(couponElementRef => {

        const couponName = couponElementRef.nativeElement as HTMLInputElement;

        if (this.product!.coupon.couponName === 'is50OffOneProduct')
          if (couponName.id === 'is50OffOneProduct') 
            couponName.checked = true;
        
        if (this.product!.coupon.couponName === 'isFreeShipping')
          if (couponName.id === 'isFreeShipping') 
            couponName.checked = true;

        if (this.product!.coupon.couponName === 'is50Discount')
          if (couponName.id === 'is50Discount') 
            couponName.checked = true;

      });

    }


  }

  clearSelection(){
    this.couponNames.forEach(e => (e.nativeElement as HTMLInputElement).checked = false);
    this.setCouponName(null);
  }

  validateCoupon() {

    return (control:AbstractControl) => {

      const couponName = control.get("coupon.couponName")?.value;
      const discount = control.get("coupon.discount")?.value;
      
      if (couponName && discount > 0)

        return { conflictingDiscountSelection: true };

      return null;
    }

  }

  convertToImage(urls: string[]) {

    const files = urls.map(async url => {

      const response = await fetch(url);
      const data = await response.blob();
      const metadata = {
        type: 'image/jpeg'
      };

      const file = new File([data], "name", metadata);

      return file;

    });

    forkJoin(files).subscribe(files => this.files = files);

  }

  deleteImage = (i: number) => this.files.splice(i, 1); 


  constructor(
    private fb:FormBuilder, 
    private toastr:ToastrService,
    private router:Router,
    private productService:ProductService
  ){

    this.form = this.fb.group({
      images        : [null, [Validators.minLength(4), Validators.maxLength(4)]],
      title         : [null, [Validators.required, Validators.maxLength(160)]],
      description   : [null, [Validators.required, Validators.maxLength(2000)]],
      price         : [null, [Validators.required, Validators.min(1), Validators.max(50000)]],
      categoryId    : [0, [Validators.required, Validators.min(1)]],
      conditionId   : [0, [Validators.required, Validators.min(1)]],
      shippingPrice : [null, [Validators.required, Validators.min(0), Validators.max(10000)]],
      stock         : [null, [Validators.required, Validators.min(1), Validators.max(9999)]],
      coupon        : this.fb.group({
        couponName  : [null],
        discount : [0, [Validators.max(100), Validators.min(0)]],
      }),
    }, {
      validators: this.validateCoupon()
    });


  }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['product']) this.setProductToEdit();
  }
  
}
