import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';

@Injectable({providedIn: 'root'})


export class RefreshTokenService {

    getNewToken = (refreshToken: string) => this.httpClient.post<{ refreshToken:string, accessToken:string }>(`${environment.rTokenUrl}/getAccessToken`, { refreshToken }) 

    constructor(private httpClient: HttpClient) { }
    
}