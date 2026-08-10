import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImageAttachmentCaptureComponent } from './image-attachment-capture.component';

describe('ImageAttachmentCaptureComponent', () => {
  let component: ImageAttachmentCaptureComponent;
  let fixture: ComponentFixture<ImageAttachmentCaptureComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ImageAttachmentCaptureComponent]
    });
    fixture = TestBed.createComponent(ImageAttachmentCaptureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
