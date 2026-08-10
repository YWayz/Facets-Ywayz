export class InvoiceModel {
    visitorRegistrationId: string;
    visitorId: string;
    eventId: string;
    totalAmount: number;
    invoiceCancelled: boolean;
    invoiceCancelledOn?: Date;
    invoiceLineItems = new Array<InvoiceLineItemModel>();
    passCategoryId: string;
    passCategorySettingId: string;
    appliedDiscountType: string;
    passCategorySetToChargable: boolean;
    paymentStatus: string;
}

export class InvoiceLineItemModel {
    createdOn: Date;
    invoiceId: string;
    visitorAttendanceScheduleId: string;
    amount: number
    eventId: string;
    visitorId: string;
}