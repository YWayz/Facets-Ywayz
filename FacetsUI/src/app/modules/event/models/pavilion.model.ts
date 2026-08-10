import { PassCategoryPavilionSettingsModel } from "./pass-category-pavilion-settings.model";

export class PavilionModel {
    id: string;
    numberOfSession: number;
    name: string;
    status: string;
    passCategoryPavilionSettings = new Array<PassCategoryPavilionSettingsModel>();
}