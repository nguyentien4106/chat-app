import React from "react";
import { Button } from "@/components/ui/button";
import { Paperclip, X } from "lucide-react";
import { useChatContext } from "@/contexts/ChatContext";

const formatFileSize = (bytes?: number): string => {
  if (!bytes) return "0 B";
  const k = 1024;
  const sizes = ["B", "KB", "MB", "GB"];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + " " + sizes[i];
};

export const FilePreview: React.FC = () => {
  const { selectedFile, previewUrl, handleClearFile } = useChatContext();

  if (!selectedFile) return null;

  return (
    <div className="bg-card border-t p-4 flex-shrink-0">
      <div className="flex items-center space-x-3 bg-muted p-3 rounded-lg">
        {previewUrl ? (
          <img
            src={previewUrl}
            alt="Preview"
            className="w-16 h-16 object-cover rounded"
          />
        ) : (
          <div className="w-16 h-16 bg-muted-foreground/20 rounded flex items-center justify-center">
            <Paperclip className="w-8 h-8 text-muted-foreground" />
          </div>
        )}
        <div className="flex-1 min-w-0">
          <p className="font-medium truncate text-foreground">{selectedFile?.name}</p>
          <p className="text-sm text-muted-foreground">
            {formatFileSize(selectedFile?.size)}
          </p>
        </div>
        <Button variant="ghost" size="sm" onClick={handleClearFile}>
          <X className="w-4 h-4" />
        </Button>
      </div>
    </div>
  );
};
