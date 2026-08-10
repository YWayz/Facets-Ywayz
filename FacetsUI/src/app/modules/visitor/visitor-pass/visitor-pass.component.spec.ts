import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorPassComponent } from './visitor-pass.component';

describe('VisitorPassComponent', () => {
  let component: VisitorPassComponent;
  let fixture: ComponentFixture<VisitorPassComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorPassComponent]
    });
    fixture = TestBed.createComponent(VisitorPassComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
