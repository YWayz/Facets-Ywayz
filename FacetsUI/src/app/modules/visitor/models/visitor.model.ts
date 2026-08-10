import { AddressModel } from "src/app/shared/models/address.model";

export class VisitorModel {
    id: string;
    isAssocifyMember: boolean;
    visitorIdentityType: string = 'NIC';
    nicNumber?: string;
    passportNumber?: string;
    countryId: string;
    visitorCategory: string;
    registeredOnline: boolean;
    otpVerified: boolean;
    otpVerificationRequired: boolean;
    firstName: string;
    lastName: string;
    mobileNumber: string;
    companyName?: string;
    email?: string | null;
    countryName: string;
    imageUrl: string = '../../../../../assets/images/no-image.png';
    visitorStatus: string;
    address = new AddressModel();

    mapImage() {
        this.imageUrl = '../../../../../assets/images/no-image.png'
    }
}