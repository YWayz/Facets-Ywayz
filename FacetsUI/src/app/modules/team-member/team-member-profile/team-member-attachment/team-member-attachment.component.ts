import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, inject } from '@angular/core';
import { TeamMemberService } from '../../services/team-member.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { TeamMemberDocumentModel } from '../../models/team-member-document.model';
import { SuperAdminPermissions, TeamMemberRegistrationPermissions } from 'src/app/core/extensions/permission-constants';
import { FileModel } from 'src/app/shared/models/file.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { Router } from '@angular/router';

@Component({
  selector: 'facets-team-member-attachment',
  templateUrl: './team-member-attachment.component.html',
  styleUrls: ['./team-member-attachment.component.scss']
})
export class TeamMemberAttachmentComponent implements OnInit {

  isBlocked = false;

  superAdminPermissions = SuperAdminPermissions;
  teamMemberRegistrationPermissions = TeamMemberRegistrationPermissions;

  teamMemberDocumentModel = new Array<TeamMemberDocumentModel>();
  nicFileList: File[] = [];
  otherFileList: File[] = [];

  nicFileDatas = new Array<any>();
  nicFileModels = new Array<FileModel>();
  otherFileDatas = new Array<any>();
  otherFileModels = new Array<FileModel>();

  teamMemberService = inject(TeamMemberService);
  toasterService = inject(ToasterService);
  router = inject(Router);

  @Input() teamMemberId: string;

  ngOnInit(): void {
    if (this.teamMemberId !== null) {
      this.isBlocked = true;
      this.getTeamMemberAttachments();
    }
  }

  getTeamMemberAttachments() {
    this.teamMemberService.getTeamMemberDocuments(this.teamMemberId).subscribe({
      next: (res: ResponseResult<FileModel[]>) => {
        this.nicFileModels = [];
        this.nicFileDatas = [];
        
        this.otherFileModels = [];
        this.otherFileDatas = [];

        this.nicFileModels = res.data;
        this.nicFileModels.forEach(file => {
          if (file.extenstionData?.attachmentType == 'NIC') {            
            this.nicFileDatas.push({name: file.fileName, img: file.uri});
          }
          else if (file.extenstionData?.attachmentType == 'OtherAttachment') {       
            this.otherFileModels.push(file);
            this.otherFileDatas.push({name: file.fileName, img: file.uri});
          }
        });
        this.isBlocked = false;
      },
    })
  }

  getNICFiles(files: File[]) {
    this.nicFileList = files;
  }

  getOtherFiles(files: File[]) {
    this.otherFileList = files;
  }

  uploadNicImage(teamMemberId: string) {
    this.isBlocked = true;
    const formData = new FormData();
    this.InitializeNicAttachment(formData);
    
    this.InitializeOtherAttachment(formData);
    
    this.teamMemberService.uploadDocument(teamMemberId, formData)
      .subscribe({
        next: () => {
          this.nicFileList = [];
          this.otherFileList = [];

          this.nicFileDatas = [];
          this.otherFileDatas = [];
          
          this.getTeamMemberAttachments();
          this.toasterService.success("Attachment has been uploaded successfully.");

          this.isBlocked = false;
          // this.router.navigate([`admin/team-member/${teamMemberId}`]);
        },
        error: (err: ErrorResponse) => {
          this.isBlocked = false;
          this.toasterService.error(err);
        }
      });
  }
  private InitializeNicAttachment(formData: FormData) {
    for (let index = 0; index < this.nicFileList.length; index++) {
      formData.append(`files[${index}].key`, 'nic');
      formData.append(`files[${index}].value`, this.nicFileList[index]);
    }
  }

  private InitializeOtherAttachment(formData: FormData) {
    for (let index = 0; index < this.otherFileList.length; index++) {
      formData.append(`files[${this.nicFileList.length + index}].key`, 'otherAttachment');
      formData.append(`files[${this.nicFileList.length + index}].value`, this.otherFileList[index]);
    }
  }

}
