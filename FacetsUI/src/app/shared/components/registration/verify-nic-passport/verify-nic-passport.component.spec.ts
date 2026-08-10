import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VerifyNicPassportComponent } from './verify-nic-passport.component';

describe('VerifyNicPassportComponent', () => {
  let component: VerifyNicPassportComponent;
  let fixture: ComponentFixture<VerifyNicPassportComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VerifyNicPassportComponent]
    });
    fixture = TestBed.createComponent(VerifyNicPassportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
