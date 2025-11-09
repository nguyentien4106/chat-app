import React from "react";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Conversation } from "@/types/chat.types";
import { useAuth } from "@/contexts/AuthContext";

interface ConversationItemProps {
  conversation: Conversation;
  isActive: boolean;
  onSelect: () => void;
}

export const ConversationItem: React.FC<ConversationItemProps> = ({
  conversation,
  isActive,
  onSelect,
}) => {
  const { applicationUserId } = useAuth()
  return (
    <div
      onClick={onSelect}
      className={`p-4 border-b cursor-pointer hover:bg-muted/50 transition-colors ${
        isActive ? "bg-accent" : ""
      }`}
    >
      <div className="flex items-center space-x-3">
        <Avatar>
          <AvatarFallback>
            {conversation.userName[0].toUpperCase()}
          </AvatarFallback>
        </Avatar>
        <div className="flex-1 min-w-0">
          <p className="font-medium truncate text-foreground">{conversation.userFullName}</p>
          <p className="text-sm text-muted-foreground truncate">
            {conversation.lastMessageSenderId === applicationUserId ? "You: " : ""}
            {conversation.lastMessage}
          </p>
        </div>
        {conversation.unreadCount > 0 && (
          <span className="bg-primary text-primary-foreground text-xs rounded-full px-2 py-1">
            {conversation.unreadCount}
          </span>
        )}
      </div>
    </div>
  );
};
