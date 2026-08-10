import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, inject } from '@angular/core';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { SharedService } from 'src/app/core/services/shared.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { FileModel } from 'src/app/shared/models/file.model';
import { VisitorsService } from 'src/app/modules/visitor/services/visitors.service';
import { SharedModule } from 'src/app/shared/shared.module';
import { AttachmentNicPassportComponent } from "../../../shared/components/attachment/attachment-nic-passport/attachment-nic-passport.component";
import { AttachmentOtherComponent } from "../../../shared/components/attachment/attachment-other/attachment-other.component";
import { PublicSiteService } from '../../services/public-site.service';
import { VisitorSearchModel } from 'src/app/modules/visitor/models/visitor-search.model';
import { VisitorVerificationModel } from '../../models/visitor-verification.model';
import { Router } from '@angular/router';
import { VisitorRegisterModel } from 'src/app/modules/visitor/models/visitor-register.model';
import { VisitorModel } from 'src/app/modules/visitor/models/visitor.model';

@Component({
    selector: 'facets-visitor-registration-attachments',
    templateUrl: './visitor-registration-attachments.component.html',
    styleUrls: ['./visitor-registration-attachments.component.scss'],
    standalone: true,
    imports: [SharedModule, AttachmentNicPassportComponent, AttachmentOtherComponent]
})
export class VisitorRegistrationAttachmentsComponent implements OnInit, OnDestroy {

  isBlocked = false;
  fileModel = new Array<FileModel>();
  nicFileList = new Array<File>();
  otherFileList = new Array<File>();
  nicFileModels = new Array<FileModel>();
  nicAttachmentViewOnly = false;
  
  otherFileModels = new Array<FileModel>();
  nicFileDatas = new Array<any>();
  otherFileDatas = new Array<any>();
  otherAttachmentViewOnly = false;

  identityNumber: string;

  toasterService = inject(ToasterService);
  publicSiteService = inject(PublicSiteService);
  sharedService = inject(SharedService);
  router = inject(Router);

  @Input() visitorId: string;
  @Output() isPass = new EventEmitter<string>();
  @Output() isProfile = new EventEmitter<string>();
  @Output() isOtp = new EventEmitter<VisitorVerificationModel>();

  ngOnInit(): void {
    // if (this.sharedService.isUpdateAttachment) {
      this.getVisitorAttachment();
    // }
  }

  getNICFiles(files: File[]) {
    this.nicFileList = files;
  }

  getOtherFiles(files: File[]) {
    this.otherFileList = files;
  }

  getVisitorAttachment() {
    this.isBlocked = true;
    this.publicSiteService.getVisitorDocuments(this.visitorId, 'attachmentTypes=nic&attachmentTypes=otherAttachment').subscribe({
      next: (result: ResponseResult<FileModel[]>) => {
        this.isBlocked = false;
        this.getVisitorById();
        this.fileModel = result.data;
        this.fileModel.forEach(file => {
          if (file.extenstionData?.attachmentType == 'NIC') {
            this.nicFileModels.push(file);
            this.nicFileDatas.push({name: file.fileName, img: file.uri});
          }
          else if (file.extenstionData?.attachmentType == 'OtherAttachment') {
            this.otherFileModels.push(file);
            this.otherFileDatas.push({name: file.fileName, img: file.uri});
          }
        });

        if (this.nicFileDatas.length > 0) {
          this.nicAttachmentViewOnly = true;
        }

        if(this.otherFileDatas.length > 0) {
          this.otherAttachmentViewOnly = true;
        }
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }
  
  getVisitorById() {
    this.isBlocked = true;
    this.publicSiteService.getById(this.visitorId).subscribe({
      next: (result: ResponseResult<VisitorModel>) => {
        this.isBlocked = false;
        this.identityNumber = result.data.nicNumber != '' || result.data.nicNumber!= undefined ? result.data.nicNumber! : result.data.passportNumber!;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  back() {
    this.isProfile.emit(this.identityNumber);
  }

  saveAttachment() {
    this.isBlocked = true;
    const formData = new FormData();
    for (let index = 0; index < this.nicFileList.length; index++) {
      formData.append(`files[${index}].key`, 'nic');
      formData.append(`files[${index}].value`, this.nicFileList[index]);
    }

    for (let index = 0; index < this.otherFileList.length; index++) {
      formData.append(`files[${this.nicFileList.length + index}].key`, 'otherAttachment');
      formData.append(`files[${this.nicFileList.length + index}].value`, this.otherFileList[index]);
    }

    if (this.nicFileList.length === 0 && this.otherFileList.length === 0) this.isPass.emit(this.visitorId);
    else {
      this.publicSiteService.uploadDocument(this.visitorId, formData)
        .subscribe({
          next: () => {
            this.isBlocked = false;
            this.sharedService.isUpdateAttachment = true;
            this.isPass.emit(this.visitorId);
          },
          error: (err: ErrorResponse) => {
            this.isBlocked = false;
            this.toasterService.error(err);
          }
        });
    }
  }

  ngOnDestroy(): void {
    this.sharedService.visitorRegistrationModel = undefined;
  }
}
