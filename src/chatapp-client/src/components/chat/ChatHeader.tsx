import React, { useState } from "react";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Info } from "lucide-react";
import { GroupInfoDialog } from "./GroupInfoDialog";

interface ActiveChat {
  id: string;
  name: string;
  type: "user" | "group";
}

interface ChatHeaderProps {
  activeChat: ActiveChat;
}

export const ChatHeader: React.FC<ChatHeaderProps> = ({ activeChat }) => {
  const [showGroupInfo, setShowGroupInfo] = useState(false);

  return (
    <>
      <div className="bg-white border-b p-4 flex-shrink-0">
        <div className="flex items-center justify-between">
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
          
          {activeChat.type === "group" && (
            <Button
              variant="ghost"
              size="icon"
              onClick={() => setShowGroupInfo(true)}
              title="Group Info"
            >
              <Info className="h-5 w-5" />
            </Button>
          )}
        </div>
      </div>

      {activeChat.type === "group" && (
        <GroupInfoDialog
          open={showGroupInfo}
          onOpenChange={setShowGroupInfo}
          groupId={activeChat.id}
        />
      )}
    </>
  );
};
