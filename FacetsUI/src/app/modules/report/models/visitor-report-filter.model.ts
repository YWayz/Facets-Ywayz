import { BehaviorSubject, Subject } from "rxjs";

export class VisitorReportFilterModel {
    eventId: string;
    eventDateIds: string[];
    visitorCountryIds?: string[];
    visitorStatuses?: string[];
    passCategoryIds?: string[];
    visitorEventRegistrationSpecificDate?: Date | string;
    visitorEventRegistrationFromDate?: Date | string;
    visitorEventRegistrationToDate?: Date | string;
}