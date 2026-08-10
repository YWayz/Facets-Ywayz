import { AfterViewInit, Component, ElementRef, Inject, OnInit, ViewChildren, inject } from '@angular/core';
import { FormBuilder, FormControlName, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Cmyk, ColorPickerService } from 'ngx-color-picker';
import { Observable, fromEvent, merge } from 'rxjs';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { PassCategoryModel } from 'src/app/modules/event/models/pass-category.model';
import { PassCategoryService } from 'src/app/modules/event/services/pass-category.service';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { ValidationModel } from 'src/app/shared/validators/validation.model';

@Component({
  selector: 'facets-pass-category-create',
  templateUrl: './pass-category-create.component.html',
  styleUrls: ['./pass-category-create.component.scss']
})
export class PassCategoryCreateComponent implements OnInit, AfterViewInit {

  isFormSubmitted = false;
  isBlocked = false;
  color: string = '#FFFFFF';
  cmykValue: string = '';
  cmykColor: Cmyk = new Cmyk(0, 0, 0, 0);

  passCategoryForm: FormGroup;
  passCategoryModel = new PassCategoryModel();
  validationModel: ValidationModel = new ValidationModel();

  formBuilder = inject(FormBuilder);
  passCategoryService = inject(PassCategoryService);
  toasterService = inject(ToasterService);
  colorPickerService = inject(ColorPickerService);
  dialogRef = inject(MatDialogRef<PassCategoryCreateComponent>);
  
  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  constructor(@Inject(MAT_DIALOG_DATA) public dialogData: { data: { id: string, isEdit: boolean, eventId: string } }) {
    this.validationModel.validationMessages = {
      name: {
        required: 'Pass category name is required',
      }
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.createPassCategoryForm();
    if(this.dialogData.data.isEdit) {
      this.getPassCategory();
    }
  }

  getPassCategory() {
    this.isBlocked = true;
    this.passCategoryService.getPassCategory(this.dialogData.data.eventId, this.dialogData.data.id).subscribe({
      next: (result: ResponseResult<PassCategoryModel>) => {
        this.isBlocked = false;
        this.passCategoryModel = result.data
        this.passCategoryForm.patchValue(this.passCategoryModel);
        this.color = this.passCategoryModel.color;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    })
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.passCategoryForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.passCategoryForm, this.isFormSubmitted);
  }

  createPassCategoryForm() {
    this.passCategoryForm = this.formBuilder.group({
      name: ['', Validators.required],
      description: [''],
      passCategoryType: ['Custom_Visitor']
    });
  }

  create() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.passCategoryForm.invalid) { return; }

    this.isBlocked = true;
    this.passCategoryModel = Object.assign({}, this.passCategoryModel, this.passCategoryForm.value);
    this.passCategoryModel.color = this.color;
    this.passCategoryService.create(this.dialogData.data.eventId, this.passCategoryModel).subscribe({
      next: () => {
        this.isBlocked = false;
        this.dialogRef.close();
        this.toasterService.successfullyCreated("Pass category");
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.toasterService.error(err);
      }
    });
  }

  update() {
    this.isFormSubmitted = true;
    this.validate();
    if (this.passCategoryForm.invalid) { return; }

    this.isBlocked = true;
    this.passCategoryModel = Object.assign({}, this.passCategoryModel, this.passCategoryForm.value);
    this.passCategoryModel.color = this.color;
    this.passCategoryService.update(this.dialogData.data.eventId, this.passCategoryModel.id, this.passCategoryModel).subscribe({
      next: () => {
        this.isBlocked = false;
        this.dialogRef.close();
        this.toasterService.successfullyUpdated("Pass category");
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.isFormSubmitted = false;
        this.toasterService.error(err);
      }
    });
  }

  discard() {
    this.dialogRef.close();
  }

  onChangeColorCmyk(color: string): Cmyk {
    const hsva = this.colorPickerService.stringToHsva(color);

    if (hsva) {
      const rgba = this.colorPickerService.hsvaToRgba(hsva);

      return this.colorPickerService.rgbaToCmyk(rgba);
    }

    return new Cmyk(0, 0, 0, 0);
  }
}