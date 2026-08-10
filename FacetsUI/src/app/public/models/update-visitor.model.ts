import { AddressModel } from "src/app/shared/models/address.model";

export class UpdateVisitorModel {
    visitorIdentityType: string;
    nICNumber: string | null;
    passportNumber: string | null;
    countryId: string;
    firstName: string;
    lastName: string;
    mobileNumber: string;
    companyName: string | null;
    email: string | null;
    address: AddressModel | null;
    passCategoryId: string;
}