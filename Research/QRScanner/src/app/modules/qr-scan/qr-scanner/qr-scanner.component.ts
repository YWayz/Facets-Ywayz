import { Component } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { BarcodeFormat } from '@zxing/library';

@Component({
  selector: 'app-qr-scanner',
  templateUrl: './qr-scanner.component.html',
  styleUrls: ['./qr-scanner.component.scss']
})
export class QrScannerComponent {

  availableDevices!: MediaDeviceInfo[];
  currentDevice!: MediaDeviceInfo | undefined;

  hasDevices: boolean = false;
  hasPermission: boolean = false;

  formatsEnabled: BarcodeFormat[] = [
    BarcodeFormat.QR_CODE,
  ];

  qrResultString!: string | null;

  torchEnabled = false;
  torchAvailable$ = new BehaviorSubject<boolean>(false);
  tryHarder = false;
  
  onDeviceSelectChange(event: Event) {
    const selected = (event.target as HTMLInputElement).value;
    const device = this.availableDevices.find(x => x.deviceId === selected);
    this.currentDevice = device || undefined;
  }

  clearResult(): void {
    this.qrResultString = null;
  }

  onCamerasFound(devices: MediaDeviceInfo[]): void {
    this.availableDevices = devices;
    this.hasDevices = Boolean(devices && devices.length);
  }

  onCodeResult(resultString: string) {
    this.qrResultString = resultString;
  }

  onHasPermission(has: boolean) {
    this.hasPermission = has;
  }

  onTorchCompatible(isCompatible: boolean): void {
    this.torchAvailable$.next(isCompatible || false);
  }
}
