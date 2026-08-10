export class FileModel {
    id: string;
    fileName: string;
    uniqueName: string;
    uri: string;
    relatedEntityId: string;
    extenstionData?: ExtensionData;
}

export class ExtensionData {
    attachmentType: string;
}