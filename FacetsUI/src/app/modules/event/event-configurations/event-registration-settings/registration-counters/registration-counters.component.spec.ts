import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegistrationCountersComponent } from './registration-counters.component';

describe('RegistrationCountersComponent', () => {
  let component: RegistrationCountersComponent;
  let fixture: ComponentFixture<RegistrationCountersComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RegistrationCountersComponent]
    });
    fixture = TestBed.createComponent(RegistrationCountersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
