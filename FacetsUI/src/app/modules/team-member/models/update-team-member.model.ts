import { Title } from "src/app/core/enums/title.enum";
import { VisitorIdentityType } from "src/app/core/enums/visitor-identityType.enum";

export class UpdateTeamMemberModel {
    id: string;
    title: Title;
    identityType: VisitorIdentityType
    nICNumber: string;
    passportNumber: string;
    countryId: string;
    firstName: string;
    lastName: string;
    eventId: string;
    mobileNumber: string;
    email: string;
    imageURL: string = '../../assets/images/no-image.png';
    address = new Address();
}

export class Address {
    address: string;
}