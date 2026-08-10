import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorRegistrationPaymentComponent } from './visitor-registration-payment.component';

describe('VisitorRegistrationPaymentComponent', () => {
  let component: VisitorRegistrationPaymentComponent;
  let fixture: ComponentFixture<VisitorRegistrationPaymentComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorRegistrationPaymentComponent]
    });
    fixture = TestBed.createComponent(VisitorRegistrationPaymentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
