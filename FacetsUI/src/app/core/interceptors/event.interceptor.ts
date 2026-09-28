import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { appConstant } from '../extensions/app-constants';


@Injectable()
export class EventInterceptor implements HttpInterceptor {
    constructor() { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        // The API checks this header against the event in the route and against the user's event access,
        // so it must always be the event the user actually selected, never a fixed id.
        const eventId = localStorage.getItem(appConstant.selectedEventId);
        request = request.clone({
            setHeaders: { 'x-facets-event-id': eventId ?? '' }
        });

        return next.handle(request);
    }
}