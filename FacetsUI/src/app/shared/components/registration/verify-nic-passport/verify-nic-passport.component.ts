import { CommonModule } from '@angular/common';
import { Component, ElementRef, EventEmitter, Input, OnChanges, OnInit, Output, Renderer2, SimpleChanges, ViewChild, inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { TeamMemberSearchModel } from 'src/app/modules/team-member/models/team-member-search.model';
import { TeamMemberService } from 'src/app/modules/team-member/services/team-member.service';
import { VisitorSearchModel } from 'src/app/modules/visitor/models/visitor-search.model';
import { VisitorsService } from 'src/app/modules/visitor/services/visitors.service';
import { VisitorVerificationModel } from 'src/app/public/models/visitor-verification.model';
import { PublicSiteService } from 'src/app/public/services/public-site.service';
import { MaterialModule } from 'src/app/shared/material.module';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'facets-verify-nic-passport',
  templateUrl: './verify-nic-passport.component.html',
  styleUrls: ['./verify-nic-passport.component.scss'],
  standalone: true,
  imports: [CommonModule, SharedModule, MaterialModule]
})
export class VerifyNicPassportComponent implements OnInit, OnChanges {

  isFormSubmitted = false;
  isBlocked = false;

  visitorSearchModel = new VisitorSearchModel();
  teamMemberSearchModel = new TeamMemberSearchModel();

  toasterService = inject(ToasterService);
  visitorsService = inject(VisitorsService);
  teamMemberService = inject(TeamMemberService);
  publicSiteService = inject(PublicSiteService);
  renderer2 = inject(Renderer2);

  @Input() searchValue: string;
  @Input() isPublic: boolean = false;
  @Input() isPayLater: boolean = false;
  @Input() isDisableNicPassport: boolean = false;
  @Input() memberType: 'visitor' | 'teamMember';
  @Input() selectedDocumentType: 'nic' | 'passport' = 'nic';
  @Output() sendVisitorSearchModel = new EventEmitter<VisitorSearchModel>();
  @Output() sendTeamMemberSearchModel = new EventEmitter<TeamMemberSearchModel>();
  @Output() sendVisitorAvailabilityModel = new EventEmitter<VisitorVerificationModel>();
  @Output() sendSearchValueChange = new EventEmitter<boolean>();
  @ViewChild('verify', { static: false }) verify: ElementRef;

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(this.isPayLater) {
      this.searchVisitor()
    }
  }

  changeSearchValue() {
    this.sendSearchValueChange.emit(true);
  }

  verificationProcess(verficationForm: NgForm) {
    if (!this.isPublic) {
      if (this.memberType == 'visitor') {
        this.verifyVisitor(verficationForm)
      }
      else {
        this.verifyTeamMember(verficationForm)
      }
    }
    else {
      this.verifyVisitorPublicSite();
    }
  }

  verifyVisitor(verficationForm: NgForm) {
    this.isFormSubmitted = true;
    if (verficationForm.invalid) { return; }

    this.isBlocked = true;
    this.searchVisitor();
  }

  searchVisitor() {
    this.visitorsService.searchVisitor(this.searchValue).subscribe({
      next: (result: ResponseResult<VisitorSearchModel>) => {
        this.isFormSubmitted = false;
        this.visitorSearchModel = result.data;
        if (this.visitorSearchModel.nicNumber != null) this.selectedDocumentType = 'nic';
        else if (this.visitorSearchModel.passportNumber != null) this.selectedDocumentType = 'passport';
        this.visitorSearchModel.visitorIdentityType = this.selectedDocumentType;
        if (this.selectedDocumentType == "nic") this.visitorSearchModel.nicNumber = this.searchValue;
        else if (this.selectedDocumentType == "passport") this.visitorSearchModel.passportNumber = this.searchValue;
        this.isBlocked = false;
        this.sendVisitorSearchModel.emit(result.data);
      },
      error: (err: ErrorResponse) => {
        this.isFormSubmitted = false;
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  verifyTeamMember(verficationForm: NgForm) {
    this.isFormSubmitted = true;
    if (verficationForm.invalid) { return; }

    this.isBlocked = true;
    this.teamMemberService.searchTeamMember(this.searchValue).subscribe({
      next: (result: ResponseResult<TeamMemberSearchModel>) => {
        this.isFormSubmitted = false;
        this.teamMemberSearchModel = result.data;
        if (this.teamMemberSearchModel.identityType == "NIC") {
          this.selectedDocumentType = "nic"
          this.teamMemberSearchModel.identityType == "NIC" ? this.teamMemberSearchModel.nicNumber = this.searchValue : this.teamMemberSearchModel.passportNumber = this.searchValue;
        } else if (this.teamMemberSearchModel.identityType == "Passport") {
          this.selectedDocumentType = "passport"
          this.teamMemberSearchModel.identityType == "Passport" ? this.teamMemberSearchModel.nicNumber = this.searchValue : this.teamMemberSearchModel.passportNumber = this.searchValue;
        } else {
          this.teamMemberSearchModel.identityType = this.selectedDocumentType;
          this.teamMemberSearchModel.identityType == "nic" ? this.teamMemberSearchModel.nicNumber = this.searchValue : this.teamMemberSearchModel.passportNumber = this.searchValue;
        }

        this.isBlocked = false;
        this.sendTeamMemberSearchModel.emit(result.data);
      },
      error: (err: ErrorResponse) => {
        this.isFormSubmitted = false;
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  verifyVisitorPublicSite() {
    this.isBlocked = true;
    this.publicSiteService.visitorAvailabilityCheck(this.searchValue).subscribe({
      next: (result: ResponseResult<VisitorVerificationModel>) => {
        this.isBlocked = false;
        result.data.identityType = this.selectedDocumentType;
        this.sendVisitorAvailabilityModel.emit(result.data);
      },
      error: (err: ErrorResponse) => {
        this.isFormSubmitted = false;
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }
}
