import { Component, OnInit, inject } from '@angular/core';
import { VisitorPavilionVerificationSelectionComponent } from './visitor-pavilion-verification-selection/visitor-pavilion-verification-selection.component';
import { ModalService } from 'src/app/core/services/modal.service';
import { PassVerificationModel } from '../models/pass-verification.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { QRVerifiedVisitorModel } from '../models/qr-verified-visitor.model';
import { VisitorPassService } from '../services/visitor-pass.service';
import { ToasterService } from 'src/app/core/services/toaster.service';

@Component({
  selector: 'facets-visitor-pass-verification-pavilion',
  templateUrl: './visitor-pass-verification-pavilion.component.html',
  styleUrls: ['./visitor-pass-verification-pavilion.component.scss']
})
export class VisitorPassVerificationPavilionComponent implements OnInit {

  isBlocked = false;
  isShowScanner = false;
  isShow = false;
  isVerified = false;
  isUnVerified = false;
  pavilionSessionName = '';
  pavilionId: string = '';
  pavilionSessionId: string = '';
  displayStyle = 'none';
  displayVerifiedStyle = 'none';

  passVerificationModel = new PassVerificationModel();
  qrVerifiedVisitorModel = new QRVerifiedVisitorModel();

  visitorPassService = inject(VisitorPassService);
  toasterService = inject(ToasterService);
  modalService = inject(ModalService)

  ngOnInit(): void {
    this.openPavilionSelectionPopup();
  }

  openPavilionSelectionPopup() {
    this.modalService.displayDialog(VisitorPavilionVerificationSelectionComponent)
    this.modalService.confirmed().subscribe((data) => {
      if (data != undefined) {
        this.pavilionSessionName = data.pavilionSessionName;
        this.pavilionId = data.pavilionId;
        this.pavilionSessionId = data.pavilionSessionId;
      }
    });
  }

  verifyQr(qrCodeResult: string) {
    this.isBlocked = true;
    this.isShow = true;
    const qrResult = JSON.parse(qrCodeResult);
    this.passVerificationModel.type = qrResult.type;

    if (this.passVerificationModel.type == 'visitor') {
      this.passVerificationModel.attendanceSheduleId = qrResult.attendanceSheduleId;
      this.passVerificationModel.visitorId = qrResult.visitorId;
      this.passVerificationModel.pavilionId = this.pavilionId;
      this.passVerificationModel.pavilionSessionId = this.pavilionSessionId;
      this.getVisitorDetail();
    }
  }

  showScanner() {
    if (this.pavilionId != '' && this.pavilionSessionId != '' && this.pavilionSessionName != '') this.isShowScanner = true;
    else {
      this.toasterService.warning('Please select a pavilion and session to verify')
      this.openPavilionSelectionPopup();
    }
  }

  getVisitorDetail() {
    this.visitorPassService.updatePavilion(this.passVerificationModel).subscribe({
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
