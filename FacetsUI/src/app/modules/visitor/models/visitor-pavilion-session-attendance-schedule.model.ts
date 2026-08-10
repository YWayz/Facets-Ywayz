export class VisitorPavilionSessionAttendanceScheduleModel {
    visitorPavilionAttendanceScheduleId: string;
    eventDateId: string;
    pavilionId: string;
    pavilionSessionId: string;
    isCancelled: boolean;
    isInvoiced: boolean;
    pavilionName: string;
    pavilionSessionDate: Date;
    startTime: Date;
    endTime: Date;
    registeredMethod: string;
    registeredDateTime: Date;
    passGeneratedAt: Date;
}