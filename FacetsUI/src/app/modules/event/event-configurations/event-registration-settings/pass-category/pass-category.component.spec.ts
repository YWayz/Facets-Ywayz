import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PassCategoryComponent } from './pass-category.component';

describe('PassCategoryComponent', () => {
  let component: PassCategoryComponent;
  let fixture: ComponentFixture<PassCategoryComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PassCategoryComponent]
    });
    fixture = TestBed.createComponent(PassCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
