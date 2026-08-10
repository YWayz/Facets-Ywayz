import { Component, OnInit, inject } from '@angular/core';
import { SharedService } from 'src/app/core/services/shared.service';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { FileModel } from 'src/app/shared/models/file.model';
import { VisitorsService } from '../../../services/visitors.service';
import { ActivatedRoute, Params } from '@angular/router';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';

@Component({
  selector: 'facets-visitor-view-attachment',
  templateUrl: './visitor-view-attachment.component.html',
  styleUrls: ['./visitor-view-attachment.component.scss']
})
export class VisitorViewAttachmentComponent implements OnInit {

  isBlocked = false;
  visitorId: string;

  fileModel = new Array<FileModel>();
  nicFileList = new Array<File>();
  otherFileList = new Array<File>();
  nicFileModels = new Array<FileModel>();
  otherFileModels = new Array<FileModel>();
  nicFileDatas = new Array<any>();
  otherFileDatas = new Array<any>();

  toasterService = inject(ToasterService);
  visitorsService = inject(VisitorsService);
  activatedRoute = inject(ActivatedRoute);

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((params: Params) => {
      this.visitorId = params['visitorId']
      if (this.visitorId != null || this.visitorId != undefined || this.visitorId != '') {
        this.getVisitorAttachment();
      }
    });
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
            this.nicFileDatas.push({ name: file.fileName, img: file.uri });
          }
          else if (file.extenstionData?.attachmentType == 'OtherAttachment') {
            this.otherFileModels.push(file);
            this.otherFileDatas.push({ name: file.fileName, img: file.uri });
          }
        });
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
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

    this.visitorsService.uploadDocument(this.visitorId, formData)
      .subscribe({
        next: () => {
          this.isBlocked = false;
          this.toasterService.success("Attachments has been uploaded successfully");
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      });
  }
}
