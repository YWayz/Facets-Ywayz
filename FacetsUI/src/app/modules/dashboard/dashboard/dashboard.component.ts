import { Component, OnInit, inject } from '@angular/core';
import { UserProfileService } from '../../user/services/user-profile.service';
import { ResponseResult } from 'src/app/core/models/response-result.model';
import { UserProfileModel } from '../../user/models/user-profile.model';
import { AuthService } from '../../auth/services/auth.service';

@Component({
  selector: 'facets-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  user = new UserProfileModel();

  userProfileService = inject(UserProfileService);
  authService = inject(AuthService);

  ngOnInit(): void {
    this.getUserById(this.authService.userId!)
  }

  getUserById(id: string) {
    this.userProfileService.getById(id).subscribe({
      next: (res: ResponseResult<UserProfileModel>) => {
        this.user = res.data;
      },
    })
  }
}
