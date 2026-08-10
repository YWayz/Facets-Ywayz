export class PaymentModel {
    registrationId: string;
    amount: number;
    visitorId: string;
    paymentMethod: string;
    lastFourDigitsofCard?: string;
    referenceNumber?: string;
    rateType: string;
}