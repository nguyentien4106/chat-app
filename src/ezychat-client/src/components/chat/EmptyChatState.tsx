import React from "react";
import { MessageSquare } from "lucide-react";

export const EmptyChatState: React.FC = () => {
  return (
    <div className="flex-1 flex items-center justify-center text-gray-500">
      <div className="text-center">
        <MessageSquare className="w-16 h-16 mx-auto mb-4 opacity-20" />
        <p className="text-lg">Select a conversation to start chatting</p>
      </div>
    </div>
  );
};
