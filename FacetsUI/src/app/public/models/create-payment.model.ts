export class CreatePaymentModel {
    registrationId: string;
    amount: number;
    visitorId: string;
    lastFourDigitsofCard?: string;
    referenceNumber?: string;
    rateType: string;
    paymentMethod: string;
}