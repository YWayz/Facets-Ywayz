import { Address } from "./create-team-member.model";

export class TeamMemberModel {
    id: string;
    title: string;
    identityType: string;
    nicNumber: string;
    passportNumber: string;
    countryId: string;
    countryName: string;
    firstName: string;
    lastName: string;
    mobileNumber: string;
    email: string;
    address = new Address();
    passCategoryId: string;
    passCategoryName: string;
    imageURL: string = '../../assets/images/no-image2.png';
    teamMemberStatus: string;
    companyName: string | null;
    isBlackListed : boolean;
}