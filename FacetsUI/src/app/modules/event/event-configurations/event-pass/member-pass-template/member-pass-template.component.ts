import { Component, OnInit, inject } from '@angular/core';
import { fabric } from 'fabric';
import { appConstant } from 'src/app/core/extensions/app-constants';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { PassTemplateModel } from '../../../models/pass-template.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { ActivatedRoute, Params } from '@angular/router';
import { PassTemplateService } from '../../../services/pass-template.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { PassTemplateCreateModel } from '../../../models/pass-template-create.model';
import { SuperAdminPermissions, PassCategoryPermissions } from 'src/app/core/extensions/permission-constants';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';

@Component({
  selector: 'facets-member-pass-template',
  templateUrl: './member-pass-template.component.html',
  styleUrls: ['./member-pass-template.component.scss']
})
export class MemberPassTemplateComponent implements OnInit {

  isBlocked = false;
  canvasTeamMember: any;
  canvasTeamMemberPreview: any;
  eventId: string;
  height = 0;
  width = 0;
  size: 'A6' | 'A7' = 'A6';
  event: any;
  scalingProperties = {
    'left': 0,
    'top': 0,
    'scaleX': 0,
    'scaleY': 0
  }
  superAdminPermissions = SuperAdminPermissions;
  passCategoryPermissions = PassCategoryPermissions;

  passTemplateModel: PassTemplateModel;
  passTemplateModelList: PassTemplateModel[];
  searchRequestModel = new SearchRequestModel(100, 1);
  createPassTemplateModel = new PassTemplateCreateModel();

  passTemplateService = inject(PassTemplateService);
  authService = inject(AuthService);
  toasterService = inject(ToasterService);
  activatedRoute = inject(ActivatedRoute);

  ngOnInit(): void {
    this.canvasTeamMember = new fabric.Canvas(document.getElementById("canvasTeamMember") as HTMLCanvasElement);
    this.canvasTeamMemberPreview = new fabric.Canvas(document.getElementById("canvasTeamMemberPreview") as HTMLCanvasElement);

    this.activatedRoute.params.subscribe((param: Params) => {
      this.eventId = param['eventId'];
      if (this.eventId != null || this.eventId != undefined || this.eventId != '') {
        if (this.authService.hasPermissionAuthorization([this.superAdminPermissions.all, this.passCategoryPermissions.managePassTemplate])) {
          this.getPassTemplate();
        }
      }
    });
  }

  setEvent(event: any) {
    this.event = event;
  }

  setDimension(event: any, isUpdate?: boolean) {
    if (isUpdate) {
      this.height = +event.height;
      this.width = +event.width;
    } else {
      this.height = +event.height * 37.79;
      this.width = +event.width * 37.79;
    }
    this.size = event.size;
    this.canvasTeamMember.setDimensions({ height: this.height });
    this.canvasTeamMemberPreview.setDimensions({ height: this.height });
    this.canvasTeamMember.setDimensions({ width: this.width });
    this.canvasTeamMemberPreview.setDimensions({ width: this.width });
  }

  allowDrop(event: any) {
    event.preventDefault();
  }

  dropElement(event: any) {
    const boxWidth = 347
    const boxHeight = 80

    if (this.width != 0 && this.height != 0) {
      event.preventDefault();
      const data = event.dataTransfer.getData("id") != null ? event.dataTransfer.getData("id") : this.event.dataTransfer.getData("id");
      const selectedData = document.getElementById(data) as any;

      if (selectedData.id == 'ele1') {
        const img = new fabric.Image(selectedData, {
          width: 150,
          height: 200
        });
        this.canvasTeamMember.add(img);
        let { canvasTeamMemberPreviewObj, canvasTeamMemberObj } = this.bindImageToCanvas();
        canvasTeamMemberPreviewObj.objects.push(...canvasTeamMemberObj)
        const canvasTeamMemberPreviewObject = JSON.stringify(canvasTeamMemberPreviewObj)
        this.canvasTeamMemberPreview.loadFromJSON(canvasTeamMemberPreviewObject);
      }
      if (selectedData.id == 'ele2') {
        const img = new fabric.Image(selectedData, {
          width: 100,
          height: 100
        });
        this.canvasTeamMember.add(img);
        let { canvasTeamMemberPreviewObj, canvasTeamMemberObj } = this.bindImageToCanvas();
        canvasTeamMemberPreviewObj.objects.push(...canvasTeamMemberObj)
        const canvasTeamMemberPreviewObject = JSON.stringify(canvasTeamMemberPreviewObj)
        this.canvasTeamMemberPreview.loadFromJSON(canvasTeamMemberPreviewObject);
      }
      else {
        if (selectedData.id == 'spanValue6' || selectedData.id == 'spanValue1' || selectedData.id == 'spanValue9') {
          const rect = new fabric.Rect({
            width: boxWidth,
            height: boxHeight,
            fill: selectedData.id == 'spanValue6' ? 'green' : (selectedData.id == 'spanValue1' || selectedData.id == 'spanValue9') ? 'transparent' : '',
          });

          const text = new fabric.Text(selectedData.innerText, {
            fontSize: selectedData.id == 'spanValue6' ? 34 : selectedData.id == 'spanValue1' ? 24 : selectedData.id == 'spanValue9' ? 16 : 14,
            fontFamily: 'Arial',
            fontWeight: selectedData.id == 'spanValue9' ? '' : 'bold',
            originX: 'center',
            originY: 'center',
            left: 0.5 * boxWidth,
            top: 0.5 * boxHeight
          });

          const group = new fabric.Group([rect, text]);
          group.setControlsVisibility({
            mt: false,
            mb: false,
            ml: false,
            mr: false,
            bl: false,
            br: false,
            tl: false,
            tr: false,
            mtr: false,
          })
          this.canvasTeamMember.add(group);
        }
        else {
          const text = new fabric.Text(selectedData.innerText, {
            fontSize: 14
          });

          this.canvasTeamMember.add(text);
        }

        let rectangleObj = []
        let { canvasTeamMemberPreviewObj, canvasTeamMemberObj } = this.bindTextToCanvas();
        const localVisitorObj = this.canvasTeamMember.toObject().objects.filter((f: any) => f.type == 'group');
        if (localVisitorObj.length > 0) {
          localVisitorObj.forEach((visitor: any) => {
            const visitorPassCategoryObj = visitor.objects.find((f: any) => f.type == 'text' && f.text == 'Pass Category')
            if (visitorPassCategoryObj != undefined) visitorPassCategoryObj.text = 'LIAISON OFFICER';

            const visitorFullNameObj = visitor.objects.find((f: any) => f.type == 'text' && f.text == 'Full Name')
            if (visitorFullNameObj != undefined) visitorFullNameObj.text = 'John Abraham';

            const visitorCompanyObj = visitor.objects.find((f: any) => f.type == 'text' && f.text == 'Company')
            if (visitorCompanyObj != undefined) visitorCompanyObj.text = 'Facets';
          });
          rectangleObj.push(...localVisitorObj)
        }
        canvasTeamMemberPreviewObj.objects.push(...rectangleObj, ...canvasTeamMemberObj)
        const canvasTeamMemberPreviewObject = JSON.stringify(canvasTeamMemberPreviewObj)
        this.canvasTeamMemberPreview.loadFromJSON(canvasTeamMemberPreviewObject);
      }

      this.canvasTeamMember.on('object:moving', (e: any) => {
        this.movingRotatingWithinBounds(e);
      });

      this.canvasTeamMember.on('object:scaling', (e: any) => {
        this.objectScaling(e);
      });

      this.canvasTeamMember.on('object:rotating', (e: any) => {
        this.movingRotatingWithinBounds(e);
      });
    }
  }

  bindImageToCanvas() {
    let canvasTeamMemberObj = this.canvasTeamMember.toObject().objects.filter((f: any) => f.type == 'image');
    canvasTeamMemberObj.forEach((img: any) => {
      const isUserProfileImage = img.src.endsWith("user-profile.png")
      if (isUserProfileImage) {
        img.src = '../../../../../assets/images/dummy-user-profile.jpg'
      }
      else {
        img.src = '../../../../../assets/images/qr-code.png'
      }
    });
    let canvasTeamMemberPreviewObj = Object.assign(this.canvasTeamMemberPreview.toObject());
    return { canvasTeamMemberPreviewObj, canvasTeamMemberObj };
  }

  bindTextToCanvas() {
    let canvasTeamMemberObj = this.canvasTeamMember.toObject().objects.filter((f: any) => f.type == 'text');
    canvasTeamMemberObj.map((selectedValue: any) => {
      switch (selectedValue.text) {
        case 'Event Name':
          selectedValue.text = 'Facets Event'
          break;
        case 'First Name':
          selectedValue.text = 'John'
          break;
        case 'Last Name':
          selectedValue.text = 'Abraham'
          break;
        case 'NIC Number/ Passport Number':
          selectedValue.text = '19987548556565'
          break;
        case 'Mobile Number':
          selectedValue.text = '+94778989898'
          break;
        case 'Pass Category':
          selectedValue.text = 'LOCAL - VISITOR'
          break;
        case 'Pass Generated Date & Time':
          selectedValue.text = '2022-10-24 12:00:00'
          break;
        case 'Pass Date':
          selectedValue.text = '2022-10-24'
          break;
        case 'Full Name':
          selectedValue.text = 'John Abraham';
          break;
        case 'Company':
          selectedValue.text = 'Facets'
          break;
        default:
          break;
      }
    });

    let canvasTeamMemberPreviewObj = Object.assign(this.canvasTeamMemberPreview.toObject());
    return { canvasTeamMemberPreviewObj, canvasTeamMemberObj };
  }

  movingRotatingWithinBounds(event: any) {
    const obj = event.target;
    this.objectSelectorFront(obj);
  }

  objectScaling(event: any) {
    let shape = event.target;
    let maxWidth = shape.canvas.width;
    let maxHeight = shape.canvas.height;

    //left border
    if (shape.left < 0) {
      shape.left = this.scalingProperties.left;
      shape.scaleX = this.scalingProperties.scaleX
    } else {
      this.scalingProperties.left = shape.left;
      this.scalingProperties.scaleX = shape.scaleX;
    }

    //right border
    if ((this.scalingProperties.scaleX * shape.width) + shape.left >= maxWidth) {
      shape.scaleX = (maxWidth - this.scalingProperties.left) / shape.width;
    } else {
      this.scalingProperties.scaleX = shape.scaleX;
    }

    //top border
    if (shape.top < 0) {
      shape.top = this.scalingProperties.top;
      shape.scaleY = this.scalingProperties.scaleY;
    } else {
      this.scalingProperties.top = shape.top;
      this.scalingProperties.scaleY = shape.scaleY;
    }

    //bottom border
    if ((this.scalingProperties.scaleY * shape.height) + shape.top >= maxHeight) {
      shape.scaleY = (maxHeight - this.scalingProperties.top) / shape.height;
    } else {
      this.scalingProperties.scaleY = shape.scaleY;
    }

    this.objectSelectorFront(shape);
  }

  objectSelectorFront(obj: any) {
    if (obj._element != undefined && obj._element.localName == 'img') {
      this.imageBindingFront();
    }
    else if (obj.text == undefined) {
      let rectangleObj = []
      const localTeamMemberObj = this.canvasTeamMember.toObject().objects.filter((f: any) => f.type == 'group');
      if (localTeamMemberObj.length > 0) {
        localTeamMemberObj.forEach((visitor: any) => {
          const visitorPassCategoryObj = visitor.objects.find((f: any) => f.type == 'text' && f.text == 'Pass Category')
          if (visitorPassCategoryObj != undefined) visitorPassCategoryObj.text = 'LIAISON OFFICER';

          const visitorFullNameObj = visitor.objects.find((f: any) => f.type == 'text' && f.text == 'Full Name')
          if (visitorFullNameObj != undefined) visitorFullNameObj.text = 'John Abraham';

          const visitorCompanyObj = visitor.objects.find((f: any) => f.type == 'text' && f.text == 'Company')
          if (visitorCompanyObj != undefined) visitorCompanyObj.text = 'Facets';
        });
        rectangleObj.push(...localTeamMemberObj)
      }
      let canvasTeamMemberPreviewObj = Object.assign(this.canvasTeamMemberPreview.toObject());
      const others = canvasTeamMemberPreviewObj.objects.filter((f: any) => f.type != 'group');
      canvasTeamMemberPreviewObj.objects = others;
      canvasTeamMemberPreviewObj.objects.push(...rectangleObj);
      const canvasTeamMemberPreviewObject = JSON.stringify(canvasTeamMemberPreviewObj);
      this.canvasTeamMemberPreview.loadFromJSON(canvasTeamMemberPreviewObject);
    }
    else if (obj.__lastIsEditing == undefined && obj.__lastClickTime == undefined) {
      this.textBindingFront();
    }
  }

  imageBindingFront() {
    let { canvasTeamMemberPreviewObj, canvasTeamMemberObj } = this.bindImageToCanvas();
    const textOnly = canvasTeamMemberPreviewObj.objects.filter((f: any) => f.type != 'image');
    canvasTeamMemberPreviewObj.objects = textOnly;
    canvasTeamMemberPreviewObj.objects.push(...canvasTeamMemberObj);
    const canvasTeamMemberPreviewObject = JSON.stringify(canvasTeamMemberPreviewObj);
    this.canvasTeamMemberPreview.loadFromJSON(canvasTeamMemberPreviewObject);
  }

  textBindingFront() {
    let { canvasTeamMemberPreviewObj, canvasTeamMemberObj } = this.bindTextToCanvas();
    const imageOnly = canvasTeamMemberPreviewObj.objects.filter((f: any) => f.type != 'text');
    canvasTeamMemberPreviewObj.objects = imageOnly;
    canvasTeamMemberPreviewObj.objects.push(...canvasTeamMemberObj);
    const canvasTeamMemberPreviewObject = JSON.stringify(canvasTeamMemberPreviewObj);
    this.canvasTeamMemberPreview.loadFromJSON(canvasTeamMemberPreviewObject);
  }

  removeObjectFront() {
    let selectedObject = this.canvasTeamMember.getActiveObject();

    if (selectedObject != undefined) {
      if (selectedObject.type === 'activeSelection') {
        selectedObject.canvas = this.canvasTeamMember;
        selectedObject.forEachObject((obj: any) => {
          this.canvasTeamMember.remove(obj);
        });
      }
      else {
        const activeObject = this.canvasTeamMember.getActiveObject();
        if (activeObject !== null) {
          this.canvasTeamMember.remove(activeObject);
          this.objectSelectorFront(activeObject);
        }
      }

      if (selectedObject.type === 'activeSelection') {
        let canvasTeamMemberObj = Object.assign(this.canvasTeamMember.toObject());
        let canvasTeamMemberPreviewObj = Object.assign(this.canvasTeamMember.toObject());
        canvasTeamMemberPreviewObj.objects = canvasTeamMemberObj.objects;
        const canvasTeamMemberPreviewObject = JSON.stringify(canvasTeamMemberPreviewObj);
        this.canvasTeamMemberPreview.loadFromJSON(canvasTeamMemberPreviewObject);
      }

      this.canvasTeamMember.discardActiveObject().renderAll();
    }
  }

  getPassTemplate() {
    this.isBlocked = true;
    this.passTemplateService.getAll(this.searchRequestModel, this.eventId, 'TeamMember').subscribe({
      next: (result: ResponseResult<PassTemplateModel[]>) => {
        this.isBlocked = false;
        this.passTemplateModelList = result.data;
        this.passTemplateModel = result.data[0];
        if (this.passTemplateModelList.length > 0) {
          this.patchPassTemplateForm(this.passTemplateModel);
        }
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    })
  }

  patchPassTemplateForm(passTemplateModel: PassTemplateModel) {
    this.setDimension({ width: passTemplateModel.width, height: passTemplateModel.height, size: passTemplateModel.sizeType }, true);

    const jsonTemplate = JSON.parse(passTemplateModel.templateText);
    const groupedValue = jsonTemplate.objects.filter((f: any) => f.type == 'group');
    if (groupedValue.length > 0) {
      groupedValue.forEach((visitor: any) => {
        visitor._controlsVisibility = {
          mt: false,
          mb: false,
          ml: false,
          mr: false,
          bl: false,
          br: false,
          tl: false,
          tr: false,
          mtr: false
        }
      });
    }

    const updatedJson = JSON.stringify(jsonTemplate);
    this.canvasTeamMember.loadFromJSON(updatedJson);
    if (groupedValue.length > 0) {
      groupedValue.forEach((visitor: any) => {
        const visitorPassCategoryObj = visitor.objects.find((f: any) => f.type == 'text' && f.text == 'Pass Category')
        if (visitorPassCategoryObj != undefined) visitorPassCategoryObj.text = 'LIAISON OFFICER';

        const visitorFullNameObj = visitor.objects.find((f: any) => f.type == 'text' && f.text == 'Full Name')
        if (visitorFullNameObj != undefined) visitorFullNameObj.text = 'John Abraham';

        const visitorCompanyObj = visitor.objects.find((f: any) => f.type == 'text' && f.text == 'Company')
        if (visitorCompanyObj != undefined) visitorCompanyObj.text = 'Facets';
      });
    }

    const textValues = jsonTemplate.objects.filter((f: any) => f.type == 'text');
    if (textValues.length > 0) {
      this.getTemplateDetail(textValues);
    }

    let canvasTeamMemberObj = jsonTemplate.objects.filter((f: any) => f.type == 'image');
    if (canvasTeamMemberObj.length > 0) {
      canvasTeamMemberObj.forEach((img: any) => {
        const isUserProfileImage = img.src.endsWith("user-profile.png")
        if (isUserProfileImage) {
          img.src = '../../../../../assets/images/dummy-user-profile.jpg'
        }
        else {
          img.src = '../../../../../assets/images/qr-code.png'
        }
      });
    }

    const previewJson = JSON.stringify(jsonTemplate);
    this.canvasTeamMemberPreview.loadFromJSON(previewJson);

    this.canvasTeamMember.on('object:moving', (e: any) => {
      this.movingRotatingWithinBounds(e);
    });

    this.canvasTeamMember.on('object:scaling', (e: any) => {
      this.objectScaling(e);
    });

    this.canvasTeamMember.on('object:rotating', (e: any) => {
      this.movingRotatingWithinBounds(e);
    });
  }

  getTemplateDetail(textValues: any) {
    textValues.map((selectedValue: any) => {
      switch (selectedValue.text) {
        case 'Event Name':
          selectedValue.text = 'Facets Event';
          break;
        case 'First Name':
          selectedValue.text = 'John';
          break;
        case 'Last Name':
          selectedValue.text = 'Abraham';
          break;
        case 'NIC Number/ Passport Number':
          selectedValue.text = '19987548556565';
          break;
        case 'Mobile Number':
          selectedValue.text = '+94778989898';
          break;
        case 'Pass Category':
          selectedValue.text = 'LOCAL - VISITOR';
          break;
        case 'Pass Generated Date & Time':
          selectedValue.text = '2022-10-24 12:00:00';
          break;
        case 'Pass Date':
          selectedValue.text = '2022-10-24';
          break;
        case 'Pass Rate':
          selectedValue.text = '500';
          break;
        case 'Country':
          selectedValue.text = 'Sri Lanka';
          break;
        case 'Company':
          selectedValue.text = 'Facets';
          break;
        case 'Full Name':
          selectedValue.text = 'John Abraham';
          break;
        case 'Company':
          selectedValue.text = 'Facets'
          break;
        default:
          break;
      }
    });
  }

  create() {
    this.isBlocked = true;
    this.createPassTemplateModel.templateText = JSON.stringify(this.canvasTeamMember);
    this.createPassTemplateModel.previewTemplateText = JSON.stringify(this.canvasTeamMemberPreview);
    this.createPassTemplateModel.width = this.width;
    this.createPassTemplateModel.height = this.height;
    this.createPassTemplateModel.sizeType = this.size;
    this.createPassTemplateModel.passType = 'TeamMember';

    this.passTemplateService.create(this.eventId, this.createPassTemplateModel).subscribe({
      next: (res: ResponseResult<PassTemplateModel>) => {
        this.isBlocked = false;
        this.passTemplateModel = res.data;
        this.getPassTemplate();
        this.toasterService.successfullyCreated("Team Member Pass Template");
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  update() {
    this.isBlocked = true;

    this.passTemplateModel.templateText = JSON.stringify(this.canvasTeamMember);
    this.passTemplateModel.previewTemplateText = JSON.stringify(this.canvasTeamMemberPreview);
    this.passTemplateModel.width = this.width;
    this.passTemplateModel.height = this.height;
    this.passTemplateModel.sizeType = this.size;
    this.passTemplateModel.passType = 'TeamMember';

    this.passTemplateService.update(this.eventId, this.passTemplateModel).subscribe({
      next: () => {
        this.isBlocked = false;
        this.toasterService.successfullyUpdated("Team Member Pass Template");
        this.getPassTemplate();
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  save() {
    if (this.passTemplateModelList.length > 0) {
      this.update();
    }
    else {
      this.create();
    }
  }
}
