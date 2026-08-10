import {
    HttpInterceptor,
    HttpRequest,
    HttpHandler,
    HttpEvent,
} from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, catchError, throwError } from 'rxjs';
import { AuthService } from 'src/app/modules/auth/services/auth.service';

@Injectable({
    providedIn: 'root',
})
export class TokenInterceptor implements HttpInterceptor {

    router = inject(Router) as Router;
    authService = inject(AuthService) as AuthService;

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (this.authService.isAuthenticated) {
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${this.authService.token}`,
                },
            });
        }
        return next.handle(request).pipe(
            catchError((err) => {
                if (err.status === 401) {
                    this.router.navigate(['login'], {
                        queryParams: { returnUrl: document.location.pathname },
                    });
                }
                const error = err.error;
                return throwError(() => error);
            })
        );
    }
}
