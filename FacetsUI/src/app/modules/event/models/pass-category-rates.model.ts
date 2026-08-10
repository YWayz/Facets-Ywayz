export class PassCategoryRatesModel {
    id: string;
    passCategoryId: string;
    passCategoryName: string;
    isChargeable: boolean;
    rate: number;
    discountedRate: number;
    applyEarlyRegistrationDiscountedRate: boolean;
    applyOnlineRegistrationDiscountedRate: boolean;
    applyEntireEventDiscountedRate: boolean;
    earlyRegistrationDiscountedRateValidUntil: Date | string;
    visitorRegistrationStartsOn: Date;
    visitorRegistrationEndsOn: Date;
    rateType: string;
}