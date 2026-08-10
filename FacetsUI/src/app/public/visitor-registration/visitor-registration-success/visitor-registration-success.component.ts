import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'facets-visitor-registration-success',
  templateUrl: './visitor-registration-success.component.html',
  styleUrls: ['./visitor-registration-success.component.scss'],
  standalone: true,
  imports: [SharedModule]
})
export class VisitorRegistrationSuccessComponent {

  router = inject(Router);

  // goBackHome() {
  //   this.router.navigate(['/'])
  // }
  // https://www.facetssrilanka.com/
  goBackHome() {
    window.location.href = 'https://www.facetssrilanka.com/';
  }
}
