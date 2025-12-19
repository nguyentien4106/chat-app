import React, { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Send, Image, Paperclip, Loader2, Zap } from "lucide-react";
import { useChatContext } from "@/contexts/ChatContext";
import { QuickMessageDialog } from "@/components/quick-message";


export const MessageInput: React.FC = () => {
  const [showQuickMessages, setShowQuickMessages] = useState(false);
  
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

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setMessageInput(value);
    
    // Check if user typed "/" standalone
    if (value.endsWith("/")) {
      setShowQuickMessages(true);
    }
  };

  const handleSelectQuickMessage = (content: string) => {
    setMessageInput(messageInput.slice(0, messageInput.length - 1) + content);
    messageInputRef.current?.focus();
  };

  return (
    <>
      <QuickMessageDialog
        open={showQuickMessages}
        onOpenChange={setShowQuickMessages}
        onSelectMessage={handleSelectQuickMessage}
      />
      
      <div className="bg-card border-t p-4 flex-shrink-0">
      <div className="flex space-x-2">
        <Button
          variant="outline"
          size="icon"
          onClick={() => setShowQuickMessages(true)}
          disabled={!isConnected}
          title="Quick Messages"
        >
          <Zap className="w-4 h-4" />
        </Button>

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
          onChange={handleInputChange}
          onKeyDown={handleKeyDown}
          placeholder="Type a message or / for quick messages..."
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
    </>
  );
};
