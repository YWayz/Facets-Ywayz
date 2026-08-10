export class GenerateOTPModel {
    type: string;
    identityNumber: string;
    sendOTPByEmail: boolean;

    assignValues(type: string, identityNumber: string, sendOTPByEmail: boolean) {
        this.identityNumber = identityNumber;
        this.type = type;
        this.sendOTPByEmail = sendOTPByEmail;
    }
}