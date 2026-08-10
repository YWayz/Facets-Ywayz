import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'facets-pass-template-value',
  templateUrl: './pass-template-value.component.html',
  styleUrls: ['./pass-template-value.component.scss']
})
export class PassTemplateValueComponent implements OnInit {

  passValues: any = [
    "Event Name",
    "First Name",
    "Last Name",
    "NIC Number/ Passport Number",
    "Mobile Number",
    "Pass Category",
    "Pass Generated Date & Time",
    "Pass Date"
  ];
  
  @Input() memberType: 'teamMember' | 'visitor' = 'teamMember';
  @Output() sendEvent = new EventEmitter<any>();

  ngOnInit(): void {
    this.getHeaders()
  }

  getHeaders() {
    this.passValues = [
      "Event Name",
      'Full Name',
      "First Name",
      "Last Name",
      "NIC Number/ Passport Number",
      "Mobile Number",
      "Pass Category",
      "Pass Generated Date & Time",
      "Pass Date",
    ];

    if(this.memberType == 'visitor') {
      this.passValues.push('Pass Rate', 'Country', 'Company');
    }
    else {
      this.passValues.push('Company');
    }
  }

  allowDrop(event: any) {
    event.preventDefault();
  }

  dragElement(event: any) {
    event.dataTransfer.setData("id", event.target.id);
    this.sendEvent.emit(event);
  }
}
