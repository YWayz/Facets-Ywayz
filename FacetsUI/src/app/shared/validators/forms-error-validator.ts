import { FormGroup } from '@angular/forms';
import { ValidationMessages } from './validation-message.model';

export class GenericValidator {
  constructor(private validationMessages: ValidationMessages) { }

  processMessages(container: FormGroup, isFormsubmitted: boolean): { [key: string]: string } {
    const messages: any = {};
    for (const controlKey in container.controls) {
      if (container.controls.hasOwnProperty(controlKey)) {
        const c = container.controls[controlKey];

        if (c instanceof FormGroup) {
          const childMessages = this.processMessages(c, isFormsubmitted);
          Object.assign(messages, childMessages);
        } else {
          if (this.validationMessages[controlKey]) {
            messages[controlKey] = '';
            if ((c.dirty || c.touched || isFormsubmitted) && c.errors) {
              Object.keys(c.errors).map((messageKey) => {
                if (this.validationMessages[controlKey][messageKey]) {
                  messages[controlKey] +=
                    this.validationMessages[controlKey][messageKey] + ' ';
                }
              });
            }
          }
        }
      }
    }
    return messages;
  }
}
