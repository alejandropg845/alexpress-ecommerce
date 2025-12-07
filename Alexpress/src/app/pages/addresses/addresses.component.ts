import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs';
import { handleBackendErrorResponse } from '../../utils/error-handler';
import { Address } from '../../models/model.interface';
import { AddressService } from '../../services/address.service';

@Component({
  selector: 'app-addresses',
  templateUrl: './addresses.component.html',
  styles: ``
})
export class AddressesComponent implements OnInit {

  addressForm: FormGroup;

  isLoading: boolean = false;
  addressId: number = 0;
  addresses: Address[] = [];
  isVisible: boolean = false;
  isEdit: boolean = false;

  getAddresses() {
    this.addressService.getAddresses()
    .subscribe({
      next: addresses => this.addresses = addresses,
      error: err => handleBackendErrorResponse(err, this.toastr)
    });
  }

  onEditclick(addressId: number) {
    this.isEdit = true;
    this.isVisible = true;
    this.setAddressValues(addressId);
  }

  onSubmit() {

    if (!this.addressForm.valid) {
      this.toastr.error("There are missing fields", "Missing fields");
      this.addressForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;

    if (!this.isEdit) {

      this.addressService.addAddress(this.addressForm)
        .subscribe({
          next: addedAddress => {
            this.isVisible = false;
            this.isLoading = false;
            this.addresses.push(addedAddress);
          },
          error: err => {
            handleBackendErrorResponse(err, this.toastr);
            this.isLoading = false;
          }
        });

    } else {
      
      this.addressService.
      updateAddress(this.addressId, this.addressForm)
      .subscribe({
        next: updatedAddress => {
          this.isVisible = false;
          this.isLoading = false;

          const index = this.addresses.findIndex(a => a.id === this.addressId);
          this.addresses[index] = updatedAddress;
          this.addressId = 0;
        },
        error: err => {
          this.isLoading = false;
          handleBackendErrorResponse(err, this.toastr);
        }
      });
    }
  }


  setAddressValues(id: number) {

    this.addressId = id;

    const addressInfo: Address = this.addresses.find(a => a.id === id)!;

    this.addressForm.get('fullName')?.setValue(addressInfo.fullName);
    this.addressForm.get('phone')?.setValue(addressInfo.phone);
    this.addressForm.get('postalCode')?.setValue(addressInfo.postalCode);
    this.addressForm.get('city')?.setValue(addressInfo.city);
    this.addressForm.get('residence')?.setValue(addressInfo.residence);
    this.addressForm.get('country')?.setValue(addressInfo.country);

  }

  cancel() {
    this.isVisible = false;
    this.addressForm.reset();
  }

  validateField(controlName: string) {
    const formControl = this.addressForm.get(controlName);

    return (!formControl?.valid && formControl?.touched) ? 'dark:border-red-600' : 'border-cyan-300';
    
  }

  constructor(private fb: FormBuilder, private toastr: ToastrService, private addressService: AddressService) {

    this.addressForm = this.fb.group({
      fullName: [null, [Validators.required, Validators.maxLength(40)]],
      phone: [null, [Validators.required, Validators.minLength(10), Validators.maxLength(10)]],
      postalCode: [null, [Validators.required, Validators.maxLength(10)]],
      residence: [null, [Validators.required, Validators.maxLength(60)]],
      country: [null, [Validators.required, Validators.maxLength(15)]],
      city: [null, [Validators.required, Validators.maxLength(25)]]
    });

  }

  ngOnInit(): void {
    this.getAddresses();
  }

}
