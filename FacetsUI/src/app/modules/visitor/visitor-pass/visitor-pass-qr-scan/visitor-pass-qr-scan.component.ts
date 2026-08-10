import { Component, inject } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'facets-visitor-pass-qr-scan',
  templateUrl: './visitor-pass-qr-scan.component.html',
  styleUrls: ['./visitor-pass-qr-scan.component.scss']
})
export class VisitorPassQrScanComponent {

  attendanceSheduleId: string;

  dialogRef = inject(MatDialogRef<VisitorPassQrScanComponent>);

  verifyQr(qrCodeResult: string) {
    const qrResult = JSON.parse(qrCodeResult);
    this.attendanceSheduleId = qrResult.attendanceSheduleId;
    this.discard()
  }

  discard() {
    this.dialogRef.close(this.attendanceSheduleId);
  }
}
