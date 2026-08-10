import { PassCategoryPavilionSettingsModel } from "./pass-category-pavilion-settings.model";
import { PassCategoryRatesModel } from "./pass-category-rates.model";

export class PassCategoryModel {
    id: string;
    eventId: string;
    name: string;
    description: string;
    passTypeName: string;
    visitorPassCategoryType: string;
    isDefault: boolean;
    passCategorySettings = new Array<PassCategoryRatesModel>();
    passCategoryPavilionSettings = new Array<PassCategoryPavilionSettingsModel>();
    passCategoryType: string;
    color: string;
}