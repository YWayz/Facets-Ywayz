import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorPavilionVerificationSelectionComponent } from './visitor-pavilion-verification-selection.component';

describe('VisitorPavilionVerificationSelectionComponent', () => {
  let component: VisitorPavilionVerificationSelectionComponent;
  let fixture: ComponentFixture<VisitorPavilionVerificationSelectionComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorPavilionVerificationSelectionComponent]
    });
    fixture = TestBed.createComponent(VisitorPavilionVerificationSelectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
