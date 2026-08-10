import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorPassQrScanComponent } from './visitor-pass-qr-scan.component';

describe('VisitorPassQrScanComponent', () => {
  let component: VisitorPassQrScanComponent;
  let fixture: ComponentFixture<VisitorPassQrScanComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorPassQrScanComponent]
    });
    fixture = TestBed.createComponent(VisitorPassQrScanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
