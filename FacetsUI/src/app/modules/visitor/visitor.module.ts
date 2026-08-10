import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from 'src/app/shared/shared.module';
import { VisitorRegistrationAttachmentsComponent } from './visitor-registration/visitor-registration-attachments/visitor-registration-attachments.component';
import { VisitorRegistrationPassComponent } from './visitor-registration/visitor-registration-pass/visitor-registration-pass.component';
import { VisitorRegistrationPaymentComponent } from './visitor-registration/visitor-registration-payment/visitor-registration-payment.component';
import { VisitorRegistrationSuccessComponent } from './visitor-registration/visitor-registration-success/visitor-registration-success.component';
import { VisitorPassVerificationComponent } from './visitor-pass-verification/visitor-pass-verification.component';
import { VisitorPassProfileComponent } from './visitor-pass/visitor-pass-profile/visitor-pass-profile.component';
import { VisitorPassComponent } from './visitor-pass/visitor-pass.component';
import { VisitorManageComponent } from './visitor-manage/visitor-manage.component';
import { VisitorRoutingModule } from './visitor-routing.module';
import { VerifyNicPassportComponent } from "../../shared/components/registration/verify-nic-passport/verify-nic-passport.component";
import { VisitorRegistrationOverviewComponent } from './visitor-registration/visitor-registration-overview/visitor-registration-overview.component';
import { VisitorRegistrationProfileComponent } from './visitor-registration/visitor-registration-profile/visitor-registration-profile.component';
import { AttachmentNicPassportComponent } from "../../shared/components/attachment/attachment-nic-passport/attachment-nic-passport.component";
import { AttachmentOtherComponent } from "../../shared/components/attachment/attachment-other/attachment-other.component";
import { VisitorRegistrationCounterComponent } from './visitor-registration/visitor-registration-counter/visitor-registration-counter.component';
import { VisitorViewComponent } from './visitor-manage/visitor-view/visitor-view.component';
import { VisitorViewProfileComponent } from './visitor-manage/visitor-view/visitor-view-profile/visitor-view-profile.component';
import { VisitorViewPassComponent } from './visitor-manage/visitor-view/visitor-view-pass/visitor-view-pass.component';
import { VisitorViewAttachmentComponent } from './visitor-manage/visitor-view/visitor-view-attachment/visitor-view-attachment.component';
import { VisitorViewActivityComponent } from './visitor-manage/visitor-view/visitor-view-activity/visitor-view-activity.component';
import { VisitorCancelRegistrationComponent } from './visitor-manage/visitor-view/visitor-view-profile/visitor-cancel-registration/visitor-cancel-registration.component';
import { VisitorBlacklistComponent } from './visitor-manage/visitor-view/visitor-view-profile/visitor-blacklist/visitor-blacklist.component';
import { QRCodeModule } from 'angularx-qrcode';
import { VisitorPassQrScanComponent } from './visitor-pass/visitor-pass-qr-scan/visitor-pass-qr-scan.component';
import { VisitorRegistrationPavilionComponent } from './visitor-registration/visitor-registration-pavilion/visitor-registration-pavilion.component';
import { VisitorPassVerificationPavilionComponent } from './visitor-pass-verification-pavilion/visitor-pass-verification-pavilion.component';
import { VisitorPavilionVerificationSelectionComponent } from './visitor-pass-verification-pavilion/visitor-pavilion-verification-selection/visitor-pavilion-verification-selection.component';
import { PavilionVisitorComponent } from './pavilion-visitor/pavilion-visitor.component';
import { VisitorPayLaterComponent } from './visitor-pass/visitor-pay-later/visitor-pay-later.component';
import { CounterSelectionComponent } from './visitor-pass/counter-selection/counter-selection.component';

@NgModule({
    declarations: [VisitorRegistrationAttachmentsComponent,
        VisitorRegistrationPaymentComponent, VisitorRegistrationPassComponent, VisitorRegistrationSuccessComponent, VisitorPassVerificationComponent, VisitorPassComponent, VisitorPassProfileComponent, VisitorManageComponent, VisitorRegistrationOverviewComponent, VisitorRegistrationProfileComponent, VisitorRegistrationCounterComponent, VisitorViewComponent, VisitorViewProfileComponent, VisitorViewPassComponent, VisitorViewAttachmentComponent, VisitorViewActivityComponent, VisitorCancelRegistrationComponent, VisitorBlacklistComponent, VisitorPassQrScanComponent, VisitorRegistrationPavilionComponent, VisitorPassVerificationPavilionComponent, VisitorPavilionVerificationSelectionComponent, PavilionVisitorComponent, VisitorPayLaterComponent, CounterSelectionComponent
    ],
    imports: [
        CommonModule,
        SharedModule,
        VisitorRoutingModule,
        VerifyNicPassportComponent,
        AttachmentNicPassportComponent,
        AttachmentOtherComponent,
        QRCodeModule
    ]
})
export class VisitorModule { }
