import React from "react";
import { MoreVertical, Pin, PinOff } from "lucide-react";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Message } from "@/types/chat.types";

interface MessageOptionsProps {
  message: Message;
  onPin: (message: Message) => void;
  onUnpin: (message: Message) => void;
}

export const MessageOptions: React.FC<MessageOptionsProps> = ({
  message,
  onPin,
  onUnpin,
}) => {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <button className="p-1 rounded hover:bg-muted/50 opacity-0 group-hover:opacity-100 transition-opacity">
          <MoreVertical className="w-4 h-4" />
        </button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        {message.isPinned ? (
          <DropdownMenuItem
            onClick={() => onUnpin(message)}
            className="cursor-pointer"
          >
            <PinOff className="w-4 h-4 mr-2" />
            Unpin Message
          </DropdownMenuItem>
        ) : (
          <DropdownMenuItem
            onClick={() => onPin(message)}
            className="cursor-pointer"
          >
            <Pin className="w-4 h-4 mr-2" />
            Pin Message
          </DropdownMenuItem>
        )}
      </DropdownMenuContent>
    </DropdownMenu>
  );
};
