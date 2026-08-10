import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorRegistrationPassComponent } from './visitor-registration-pass.component';

describe('VisitorRegistrationPassComponent', () => {
  let component: VisitorRegistrationPassComponent;
  let fixture: ComponentFixture<VisitorRegistrationPassComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorRegistrationPassComponent]
    });
    fixture = TestBed.createComponent(VisitorRegistrationPassComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
