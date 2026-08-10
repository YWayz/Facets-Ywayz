import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthorizationDirective } from "./directives/authorization.directive";
import { BlockUiComponent } from './modals/block-ui/block-ui.component';
import { MaterialModule } from './material.module';
import { ConfirmationModalComponent } from './modals/confirmation-modal/confirmation-modal.component';
import { CamCaptureComponent } from './components/cam-capture/cam-capture.component';
import { WebcamModule } from 'ngx-webcam';
import { ImageAttachmentCaptureComponent } from './components/image-attachment-capture/image-attachment-capture.component';
import { TwoDigitDecimalNumberDirective } from './directives/two-digit-decimal-number.directive';
import { NoRecordsComponent } from './components/no-records/no-records.component';
import { AccessDeniedComponent } from './components/access-denied/access-denied.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { FileDragDropDirective } from './directives/file-drag-drop.directive';
import { MultiImageAttachmentCaptureComponent } from './components/multi-image-attachment-capture/multi-image-attachment-capture.component';
import { ZXingScannerModule } from '@zxing/ngx-scanner';
import { QrScannerComponent } from './components/qr-scanner/qr-scanner.component';
import { LOAD_WASM, NgxScannerQrcodeModule } from 'ngx-scanner-qrcode';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';
import { ColorPickerModule } from 'ngx-color-picker';
import { ImageCropperModule } from 'ngx-image-cropper';

LOAD_WASM().subscribe((res: any) => { });

@NgModule({
  declarations: [
    BlockUiComponent,
    ConfirmationModalComponent,
    CamCaptureComponent,
    ImageAttachmentCaptureComponent,
    TwoDigitDecimalNumberDirective,
    NoRecordsComponent,
    AuthorizationDirective,
    AccessDeniedComponent,
    PageNotFoundComponent,
    FileDragDropDirective,
    MultiImageAttachmentCaptureComponent,
    QrScannerComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    ReactiveFormsModule,
    RouterModule,
    MaterialModule,
    WebcamModule,
    ZXingScannerModule,
    NgxScannerQrcodeModule,
    TimepickerModule.forRoot(),
    ColorPickerModule,
    ImageCropperModule
  ],
  exports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    ReactiveFormsModule,
    RouterModule,
    MaterialModule,
    BlockUiComponent,
    ConfirmationModalComponent,
    CamCaptureComponent,
    ImageAttachmentCaptureComponent,
    TwoDigitDecimalNumberDirective,
    NoRecordsComponent,
    AuthorizationDirective,
    AccessDeniedComponent,
    PageNotFoundComponent,
    FileDragDropDirective,
    MultiImageAttachmentCaptureComponent,
    ZXingScannerModule,
    QrScannerComponent,
    NgxScannerQrcodeModule,
    TimepickerModule,
    ColorPickerModule,
    ImageCropperModule
  ]
})
export class SharedModule { }
