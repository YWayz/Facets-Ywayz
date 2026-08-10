import { Component, Inject, OnInit, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { CancelVisitorAttendanceModel } from 'src/app/modules/visitor/models/cancel-visitor-attendance.model';
import { VisitorAttendedScheduleModel } from 'src/app/modules/visitor/models/visitor-pass-registration.model';
import { VisitorRegistrationService } from 'src/app/modules/visitor/services/visitor-registration.service';

@Component({
  selector: 'facets-visitor-cancel-registration',
  templateUrl: './visitor-cancel-registration.component.html',
  styleUrls: ['./visitor-cancel-registration.component.scss']
})
export class VisitorCancelRegistrationComponent implements OnInit {

  isBlocked = false;
  isAllCancelled = false;
  checkedList = new Array<any>();
  
  toasterService = inject(ToasterService);
  visitorRegistrationService = inject(VisitorRegistrationService);
  dialogRef = inject(MatDialogRef<VisitorCancelRegistrationComponent>);

  constructor(@Inject(MAT_DIALOG_DATA) public dialogData: { data: { visitorRegistrationId: string, attendanceSchedules: VisitorAttendedScheduleModel[] } }) { }
 
  ngOnInit(): void {
    this.isAllCancelled = this.dialogData.data.attendanceSchedules.every(e => e.cancelled == true);
  }

  checkUncheck(event: Event, attendanceScheduleId: string) {
    const isChecked = (event.target as HTMLInputElement).checked;

    if (isChecked) {
      this.checkedList.push(attendanceScheduleId);
      this.dialogData.data.attendanceSchedules.find(f => f.attendanceScheduleId == attendanceScheduleId)!.isSelected = true;
    }
    else {
      const index = this.checkedList.findIndex(f => f.key == attendanceScheduleId);
      this.checkedList.splice(index, 1);
    }
  }

  discard() {
    this.dialogRef.close();
  }

  cancelAttendenceSchedule() {
    this.isBlocked = true;
    this.visitorRegistrationService.cancelAttendenceSchedule(this.dialogData.data.visitorRegistrationId, new CancelVisitorAttendanceModel(this.checkedList)).subscribe({
      next: () => {
        this.discard();
        this.toasterService.success('Event dates have been cancelled successfully');
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }
}
