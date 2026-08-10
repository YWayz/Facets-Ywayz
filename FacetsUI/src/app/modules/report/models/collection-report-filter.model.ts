export class CollectionReportFilterModel {
    eventId: string;
    eventDateIds: string[];
    registrationUserIds: string[];
    passCategoryIds?: string[];
    counterIds?: string[];
    paidFromDate?: Date | string;
    paidToDate?: Date | string;
    specificDate?: Date | string;
}