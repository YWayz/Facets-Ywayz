import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { ToasterService } from 'src/app/core/services/toaster.service';
import { VisitorsService } from '../../../services/visitors.service';
import { SearchRequestModel } from 'src/app/core/models/search-request.model';
import { VisitorActivityModel } from '../../../models/visitor-activity.model';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { ErrorResponse } from 'src/app/core/models/error-response.model';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'facets-visitor-view-activity',
  templateUrl: './visitor-view-activity.component.html',
  styleUrls: ['./visitor-view-activity.component.scss']
})
export class VisitorViewActivityComponent implements OnInit {

  isBlocked = false;
  visitorId: string;
  searchModel = new SearchRequestModel(10, 1);
  pageSizeOptions: number[] = [10, 25, 50, 100];

  visitorActivities: VisitorActivityModel[];

  toasterService = inject(ToasterService);
  visitorsService = inject(VisitorsService);
  activatedRoute = inject(ActivatedRoute);

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((params: Params) => {
      this.visitorId = params['visitorId']
      if (this.visitorId != null || this.visitorId != undefined || this.visitorId != '') {
        this.getVisitorActivies();
      }
    });
  }

  getVisitorActivies() {
    this.isBlocked = true;
    this.visitorsService.getVisitorActivities(this.searchModel, this.visitorId).subscribe({
      next: (result: ResponseResult<VisitorActivityModel[]>) => {
        this.isBlocked = false;
        this.searchModel.totalRecords = result.totalRecordCount;
        this.visitorActivities = result.data;
      },
      error: (err: ErrorResponse) => {
        this.isBlocked = false;
        this.toasterService.error(err);
      }
    });
  }

  public pageChanged(event: PageEvent): void {
    this.searchModel.pageSize = event.pageSize
    this.searchModel.pageNumber = event.pageIndex + 1;
    this.getVisitorActivies();
  }
}
