export class PavilionSessionVisitorModel {
    visitorFullName: string;
    visitorPavilionSessions = new Array<VisitorPavilionSessionModel>();
    isExpanded: boolean = false;
}

export class VisitorPavilionSessionModel {
    pavilionName: string;
    sessionDate: Date;
    sessionDateId: string;
    sessionStartTime: Date;
    sessionEndTime: Date;
    cancelled: boolean;
}