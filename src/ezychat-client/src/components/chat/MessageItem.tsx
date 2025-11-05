import React from "react";
import { Download, Paperclip } from "lucide-react";
import { Message, MessageType } from "@/types/chat.types";

interface MessageItemProps {
  message: Message;
  isOwn: boolean;
  showSender?: boolean;
}

const formatFileSize = (bytes?: number): string => {
  if (!bytes) return "0 B";
  const k = 1024;
  const sizes = ["B", "KB", "MB", "GB"];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + " " + sizes[i];
};

export const MessageItem: React.FC<MessageItemProps> = ({
  message,
  isOwn,
  showSender = false,
}) => {
  // Notification messages - centered and styled differently
  if (message.messageType === MessageType.Notification) {
    return (
      <div className="flex justify-center items-center my-2 w-full">
        <div className="text-xs text-muted-foreground text-center px-3 py-1">
          {message.content}
        </div>
      </div>
    );
  }

  return (
    <div className={`flex ${isOwn ? "justify-end" : "justify-start"}`}>
      <div
        className={`max-w-xs lg:max-w-md px-4 py-2 rounded-lg ${
          isOwn
            ? "bg-primary text-primary-foreground"
            : "bg-muted text-foreground border"
        }`}
      >
        {showSender && !isOwn && message.senderUserName && (
          <p className="text-xs font-semibold mb-1 opacity-70">
            {message.senderUserName}
          </p>
        )}

        {message.messageType === MessageType.Image && message.fileUrl && (
          <div className="mb-2">
            <img
              src={message.fileUrl}
              alt={message.fileName || "Image"}
              className="rounded max-w-full h-auto cursor-pointer"
              onClick={() => window.open(message.fileUrl, "_blank")}
            />
          </div>
        )}

        {message.messageType === MessageType.File && message.fileUrl && (
          <div className="flex items-center space-x-2 mb-2 p-2 bg-background/20 rounded">
            <Paperclip className="w-4 h-4" />
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium truncate">{message.fileName}</p>
              <p className="text-xs opacity-70">
                {formatFileSize(message.fileSize)}
              </p>
            </div>
            <a
              href={message.fileUrl}
              download={message.fileName}
              target="_blank"
              rel="noopener noreferrer"
            >
              <Download className="w-4 h-4 cursor-pointer" />
            </a>
          </div>
        )}

        {message.content && <p className="break-words">{message.content}</p>}

        <p className="text-xs opacity-70 mt-1">
          {new Date(message.createdAt).toLocaleTimeString()}
        </p>
      </div>
    </div>
  );
};
