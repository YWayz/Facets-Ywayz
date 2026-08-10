import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AttachmentOtherComponent } from './attachment-other.component';

describe('AttachmentOtherComponent', () => {
  let component: AttachmentOtherComponent;
  let fixture: ComponentFixture<AttachmentOtherComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AttachmentOtherComponent]
    });
    fixture = TestBed.createComponent(AttachmentOtherComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
