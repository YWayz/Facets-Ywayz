import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from 'src/app/core/services/base.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { EventModel } from '../models/event.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { EventDetailModel } from '../models/event-detail.model';


@Injectable({
    providedIn: 'root',
})
export class EventService extends BaseService {

    constructor() {
        super();
    }

    create(eventModel: EventModel): Observable<ResponseResult<EventModel>> {
        return this.post<ResponseResult<EventModel>>('events', eventModel);
    }

    getAll(searchModel: SearchRequestModel): Observable<ResponseResult<EventModel[]>> {
        return this.get<ResponseResult<EventModel[]>>(`events?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}`);
    }
    getById(id: string): Observable<ResponseResult<EventDetailModel>> {
        return this.get<ResponseResult<EventDetailModel>>(`events/${id}`);
    }

    update(eventModel: EventModel) {
        return this.put(`events/${eventModel.id}`, eventModel)
    }

    uploadImage(eventId: string, files: File[]) {
        return this.filePost(`events/${eventId}/logo`, files)
    }

    updateStatus(eventId: string, body: { eventStatus: string }) {
        return this.put(`events/${eventId}/status`, body)
    }

    deleteEvent(eventId: string) {
        return this.delete(`events/${eventId}`);
    }
}
