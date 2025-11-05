import React, { useState } from "react";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Info } from "lucide-react";
import { GroupInfoDialog } from "./GroupInfoDialog";
import { ActiveChat } from "@/types/chat.types";

interface ChatHeaderProps {
  activeChat: ActiveChat;
}

export const ChatHeader: React.FC<ChatHeaderProps> = ({ activeChat }) => {
  const [showGroupInfo, setShowGroupInfo] = useState(false);

  return (
    <>
      <div className="bg-card border-b p-4 flex-shrink-0">
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-3">
            <Avatar className="h-10 w-10">
              <AvatarFallback className="bg-primary text-primary-foreground">
                {activeChat.userFullName ? activeChat.userFullName[0].toUpperCase() : activeChat.name[0].toUpperCase()}
              </AvatarFallback>
            </Avatar>
            <div className="flex flex-col">
              <h2 className="font-semibold text-foreground leading-tight">
                {activeChat.userFullName || activeChat.name}
              </h2>
              {activeChat.userFullName && activeChat.name && (
                <p className="text-xs text-muted-foreground leading-tight">
                  @{activeChat.name}
                </p>
              )}
              <p className="text-xs text-muted-foreground/80 mt-0.5">
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
