import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorRegistrationPavilionComponent } from './visitor-registration-pavilion.component';

describe('VisitorRegistrationPavilionComponent', () => {
  let component: VisitorRegistrationPavilionComponent;
  let fixture: ComponentFixture<VisitorRegistrationPavilionComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorRegistrationPavilionComponent]
    });
    fixture = TestBed.createComponent(VisitorRegistrationPavilionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
