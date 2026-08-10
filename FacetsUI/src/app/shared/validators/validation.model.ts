import { DisplayMessageModel } from "./display-message.model";
import { GenericValidator } from "./forms-error-validator";
import { ValidationMessages } from "./validation-message.model";

export class ValidationModel {
    displayMessage: DisplayMessageModel = new DisplayMessageModel();
    validationMessages: ValidationMessages;
    formsErrorValidator: GenericValidator;
}