import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from 'src/app/core/services/base.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { EventDetailModel } from '../../event/models/event-detail.model';
import { VisitorReportModel } from '../models/visitor-report.model';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { VisitorReportFilterModel } from '../models/visitor-report-filter.model';
import { convertDateToUtc } from 'src/app/core/extensions/helpers';
import { CollectionReportFilterModel } from '../models/collection-report-filter.model';
import { CollectionReportModel } from '../models/collection-report.model';
import { AttendanceReportModel } from '../models/attendence-report-model';


@Injectable({
    providedIn: 'root',
})
export class ReportService extends BaseService {

    constructor() {
        super();
    }

    getEventById(id: string): Observable<ResponseResult<EventDetailModel>> {
        return this.get<ResponseResult<EventDetailModel>>(`events/${id}`);
    }

    getVisitorReport(searchModel: SearchRequestModel, filter: VisitorReportFilterModel): Observable<ResponseResult<VisitorReportModel[]>> {
        var url = `reports/visitor?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}&eventId=${filter.eventId}`;

        filter.eventDateIds.forEach(p => {
            url = `${url}&eventDateIds=${p}`
        });

        if (filter.visitorCountryIds != undefined && filter.visitorCountryIds != null) {
            filter.visitorCountryIds!.forEach(p => {
                url = `${url}&visitorCountryIds=${p}`
            });
        }
        if (filter.visitorStatuses != undefined && filter.visitorStatuses != null) {
            filter.visitorStatuses!.forEach(p => {
                url = `${url}&visitorStatuses=${p}`
            });
        }
        if (filter.passCategoryIds != undefined && filter.passCategoryIds != null) {
            filter.passCategoryIds!.forEach(p => {
                url = `${url}&passCategoryIds=${p}`
            });
        }

        if (filter.visitorEventRegistrationSpecificDate != undefined && filter.visitorEventRegistrationSpecificDate != null)
            url = `${url}&visitorEventRegistrationSpecificDate=${convertDateToUtc(filter.visitorEventRegistrationSpecificDate)}`

        if (filter.visitorEventRegistrationFromDate != undefined && filter.visitorEventRegistrationFromDate != null)
            url = `${url}&visitorEventRegistrationFromDate=${convertDateToUtc(filter.visitorEventRegistrationFromDate)}`;

        if (filter.visitorEventRegistrationToDate != undefined && filter.visitorEventRegistrationToDate != null)
            url = `${url}&visitorEventRegistrationToDate=${convertDateToUtc(filter.visitorEventRegistrationToDate)}`

        return this.get<ResponseResult<VisitorReportModel[]>>(url);
    }

    getCollectionReport(filter: CollectionReportFilterModel): Observable<ResponseResult<CollectionReportModel[]>> {
        var url = `reports/collection?eventId=${filter.eventId}`;

        if (filter.paidFromDate != undefined && filter.paidFromDate != null)
            url = `${url}&paidFromDate=${convertDateToUtc(filter.paidFromDate)}`;

        if (filter.paidToDate != undefined && filter.paidToDate != null)
            url = `${url}&paidToDate=${convertDateToUtc(filter.paidToDate)}`

        if (filter.specificDate != undefined && filter.specificDate != null)
            url = `${url}&specificDate=${convertDateToUtc(filter.specificDate)}`

        if (filter.passCategoryIds != undefined && filter.passCategoryIds != null) {
            filter.passCategoryIds!.forEach(p => {
                url = `${url}&passCategoryIds=${p}`
            });
        }

        if (filter.counterIds != undefined && filter.counterIds != null) {
            filter.counterIds!.forEach(p => {
                url = `${url}&counterIds=${p}`
            });
        }

        if (filter.registrationUserIds != undefined && filter.registrationUserIds != null) {
            filter.registrationUserIds!.forEach(p => {
                url = `${url}&registrationUserIds=${p}`
            });
        }
        return this.get<ResponseResult<CollectionReportModel[]>>(url);
    }


    getAttendenceReport(searchModel: SearchRequestModel, filter: VisitorReportFilterModel): Observable<ResponseResult<AttendanceReportModel[]>> {
        var url = `attendancereport/visitor?pageSize=${searchModel.pageSize}&pageNumber=${searchModel.pageNumber}&eventId=${filter.eventId}`;

        filter.eventDateIds.forEach(p => {
            url = `${url}&eventDateIds=${p}`
        });

        if (filter.visitorCountryIds != undefined && filter.visitorCountryIds != null) {
            filter.visitorCountryIds!.forEach(p => {
                url = `${url}&visitorCountryIds=${p}`
            });
        }
        if (filter.visitorStatuses != undefined && filter.visitorStatuses != null) {
            filter.visitorStatuses!.forEach(p => {
                url = `${url}&visitorStatuses=${p}`
            });
        }
        if (filter.passCategoryIds != undefined && filter.passCategoryIds != null) {
            filter.passCategoryIds!.forEach(p => {
                url = `${url}&passCategoryIds=${p}`
            });
        }

        if (filter.visitorEventRegistrationSpecificDate != undefined && filter.visitorEventRegistrationSpecificDate != null)
            url = `${url}&visitorEventRegistrationSpecificDate=${convertDateToUtc(filter.visitorEventRegistrationSpecificDate)}`

        if (filter.visitorEventRegistrationFromDate != undefined && filter.visitorEventRegistrationFromDate != null)
            url = `${url}&visitorEventRegistrationFromDate=${convertDateToUtc(filter.visitorEventRegistrationFromDate)}`;

        if (filter.visitorEventRegistrationToDate != undefined && filter.visitorEventRegistrationToDate != null)
            url = `${url}&visitorEventRegistrationToDate=${convertDateToUtc(filter.visitorEventRegistrationToDate)}`

        return this.get<ResponseResult<AttendanceReportModel[]>>(url);
    }
}
