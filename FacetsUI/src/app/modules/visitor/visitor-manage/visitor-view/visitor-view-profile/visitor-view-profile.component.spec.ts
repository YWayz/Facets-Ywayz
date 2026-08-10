import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorViewProfileComponent } from './visitor-view-profile.component';

describe('VisitorViewProfileComponent', () => {
  let component: VisitorViewProfileComponent;
  let fixture: ComponentFixture<VisitorViewProfileComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorViewProfileComponent]
    });
    fixture = TestBed.createComponent(VisitorViewProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
