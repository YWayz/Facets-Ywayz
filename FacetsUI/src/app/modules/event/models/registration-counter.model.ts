export class RegistrationCounterModel {
    id: string;
    name: string;
    description: string;
    isLocked: boolean;
    eventId: string;
    userName: string | null;
    counterType: string;
}