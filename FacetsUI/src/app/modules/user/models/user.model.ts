import { UserClaimModel } from "./user-role.model";

export class UserModel {
    id: string
    userName: string;
    email: string;
    password: string;
    confirmPassword: string;
    firstName: string;
    lastName: string;
    timeZone: string;
    role: string;
    roles = new Array<string>();
    eventIds = new Array<string>();
    permissions = new Array<string>();
    claims = new Array<UserClaimModel>();
    accessibleEvents = new Array<any>();
}