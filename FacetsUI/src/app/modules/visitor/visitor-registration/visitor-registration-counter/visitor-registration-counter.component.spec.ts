import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorRegistrationCounterComponent } from './visitor-registration-counter.component';

describe('VisitorRegistrationCounterComponent', () => {
  let component: VisitorRegistrationCounterComponent;
  let fixture: ComponentFixture<VisitorRegistrationCounterComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorRegistrationCounterComponent]
    });
    fixture = TestBed.createComponent(VisitorRegistrationCounterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
