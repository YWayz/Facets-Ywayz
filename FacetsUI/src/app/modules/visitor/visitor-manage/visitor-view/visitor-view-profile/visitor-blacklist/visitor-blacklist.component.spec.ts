import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorBlacklistComponent } from './visitor-blacklist.component';

describe('VisitorBlacklistComponent', () => {
  let component: VisitorBlacklistComponent;
  let fixture: ComponentFixture<VisitorBlacklistComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorBlacklistComponent]
    });
    fixture = TestBed.createComponent(VisitorBlacklistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
