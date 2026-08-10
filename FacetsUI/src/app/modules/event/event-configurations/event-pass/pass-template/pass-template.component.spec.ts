import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PassTemplateComponent } from './pass-template.component';

describe('PassTemplateComponent', () => {
  let component: PassTemplateComponent;
  let fixture: ComponentFixture<PassTemplateComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PassTemplateComponent]
    });
    fixture = TestBed.createComponent(PassTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
