import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { QrScannerComponent } from './qr-scanner/qr-scanner.component';

const routes: Routes = [
  {
    path: 'scan-me',
    component: QrScannerComponent 
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class QrScanRoutingModule { }
