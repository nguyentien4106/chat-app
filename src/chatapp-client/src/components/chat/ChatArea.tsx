import React from "react";
import { ChatHeader } from "./ChatHeader";
import { MessageList } from "./MessageList";
import { FilePreview } from "./FilePreview";
import { MessageInput } from "./MessageInput";
import { useChatContext } from "@/contexts/ChatContext";

export const ChatArea: React.FC = () => {
  const {
    activeChat,
  } = useChatContext();
  
  if (!activeChat) return null;
  
  return (
    <>
      <ChatHeader activeChat={activeChat} />
      <MessageList />
      <FilePreview />
      <MessageInput/>
    </>
  );
};
