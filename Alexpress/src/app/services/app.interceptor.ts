import { inject, Injectable } from '@angular/core';
import { HttpInterceptor, HttpEvent, HttpHandler, HttpRequest, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { catchError, finalize, Observable, switchMap, throwError } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { LoadingDialogService } from './loading-dialog.service';
import { RefreshTokenService } from './rToken.service';
import { UserService } from './user.service';

@Injectable()
export class AppInterceptor implements HttpInterceptor {

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        
        const token = localStorage.getItem('alexpressAccessToken');
        const refreshTokenService = inject(RefreshTokenService);
        const loading = inject(LoadingDialogService);
        const userService = inject(UserService);

        loading.showSafety();

        let clonedReq;

        if (req.url.startsWith(environment.baseUrl) && token) {
            clonedReq = req.clone({
                setHeaders: {
                    Authorization: `Bearer ${token}`
                }
            });
        }

        return next.handle(clonedReq ?? req).pipe(

            catchError((err:HttpErrorResponse) => {

                if (err.status !== 401) {
                    localStorage.removeItem("alexpressAccessToken");
                    localStorage.removeItem('alexpressRefreshToken');
                    userService.setNullUsername();
                    return throwError(() => err);
                }
                    
                const refreshToken = localStorage.getItem('alexpressRefreshToken');

                return refreshTokenService.getNewToken(refreshToken!)
                .pipe(

                    switchMap(res => {

                        localStorage.setItem("alexpressAccessToken", res.accessToken);
                        localStorage.setItem('alexpressRefreshToken', res.refreshToken);
                        const newReq = req.clone({
                            setHeaders: {
                                Authorization: `bearer ${res.accessToken}`
                            }
                        });
                        return next.handle(newReq);
                    })

                );

                
                
            }),
            finalize(() => loading.hideSafety())
        );
    }
}