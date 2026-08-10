import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PassRatesComponent } from './pass-rates.component';

describe('PassRatesComponent', () => {
  let component: PassRatesComponent;
  let fixture: ComponentFixture<PassRatesComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PassRatesComponent]
    });
    fixture = TestBed.createComponent(PassRatesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
