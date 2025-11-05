
// src/hooks/useFileUpload.ts
import { useState, useCallback } from 'react';
import { fileService } from '@/services/fileService';

interface FileUploadResponse {
  fileUrl: string;
  fileName: string;
  fileType: string;
  fileSize: number;
  messageType: number;
}

export const useFileUpload = () => {
  const [isUploading, setIsUploading] = useState(false);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);

  const selectFile = useCallback((file: File, isImage: boolean) => {
    const maxSize = isImage ? 5 * 1024 * 1024 : 10 * 1024 * 1024;
    
    if (file.size > maxSize) {
      throw new Error(`File size must be less than ${maxSize / 1024 / 1024}MB`);
    }

    setSelectedFile(file);
    
    if (isImage) {
      const url = URL.createObjectURL(file);
      setPreviewUrl(url);
    }
  }, []);

  const clearSelection = useCallback(() => {
    setSelectedFile(null);
    if (previewUrl) {
      URL.revokeObjectURL(previewUrl);
      setPreviewUrl(null);
    }
  }, [previewUrl]);

  const uploadFile = useCallback(async (file: File): Promise<FileUploadResponse> => {
    setIsUploading(true);
    try {
      const response = await fileService.uploadFile(file);
      return response;
    } catch (error) {
      console.error('Error uploading file:', error);
      throw error;
    } finally {
      setIsUploading(false);
    }
  }, []);

  return {
    isUploading,
    selectedFile,
    previewUrl,
    selectFile,
    clearSelection,
    uploadFile
  };
};