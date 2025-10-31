import React from "react";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Loader2 } from "lucide-react";
import { ActiveChat } from "@/types/chat.types";

interface Conversation {
  conversationId: string;
  userId: string;
  username: string;
  lastMessage: string;
  unreadCount: number;
}


interface ConversationsListProps {
  conversations: Conversation[];
  activeChat: ActiveChat | null;
  isLoadingConversations: boolean;
  onChatSelect: (chat: ActiveChat) => Promise<void>;
}

export const ConversationsList: React.FC<ConversationsListProps> = ({
  conversations,
  activeChat,
  isLoadingConversations,
  onChatSelect,
}) => {
  return (
    <ScrollArea className="h-full px-4">
      {isLoadingConversations ? (
        <div className="flex items-center justify-center p-8">
          <Loader2 className="w-6 h-6 animate-spin text-gray-400" />
        </div>
      ) : conversations.length === 0 ? (
        <div className="p-4 text-center text-gray-500">
          No conversations yet
        </div>
      ) : (
        conversations.map((conv) => (
          <div
            key={conv.userId}
            onClick={() =>
              onChatSelect({
                id: conv.userId,
                name: conv.username,
                type: "user",
                conversationId: conv.conversationId,
                receiverId: conv.userId,
              })
            }
            className={`p-4 border-b cursor-pointer hover:bg-gray-50 ${
              activeChat?.id === conv.userId ? "bg-blue-50" : ""
            }`}
          >
            <div className="flex items-center space-x-3">
              <Avatar>
                <AvatarFallback>
                  {conv.username[0].toUpperCase()}
                </AvatarFallback>
              </Avatar>
              <div className="flex-1 min-w-0">
                <p className="font-medium truncate">{conv.username}</p>
                <p className="text-sm text-gray-500 truncate">
                  {conv.lastMessage}
                </p>
              </div>
              {conv.unreadCount > 0 && (
                <span className="bg-blue-500 text-white text-xs rounded-full px-2 py-1">
                  {conv.unreadCount}
                </span>
              )}
            </div>
          </div>
        ))
      )}
    </ScrollArea>
  );
};
