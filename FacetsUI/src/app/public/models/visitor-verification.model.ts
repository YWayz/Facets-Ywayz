export class VisitorVerificationModel {
    available: boolean;
    identificationNumber: string;
    visitorStatus: string;
    identityType: string;
    otpType: string;

    isAssocifyMember: boolean;
    isRegisteredToFacets: boolean;
    firstName: string;
    lastName: string;
    nicNumber: string;
    passportNumber: string;
    phoneNumber: string;
    email: string;
    visitorId: string;
    visitorIdentityType: string;

    initializeVisitorDetails(isAssocifyMember: boolean, isRegisteredToFacets: boolean, firstName: string, lastName: string, nicNumber: string, passportNumber: string, phoneNumber: string, email: string, visitorId: string, visitorIdentityType: string){
        this.isAssocifyMember = isAssocifyMember;
        this.isRegisteredToFacets = isRegisteredToFacets;
        this.firstName = firstName;
        this.lastName = lastName;
        this.nicNumber = nicNumber;
        this.passportNumber = passportNumber;
        this.phoneNumber = phoneNumber;
        this.email = email;
        this.visitorId = visitorId;
        this.visitorIdentityType = visitorIdentityType;
    }
}