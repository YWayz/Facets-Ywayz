export class PavilionSessionModel{
    id: string;
    eventDateId: string;
    eventDate: string;
    startTime: Date;
    endTime: Date;
    allowedVisitorCount: number;
    pavilionId: string;
    isVisitorRegistered: boolean;
}