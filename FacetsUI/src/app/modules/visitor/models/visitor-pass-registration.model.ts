import { VisitorPavilionSessionAttendanceScheduleModel } from "./visitor-pavilion-session-attendance-schedule.model";

export class VisitorPassRegistrationModel {
    attendanceSchedules = new Array<VisitorAttendedScheduleModel>();
    passCategoryName: string;
    visitorRegistrationId: string;
    visitorPavilionSessionAttendanceSchedules = new Array<VisitorPavilionSessionAttendanceScheduleModel>();
}

export class VisitorAttendedScheduleModel {
    attendanceScheduleId: string;
    registeredMethod: string;
    registeredDateTime: Date;
    registeredEventDate: Date;
    visitorAttended: boolean;
    passGeneratedAt?: Date;
    eventDateId: string;
    cancelled: boolean;
    isSelected: boolean;
    amount: number;
}