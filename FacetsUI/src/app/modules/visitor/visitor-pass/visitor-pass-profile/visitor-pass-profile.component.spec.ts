import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorPassProfileComponent } from './visitor-pass-profile.component';

describe('VisitorPassProfileComponent', () => {
  let component: VisitorPassProfileComponent;
  let fixture: ComponentFixture<VisitorPassProfileComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorPassProfileComponent]
    });
    fixture = TestBed.createComponent(VisitorPassProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
