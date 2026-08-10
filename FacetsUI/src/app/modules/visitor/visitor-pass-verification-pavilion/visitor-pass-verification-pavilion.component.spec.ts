import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorPassVerificationPavilionComponent } from './visitor-pass-verification-pavilion.component';

describe('VisitorPassVerificationPavilionComponent', () => {
  let component: VisitorPassVerificationPavilionComponent;
  let fixture: ComponentFixture<VisitorPassVerificationPavilionComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorPassVerificationPavilionComponent]
    });
    fixture = TestBed.createComponent(VisitorPassVerificationPavilionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
