import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorRegistrationOnlinePaymentSuccessComponent } from './visitor-registration-online-payment-success.component';

describe('VisitorRegistrationOnlinePaymentSuccessComponent', () => {
  let component: VisitorRegistrationOnlinePaymentSuccessComponent;
  let fixture: ComponentFixture<VisitorRegistrationOnlinePaymentSuccessComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorRegistrationOnlinePaymentSuccessComponent]
    });
    fixture = TestBed.createComponent(VisitorRegistrationOnlinePaymentSuccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
