import { AfterViewInit, Component, EventEmitter, OnDestroy, Output, ViewChild } from '@angular/core';
import { ScannerQRCodeConfig,  NgxScannerQrcodeComponent,  ScannerQRCodeResult } from 'ngx-scanner-qrcode';

@Component({
  selector: 'facets-qr-scanner',
  templateUrl: './qr-scanner.component.html',
  styleUrls: ['./qr-scanner.component.scss'],
})
export class QrScannerComponent implements AfterViewInit, OnDestroy {

  public config: ScannerQRCodeConfig = {
    constraints: {
      video: {
        width: window.innerWidth
      },
    },
  };

  @ViewChild('action') action!: NgxScannerQrcodeComponent;
  @Output() qrCodeResult = new EventEmitter<string>();

  ngAfterViewInit(): void {
    this.action.isReady.subscribe((res: any) => {
    });
  }

  public onEvent(result: ScannerQRCodeResult[], action?: any): void {
    this.qrCodeResult.emit(result[0].value);
  }

  public handle(action: any, fn: string): void {
    const playDeviceFacingBack = (devices: any[]) => {
      const device = devices.find(f => (/back|rear|environment/gi.test(f.label)));
      action.playDevice(device ? device.deviceId : devices[0].deviceId);
    }

    if (fn === 'start') {
      action[fn](playDeviceFacingBack).subscribe((r: any) => console.log(fn, r), alert);
    } else {
      action[fn]().subscribe((r: any) => console.log(fn, r), alert);
    }
  }

  ngOnDestroy(): void {
    this.action.stop();
  }
}
