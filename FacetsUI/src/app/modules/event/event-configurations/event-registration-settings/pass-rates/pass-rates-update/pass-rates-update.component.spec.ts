import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PassRatesUpdateComponent } from './pass-rates-update.component';

describe('PassRatesUpdateComponent', () => {
  let component: PassRatesUpdateComponent;
  let fixture: ComponentFixture<PassRatesUpdateComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PassRatesUpdateComponent]
    });
    fixture = TestBed.createComponent(PassRatesUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
