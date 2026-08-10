import { convertDateToUtc, convertOnlyDate } from "src/app/core/extensions/helpers";

export class EventModel {

    id: string;
    name: string;
    description: string;
    eventDates = new Array<string>();
    visitorRegistrationStartDate: Date | string = new Date();
    visitorRegistrationEndDate: Date | string = new Date();
    logoURL: string = '../../assets/images/no-image2.png';
    eventStatus: string;

    mapModel(startDate: string | Date, endDate: string | Date, selectedEventDates: any[]) {
        this.visitorRegistrationStartDate = convertDateToUtc(startDate);
        this.visitorRegistrationEndDate = convertDateToUtc(endDate);
        selectedEventDates.forEach(p => {
            this.eventDates.push(convertDateToUtc(p));
        });
    }

    setLogoUrl() {
        this.logoURL = '../../assets/images/no-image2.png';
    }
} 