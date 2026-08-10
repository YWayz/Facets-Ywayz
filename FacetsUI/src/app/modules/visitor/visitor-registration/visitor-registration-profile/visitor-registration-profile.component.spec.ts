import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorRegistrationProfileComponent } from './visitor-registration-profile.component';

describe('VisitorRegistrationProfileComponent', () => {
  let component: VisitorRegistrationProfileComponent;
  let fixture: ComponentFixture<VisitorRegistrationProfileComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorRegistrationProfileComponent]
    });
    fixture = TestBed.createComponent(VisitorRegistrationProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
