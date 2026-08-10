import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorViewAttachmentComponent } from './visitor-view-attachment.component';

describe('VisitorViewAttachmentComponent', () => {
  let component: VisitorViewAttachmentComponent;
  let fixture: ComponentFixture<VisitorViewAttachmentComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorViewAttachmentComponent]
    });
    fixture = TestBed.createComponent(VisitorViewAttachmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
