import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorViewActivityComponent } from './visitor-view-activity.component';

describe('VisitorViewActivityComponent', () => {
  let component: VisitorViewActivityComponent;
  let fixture: ComponentFixture<VisitorViewActivityComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorViewActivityComponent]
    });
    fixture = TestBed.createComponent(VisitorViewActivityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
