import React from "react";
import { JWT_CLAIMS } from "@/constants/jwtClaims";

interface SidebarHeaderProps {
  user: any;
  isConnected: boolean;
}

export const SidebarHeader: React.FC<SidebarHeaderProps> = ({ user, isConnected }) => {
  return (
    <div className="p-4 border-b">
      <h1 className="text-xl font-bold">Chat App</h1>
      <p className="text-sm text-gray-500">
        {user?.userName ||
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
