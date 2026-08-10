import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScanMeComponent } from './scan-me.component';

describe('ScanMeComponent', () => {
  let component: ScanMeComponent;
  let fixture: ComponentFixture<ScanMeComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ScanMeComponent]
    });
    fixture = TestBed.createComponent(ScanMeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
