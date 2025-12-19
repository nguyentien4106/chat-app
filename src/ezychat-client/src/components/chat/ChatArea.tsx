import React, { useEffect } from "react";
import { ChatHeader } from "./ChatHeader";
import { MessageList } from "./MessageList";
import { FilePreview } from "./FilePreview";
import { MessageInput } from "./MessageInput";
import { PinnedMessages } from "./PinnedMessages";
import { useChatContext } from "@/contexts/ChatContext";
import { toast } from "sonner";

export const ChatArea: React.FC = () => {
  const {
    activeChat,
    pinMessages,
    loadPinMessages,
    unpinMessage,
  } = useChatContext();
  
  // Load pinned messages when active chat changes
  useEffect(() => {
    if (activeChat) {
      const conversationId = activeChat.type === 'user' ? activeChat.conversationId : undefined;
      const groupId = activeChat.type === 'group' ? activeChat.id : undefined;
      loadPinMessages(conversationId, groupId);
    }
  }, [activeChat, loadPinMessages]);

  const handleUnpin = async (messageId: string) => {
    try {
      const conversationId = activeChat?.type === 'user' ? activeChat.conversationId : undefined;
      const groupId = activeChat?.type === 'group' ? activeChat.id : undefined;
      await unpinMessage(messageId, conversationId, groupId);
      // Reload pinned messages after unpinning
      await loadPinMessages(conversationId, groupId);
      toast.success("Message unpinned");
    } catch (error) {
      console.error("Error unpinning message:", error);
      toast.error("Failed to unpin message");
    }
  };
  
  if (!activeChat) return null;
  
  return (
    <>
      <ChatHeader activeChat={activeChat} />
      <PinnedMessages pinnedMessages={pinMessages} onUnpin={handleUnpin} />
      <MessageList />
      <FilePreview />
      <MessageInput/>
    </>
  );
};
