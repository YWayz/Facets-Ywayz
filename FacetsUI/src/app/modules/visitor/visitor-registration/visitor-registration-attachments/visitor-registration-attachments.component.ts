import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, inject } from '@angular/core';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { VisitorsService } from '../../services/visitors.service';
import { SharedService } from 'src/app/core/services/shared.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { FileModel } from 'src/app/shared/models/file.model';

@Component({
  selector: 'facets-visitor-registration-attachments',
  templateUrl: './visitor-registration-attachments.component.html',
  styleUrls: ['./visitor-registration-attachments.component.scss']
})
export class VisitorRegistrationAttachmentsComponent implements OnInit, OnDestroy {

  isBlocked = false;
  fileModel = new Array<FileModel>();
  nicFileList = new Array<File>();
  otherFileList = new Array<File>();
  nicFileModels = new Array<FileModel>();
  otherFileModels = new Array<FileModel>();
  nicFileDatas = new Array<any>();
  otherFileDatas = new Array<any>();

  toasterService = inject(ToasterService);
  visitorsService = inject(VisitorsService);
  sharedService = inject(SharedService);

  @Input() visitorId: string;
  @Output() isPass = new EventEmitter<string>();
  @Output() isProfile = new EventEmitter<string>();

  ngOnInit(): void {
    if (this.sharedService.isUpdateAttachment) {
      this.getVisitorAttachment();
    }
  }

  getNICFiles(files: File[]) {
    this.nicFileList = files;
  }

  getOtherFiles(files: File[]) {
    this.otherFileList = files;
  }

  getVisitorAttachment() {
    this.isBlocked = true;
    this.visitorsService.getVisitorDocuments(this.visitorId, 'attachmentTypes=nic&attachmentTypes=otherAttachment').subscribe({
      next: (result: ResponseResult<FileModel[]>) => {
        this.isBlocked = false;
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
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  back() {
    this.isProfile.emit(this.visitorId);
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
      this.visitorsService.uploadDocument(this.visitorId, formData)
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
