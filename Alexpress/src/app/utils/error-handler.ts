import { HttpErrorResponse } from "@angular/common/http";
import { ToastrService } from "ngx-toastr";


export function handleBackendErrorResponse(res:HttpErrorResponse, toastr:ToastrService){
    if(res.status === 0) {
      toastr.info("Seems like you don't have internet connection for this action");
    } else if (res.error?.message) {
      toastr.error(res.error.message);
    } else if (res.error?.errors){
        for (const field in res.error.errors) {
          if (res.error.errors.hasOwnProperty(field)) {
            const fieldErrors = res.error.errors[field];
            fieldErrors.forEach((message: string) => {
              toastr.error(message, field);
            });
          }
        }
    } else {
      toastr.error("An unknown error occurred");
    }
  }
  
export function handleCloudinaryErrorResponse(error:HttpErrorResponse, toastr:ToastrService){
    if(error.status === 0) {
      toastr.info("Seems like you don't have internet connection for this action");
    } else {
      toastr.error("Failed to upload image. Please try again.");
    }
  }
