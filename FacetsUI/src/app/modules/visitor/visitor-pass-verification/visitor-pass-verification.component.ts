import { Component, inject } from '@angular/core';
import { VisitorPassService } from '../services/visitor-pass.service';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { QRVerifiedVisitorModel } from '../models/qr-verified-visitor.model';
import { TeamMemberPassService } from '../../team-member/services/team-member-pass.service';
import { PassVerificationModel } from '../models/pass-verification.model';
import { TeamMemberPassVerificationModel } from '../../team-member/models/team-member-pass-verification.model';

@Component({
  selector: 'facets-visitor-pass-verification',
  templateUrl: './visitor-pass-verification.component.html',
  styleUrls: ['./visitor-pass-verification.component.scss']
})
export class VisitorPassVerificationComponent {

  isBlocked = false;
  isShowScanner = false;
  isShow = false;
  isVerified = false;
  isUnVerified = false;
  displayStyle = 'none';
  displayVerifiedStyle = 'none';

  qrVerifiedVisitorModel = new QRVerifiedVisitorModel();
  passVerificationModel = new PassVerificationModel();
  teamMemberPassVerificationModel = new TeamMemberPassVerificationModel();

  visitorPassService = inject(VisitorPassService);
  teamMemberPassService = inject(TeamMemberPassService);
  toasterService = inject(ToasterService);

  showScanner() {
    this.isShowScanner = true;
  }

  verifyQr(qrCodeResult: string) {
    this.isBlocked = true;
    this.isShow = true;
    const qrResult = JSON.parse(qrCodeResult);
    this.passVerificationModel.type = qrResult.type;

    if (this.passVerificationModel.type == 'visitor') {
      this.passVerificationModel.attendanceSheduleId = qrResult.attendanceSheduleId;
      this.passVerificationModel.visitorId = qrResult.visitorId;
      this.getVisitorDetail();
    }
    else {
      this.teamMemberPassVerificationModel.eventId = qrResult.eventId;
      this.teamMemberPassVerificationModel.teamMemberId = qrResult.teamMemberId;
      this.getTeamMemberDetail();
    }
  }

  getVisitorDetail() {
    this.visitorPassService.update(this.passVerificationModel).subscribe({
      next: (result: ResponseResult<QRVerifiedVisitorModel>) => {
        this.qrVerifiedVisitorModel = result.data;
        this.isVerified = true;
        this.isUnVerified = false;
        this.isBlocked = false;
        this.displayVerifiedStyle = 'block';
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.isUnVerified = true;
        this.isVerified = false;
        this.displayStyle = 'block';
      }
    });
  }

  getTeamMemberDetail() {
    this.teamMemberPassService.update(this.teamMemberPassVerificationModel).subscribe({
      next: (result: ResponseResult<QRVerifiedVisitorModel>) => {
        this.qrVerifiedVisitorModel = result.data;
        this.isVerified = true;
        this.isUnVerified = false;
        this.isBlocked = false;
        this.displayVerifiedStyle = 'block';

      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.isUnVerified = true;
        this.isVerified = false;
        this.displayStyle = 'block';
      }
    });
  }

  close() {
    this.displayStyle = "none";
  }

  closeVerifiedModal() {
    this.displayVerifiedStyle = "none";
  }
}