import React from "react";
import { ChatHeader } from "./ChatHeader";
import { MessageList } from "./MessageList";
import { FilePreview } from "./FilePreview";
import { MessageInput } from "./MessageInput";
import { useChatContext } from "@/contexts/ChatContext";

export const ChatArea: React.FC = () => {
  const {
    activeChat,
    messages,
    currentUserId,
    messageInput,
    isConnected,
    isUploading,
    selectedFile,
    previewUrl,
    messagesEndRef,
    setMessageInput,
    handleSendMessage,
    handleKeyDown,
    handleImageSelect,
    handleFileButtonSelect,
    handleClearFile,
  } = useChatContext();
  
  if (!activeChat) return null;
  
  return (
    <>
      <ChatHeader activeChat={activeChat} />
      
      <MessageList
        messages={messages}
        activeChat={activeChat}
        currentUserId={currentUserId}
        messagesEndRef={messagesEndRef}
      />
      
      {selectedFile && (
        <FilePreview
          file={selectedFile}
          previewUrl={previewUrl}
          onClear={handleClearFile}
        />
      )}
      
      <MessageInput
        messageInput={messageInput}
        isConnected={isConnected}
        isUploading={isUploading}
        hasSelectedFile={!!selectedFile}
        onMessageChange={setMessageInput}
        onSendMessage={handleSendMessage}
        onKeyDown={handleKeyDown}
        onImageSelect={handleImageSelect}
        onFileSelect={handleFileButtonSelect}
      />
    </>
  );
};
