import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'facets-pass-template-size',
  templateUrl: './pass-template-size.component.html',
  styleUrls: ['./pass-template-size.component.scss']
})
export class PassTemplateSizeComponent implements OnInit {
  
  width = '';
  height = '';
  
  @Input() selectedTemplateSize: 'A6' | 'A7' = 'A6';
  @Output() sendDimension = new EventEmitter<any>();
  
  ngOnInit(): void {
    this.changeSearchValue();
  }
  
  changeSearchValue() {
    if(this.selectedTemplateSize == 'A6') {
      this.width = '9.2';
      this.height = '14';
    }
    else {
      this.width = '7.4';
      this.height = '10.5';
    }

    this.sendDimension.emit({ width: this.width, height: this.height, size: this.selectedTemplateSize });
  }
}
