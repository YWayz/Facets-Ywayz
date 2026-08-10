import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReportCollectionComponent } from './report-collection/report-collection.component';
import { ReportVisitorListComponent } from './report-visitor-list/report-visitor-list.component';
import { AttendanceReportComponent } from './attendance-report/attendance-report/attendance-report.component';

const routes: Routes = [
  {
    path: 'visitor',
    component: ReportVisitorListComponent
  },
  {
    path: 'collection',
    component: ReportCollectionComponent
  },
  {
    path: 'attendance',
    component: AttendanceReportComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportRoutingModule { }
