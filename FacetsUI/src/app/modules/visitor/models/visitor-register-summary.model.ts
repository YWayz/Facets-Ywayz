export class VisitorRegisterSummaryModel {
    id: string;
    visitorId: string;
    firstame: string;
    lastame: string;
    nicNumber?: string;
    passportNumber?: string;
    mobileNumber: string;
    countryName: string;
    visitorStatus: string
    registrationCancelled: boolean;
    registrationCancelledOn?: Date;
    visitorIdentityType: string;
    eventId: string;
}