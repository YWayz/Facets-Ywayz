import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChildren, inject } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { PublicSiteService } from '../../services/public-site.service';
import { GenerateOTPModel } from '../../models/generate-otp.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { PublicUserAuthenticatedModel } from '../../models/public-user-authenticated.model';
import { VerifyOTPModel } from '../../models/verify-otp.model';
import { FormBuilder, FormControlName, FormGroup, Validators } from '@angular/forms';
import { ValidationModel } from 'src/app/shared/validators/validation.model';
import { GenericValidator } from 'src/app/shared/validators/forms-error-validator';
import { Observable, Subscription, fromEvent, interval, map, merge, takeWhile, timer } from 'rxjs';
import { OTPType, appConstant } from 'src/app/core/extensions/app-constants';
import { compareOnlyTime } from 'src/app/core/extensions/helpers';
import { SharedService } from 'src/app/core/services/shared.service';
import { Router } from '@angular/router';
import { VisitorVerificationModel } from '../../models/visitor-verification.model';
import { VisitorSearchModel } from 'src/app/modules/visitor/models/visitor-search.model';

@Component({
  selector: 'facets-visitor-registration-otp',
  templateUrl: './visitor-registration-otp.component.html',
  styleUrls: ['./visitor-registration-otp.component.scss'],
  standalone: true,
  imports: [SharedModule]
})
export class VisitorRegistrationOtpComponent implements OnInit, AfterViewInit {

  isBlocked = false;
  isFormSubmitted = false;
  isOtpVerifying = false;
  disableResendOtp = false;
  isAlreadyChangedAndSent = false;

  sendOTPByEmail = false;
  resendOTP = false;

  displayTime: string;
  countdown$: Observable<number>;
  countdownSubscription: Subscription;
  initialTime: number = 5 * 60;

  isIdentityNumber = false;

  otpStartTime: Date;
  otpSentTo: string;

  publicUserAuthenticatedModel: PublicUserAuthenticatedModel;
  verifyOTPModel: VerifyOTPModel;
  visitorSearchModel: VisitorSearchModel;

  otpForm: FormGroup;
  validationModel: ValidationModel = new ValidationModel();

  formBuilder = inject(FormBuilder);
  publicSiteService = inject(PublicSiteService);
  sharedService = inject(SharedService);
  toasterService = inject(ToasterService);
  router = inject(Router);

  @Input() visitorId: string;
  @Input() identityNumber: string;
  @Input() model: VisitorVerificationModel;
  @Output() isProfile = new EventEmitter<string>();
  @Output() isAttachment = new EventEmitter<string>();

  @ViewChildren(FormControlName, { read: ElementRef }) formInputElements: ElementRef[];

  constructor() {
    this.validationModel.validationMessages = {
      code: {
        required: 'Code is required',
      },
    };
    this.validationModel.formsErrorValidator = new GenericValidator(this.validationModel.validationMessages);
  }

  ngOnInit(): void {
    this.otpStartTime = new Date();
    this.createOtpForm();
    this.GenerateOTP();
  }

  ngAfterViewInit(): void {
    const controlBlurs: Observable<any>[] = this.formInputElements.map((formControl: ElementRef) => fromEvent(formControl.nativeElement, 'blur'));
    merge(this.otpForm.valueChanges, ...controlBlurs).subscribe(() => {
      this.validate();
    });
  }

  validate(): void {
    this.validationModel.displayMessage = this.validationModel.formsErrorValidator.processMessages(this.otpForm, this.isFormSubmitted);
  }


  createOtpForm() {
    this.otpForm = this.formBuilder.group({
      code: ['', [Validators.required]],
    });
  }

  changeSearchValue(event: any) {
    if (event.target.value === 'mobile') {
      this.sendOTPByEmail = false;
    } else {
      this.sendOTPByEmail = true;
    }    
    this.isAlreadyChangedAndSent = true;
  }

  resendOtp() {
    this.resendOTP = true;
    this.countdownSubscription.unsubscribe();
    this.displayTime = this.formatTime(this.initialTime);
    this.GenerateOTP();
  }

  GenerateOTP() {
    this.isBlocked = true;
    this.otpStartTime = new Date();
    let generateOtpModel = new GenerateOTPModel();
    if (this.model.isRegisteredToFacets == true) {
      generateOtpModel.assignValues(this.model.otpType, this.identityNumber, this.sendOTPByEmail);
      this.otpGeneration(generateOtpModel);
    }
    else if (this.model.isAssocifyMember == true && this.model.isRegisteredToFacets == false) {
      if (this.resendOTP == true) {
        generateOtpModel.assignValues(this.model.otpType = OTPType.NewVisitorOnlineRegistration, this.identityNumber, this.sendOTPByEmail);
        this.otpGeneration(generateOtpModel);
      }
      else{
        this.otpSentTo = this.sharedService.phoneNumber;
      }
      this.isBlocked = false;
    }
    else if (this.model.isRegisteredToFacets == false) {
      generateOtpModel.assignValues(this.model.otpType, this.identityNumber, this.sendOTPByEmail);
      this.otpGeneration(generateOtpModel);
    }
    else if (this.model.available == false) {
      if (this.resendOTP == true) {
        generateOtpModel.assignValues(this.model.otpType, this.identityNumber, this.sendOTPByEmail);
        this.otpGeneration(generateOtpModel);
      }
      else {
        this.toasterService.success("The OTP has been sent", "OTP Sent");
        this.maskSendTo(this.sharedService.phoneNumber);
      }
      this.isBlocked = false;
    }
    else {
      generateOtpModel.assignValues(this.model.otpType, this.identityNumber, this.sendOTPByEmail);
      this.otpGeneration(generateOtpModel);
    }


    this.runOtpTimer();

  }

  runOtpTimer(){
    if(this.resendOTP || this.isAlreadyChangedAndSent){
      this.disableResendOtp = true;
    }
    this.countdown$ = interval(1000).pipe(
      map((tick) => this.initialTime - tick),
      takeWhile((time) => time >= 0)
    );

    this.countdownSubscription = this.countdown$.subscribe((time) => {
      this.displayTime = this.formatTime(time);
      if (this.displayTime == '00:00') {
        this.disableResendOtp = false;
        this.isAlreadyChangedAndSent = false;
        this.resendOTP = false;
      }
    });
  }

  formatTime(time: number): string {
    const minutes = Math.floor(time / 60);
    const seconds = time % 60;
    return `${minutes.toString().padStart(2, '0')}:${seconds
      .toString()
      .padStart(2, '0')}`;
  }

  private otpGeneration(generateOtpModel: GenerateOTPModel) {
    this.publicSiteService.generateOtp(generateOtpModel).subscribe({
      next: (res: ResponseResult<string>) => {
        this.toasterService.success("The OTP has been sent", "OTP Sent");
        this.otpSentTo = res.data;
        this.sharedService.isSriLankaNumber(this.otpSentTo);
        this.resendOTP = false;
        this.isBlocked = false;
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
        this.disableResendOtp = false;
        this.resendOTP = false;
        this.isAlreadyChangedAndSent = false;
        this.isBlocked = false;
      },
    });
  }

  verifyOtp() {
    this.isFormSubmitted = true;
    this.validate();

    if (this.otpForm.invalid) { return; }

    this.isOtpVerifying = true;

    var isValid = compareOnlyTime(this.otpStartTime, new Date(), 5);

    if (isValid) {
      this.isBlocked = true;
      this.verifyOTPModel = Object.assign({}, this.verifyOTPModel, this.otpForm.value);

      this.verifyOTPModel.identityNumber = this.identityNumber;

      this.verifyOTPModel.type = this.model.otpType == undefined ? OTPType.NewVisitorOnlineRegistration : this.model.otpType;

      this.publicSiteService.verifyOtp(this.verifyOTPModel).subscribe({
        next: (res: ResponseResult<PublicUserAuthenticatedModel>) => {
          this.publicUserAuthenticatedModel = res.data;
          localStorage.setItem(appConstant.jwtTokenName, JSON.stringify({ bearerToken: this.publicUserAuthenticatedModel.authToken }));
          this.isFormSubmitted = false;
          this.isBlocked = false;
          this.toasterService.success("You have successfully verified", "OTP verification Successful");
          this.sharedService.isUpdateOtp = true;
          if (this.model.available == false) {
            this.searchVisitor(this.model.identificationNumber);
          }
          else if (this.model.available == true && this.model.visitorStatus == "Active") {
            this.sharedService.isUpdateProfile = true;
            this.isProfile.emit(this.verifyOTPModel.identityNumber);
          }
          else {
            // assocify member
            this.publicSiteService
              .searchVisitor(this.model.nicNumber).subscribe({
                next: (res: ResponseResult<VisitorSearchModel>) => {
                  if (res.data.isRegisteredToFacets == true) {
                    this.isAttachment.emit(res.data.visitorId);

                  }
                  this.isBlocked = false;
                },
              })
          }
        },
        error: (err: ErrorResponse) => {
          this.toasterService.error(err);
          this.isOtpVerifying = false;
          this.isBlocked = false;
          this.isFormSubmitted = false;
          this.toasterService.error(err);
        },
      })

    }
    else {
      this.toasterService.warning("Time has been expired please resend the otp again!");
      this.isOtpVerifying = false;
    }

  }

  searchVisitor(identityNumber: string) {
    this.isBlocked = true;
    this.publicSiteService.searchVisitor(identityNumber).subscribe({
      next: (res: ResponseResult<VisitorSearchModel>) => {
        this.visitorSearchModel = res.data;
        if (this.visitorSearchModel.visitorId) {
          this.isAttachment.emit(this.visitorSearchModel.visitorId);
        }
        this.isBlocked = false;
      },
      error: (err: ErrorResponse) => {
        this.toasterService.error(err);
        this.isBlocked = false;
      },
    })

  }

  goBackHome() {
    this.isProfile.emit(this.identityNumber);
  }


  maskSendTo(sendTo: string) {
    let formattedSendTo = sendTo;

    // if (sendTo.length <= 2) {
    //   this.otpSentTo = formattedSendTo;
    //   return;
    // }

    // let last2Digits = sendTo.slice(-2);

    // formattedSendTo = last2Digits.padStart(sendTo.length - 2, 'X');

    this.otpSentTo = formattedSendTo;
  }
}
