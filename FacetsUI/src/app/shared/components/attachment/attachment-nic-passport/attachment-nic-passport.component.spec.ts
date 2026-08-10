import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AttachmentNicPassportComponent } from './attachment-nic-passport.component';

describe('AttachmentNicPassportComponent', () => {
  let component: AttachmentNicPassportComponent;
  let fixture: ComponentFixture<AttachmentNicPassportComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AttachmentNicPassportComponent]
    });
    fixture = TestBed.createComponent(AttachmentNicPassportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
