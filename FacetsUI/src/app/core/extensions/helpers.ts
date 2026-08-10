import * as moment from 'moment';
import { CreatePavilionSessionModel } from 'src/app/modules/event/models/create-pavilion-session.model';
import { PavilionSessionModel } from 'src/app/modules/event/models/pavilion-session.model';

export function isArrayEmpty(obj: any) {
    return obj.length == 0 ? true : false;
}

export function isZero(obj: any) {
    return obj == 0 ? true : false;
}

export function isNull(obj: any) {
    return obj == null ? true : false;
}

export function isUndefined(obj: any) {
    return obj == undefined ? true : false;
}

export function isEmpty(obj: any) {
    return obj == '' ? true : false;
}

export function convertDateToUtc(date: Date | string): string {
    if (!date)
        return '';

    let currentdate = new Date();
    let formattedDate = new Date(date);
    formattedDate.setHours(currentdate.getHours());
    formattedDate.setMinutes(currentdate.getMinutes());
    formattedDate.setSeconds(currentdate.getSeconds());
    let utcDate = moment(formattedDate).utc().toISOString();
    return utcDate;
}

export function convertOnlyDate(date: Date | string): string {
    if (!date)
        return '';

    let currentdate = new Date();
    let formattedDate = new Date(date);
    formattedDate.setHours(currentdate.getHours());
    formattedDate.setMinutes(currentdate.getMinutes());
    formattedDate.setSeconds(currentdate.getSeconds());
    let utcDate = moment(formattedDate).utc().format("YYYY-MM-DD");
    return utcDate;
}


export function compareOnlyTime(startTime: Date | string, currentTime: Date | string, addMins: number): boolean {
    var beginningTime = moment(startTime, 'h:mma');
    var endingTime = beginningTime;
    var currentOtpTime = moment(currentTime, 'h:mma');
    if (addMins)
        endingTime.add(addMins, 'minutes')

    if (currentOtpTime.isBefore(endingTime)) {
        return true;
    }
    else {
        return false;
    }
}


export function dataURItoBlob(dataURI: string) {
    const byteString = window.atob(dataURI);
    const arrayBuffer = new ArrayBuffer(byteString.length);
    const int8Array = new Uint8Array(arrayBuffer);
    for (let i = 0; i < byteString.length; i++) {
        int8Array[i] = byteString.charCodeAt(i);
    }
    const blob = new Blob([int8Array], { type: 'image/png' });
    return blob;
}

export function generateRandomId() {
    return Math.random().toString(16).slice(2);
}

export function convertUrlToBlob(url: string, mode: RequestMode | string): Blob {
    let blob = new Blob();
    fetch(url, {
        mode: mode as RequestMode,
    }).then(res => res.blob().then(blobFile => {
        blob = blobFile;
    }));
    return blob;
}

export function checkTimeOverLap(pavilionSessionModels: PavilionSessionModel[], pavilionSessionModel: CreatePavilionSessionModel): boolean {
    pavilionSessionModels = pavilionSessionModels.filter(s => s.eventDateId == pavilionSessionModel.eventDateId);

    const modelStartTime = new Date(pavilionSessionModel.startTime);
    const modelEndTime = new Date(pavilionSessionModel.endTime);

    var hasOverlap = pavilionSessionModels.some(x => {
        const existingStartTime = new Date(x.startTime);
        const existingEndTime = new Date(x.endTime);

        return (
            (existingStartTime.getTime() < modelEndTime.getTime() && existingEndTime.getTime() > modelStartTime.getTime()) ||
            (existingStartTime.getTime() <= modelStartTime.getTime() && existingEndTime.getTime() >= modelEndTime.getTime())
        );
    });


    return hasOverlap;
}