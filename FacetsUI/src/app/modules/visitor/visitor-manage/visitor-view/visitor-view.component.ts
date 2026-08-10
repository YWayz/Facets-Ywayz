import { Component, inject } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

@Component({
  selector: 'facets-visitor-view',
  templateUrl: './visitor-view.component.html',
  styleUrls: ['./visitor-view.component.scss']
})
export class VisitorViewComponent {

  fullName = '';
  pageNumber = 0;

  router = inject(Router);
  activatedRoute = inject(ActivatedRoute);

  constructor() {
    this.activatedRoute.queryParams.subscribe({
      next: (params: Params) => {
        const pageNumber = params['pageNumber'];
        if (pageNumber != undefined || pageNumber != null) {
          this.pageNumber = pageNumber;
        }
      }
    });
  }

  goToVisitors() {
    this.router.navigate(['admin/visitor/visitor-management'], { queryParams: { pageNumber: this.pageNumber } })
  }
}
