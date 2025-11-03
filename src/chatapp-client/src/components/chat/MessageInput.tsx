import React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Send, Image, Paperclip, Loader2 } from "lucide-react";
import { useChatContext } from "@/contexts/ChatContext";


export const MessageInput: React.FC = () => {
  const {
  messageInput,
  messageInputRef,
  isConnected,
  isUploading,
  selectedFile,
  setMessageInput,
  handleSendMessage,
  handleKeyDown,
  handleImageSelect,
  handleFileButtonSelect,
} = useChatContext()

const hasSelectedFile = !!selectedFile;

  return (
    <div className="bg-white border-t p-4 flex-shrink-0">
      <div className="flex space-x-2">
        <Button
          variant="outline"
          size="icon"
          onClick={handleImageSelect}
          disabled={!isConnected || isUploading}
        >
          <Image className="w-4 h-4" />
        </Button>

        <Button
          variant="outline"
          size="icon"
          onClick={handleFileButtonSelect}
          disabled={!isConnected || isUploading}
        >
          <Paperclip className="w-4 h-4" />
        </Button>

        <Input
          ref={messageInputRef}
          value={messageInput}
          onChange={(e) => setMessageInput(e.target.value)}
          onKeyDown={handleKeyDown}
          placeholder="Type a message..."
          className="flex-1"
          disabled={!isConnected || isUploading}
        />
        
        <Button
          onClick={handleSendMessage}
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
