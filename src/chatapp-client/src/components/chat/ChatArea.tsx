import React from "react";
import { ChatHeader } from "./ChatHeader";
import { MessageList } from "./MessageList";
import { FilePreview } from "./FilePreview";
import { MessageInput } from "./MessageInput";
import { Message } from "@/types/chat.types";

interface ActiveChat {
  id: string;
  name: string;
  type: "user" | "group";
}

interface ChatAreaProps {
  activeChat: ActiveChat;
  messages: Message[];
  currentUserId: string | undefined;
  messageInput: string;
  isConnected: boolean;
  isUploading: boolean;
  selectedFile: File | null;
  previewUrl: string | null;
  messagesEndRef: React.RefObject<HTMLDivElement | null>;
  onMessageChange: (value: string) => void;
  onSendMessage: () => void;
  onKeyDown: (e: React.KeyboardEvent<HTMLInputElement>) => void;
  onImageSelect: () => void;
  onFileSelect: () => void;
  onClearFile: () => void;
}

export const ChatArea: React.FC<ChatAreaProps> = ({
  activeChat,
  messages,
  currentUserId,
  messageInput,
  isConnected,
  isUploading,
  selectedFile,
  previewUrl,
  messagesEndRef,
  onMessageChange,
  onSendMessage,
  onKeyDown,
  onImageSelect,
  onFileSelect,
  onClearFile,
}) => {
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
          onClear={onClearFile}
        />
      )}

      <MessageInput
        messageInput={messageInput}
        isConnected={isConnected}
        isUploading={isUploading}
        hasSelectedFile={!!selectedFile}
        onMessageChange={onMessageChange}
        onSendMessage={onSendMessage}
        onKeyDown={onKeyDown}
        onImageSelect={onImageSelect}
        onFileSelect={onFileSelect}
      />
    </>
  );
};
