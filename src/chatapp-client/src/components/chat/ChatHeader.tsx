import React from "react";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";

interface ActiveChat {
  id: string;
  name: string;
  type: "user" | "group";
}

interface ChatHeaderProps {
  activeChat: ActiveChat;
}

export const ChatHeader: React.FC<ChatHeaderProps> = ({ activeChat }) => {
  return (
    <div className="bg-white border-b p-4 flex-shrink-0">
      <div className="flex items-center space-x-3">
        <Avatar>
          <AvatarFallback>
            {activeChat.name[0].toUpperCase()}
          </AvatarFallback>
        </Avatar>
        <div>
          <h2 className="font-semibold">{activeChat.name}</h2>
          <p className="text-sm text-gray-500">
            {activeChat.type === "group" ? "Group Chat" : "Direct Message"}
          </p>
        </div>
      </div>
    </div>
  );
};
