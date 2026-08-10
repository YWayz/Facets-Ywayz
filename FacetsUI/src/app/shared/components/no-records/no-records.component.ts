import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'facets-no-records',
  templateUrl: './no-records.component.html',
  styleUrls: ['./no-records.component.scss']
})
export class NoRecordsComponent {

  @Input() items: any []; 

}
