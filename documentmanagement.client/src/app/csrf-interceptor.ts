import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CsrfTokenService } from './services/csrf-token.service';

@Injectable()
export class CsrfInterceptor implements HttpInterceptor {
    constructor(private csrfTokenService: CsrfTokenService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const csrfToken = this.csrfTokenService.getCsrfToken();

        if (csrfToken) {
            const clonedRequest = request.clone({
                setHeaders: {
                    'X-XSRF-TOKEN': csrfToken
                },
                withCredentials: true
            });
            return next.handle(clonedRequest);
        }

        return next.handle(request);
    }
}
