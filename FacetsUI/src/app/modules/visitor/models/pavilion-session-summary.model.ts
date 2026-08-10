export class PavilionSessionSummaryModel {
    id: string;
    eventDateId: string;
    startTime: Date;
    endTime: string;
    allowedVisitorCount: number;
    pavilionId: string;
    eventDate: string;
    amount: number;
    isSelected: boolean = false;
    isInvoiced: boolean = false;
    isCancelled: boolean = false;
}