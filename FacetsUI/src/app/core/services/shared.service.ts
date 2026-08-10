import { Injectable } from "@angular/core";
import { BaseService } from "./base.service";
import { MobileNumberLength, appConstant } from "../extensions/app-constants";
import { BehaviorSubject } from "rxjs";
import { VisitorRegisterModel } from "src/app/modules/visitor/models/visitor-register.model";
import { VisitorModel } from "src/app/modules/visitor/models/visitor.model";

@Injectable({
    providedIn: 'root'
})
export class SharedService {

    slCodes = ["0094", "+94", "94"];

    nicPassport = '';

    isSriLankanNumber = false;
    isUpdateOtp = false;
    isUpdateProfile = false;
    isUpdateAttachment = false;
    isUpdatePass = false;
    isUpdatePavilion = false;
    isPayLater = false;
    onSitePayingMode: string;
    eventId: string;
    phoneNumber: string;
    isEventStatusChanged: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    visitorRegistrationModel: VisitorRegisterModel | undefined;
    visitorModelJson: string;

    getEventId(): string {
        this.eventId = localStorage.getItem(appConstant.selectedEventId)!;
        return this.eventId;
    }

    setEventStatus(status: boolean) {
        this.isEventStatusChanged.next(status);
    }

    isSriLankaNumber(mobileNumber: string): boolean {
        this.isSriLankanNumber = this.slCodes
                                     .some(c => mobileNumber.startsWith(c)) ||
                                                mobileNumber.length <= MobileNumberLength.maximumLengthOfSriLankaPhoneNumberWithoutCountryCode;

        return this.isSriLankanNumber;
    }
}
