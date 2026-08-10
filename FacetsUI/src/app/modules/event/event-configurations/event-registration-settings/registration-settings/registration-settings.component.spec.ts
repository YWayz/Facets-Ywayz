import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegistrationSettingsComponent } from './registration-settings.component';

describe('RegistrationSettingsComponent', () => {
  let component: RegistrationSettingsComponent;
  let fixture: ComponentFixture<RegistrationSettingsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RegistrationSettingsComponent]
    });
    fixture = TestBed.createComponent(RegistrationSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
