import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorPassVerificationComponent } from './visitor-pass-verification.component';

describe('VisitorPassVerificationComponent', () => {
  let component: VisitorPassVerificationComponent;
  let fixture: ComponentFixture<VisitorPassVerificationComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorPassVerificationComponent]
    });
    fixture = TestBed.createComponent(VisitorPassVerificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
