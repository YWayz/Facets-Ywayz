import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LayoutComponent } from './modules/layout/layout/layout.component';
import { ScanMeComponent } from './modules/qr-scan/scan-me/scan-me.component';

const routes: Routes = [
  {
    path: 'qr',
    component: LayoutComponent,
    children: [
      {
        path: 'scanner',
        loadChildren: () => import('./modules/qr-scan/qr-scan.module').then(t => t.QrScanModule)
      },
    ]
  },
  {
    path: '', 
    component: ScanMeComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
