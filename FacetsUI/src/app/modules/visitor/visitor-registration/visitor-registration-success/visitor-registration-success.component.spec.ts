import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorRegistrationSuccessComponent } from './visitor-registration-success.component';

describe('VisitorRegistrationSuccessComponent', () => {
  let component: VisitorRegistrationSuccessComponent;
  let fixture: ComponentFixture<VisitorRegistrationSuccessComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorRegistrationSuccessComponent]
    });
    fixture = TestBed.createComponent(VisitorRegistrationSuccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
