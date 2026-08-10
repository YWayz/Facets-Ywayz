import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegistrationCountersCreateComponent } from './registration-counters-create.component';

describe('RegistrationCountersCreateComponent', () => {
  let component: RegistrationCountersCreateComponent;
  let fixture: ComponentFixture<RegistrationCountersCreateComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RegistrationCountersCreateComponent]
    });
    fixture = TestBed.createComponent(RegistrationCountersCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
