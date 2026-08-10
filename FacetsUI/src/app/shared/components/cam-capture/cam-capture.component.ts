import { Component, HostListener, Inject, Input, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { WebcamInitError, WebcamImage, WebcamUtil } from 'ngx-webcam';
import { Subject, Observable } from 'rxjs';

@Component({
  selector: 'facets-cam-capture',
  templateUrl: './cam-capture.component.html',
  styleUrls: ['./cam-capture.component.scss']
})
export class CamCaptureComponent implements OnInit {

  @HostListener("keydown.esc")

  isOpenCamera = false;
  public multipleWebcamsAvailable = false;

  public errors: WebcamInitError[] = [];
  public webcamImage: WebcamImage;

  private trigger: Subject<void> = new Subject<void>();
  private nextWebcam: Subject<boolean | string> = new Subject<boolean | string>();

  public onEsc() {
    this.close(false);
  }

  constructor(@Inject(MAT_DIALOG_DATA) public data: { isOpenCamera: boolean, image: string },
    private mdDialogRef: MatDialogRef<CamCaptureComponent>) {
    this.isOpenCamera = data.isOpenCamera;
  }
  ngOnInit(): void {
    WebcamUtil.getAvailableVideoInputs()
      .then((mediaDevices: MediaDeviceInfo[]) => {
        this.multipleWebcamsAvailable = mediaDevices && mediaDevices.length > 1;

        if (this.multipleWebcamsAvailable) {
          this.nextWebcamObservable;
        }
      });
  }

  public get nextWebcamObservable(): Observable<boolean | string> {
    return this.nextWebcam.asObservable();
  }

  public get triggerObservable(): Observable<void> {
    return this.trigger.asObservable();
  }

  public handleImage(webcamImage: WebcamImage): void {
    this.webcamImage = webcamImage;
    this.data.image = this.webcamImage.imageAsBase64
    this.close(this.data);
  }

  public triggerSnapshot(): void {
    this.trigger.next();
  }

  public cancel() {
    this.close(false);
  }
  public close(value: any) {
    this.mdDialogRef.close(value);
  }
  public confirm() {
    this.close(true);
  }

  public showNextWebcam(directionOrDeviceId: boolean | string): void {
    // true => move forward through devices
    // false => move backwards through devices
    // string => move to device with given deviceId
    this.nextWebcam.next(directionOrDeviceId);
  }
}
