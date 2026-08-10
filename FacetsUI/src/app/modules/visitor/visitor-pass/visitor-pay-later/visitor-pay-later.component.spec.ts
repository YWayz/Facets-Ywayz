import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorPayLaterComponent } from './visitor-pay-later.component';

describe('VisitorPayLaterComponent', () => {
  let component: VisitorPayLaterComponent;
  let fixture: ComponentFixture<VisitorPayLaterComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorPayLaterComponent]
    });
    fixture = TestBed.createComponent(VisitorPayLaterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
