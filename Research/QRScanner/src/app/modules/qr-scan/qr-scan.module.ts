import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { QrScanRoutingModule } from './qr-scan-routing.module';
import { SharedModule } from '../shared/shared.module';
import { QrScannerComponent } from './qr-scanner/qr-scanner.component';
import { ScanMeComponent } from './scan-me/scan-me.component';


@NgModule({
  declarations: [
    QrScannerComponent,
    ScanMeComponent
  ],
  imports: [
    CommonModule,
    QrScanRoutingModule,
    SharedModule
  ]
})
export class QrScanModule { }
