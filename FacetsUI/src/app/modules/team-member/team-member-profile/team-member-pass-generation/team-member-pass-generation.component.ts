import { Component, inject } from '@angular/core';
import { TeamMemberPassTemplateModel } from '../../models/team-member-pass-template.model';
import { ActivatedRoute, Params } from '@angular/router';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { TeamMemberPassService } from '../../services/team-member-pass.service';
import { SafeUrl } from '@angular/platform-browser';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { CommonModule } from '@angular/common';
import { QRCodeModule } from 'angularx-qrcode';
import { fabric } from "fabric";
import { SharedModule } from 'src/app/shared/shared.module';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'facets-team-member-pass-generation',
  templateUrl: './team-member-pass-generation.component.html',
  styleUrls: ['./team-member-pass-generation.component.scss'],
  standalone: true,
  imports: [SharedModule, CommonModule, QRCodeModule]
})
export class TeamMemberPassGenerationComponent {
  isBlocked = false;
  qrCodeSrc: string;
  teamMemberId: string;
  eventDateId: string;
  passVerificationJson: string;

  teamMemberPassTemplateModel = new TeamMemberPassTemplateModel();

  activatedRoute = inject(ActivatedRoute);
  teamMemberPassService = inject(TeamMemberPassService);
  toasterService = inject(ToasterService);

  constructor() {
    this.activatedRoute.params.subscribe({
      next: (params: Params) => {
        this.teamMemberId = params['teamMemberId'];
        if (this.teamMemberId != undefined)
          this.passVerificationJson = JSON.stringify({ eventId: localStorage.getItem(appConstant.selectedEventId), teamMemberId: this.teamMemberId, type: 'teamMember' });
      }
    });
  }

  getTeamMemberPassTemplate(teamMemberId: string) {
    this.isBlocked = true;
    this.teamMemberPassService.getTemplate(teamMemberId)
      .subscribe({
        next: (res: ResponseResult<TeamMemberPassTemplateModel>) => {
          this.isBlocked = false;
          this.teamMemberPassTemplateModel = res.data;

          let canvasFront = new fabric.Canvas(document.getElementById('canvasFront') as any);
          canvasFront.setDimensions({
            height: this.teamMemberPassTemplateModel.height,
            width: this.teamMemberPassTemplateModel.width
          });

          const context = canvasFront.getContext();
          // var yy=this.qrCodeSrc;
          // console.log("Genarated : QR"+yy);
          // var xx=this.teamMemberPassTemplateModel.template.replace(`https://exhibition.facetssrilanka.com/assets/images/qr-code.png`, this.qrCodeSrc);
          // console.log("Selected : QR"+xx);
          
         // this.teamMemberPassTemplateModel.template = this.teamMemberPassTemplateModel.template.replace(`${environment.baseWebEndPoint}/assets/images/qr-code.png`, this.qrCodeSrc);
          this.teamMemberPassTemplateModel.template = this.teamMemberPassTemplateModel.template.replace(`https://exhibition.facetssrilanka.com/assets/images/qr-code.png`, this.qrCodeSrc);
          // console.log("QR"+this.qrCodeSrc);
          // console.log("QR"+'${environment.baseWebEndPoint}+"/assets/images/qr-code.png');
          // console.log("QR"+environment.baseWebEndPoint);
          let jsonObj = JSON.parse(this.teamMemberPassTemplateModel.template);
          const groupObj = jsonObj.objects.filter((f: any) => f.type == 'group');
          if (groupObj.length > 0) {
            groupObj.forEach((groupedElement: any) => {
              const passCategoryObj = groupedElement.objects.find((f: any) => f.type == 'text' && f.text == 'Pass Category')
              if (passCategoryObj) {
                passCategoryObj.fill = 'white'
                passCategoryObj.text = this.teamMemberPassTemplateModel.passCategoryName.toUpperCase();

                const shapeObj = groupedElement.objects.filter((f: any) => f.type == 'rect')[0];
                if (shapeObj) {
                  shapeObj.fill = this.teamMemberPassTemplateModel.passCategoryColor;
                }
              }

              const boxWidth = 347
              const fullNameObj = groupedElement.objects.find((f: any) => f.type == 'text' && f.text == "Full Name")
              if (fullNameObj) {
                fullNameObj.text = this.teamMemberPassTemplateModel.fullName;
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

          this.teamMemberPassTemplateModel.template = JSON.stringify(jsonObj);
          canvasFront.loadFromJSON(this.teamMemberPassTemplateModel.template, () => { });
          this.isBlocked = false;
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      });
  }

  onChangeURL(url: SafeUrl) {
   // console.log(url);
    this.qrCodeSrc = (url as any).changingThisBreaksApplicationSecurity;
    if (this.teamMemberId != undefined)
     // console.log("TMid"+this.teamMemberId);
      this.getTeamMemberPassTemplate(this.teamMemberId);
  }

  print() {
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
