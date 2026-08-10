import { Component, OnInit, inject } from '@angular/core';
import { MatSelectChange } from '@angular/material/select';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { PavilionSummaryModel } from '../../models/pavilion-summary.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { PavilionService } from 'src/app/modules/event/services/pavilion.service';
import { PavilionSessionSummaryModel } from '../../models/pavilion-session-summary.model';
import { MatDialogRef } from '@angular/material/dialog';
import { NgForm } from '@angular/forms';
import { formatDate } from '@angular/common';
import { convertOnlyDate } from 'src/app/core/extensions/helpers';
import { LookupsService } from 'src/app/core/services/lookups.service';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { KeyValue } from 'src/app/core/models/key-value.model';

@Component({
  selector: 'facets-visitor-pavilion-verification-selection',
  templateUrl: './visitor-pavilion-verification-selection.component.html',
  styleUrls: ['./visitor-pavilion-verification-selection.component.scss']
})
export class VisitorPavilionVerificationSelectionComponent implements OnInit {

  isBlocked = false;
  isFormSubmitted = false;

  pavilionId: string;
  pavilionSessionId: string;

  pavilionSummariesModel: KeyValue<string, string>[] = [];
  pavilionSessionSummariesModel: PavilionSessionSummaryModel[]

  toasterService = inject(ToasterService);
  lookupService = inject(LookupsService);
  dialogRef = inject(MatDialogRef<VisitorPavilionVerificationSelectionComponent>);

  ngOnInit(): void {
    this.getPavilions();
  }

  getPavilions() {
    this.lookupService.getPavilions(localStorage.getItem(appConstant.selectedEventId)!).subscribe({
      next: (result: ResponseResult<KeyValue<string, string>[]>) => {
        this.isBlocked = false;
        this.pavilionSummariesModel = result.data;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  getPavilionSessions(pavilionId: string) {
    this.lookupService.getPavilionSessions(localStorage.getItem(appConstant.selectedEventId)!, pavilionId).subscribe({
      next: (result: ResponseResult<PavilionSessionSummaryModel[]>) => {
        this.isBlocked = false;
        this.pavilionSessionSummariesModel = result.data;
        if (this.pavilionSessionSummariesModel) {
          this.pavilionSessionSummariesModel = this.pavilionSessionSummariesModel.filter(f => convertOnlyDate(f.eventDate) >= convertOnlyDate(new Date()));
        }
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  onPavilionChange(event: MatSelectChange) {
    this.getPavilionSessions(event.value);
  }

  confirm(pavilionSelectionForm: NgForm) {
    this.isFormSubmitted = true;
    if (pavilionSelectionForm.invalid) {
      return;
    }

    const pavilionSession = this.pavilionSessionSummariesModel.find(f => f.id == this.pavilionSessionId)!;
    const pavilionSessionName = `${formatDate(pavilionSession.eventDate, 'dd/MM/yyyy', 'en-US')}, ${formatDate(pavilionSession.startTime, 'hh:mm a', 'en-US')} - ${formatDate(pavilionSession.eventDate, 'dd/MM/yyyy', 'en-US')}, ${formatDate(pavilionSession.endTime, 'hh:mm a', 'en-US')}`

    this.dialogRef.close({ pavilionId: this.pavilionId, pavilionSessionId: this.pavilionSessionId, pavilionSessionName: pavilionSessionName })
  }

  discard() {
    this.dialogRef.close();
  }
}
