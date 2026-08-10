import { Component, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
import { dataURItoBlob, generateRandomId } from 'src/app/core/extensions/helpers';
import { ModalService } from 'src/app/core/services/modal.service';
import { CamCaptureComponent } from '../cam-capture/cam-capture.component';
import { SharedService } from 'src/app/core/services/shared.service';

@Component({
  selector: 'facets-multi-image-attachment-capture',
  templateUrl: './multi-image-attachment-capture.component.html',
  styleUrls: ['./multi-image-attachment-capture.component.scss']
})
export class MultiImageAttachmentCaptureComponent implements OnInit {

  fileList: File[] = [];

  modalService = inject(ModalService);
  sharedService = inject(SharedService);

  @Output() fileEmitter = new EventEmitter<File[]>();
  @Input() imagePath: string = '';

  ngOnInit(): void {
  }

  openCamera() {
    let options = {
      isOpenCamera: true,
    };
    this.modalService.displayDialog(CamCaptureComponent, options);
    this.modalService.confirmed().subscribe((res: any) => {
      if (res) {
        this.fileList = [];
        const imageBlob = dataURItoBlob(res.image);
        const imageFile = new File([imageBlob], `${generateRandomId()}.jpg`, { type: 'image/jpg' });

        this.imagePath = URL.createObjectURL(imageFile);
        this.fileList.push(imageFile);
        this.emitImage();
      }
    });
  }

  emitImage() {
    this.fileEmitter.emit(this.fileList);
  }
}
