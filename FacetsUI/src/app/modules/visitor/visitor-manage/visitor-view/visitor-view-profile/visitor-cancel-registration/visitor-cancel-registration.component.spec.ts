import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorCancelRegistrationComponent } from './visitor-cancel-registration.component';

describe('VisitorCancelRegistrationComponent', () => {
  let component: VisitorCancelRegistrationComponent;
  let fixture: ComponentFixture<VisitorCancelRegistrationComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorCancelRegistrationComponent]
    });
    fixture = TestBed.createComponent(VisitorCancelRegistrationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
