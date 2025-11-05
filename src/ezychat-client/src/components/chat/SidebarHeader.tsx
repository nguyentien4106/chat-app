import React from "react";
import { JWT_CLAIMS } from "@/constants/jwtClaims";
import { useChatContext } from "@/contexts/ChatContext";

export const SidebarHeader: React.FC = () => {
  const { user, isConnected } = useChatContext();

  return (
    <div className="p-4 border-b">
      <p className="text-sm text-gray-500">
        @{user?.userName ||
          user?.[JWT_CLAIMS.NAME] ||
          user?.[JWT_CLAIMS.EMAIL] ||
          "User"}
      </p>
      <div className="flex items-center mt-2">
        <div
          className={`w-2 h-2 rounded-full mr-2 ${
            isConnected ? "bg-green-500" : "bg-red-500"
          }`}
        />
        <span className="text-xs text-gray-500">
          {isConnected ? "Connected" : "Disconnected"}
        </span>
      </div>
    </div>
  );
};
