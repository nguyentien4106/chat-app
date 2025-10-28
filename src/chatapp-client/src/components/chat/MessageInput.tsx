import React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Send, Image, Paperclip, Loader2 } from "lucide-react";

interface MessageInputProps {
  messageInput: string;
  isConnected: boolean;
  isUploading: boolean;
  hasSelectedFile: boolean;
  onMessageChange: (value: string) => void;
  onSendMessage: () => void;
  onKeyDown: (e: React.KeyboardEvent<HTMLInputElement>) => void;
  onImageSelect: () => void;
  onFileSelect: () => void;
}

export const MessageInput: React.FC<MessageInputProps> = ({
  messageInput,
  isConnected,
  isUploading,
  hasSelectedFile,
  onMessageChange,
  onSendMessage,
  onKeyDown,
  onImageSelect,
  onFileSelect,
}) => {
  return (
    <div className="bg-white border-t p-4 flex-shrink-0">
      <div className="flex space-x-2">
        <Button
          variant="outline"
          size="icon"
          onClick={onImageSelect}
          disabled={!isConnected || isUploading}
        >
          <Image className="w-4 h-4" />
        </Button>

        <Button
          variant="outline"
          size="icon"
          onClick={onFileSelect}
          disabled={!isConnected || isUploading}
        >
          <Paperclip className="w-4 h-4" />
        </Button>

        <Input
          value={messageInput}
          onChange={(e) => onMessageChange(e.target.value)}
          onKeyDown={onKeyDown}
          placeholder="Type a message..."
          className="flex-1"
          disabled={!isConnected || isUploading}
        />
        
        <Button
          onClick={onSendMessage}
          disabled={
            !isConnected ||
            isUploading ||
            (!messageInput.trim() && !hasSelectedFile)
          }
        >
          {isUploading ? (
            <Loader2 className="w-4 h-4 animate-spin" />
          ) : (
            <Send className="w-4 h-4" />
          )}
        </Button>
      </div>
    </div>
  );
};
