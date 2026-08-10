import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-scan-me',
  templateUrl: './scan-me.component.html',
  styleUrls: ['./scan-me.component.scss']
})
export class ScanMeComponent {

  router: Router = inject(Router);

  scanMe() {
    this.router.navigate(['qr/scanner/scan-me']);
  }
}
