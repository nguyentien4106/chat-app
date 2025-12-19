import React from "react";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Loader2 } from "lucide-react";
import { QuickMessage } from "@/types";
import { QuickMessageItem } from "./QuickMessageItem";

interface QuickMessageListProps {
  messages: QuickMessage[];
  loading: boolean;
  onUseMessage?: (content: string) => void;
  onEdit: (message: QuickMessage) => void;
  onDelete: (id: string) => void;
}

export const QuickMessageList: React.FC<QuickMessageListProps> = ({
  messages,
  loading,
  onUseMessage,
  onEdit,
  onDelete,
}) => {
  return (
    <ScrollArea className="h-[400px]">
      {loading ? (
        <div className="flex items-center justify-center py-8">
          <Loader2 className="w-6 h-6 animate-spin" />
        </div>
      ) : messages.length === 0 ? (
        <div className="text-center py-8 text-muted-foreground">
          No quick messages yet. Create your first one!
        </div>
      ) : (
        <div className="space-y-2">
          {messages.map((message) => (
            <QuickMessageItem
              key={message.id}
              message={message}
              onUse={onUseMessage}
              onEdit={onEdit}
              onDelete={onDelete}
            />
          ))}
        </div>
      )}
    </ScrollArea>
  );
};
