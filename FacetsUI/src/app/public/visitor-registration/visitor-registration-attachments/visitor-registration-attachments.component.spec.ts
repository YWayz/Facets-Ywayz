import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorRegistrationAttachmentsComponent } from './visitor-registration-attachments.component';

describe('VisitorRegistrationAttachmentsComponent', () => {
  let component: VisitorRegistrationAttachmentsComponent;
  let fixture: ComponentFixture<VisitorRegistrationAttachmentsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorRegistrationAttachmentsComponent]
    });
    fixture = TestBed.createComponent(VisitorRegistrationAttachmentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
