import { VisitorPavilionSessionAttendanceScheduleModel } from "./visitor-pavilion-session-attendance-schedule.model";

export class VisitorRegisterModel {
    id: string;
    visitorId: string;
    passCategoryId: string;
    pavilionId: string;
    eventDateIds = new Array<string>();
    allEventDateIds = new Array<string>();
    pavilionSessionIds = new Array<string>();
    pavilionSessions = new Array<any>();
    registeredToEventOnsite: boolean;
    eventId: string;
    visitorRegistrationCounterId?: string;
    visitorAttendanceSchedules = new Array<VisitorAttendanceScheduleModel>();
    rateType: string;
    visitorPavilionSessionAttendanceSchedules = new Array<VisitorPavilionSessionAttendanceScheduleModel>()
}

export class VisitorAttendanceScheduleModel {
    visitorAttendanceScheduleId: string;
    eventDateId: string;
    isInvoiced: boolean;
    isCancelled: boolean;
}