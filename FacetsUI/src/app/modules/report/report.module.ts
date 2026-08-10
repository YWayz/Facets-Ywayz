import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from 'src/app/shared/shared.module';
import { ReportVisitorListComponent } from './report-visitor-list/report-visitor-list.component';
import { ReportCollectionComponent } from './report-collection/report-collection.component';
import { ReportRoutingModule } from './report-routing.module';
import { AttendanceReportComponent } from './attendance-report/attendance-report/attendance-report.component';



@NgModule({
  declarations: [ReportCollectionComponent, ReportVisitorListComponent, AttendanceReportComponent],
  imports: [
    CommonModule,
    SharedModule,
    ReportRoutingModule
  ]
})
export class ReportModule { }
