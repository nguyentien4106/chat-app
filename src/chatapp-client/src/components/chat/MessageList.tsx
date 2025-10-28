import React from "react";
import { ScrollArea } from "@/components/ui/scroll-area";
import { MessageItem } from "./MessageItem";
import { useChatContext } from "@/contexts/ChatContext";

export const MessageList: React.FC = () => {
  const { activeChat, messages, currentUserId, messagesEndRef } = useChatContext();

  return (
    <div className="flex-1 overflow-hidden min-h-0">
      <ScrollArea className="h-full p-4">
        <div className="space-y-4">
          {messages.length === 0 ? (
            <div className="text-center text-gray-500 mt-8">
              No messages yet. Start the conversation!
            </div>
          ) : (
            messages.map((msg) => (
              <MessageItem
                key={msg.id}
                message={msg}
                isOwn={msg.senderId === currentUserId}
                showSender={activeChat?.type === "group"}
              />
            ))
          )}
          <div ref={messagesEndRef} />
        </div>
      </ScrollArea>
    </div>
  );
};
