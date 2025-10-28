import React from "react";
import { Button } from "@/components/ui/button";
import { Paperclip, X } from "lucide-react";

const formatFileSize = (bytes?: number): string => {
  if (!bytes) return "0 B";
  const k = 1024;
  const sizes = ["B", "KB", "MB", "GB"];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + " " + sizes[i];
};

interface FilePreviewProps {
  file: File;
  previewUrl: string | null;
  onClear: () => void;
}

export const FilePreview: React.FC<FilePreviewProps> = ({
  file,
  previewUrl,
  onClear,
}) => {
  return (
    <div className="bg-white border-t p-4 flex-shrink-0">
      <div className="flex items-center space-x-3 bg-gray-100 p-3 rounded-lg">
        {previewUrl ? (
          <img
            src={previewUrl}
            alt="Preview"
            className="w-16 h-16 object-cover rounded"
          />
        ) : (
          <div className="w-16 h-16 bg-gray-200 rounded flex items-center justify-center">
            <Paperclip className="w-8 h-8 text-gray-400" />
          </div>
        )}
        <div className="flex-1 min-w-0">
          <p className="font-medium truncate">{file.name}</p>
          <p className="text-sm text-gray-500">{formatFileSize(file.size)}</p>
        </div>
        <Button variant="ghost" size="sm" onClick={onClear}>
          <X className="w-4 h-4" />
        </Button>
      </div>
    </div>
  );
};
