import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { appConstant } from '../extensions/app-constants';


@Injectable()
export class EventInterceptor implements HttpInterceptor {
    constructor() { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        var eventId = localStorage.getItem(appConstant.selectedEventId)
        request = request.clone({
            //  setHeaders: { 'x-facets-event-id': (eventId == null && eventId == undefined) ? '' : eventId }
            //Live EventId '86d0a7c8-be73-47c2-041e-08dc119fbf38' Local 'c7c1b543-a444-4b1d-3f0c-08dcd9764f6b'
             setHeaders: { 'x-facets-event-id': (eventId == null && eventId == undefined) ? '86d0a7c8-be73-47c2-041e-08dc119fbf38' : '86d0a7c8-be73-47c2-041e-08dc119fbf38' }
        });

        return next.handle(request);
    }
}