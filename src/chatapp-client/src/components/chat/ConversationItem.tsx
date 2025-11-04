import React from "react";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Conversation } from "@/types/chat.types";

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
  return (
    <div
      onClick={onSelect}
      className={`p-4 border-b cursor-pointer hover:bg-gray-50 transition-colors ${
        isActive ? "bg-blue-50" : ""
      }`}
    >
      <div className="flex items-center space-x-3">
        <Avatar>
          <AvatarFallback>
            {conversation.userName[0].toUpperCase()}
          </AvatarFallback>
        </Avatar>
        <div className="flex-1 min-w-0">
          <p className="font-medium truncate">{conversation.userFullName}</p>
          <p className="text-sm text-gray-500 truncate">
            {conversation.isLastMessageMine ? "You: " : ""}
            {conversation.lastMessage}
          </p>
        </div>
        {conversation.unreadCount > 0 && (
          <span className="bg-blue-500 text-white text-xs rounded-full px-2 py-1">
            {conversation.unreadCount}
          </span>
        )}
      </div>
    </div>
  );
};
