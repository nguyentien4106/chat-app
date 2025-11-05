import { FileUploadResponse } from '@/types/file.types';
import { apiService } from './api';
export const fileService = {
  uploadFile: async (file: File) => {
    const formData = new FormData();
    formData.append('file', file);

    const response = await apiService.post<FileUploadResponse>(
      '/api/files/upload', 
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    return response;
  },
  deleteFile: (fileUrl: string) =>
    apiService.delete(`/api/files/${encodeURIComponent(fileUrl)}`)
  
};