import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PavilionsComponent } from './pavilions.component';

describe('PavilionsComponent', () => {
  let component: PavilionsComponent;
  let fixture: ComponentFixture<PavilionsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PavilionsComponent]
    });
    fixture = TestBed.createComponent(PavilionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
