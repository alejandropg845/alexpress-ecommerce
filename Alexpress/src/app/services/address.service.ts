import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment.development";
import { Address } from "../models/model.interface";
import { FormGroup } from "@angular/forms";

@Injectable({
    providedIn: 'root'
})
export class AddressService {

    getAddresses = () => this.http.get<Address[]>(environment.addressUrl);

    addAddress = (body: FormGroup) => this.http.post<Address>(environment.addressUrl, body.value);

    updateAddress(id: number, body: FormGroup) {
        return this.http.put<Address>(`${environment.addressUrl}/${id}`, body.value);
    }

    constructor(private readonly http:HttpClient){}

}