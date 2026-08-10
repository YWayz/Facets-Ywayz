export class UserRoleModel {
    roleId: string;
    roleName: string;
    claims = new Array<UserClaimModel>();
    isDefault: boolean;
}

export class UserClaimModel {
    claimType: string;
    claimValue: string;
}