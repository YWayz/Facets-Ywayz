import { Directive, EventEmitter, HostBinding, HostListener, Output } from '@angular/core';

@Directive({
  selector: '[facetsFileDragDrop]'
})
export class FileDragDropDirective {

  @Output() private filesChangeEmiter : EventEmitter<File[]> = new EventEmitter();
  @HostBinding('style.background') private background = '#eee';
  
  constructor() { }

  @HostListener('dragover', ['$event']) public onDragOver(event: DragEvent){
    event.preventDefault();
    event.stopPropagation();
    this.background = 'lightgray';
  }

  @HostListener('dragleave', ['$event']) public onDragLeave(event: DragEvent){
    event.preventDefault();
    event.stopPropagation();
    this.background = '#eee';
  }

  @HostListener('drop', ['$event']) public onDrop(event: DragEvent){
    event.preventDefault();
    event.stopPropagation();
    this.background = '#eee';
    
    let files = event.dataTransfer!.files;
    let valid_files : File[] = Array.from(files);
    this.filesChangeEmiter.emit(valid_files);
  }
}
