import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { TeamMemberService } from 'src/app/modules/team-member/services/team-member.service';
import { VisitorsService } from 'src/app/modules/visitor/services/visitors.service';
import { MaterialModule } from 'src/app/shared/material.module';
import { FileModel } from 'src/app/shared/models/file.model';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'facets-attachment-nic-passport',
  templateUrl: './attachment-nic-passport.component.html',
  styleUrls: ['./attachment-nic-passport.component.scss'],
  standalone: true,
  imports: [CommonModule, SharedModule, MaterialModule]
})
export class AttachmentNicPassportComponent {

  files: File[] = [];

  toasterService = inject(ToasterService);
  visitorsSerice = inject(VisitorsService);
  teamMemberSerice = inject(TeamMemberService);

  @Input() visitorId: string = '';
  @Input() teamMemberId: string = '';
  @Input() isUpdated: boolean = false;
  @Input() nicFileModels = new Array<FileModel>();
  @Input() fileDatas = new Array<any>();
  @Input() isAttachmentView:boolean;
  @Output() fileEmitter = new EventEmitter<File[]>();

  onFileChange(files: File[]) {
    for (let file of files) {
      if (!this.isFileExtensionAllowed(file)) this.toasterService.warning(`${file.name} is not a supported file`)
      else if (!this.isUpdated && !this.files.some(i => i.name == file.name)) {
        this.addFile(file);
      }
      else if (this.isUpdated && (!this.files.some(i => i.name == file.name) && !this.nicFileModels.some(i => i.fileName == file.name))) {
        this.addFile(file);
      }
    }
    this.emitFiles();
  }

  addFile(file: File) {
    this.files.push(file);
    const imageUrl = URL.createObjectURL(file);
    this.fileDatas.push({ name: file.name, img: imageUrl });
  }

  emitFiles() {
    this.fileEmitter.emit(this.files);
  }

  clearFiles(event: Event) {
    (event.target as HTMLInputElement).value = '';
  }

  onMultiFileChange(event: Event) {
    const files = Array.from((event.target as HTMLInputElement).files!);
    this.onFileChange(files);
  }

  isFileExtensionAllowed(file: File) {
    const selectedExtension = file.name.split('.').pop();
    return appConstant.fileExtensions.includes("." + selectedExtension);
  }

  remove(index: number, fileName: string) {
    if (this.isUpdated) {
      const file = this.nicFileModels.find(f => f.fileName == fileName)!;
      if (file != undefined) {
        if (this.visitorId != '') {
          this.deleteVisitorAttachment(file, index);
        }
        else if (this.teamMemberId != '') {
          this.deleteTeamMemberAttachment(file, index);
        }
      } else {
        this.spliceFile(fileName);
      }
    }
    else {
      this.spliceFile(fileName);
    }
  }

  spliceFile(fileName: string) {
    const selectedIndex = this.files.findIndex(f => f.name == fileName);
    if (selectedIndex != -1)
      this.files.splice(selectedIndex, 1);
    this.spliceFileData(fileName)
  }

  spliceFileData(fileName: string) {
    const index = this.fileDatas.findIndex(f => f.name == fileName);
    if (index != -1)
      this.fileDatas.splice(index, 1);
    this.emitFiles();
  }

  deleteVisitorAttachment(file: FileModel, index: number) {
    this.visitorsSerice.deleteAttachment(this.visitorId, file.id).subscribe({
      next: () => {
        this.nicFileModels.splice(index, 1);
        this.spliceFile(file.fileName);
        this.toasterService.successfullyDeleted("NIC attachment")
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
      }
    });
  }

  deleteTeamMemberAttachment(file: FileModel, index: number) {
    this.teamMemberSerice.deleteTeamMemberDocument(this.teamMemberId, file.id).subscribe({
      next: () => {
        this.nicFileModels.splice(index, 1);
        this.spliceFile(file.fileName);
        this.toasterService.successfullyDeleted("NIC attachment")
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
      }
    });
  }
}

