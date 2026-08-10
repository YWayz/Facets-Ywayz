import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PassTemplateSizeComponent } from './pass-template-size.component';

describe('PassTemplateSizeComponent', () => {
  let component: PassTemplateSizeComponent;
  let fixture: ComponentFixture<PassTemplateSizeComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PassTemplateSizeComponent]
    });
    fixture = TestBed.createComponent(PassTemplateSizeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
