export class PublicSiteEventSummaryModel {
    id: string;
    name: string;
    description: string | null;
    logoURL: string | null;
    visitorRegistrationStartDate: Date;
    visitorRegistrationEndDate: Date;
    eventDates = new Array<Date>();
    isUpComming: boolean;
}