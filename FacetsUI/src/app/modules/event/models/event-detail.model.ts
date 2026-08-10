import { KeyValue } from "src/app/core/models/key-value.model";

export class EventDetailModel {
    id: string;
    name: string;
    description: string;
    eventDates = new Array<KeyValue<string, string>>();
    visitorRegistrationStartDate: Date | string = new Date();
    visitorRegistrationEndDate: Date | string = new Date();
    logoURL: string = '../../assets/images/no-image2.png';
    eventStatus: string;

    setLogoUrl() {
        this.logoURL = '../../assets/images/no-image2.png';
    }
}