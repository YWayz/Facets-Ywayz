import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorRegistrationOverviewComponent } from './visitor-registration-overview.component';

describe('VisitorRegistrationOverviewComponent', () => {
  let component: VisitorRegistrationOverviewComponent;
  let fixture: ComponentFixture<VisitorRegistrationOverviewComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorRegistrationOverviewComponent]
    });
    fixture = TestBed.createComponent(VisitorRegistrationOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
