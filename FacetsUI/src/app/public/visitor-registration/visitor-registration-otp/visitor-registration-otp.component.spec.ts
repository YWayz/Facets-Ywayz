import { ComponentFixture, TestBed } from '@angular/core/testing';
import { VisitorRegistrationOtpComponent } from './visitor-registration-otp.component';


describe('VisitorRegistrationOtpComponent', () => {
  let component: VisitorRegistrationOtpComponent;
  let fixture: ComponentFixture<VisitorRegistrationOtpComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorRegistrationOtpComponent]
    });
    fixture = TestBed.createComponent(VisitorRegistrationOtpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
