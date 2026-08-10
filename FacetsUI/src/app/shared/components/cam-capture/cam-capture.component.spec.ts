import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CamCaptureComponent } from './cam-capture.component';

describe('CamCaptureComponent', () => {
  let component: CamCaptureComponent;
  let fixture: ComponentFixture<CamCaptureComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CamCaptureComponent]
    });
    fixture = TestBed.createComponent(CamCaptureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
