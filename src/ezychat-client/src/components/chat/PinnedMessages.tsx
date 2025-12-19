import React from "react";
import { Pin, X, ChevronDown, ChevronUp } from "lucide-react";
import { PinMessage } from "@/types/chat.types";
import { Button } from "@/components/ui/button";
import { format } from "date-fns";
import { DateTimeFormat } from "@/constants";

interface PinnedMessagesProps {
  pinnedMessages: PinMessage[];
  onUnpin: (messageId: string) => void;
}

export const PinnedMessages: React.FC<PinnedMessagesProps> = ({
  pinnedMessages,
  onUnpin,
}) => {
  const [isExpanded, setIsExpanded] = React.useState(false);

  if (pinnedMessages.length === 0) {
    return null;
  }

  const displayedMessages = isExpanded ? pinnedMessages : pinnedMessages.slice(0, 1);

  return (
    <div className="bg-muted/50 border-b">
      <div className="space-y-2 p-3">
        {displayedMessages.map((pinned) => (
          <div
            key={pinned.id}
            className="flex items-start gap-2 bg-background rounded-lg p-3 shadow-sm"
          >
            <Pin className="w-4 h-4 mt-1 text-primary flex-shrink-0" />
            <div className="flex-1 min-w-0">
              <div className="flex items-center gap-2 mb-1">
                <span className="text-xs font-semibold text-muted-foreground">
                  {pinned.message.senderUserName || "Unknown"}
                </span>
                <span className="text-xs text-muted-foreground">
                  {format(pinned.createdAt, DateTimeFormat)}
                </span>
              </div>
              <p className="text-sm truncate">{pinned.message.content || "(File)"}</p>
            </div>
            <Button
              variant="ghost"
              size="icon"
              className="h-6 w-6 flex-shrink-0"
              onClick={() => onUnpin(pinned.messageId)}
            >
              <X className="w-4 h-4" />
            </Button>
          </div>
        ))}
      </div>

      {pinnedMessages.length > 1 && (
        <div className="flex items-center justify-center pb-2">
          <Button
            variant="ghost"
            size="sm"
            className="h-6 text-xs"
            onClick={() => setIsExpanded(!isExpanded)}
          >
            {isExpanded ? (
              <>
                <ChevronUp className="w-3 h-3 mr-1" />
                Show less
              </>
            ) : (
              <>
                <ChevronDown className="w-3 h-3 mr-1" />
                Show {pinnedMessages.length - 1} more pinned message
                {pinnedMessages.length - 1 > 1 ? "s" : ""}
              </>
            )}
          </Button>
        </div>
      )}
    </div>
  );
};
