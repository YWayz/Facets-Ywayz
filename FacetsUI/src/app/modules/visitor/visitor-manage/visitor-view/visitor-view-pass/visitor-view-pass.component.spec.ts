import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorViewPassComponent } from './visitor-view-pass.component';

describe('VisitorViewPassComponent', () => {
  let component: VisitorViewPassComponent;
  let fixture: ComponentFixture<VisitorViewPassComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorViewPassComponent]
    });
    fixture = TestBed.createComponent(VisitorViewPassComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
