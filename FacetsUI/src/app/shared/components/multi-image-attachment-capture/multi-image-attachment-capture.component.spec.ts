import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MultiImageAttachmentCaptureComponent } from './multi-image-attachment-capture.component';

describe('MultiImageAttachmentCaptureComponent', () => {
  let component: MultiImageAttachmentCaptureComponent;
  let fixture: ComponentFixture<MultiImageAttachmentCaptureComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MultiImageAttachmentCaptureComponent]
    });
    fixture = TestBed.createComponent(MultiImageAttachmentCaptureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
