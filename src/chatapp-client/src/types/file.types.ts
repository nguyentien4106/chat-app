export interface FileUploadResponse {
    fileUrl: string;
    fileName: string;
    fileType: string;
    fileSize: number;
    messageType: MessageTypes;
}

export enum MessageTypes {
    Text,
    Image,
    File
};