import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChange, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { take } from 'rxjs';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { dataURItoBlob } from 'src/app/core/extensions/helpers';
import { CamCaptureService } from 'src/app/core/services/cam-capture.service';
import { ToasterService } from 'src/app/core/services/toaster.service';

@Component({
  selector: 'facets-image-attachment-capture',
  templateUrl: './image-attachment-capture.component.html',
  styleUrls: ['./image-attachment-capture.component.scss']
})
export class ImageAttachmentCaptureComponent implements OnInit {

  imageChangedEvent: any = '';
  croppedImage: any = '';
  base64Image: string = '';
  isShow: boolean = true;

  file: File;
  fileList: File[] = [];

  camCaptureService = inject(CamCaptureService);
  toasterService = inject(ToasterService);
  router = inject(Router);

  @Output() getfiles = new EventEmitter<File[]>();
  @Input() imagePath: string = '';

  ngOnInit(): void {
    this.isShow = !(this.router.url.startsWith('/admin/event/edit') || this.router.url.startsWith('/admin/event/create'));
  }

  openCamera() {
    let options = {
      isOpenCamera: true,
    };
    this.camCaptureService.open(options);
    this.camCaptureService.confirmed().subscribe((res: any) => {
      if (res) {
        if (!this.isShow) {
          const imageBlob = dataURItoBlob(res.image);
          const imageFile = new File([imageBlob], 'event.png', { type: 'image/png' });
          this.imagePath = URL.createObjectURL(imageFile);
          this.fileList = [];
          this.fileList.push(imageFile);
          this.emitImage();
        }
        else {
          this.imageChangedEvent = '';
          this.base64Image = res.image;
        }
      }
    });
  }

  fileChangeEvent(event: any): void {
    this.file = event.target.files[0];

    if (!this.isShow) {
      if (!this.isFileExtensionAllowed(this.file)) {
        this.toasterService.warning(`${this.file.name} is not a supported file`);
        this.fileList = [];
        this.emitImage();
      }
      else {
        this.imagePath = URL.createObjectURL(event.target.files[0]);
        this.fileList = [];
        this.fileList.push(this.file);
        this.emitImage();
      }
    }
    else {
      this.base64Image = '';
      this.imageChangedEvent = event;
    }
  }

  isFileExtensionAllowed(file: File) {
    const selectedExtension = file.name.split('.').pop();
    return appConstant.fileExtensions.includes("." + selectedExtension);
  }

  emitImage() {
    this.getfiles.emit(this.fileList);
  }

  imageCropped(event: ImageCroppedEvent) {
    if (this.imageChangedEvent != '') {
      if (!this.isFileExtensionAllowed(this.file)) {
        this.toasterService.warning(`${this.file.name} is not a supported file`);
        this.fileList = [];
        this.emitImage();
      }
      else {
        const imageFile = new File([event.blob!], 'event.png', { type: 'image/png' });
        this.imagePath = URL.createObjectURL(imageFile);
        this.fileList = [];
        this.fileList.push(imageFile);
        this.emitImage();
      }
    }
    else if (this.base64Image != '') {
      const imageFile = new File([event.blob!], 'event.png', { type: 'image/png' });
      this.imagePath = URL.createObjectURL(imageFile);
      this.fileList = [];
      this.fileList.push(imageFile);
      this.emitImage();
    }
  }
}
