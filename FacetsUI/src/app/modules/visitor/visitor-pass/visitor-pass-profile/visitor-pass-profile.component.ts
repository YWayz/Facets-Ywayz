import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { VisitorModel } from '../../models/visitor.model';
import { VisitorsService } from '../../services/visitors.service';
import { VisitorRegistrationService } from '../../services/visitor-registration.service';
import { FileModel } from 'src/app/shared/models/file.model';
import { VisitorPassRegistrationModel } from '../../models/visitor-pass-registration.model';
import { combineLatest, forkJoin, map } from 'rxjs';
import { InvoiceService } from '../../services/invoice.service';
import { InvoicePaymentModel } from '../../models/invoice-payment.model';
import { PassGenerationPermissions, VisitorRegistrationPermissions } from 'src/app/core/extensions/permission-constants';
import { ResponseResult } from 'src/app/core/models/response-result.model';

@Component({
  selector: 'facets-visitor-pass-profile',
  templateUrl: './visitor-pass-profile.component.html',
  styleUrls: ['./visitor-pass-profile.component.scss']
})
export class VisitorPassProfileComponent implements OnInit {

  isBlocked = false;
  visitorId: string;
  eventDateId: string;
  attendanceScheduleId: string;
  visitorRegistrationId: string;
  passGenerationPermissions = PassGenerationPermissions
  pavilionSessions = new Array<any>();

  private _visitorModel = new VisitorModel();
  public get visitorModel() {
    return this._visitorModel;
  }
  public set visitorModel(value) {
    this._visitorModel = value;
  }
  registrationInvoice = new InvoicePaymentModel();
  fileAttachmentModel = new Array<FileModel>();
  fileProfileImageModel = new Array<FileModel>();
  visitorPassRegistration = new VisitorPassRegistrationModel();
  nicFileModels = new Array<FileModel>();
  otherFileModels = new Array<FileModel>();
  nicFileDatas = new Array<any>();
  otherFileDatas = new Array<any>();
  isAttachmentView = true;

  visitorsService = inject(VisitorsService);
  toasterService = inject(ToasterService);
  activatedRoute = inject(ActivatedRoute);
  visitorRegistrationService = inject(VisitorRegistrationService);
  invoiceService = inject(InvoiceService);
  router = inject(Router);

  ngOnInit(): void {
    this.pavilionSessions = []
    combineLatest([this.activatedRoute.params, this.activatedRoute.queryParams], (params: Params, qParams: Params) => ({ params, qParams })).subscribe({
      next: (result) => {
        this.visitorId = result.params['visitorId'];
        this.attendanceScheduleId = result.params['attendanceScheduleId'];
        this.visitorRegistrationId = result.params['registrationId'];
        this.eventDateId = result.qParams['eventDateId'];
        if (this.visitorId != undefined && this.eventDateId != undefined) {
          this.getPassGenerationDetails();
        }
      }
    });
  }

  getPassGenerationDetails() {
    this.isBlocked = true;
    const visitorProfile = this.visitorsService.getById(this.visitorId);
    const visitorAttachment = this.visitorsService.getVisitorDocuments(this.visitorId, 'attachmentTypes=nic&attachmentTypes=otherAttachment');
    const visitorPassRegistration = this.visitorRegistrationService.getVisitorPassRegistration(this.visitorId, this.visitorRegistrationId, this.eventDateId);
    const visitorProfileImage = this.visitorsService.getVisitorDocuments(this.visitorId, 'attachmentTypes=profileImage');
    // const registrationInvoices = this.invoiceService.getInvoicePaymentByRegistration(this.visitorId, this.eventDateId)

    forkJoin([visitorProfile, visitorAttachment, visitorPassRegistration, visitorProfileImage]).subscribe({
      next: (result: any) => {
        this.visitorModel = result[0].data;
        this.fileAttachmentModel = result[1].data;
        this.visitorPassRegistration = result[2].data;
        this.fileProfileImageModel = result[3].data;

        this.visitorModel.imageUrl = this.fileProfileImageModel[this.fileProfileImageModel.length - 1].uri;
        this.fileAttachmentModel.forEach(file => {
          if (file.extenstionData?.attachmentType == 'NIC') {
            this.nicFileModels.push(file);
            this.nicFileDatas.push({ name: file.fileName, img: file.uri });
          }
          else if (file.extenstionData?.attachmentType == 'OtherAttachment') {
            this.otherFileModels.push(file);
            this.otherFileDatas.push({ name: file.fileName, img: file.uri });
          }
        });

        const groupedPavilion = this.groupBy(this.visitorPassRegistration.visitorPavilionSessionAttendanceSchedules, 'pavilionName');

        Object.entries(groupedPavilion).forEach(([key, value]) => {
          this.pavilionSessions.push({ pavilionName: key, pavilionSessions: value });
        });

        if (this.visitorPassRegistration.attendanceSchedules.length) {
          this.invoiceService.getInvoicePaymentByRegistration(this.visitorId, this.eventDateId).subscribe({
            next: (res: ResponseResult<InvoicePaymentModel>) => {
              this.registrationInvoice = res.data;
            },
          })
        }


        this.isBlocked = false;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });


  }

  groupBy(array: any[], property: string): { [key: string]: any[] } {
    return array.reduce((acc, obj) => {
      const key = obj[property];
      if (!acc[key]) {
        acc[key] = [];
      }
      acc[key].push(obj);
      return acc;
    }, {});
  }

  goToPassGeneration() {
    this.router.navigate(['admin/visitor/pass-generation'], { queryParams: { nicPassport: this.visitorModel.nicNumber == null ? this.visitorModel.passportNumber : this.visitorModel.nicNumber } });
  }

  goToPrint() {
    this.router.navigate(['admin/visitor/pass-generation/print', this.visitorId, this.eventDateId, this.attendanceScheduleId]);
  }
}
