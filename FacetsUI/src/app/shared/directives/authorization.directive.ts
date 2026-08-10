import { Directive, ElementRef, Input, OnInit } from '@angular/core';
import { AuthService } from 'src/app/modules/auth/services/auth.service';

@Directive({
  selector: '[facetsAuthorization]'
})
export class AuthorizationDirective implements OnInit{

  constructor(private el: ElementRef, private authorizationService: AuthService) { }
  
  @Input('facetsAuthorization') rolePermission: Array<string>;

  ngOnInit(): void {
    if(!this.authorizationService.hasPermissionAuthorization(this.rolePermission)) {
      this.el.nativeElement.style.display = 'none';
      this.el.nativeElement.className = 'disappear';
      this.el.nativeElement.remove();
    }
  }

}
