export class PermissionModel {
    key: string;
    value = new Array<PermissionListModel>();
    isAllPermission: boolean;
}

export class PermissionListModel {
    displayName: string;
    key: string;
    isSelected: boolean = false;
}