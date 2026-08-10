import { Component, Input } from '@angular/core';

@Component({
  selector: 'facets-block-ui',
  templateUrl: './block-ui.component.html',
  styleUrls: ['./block-ui.component.scss']
})
export class BlockUiComponent {
  @Input() isBlocked = false;
}
