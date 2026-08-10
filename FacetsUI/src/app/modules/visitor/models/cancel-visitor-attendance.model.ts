export class CancelVisitorAttendanceModel {
    attendanceScheduleIds = new Array<string>();

    constructor(attendanceScheduleIds: Array<string>) {
        this.attendanceScheduleIds = attendanceScheduleIds;
    }
}