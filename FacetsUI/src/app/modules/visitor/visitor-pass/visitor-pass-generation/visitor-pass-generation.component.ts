import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { VisitorPassService } from '../../services/visitor-pass.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { VisitorPassTemplateModel } from '../../models/visitor-pass-template.model';
import { fabric } from "fabric";
import { SharedModule } from 'src/app/shared/shared.module';
import { CommonModule } from '@angular/common';
import { QRCodeModule } from 'angularx-qrcode';
import { SafeUrl } from '@angular/platform-browser';
import { PassVerificationModel } from '../../models/pass-verification.model';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'facets-visitor-pass-generation',
  templateUrl: './visitor-pass-generation.component.html',
  styleUrls: ['./visitor-pass-generation.component.scss'],
  standalone: true,
  imports: [SharedModule, CommonModule, QRCodeModule]
})
export class VisitorPassGenerationComponent {

  isBlocked = false;
  qrCodeSrc: string;
  visitorId: string;
  eventDateId: string;
  attendanceScheduleId: string;
  passVerificationJson: string;

  visitorPassTemplateModel = new VisitorPassTemplateModel();
  visitorPassVerificationModel = new PassVerificationModel();


  activatedRoute = inject(ActivatedRoute);
  visitorPassService = inject(VisitorPassService);
  toasterService = inject(ToasterService);

  constructor() {
    this.activatedRoute.params.subscribe({
      next: (params: Params) => {
        this.visitorId = params['visitorId'];
        this.attendanceScheduleId = params['attendanceScheduleId'];
        this.eventDateId = params['eventDateId'];

        if (this.visitorId != undefined && this.eventDateId != undefined)
          this.getVisitorPassVerificationDetails()
      }
    });
  }

  getVisitorPassTemplate(visitorId: string, eventDateId: string) {
    this.isBlocked = true;
    this.visitorPassService.getTemplate(visitorId, eventDateId)
      .subscribe({
        next: (res: ResponseResult<VisitorPassTemplateModel>) => {
          this.visitorPassTemplateModel = res.data;

          let canvasFront = new fabric.Canvas(document.getElementById('canvasFront') as any);
          canvasFront.setDimensions({
            height: this.visitorPassTemplateModel.height,
            width: this.visitorPassTemplateModel.width
          });

          const context = canvasFront.getContext();

          //this.visitorPassTemplateModel.template = this.visitorPassTemplateModel.template.replace(`${environment.baseWebEndPoint}/assets/images/qr-code.png`, this.qrCodeSrc)
          this.visitorPassTemplateModel.template = this.visitorPassTemplateModel.template.replace(`https://exhibition.facetssrilanka.com/assets/images/qr-code.png`, this.qrCodeSrc)
          let jsonObj = JSON.parse(this.visitorPassTemplateModel.template);

          const groupObj = jsonObj.objects.filter((f: any) => f.type == 'group');
          if (groupObj.length > 0) {
            groupObj.forEach((groupedElement: any) => {
              const passCategoryObj = groupedElement.objects.find((f: any) => f.type == 'text' && f.text == 'Pass Category')
              if (passCategoryObj) {
                passCategoryObj.fill = 'white'
                passCategoryObj.text = this.visitorPassTemplateModel.passCategoryName.toUpperCase();

                const shapeObj = groupedElement.objects.filter((f: any) => f.type == 'rect')[0];
                if (shapeObj) {
                  shapeObj.fill = this.visitorPassTemplateModel.passCategoryColor;
                }
              }

              const boxWidth = 347
              const fullNameObj = groupedElement.objects.find((f: any) => f.type == 'text' && f.text == "Full Name")
              if (fullNameObj) {
                fullNameObj.text = this.visitorPassTemplateModel.fullName;
                context.font = 'bold 24px Arial';
                const textWidth = context.measureText(fullNameObj.text).width;

                if (textWidth > boxWidth) {
                  const maxCharss = Math.floor(boxWidth / (textWidth / fullNameObj.text.length));
                  const fontSize = Math.floor(boxWidth / maxCharss);
                  fullNameObj.fontSize! = fontSize;
                }
              }
            });
          }

          this.visitorPassTemplateModel.template = JSON.stringify(jsonObj);
          canvasFront.loadFromJSON(this.visitorPassTemplateModel.template, () => { });
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })
  }

  getVisitorPassVerificationDetails() {
    this.visitorPassVerificationModel.attendanceSheduleId = this.attendanceScheduleId;
    this.visitorPassVerificationModel.visitorId = this.visitorId;
    this.visitorPassVerificationModel.type = 'visitor';
    this.passVerificationJson = JSON.stringify(this.visitorPassVerificationModel);
  }

  onChangeURL(url: SafeUrl) {
    this.qrCodeSrc = (url as any).changingThisBreaksApplicationSecurity;
    if (this.visitorId != undefined && this.eventDateId != undefined)
      this.getVisitorPassTemplate(this.visitorId, this.eventDateId);
  }

  print() {
    this.visitorPassService.updatePassPrintStatus(this.visitorId, this.attendanceScheduleId)
      .subscribe({
        next: (res: any) => {

        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      })


    var dataUrl = (document.getElementById('canvasFront') as HTMLCanvasElement)!.toDataURL();
    var windowContent = '<!DOCTYPE html>';
    windowContent += '<html>'
    windowContent += '<head><title>Print Pass</title></head>';
    windowContent += '<body onload="window.print();">'
    windowContent += '<img src="' + dataUrl + '">';
    windowContent += '</body>';
    windowContent += '</html>';

    var printWin = window.open('', '')!;
    printWin.document.open();
    printWin.document.write(windowContent);
    printWin.document.close();
  }
}
